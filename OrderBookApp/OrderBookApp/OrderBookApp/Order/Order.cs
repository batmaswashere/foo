using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderBookApp.View;

namespace OrderBookApp.Order
{
    public class Order
    {
        public uint sequence { get; set; }
        public string msgType { get; set; }
        public string symbol { get; set; }
        public ulong orderID { get; set; }
        public string side { get; set; }
        public string reserved { get; set; }
        public ulong size { get; set; }
        public int price { get; set; }
        public string reserved2 { get; set; }
        public ulong tradedQuantity { get; set; }

        public string ToString()
        {
            string text = string.Format("sequence={0},\r\nmsgType = {1},\r\nsymbol = {2},\r\n"
                + "orderID = {3},\r\nside = {4},\r\nreserved = {5},\r\n"
                + "size = {6},\r\nprice = {7},\r\nreserved2 = {8},\r\ntradedQuantity = {9}\r\n",
                sequence, msgType, symbol,
                orderID.ToString(), side, reserved,
                size.ToString(), price.ToString(), reserved2, tradedQuantity);
            return text;
        }

        public Order Clone()
        {
            Order newOrder = new Order();
            newOrder.sequence = sequence;
            newOrder.msgType = msgType;
            newOrder.symbol = symbol;
            newOrder.orderID = orderID;
            newOrder.side = side;
            newOrder.reserved = reserved;
            newOrder.size = size;
            newOrder.price = price;
            newOrder.reserved2 = reserved2;
            newOrder.tradedQuantity = tradedQuantity;
            return newOrder;
        }
    }
}
