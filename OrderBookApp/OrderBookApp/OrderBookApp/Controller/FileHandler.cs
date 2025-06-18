using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Reflection.PortableExecutable;
using System.Buffers.Binary;
using OrderBookApp.Order;
using log4net;
using OrderBookApp.Util;
using OrderBookApp.View;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;

namespace OrderBookApp.Controller
{
    class FileHandler
    {
        private ILog logger;

        public string ReadFile(string fileName)
        {
            // Check file existence
            if (!File.Exists(fileName))
            {
                return string.Empty;
            }

            using (FileStream fsSource = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                // Read the source file into a byte array.
                byte[] readArr = new byte[fsSource.Length];
                int count;
                StringBuilder stringBuilder = new StringBuilder();
                while ((count = fsSource.Read(readArr, 0, readArr.Length)) > 0)
                {
                    stringBuilder.Append(Encoding.Default.GetString(readArr, 0, count));
                }
                string text = stringBuilder.ToString();
                return text;
            }
        }
        public List<Order.Order> ReadBinaryFile(string fileName)
        {
            logger = Logger.GetLogger();

            // Check file existence
            if (!File.Exists(fileName))
            {
                return new List<Order.Order>();
            }

            FileStream fsSource = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new BinaryReader(fsSource))
            {
                int index = 0;
                List<Order.Order> orderList = new List<Order.Order>();
                StringBuilder stringBuilder = new StringBuilder();
                int i = 0;
                int nextChar = 0;
                try {
                    nextChar = reader.PeekChar();
                }
                catch (ArgumentException  ae) {
                    //logger.Info(string.Format("ArgumentException = {0}", ae.Message));
                }
                
                while (nextChar != -1)
                {
                    // Message Header
                    uint sequence = reader.ReadUInt32();
                    int msgLength = (int)reader.ReadUInt32();

                    // Message Body
                    byte[] readMsgArr = new byte[msgLength];
                    int numOfBytesInArr = reader.Read(readMsgArr, index, msgLength);
                    string msgBody = Encoding.Default.GetString(readMsgArr, 0, numOfBytesInArr);
                    stringBuilder.Append(msgBody);
                    logger.Info(string.Format("sequence = {0}, msgLength = {1}, msgBody = {2}", sequence.ToString(), msgLength.ToString(), msgBody));
                    //Console.WriteLine(string.Format("sequence = {0}, msgLength = {1}, msgBody = {2}", sequence.ToString(), msgLength.ToString(), msgBody));


                    // Order Type
                    Order.Order msgOrder = new Order.Order();
                    msgOrder.sequence = sequence;
                    msgOrder.msgType = Encoding.Default.GetString(readMsgArr, 0, 1);
                    //string msgType = Encoding.Default.GetString(readMsgArr, 0, 1);
                    switch (msgOrder.msgType)
                    {
                        // Order Added, Length = 32
                        case Constants.ORDER_TYPE_ADDED:
                            logger.Info("Order Added");
                            //Console.WriteLine("Order Added");
                            msgOrder.symbol = Encoding.Default.GetString(readMsgArr, 1, 3);
                            msgOrder.orderID = BitConverter.ToUInt64(readMsgArr, 4);
                            msgOrder.side = Encoding.Default.GetString(readMsgArr, 12, 1);
                            msgOrder.reserved = Encoding.Default.GetString(readMsgArr, 13, 3);
                            msgOrder.size = BitConverter.ToUInt64(readMsgArr, 16);
                            msgOrder.price = BitConverter.ToInt32(readMsgArr, 24);
                            msgOrder.reserved2 = Encoding.Default.GetString(readMsgArr, 28, 4);
                            logger.Info(msgOrder.ToString());
                            //Console.WriteLine(msgOrder.toString());
                            break;

                        // Order Update, Length = 32
                        case Constants.ORDER_TYPE_UPDATED:
                            logger.Info("Order Updated");
                            //Console.WriteLine("Order Updated");
                            msgOrder.symbol = Encoding.Default.GetString(readMsgArr, 1, 3);
                            msgOrder.orderID = BitConverter.ToUInt64(readMsgArr, 4);
                            msgOrder.side = Encoding.Default.GetString(readMsgArr, 12, 1);
                            msgOrder.reserved = Encoding.Default.GetString(readMsgArr, 13, 3);
                            msgOrder.size = BitConverter.ToUInt64(readMsgArr, 16);
                            msgOrder.price = BitConverter.ToInt32(readMsgArr, 24);
                            msgOrder.reserved2 = Encoding.Default.GetString(readMsgArr, 28, 4);
                            logger.Info(msgOrder.ToString());
                            //Console.WriteLine(msgOrder.toString());
                            break;

                        // Order Delete, Length = 16
                        case Constants.ORDER_TYPE_DELETED:
                            logger.Info("Order Deleted");
                            //Console.WriteLine("Order Deleted");
                            msgOrder.symbol = Encoding.Default.GetString(readMsgArr, 1, 3);
                            msgOrder.orderID = BitConverter.ToUInt64(readMsgArr, 4);
                            msgOrder.side = Encoding.Default.GetString(readMsgArr, 12, 1);
                            msgOrder.reserved = Encoding.Default.GetString(readMsgArr, 13, 3);
                            logger.Info(msgOrder.ToString());
                            //Console.WriteLine(msgOrder.toString());
                            break;

                        // Order Execute, Length = 24
                        case Constants.ORDER_TYPE_EXECUTED:
                            logger.Info("Order Executed");
                            //Console.WriteLine("Order Executed");
                            msgOrder.symbol = Encoding.Default.GetString(readMsgArr, 1, 3);
                            msgOrder.orderID = BitConverter.ToUInt64(readMsgArr, 4);
                            msgOrder.side = Encoding.Default.GetString(readMsgArr, 12, 1);
                            msgOrder.reserved = Encoding.Default.GetString(readMsgArr, 13, 3);
                            msgOrder.tradedQuantity = BitConverter.ToUInt64(readMsgArr, 16);
                            logger.Info(msgOrder.ToString());
                            //Console.WriteLine(msgOrder.toString());
                            break;

                    }

                    orderList.Add(msgOrder);
                    try
                    {
                        nextChar = reader.PeekChar();
                    }
                    catch (ArgumentException ae)
                    {
                        //logger.Info(string.Format("ArgumentException = {0}", ae.Message));
                    }
                }

                logger.Info(string.Format("Order List size = {0}", orderList.Count));
                //string wholeMsg = stringBuilder.ToString();
                //logger.Info(string.Format("Whole Message = {0}", wholeMsg));
                return orderList;
            }
        }


        public void WriteFile(string fileName, string text)
        {
            // Write the byte array to the other FileStream.
            using (FileStream fsNew = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                // Transformation Format)
                byte[] writeArr = Encoding.UTF8.GetBytes(text);

                // Using the Write method write
                // the encoded byte array to
                // the textfile
                fsNew.Write(writeArr, 0, text.Length);

                // Close the FileStream object
                fsNew.Close();
            }
        }
        public void WriteOutputLogFile(string fileName, List<PriceDepthNormalView> viewList, int NLevel)
        {
            logger = Logger.GetLogger();

            // Write the byte array to the other FileStream.
            using (FileStream fsNew = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                foreach (PriceDepthNormalView view in viewList) 
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(view.sequence);
                    sb.Append(", ");
                    sb.Append(view.symbol);
                    sb.Append(", [");

                    //List<Order.Order> sortedBid = (List<Order.Order>)(from order in view.Bid
                    //                                                  group order by order.price into groupOrder
                    //                                                  orderby groupOrder.Key descending
                    //                                                  select new {groupOrder.Key, groupOrder.Sum(x => x.size)}).ToList();

                    List<Order.Order> sortedBid = (List<Order.Order>)(from order in view.Bid
                                                                      orderby order.price descending
                                                                      select order).Take(NLevel).ToList();
                    for (int i = 0; i < sortedBid.Count; i++)
                    {
                        Order.Order order = sortedBid.ElementAt(i);
                        sb.Append("(");
                        sb.Append(order.price);
                        sb.Append(", ");
                        sb.Append(order.size);
                        sb.Append(")");
                        if (i != sortedBid.Count - 1)
                        {
                            sb.Append(", ");
                        }
                    }
                    sb.Append("], ");

                    List<Order.Order> sortedAsk = (List<Order.Order>)(from order in view.Ask
                                                                      orderby order.price 
                                                                      select order).DistinctBy(x => x.price).Take(NLevel).ToList();
                    for (int i = 0; i < sortedAsk.Count; i++)
                    {
                        Order.Order order = sortedAsk.ElementAt(i);
                        sb.Append("(");
                        sb.Append(order.price);
                        sb.Append(", ");
                        sb.Append(order.size);
                        sb.Append(")");
                        if (i != sortedAsk.Count - 1)
                        {
                            sb.Append(", ");
                        }
                    }
                    sb.Append("]\r\n");

                    string text = sb.ToString();
                    // Transformation Format)
                    byte[] writeArr = Encoding.UTF8.GetBytes(text);

                    // Using the Write method write
                    // the encoded byte array to
                    // the textfile
                    fsNew.Write(writeArr, 0, text.Length);
                    Console.Write(text);
                }
                
                // Close the FileStream object
                fsNew.Close();
            }
        }
    }
}
