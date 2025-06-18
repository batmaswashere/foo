using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Repository.Hierarchy;
using OrderBookApp.Controller;
using OrderBookApp.Util;
using OrderBookApp.View;
using Logger = OrderBookApp.Util.Logger;

namespace OrderBookApp.TradeEngine
{
    class TradeOrderEngine
    {
        private ILog log;
        private List<PriceDepthNormalView> viewList;
        
        public TradeOrderEngine()
        {
            log = Logger.GetLogger();
            viewList = new List<PriceDepthNormalView>();
        }

        public List<PriceDepthNormalView> Execute(List<Order.Order> orderList)
        {
            // Prepare Output File
            string baseFilePath = ConfigurationManager.AppSettings[Constants.BASE_FOLDER];
            string outputFile = Constants.OUTPUT_FILE_1;
            string outputFilePath = baseFilePath + @"output\";
            if (!Directory.Exists(outputFilePath))
            {
                System.IO.Directory.CreateDirectory(outputFilePath);
            }
            string outputFileFullPath = String.Concat(outputFilePath, outputFile);
            FileHandler handler = new FileHandler();

            PriceDepthNormalView view = new PriceDepthNormalView();
            //foreach (Order.Order order in orderList)
            for (int i = 0; i < orderList.Count; i ++)
            {
                Order.Order order = orderList[i];
                if (i != 0)
                {
                    view = view.Clone(view);
                }
                
                view.sequence = order.sequence;
                view.symbol = order.symbol;
                switch (order.msgType)
                {
                    // Order Added, Length = 32
                    case Constants.ORDER_TYPE_ADDED:
                        if (order.side == Constants.SIDE_BUY_ORDER)
                        {
                            view.Bid.AddLast(order);
                        }
                        else if(order.side == Constants.SIDE_SELL_ORDER)
                        {
                            view.Ask.AddLast(order);
                        }
                        break;

                    // Order Update, Length = 32
                    case Constants.ORDER_TYPE_UPDATED:
                        if (order.side == Constants.SIDE_BUY_ORDER)
                        {
                            Order.Order orginalOrder = view.Bid.FirstOrDefault(n => n.orderID == order.orderID);
                            if (orginalOrder != null)
                            {
                                orginalOrder.price = order.price;
                                orginalOrder.size = order.size;
                            }
                        }
                        else if (order.side == Constants.SIDE_SELL_ORDER)
                        {
                            Order.Order orginalOrder = view.Ask.FirstOrDefault(n => n.orderID == order.orderID);
                            if (orginalOrder != null)
                            {
                                orginalOrder.price = order.price;
                                orginalOrder.size = order.size;
                            }
                        }
                        break;

                    // Order Delete, Length = 16
                    case Constants.ORDER_TYPE_DELETED:
                        if (order.side == Constants.SIDE_BUY_ORDER)
                        {
                            Order.Order orginalOrder = view.Bid.FirstOrDefault(n => n.orderID == order.orderID);
                            if (orginalOrder != null)
                            {
                                view.Bid.Remove(orginalOrder);
                            }
                        }
                        else if (order.side == Constants.SIDE_SELL_ORDER)
                        {
                            Order.Order orginalOrder = view.Ask.FirstOrDefault(n => n.orderID == order.orderID);
                            if (orginalOrder != null)
                            {
                                view.Ask.Remove(orginalOrder);
                            }
                        }
                        break;

                    // Order Execute, Length = 24
                    case Constants.ORDER_TYPE_EXECUTED:
                        if (order.side == Constants.SIDE_BUY_ORDER)
                        {
                            Order.Order orginalOrder = view.Bid.FirstOrDefault(n => n.orderID == order.orderID);
                            if (orginalOrder != null)
                            {
                                if (orginalOrder.size - order.tradedQuantity > 0)
                                    orginalOrder.size = orginalOrder.size - order.tradedQuantity;
                                else if (orginalOrder.size - order.tradedQuantity == 0)
                                    view.Bid.Remove(orginalOrder);
                            }
                        }
                        else if (order.side == Constants.SIDE_SELL_ORDER)
                        {
                            Order.Order orginalOrder = view.Ask.FirstOrDefault(n => n.orderID == order.orderID);
                            if (orginalOrder != null)
                            {
                                if (orginalOrder.size - order.tradedQuantity > 0)
                                    orginalOrder.size = orginalOrder.size - order.tradedQuantity;
                                else if (orginalOrder.size - order.tradedQuantity == 0)
                                    view.Ask.Remove(orginalOrder);
                            }
                        }
                        break;
                }
                viewList.Add(view);
                if (i % 1000 == 0) {
                    log.Info(string.Format("Proceed more than {0} view", i));
                }
            }
            log.Info(string.Format("View List size = {0}", viewList.Count));

            return viewList;
        }

        public void Execute(List<Order.Order> orderList, string fileName, int NLevel)
        {
            // Prepare Output File
            string baseFilePath = ConfigurationManager.AppSettings[Constants.BASE_FOLDER];
            string outputFile = Constants.OUTPUT_FILE_1;
            string outputFilePath = baseFilePath + @"output\";
            if (!Directory.Exists(outputFilePath))
            {
                System.IO.Directory.CreateDirectory(outputFilePath);
            }
            string outputFileFullPath = String.Concat(outputFilePath, outputFile);
            FileHandler handler = new FileHandler();

            // Write the byte array to the other FileStream.
            using (FileStream fsNew = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {

                PriceDepthNormalView view = new PriceDepthNormalView();
                //foreach (Order.Order order in orderList)
                for (int i = 0; i < orderList.Count; i++)
                {
                    Order.Order order = orderList[i];
                    if (i != 0)
                    {
                        view = view.Clone(view);
                    }

                    view.sequence = order.sequence;
                    view.symbol = order.symbol;
                    switch (order.msgType)
                    {
                        // Order Added, Length = 32
                        case Constants.ORDER_TYPE_ADDED:
                            if (order.side == Constants.SIDE_BUY_ORDER)
                            {
                                view.Bid.AddLast(order);
                            }
                            else if (order.side == Constants.SIDE_SELL_ORDER)
                            {
                                view.Ask.AddLast(order);
                            }
                            break;

                        // Order Update, Length = 32
                        case Constants.ORDER_TYPE_UPDATED:
                            if (order.side == Constants.SIDE_BUY_ORDER)
                            {
                                Order.Order orginalOrder = view.Bid.FirstOrDefault(n => n.orderID == order.orderID);
                                if (orginalOrder != null)
                                {
                                    orginalOrder.price = order.price;
                                    orginalOrder.size = order.size;
                                }
                            }
                            else if (order.side == Constants.SIDE_SELL_ORDER)
                            {
                                Order.Order orginalOrder = view.Ask.FirstOrDefault(n => n.orderID == order.orderID);
                                if (orginalOrder != null)
                                {
                                    orginalOrder.price = order.price;
                                    orginalOrder.size = order.size;
                                }
                            }
                            break;

                        // Order Delete, Length = 16
                        case Constants.ORDER_TYPE_DELETED:
                            if (order.side == Constants.SIDE_BUY_ORDER)
                            {
                                Order.Order orginalOrder = view.Bid.FirstOrDefault(n => n.orderID == order.orderID);
                                if (orginalOrder != null)
                                {
                                    view.Bid.Remove(orginalOrder);
                                }
                            }
                            else if (order.side == Constants.SIDE_SELL_ORDER)
                            {
                                Order.Order orginalOrder = view.Ask.FirstOrDefault(n => n.orderID == order.orderID);
                                if (orginalOrder != null)
                                {
                                    view.Ask.Remove(orginalOrder);
                                }
                            }
                            break;

                        // Order Execute, Length = 24
                        case Constants.ORDER_TYPE_EXECUTED:
                            if (order.side == Constants.SIDE_BUY_ORDER)
                            {
                                Order.Order orginalOrder = view.Bid.FirstOrDefault(n => n.orderID == order.orderID);
                                if (orginalOrder != null)
                                {
                                    if (orginalOrder.size - order.tradedQuantity > 0)
                                        orginalOrder.size = orginalOrder.size - order.tradedQuantity;
                                    else if (orginalOrder.size - order.tradedQuantity == 0)
                                        view.Bid.Remove(orginalOrder);
                                }
                            }
                            else if (order.side == Constants.SIDE_SELL_ORDER)
                            {
                                Order.Order orginalOrder = view.Ask.FirstOrDefault(n => n.orderID == order.orderID);
                                if (orginalOrder != null)
                                {
                                    if (orginalOrder.size - order.tradedQuantity > 0)
                                        orginalOrder.size = orginalOrder.size - order.tradedQuantity;
                                    else if (orginalOrder.size - order.tradedQuantity == 0)
                                        view.Ask.Remove(orginalOrder);
                                }
                            }
                            break;
                    }
                    //viewList.Add(view);
                    if (i % 1000 == 0)
                    {
                        log.Info(string.Format("Proceed more than {0} view", i));
                    }


                    

                    StringBuilder sb = new StringBuilder();
                    sb.Append(view.sequence);
                    sb.Append(", ");
                    sb.Append(view.symbol);
                    sb.Append(", [");

                    // SELECT Price, SUM(Size)
                    // FROM Order
                    // GROUP BY Price
                    // ORDER BY Price DESC
                    //IQueryable<IGrouping<int, ulong>> query = (IQueryable<IGrouping<int, ulong>>)(from bidOrder in view.Bid
                    //                                                                              orderby bidOrder.price descending
                    //                                                                              group bidOrder by bidOrder.price into gp
                    //                                                                            select new Order.Order { price = gp.Key, size = (ulong)gp.Sum(m => Convert.ToDecimal(m.size))})
                    //                                                                            .Take(NLevel).ToList();
                    List<Order.Order> sortedBid = (List<Order.Order>)(from bidOrder in view.Bid
                                                                      orderby bidOrder.price descending
                                                                      group bidOrder by bidOrder.price into gp
                                                                      select new Order.Order { price = gp.Key, size = (ulong)gp.Sum(m => Convert.ToDecimal(m.size)) })
                                                                      .Take(NLevel).ToList();



                    //List<Order.Order> sortedBid = (List<Order.Order>)(from bidOrder in view.Bid
                    //                                                    orderby bidOrder.price descending
                    //                                                    select bidOrder).Take(NLevel).ToList();
                    for (int j = 0; j < sortedBid.Count; j++)
                    {
                        Order.Order bidOrder = sortedBid.ElementAt(j);
                        sb.Append("(");
                        sb.Append(bidOrder.price);
                        sb.Append(", ");
                        sb.Append(bidOrder.size);
                        sb.Append(")");
                        if (j != sortedBid.Count - 1)
                        {
                            sb.Append(", ");
                        }
                    }
                    sb.Append("], ");

                    //List<Order.Order> sortedAsk = (List<Order.Order>)(from askOrder in view.Ask
                    //                                                    orderby askOrder.price
                    //                                                    select askOrder).Take(NLevel).ToList();
                    List<Order.Order> sortedAsk = (List<Order.Order>)(from askOrder in view.Ask
                                                                      orderby askOrder.price
                                                                      group askOrder by askOrder.price into gp
                                                                      select new Order.Order { price = gp.Key, size = (ulong)gp.Sum(m => Convert.ToDecimal(m.size)) })
                                                                      .Take(NLevel).ToList();

                    sb.Append("[");
                    for (int j = 0; j < sortedAsk.Count; j++)
                    {
                        Order.Order askOrder = sortedAsk.ElementAt(j);
                        sb.Append("(");
                        sb.Append(askOrder.price);
                        sb.Append(", ");
                        sb.Append(askOrder.size);
                        sb.Append(")");
                        if (j != sortedAsk.Count - 1)
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

                fsNew.Close();
            }
            //log.Info(string.Format("View List size = {0}", viewList.Count));
        }
    }
}
