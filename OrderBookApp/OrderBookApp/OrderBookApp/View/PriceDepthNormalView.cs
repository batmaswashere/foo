using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OrderBookApp.Order;

namespace OrderBookApp.View
{
    class PriceDepthNormalView
    {

        //public OrderBook Bid { get; set; }
        //public OrderBook Ask { get; set; }
        public PriceDepthNormalView() 
        {
            Bid = new LinkedList<Order.Order>();
            Ask = new LinkedList<Order.Order>();
        }

        public uint sequence { get; set; }
        public string symbol { get; set; }
        public LinkedList<Order.Order> Bid { get; set; }

        public LinkedList<Order.Order> Ask { get; set; }

        public PriceDepthNormalView Clone(PriceDepthNormalView originalView)
        {
            PriceDepthNormalView view = new PriceDepthNormalView();
            view.sequence = originalView.sequence;
            view.symbol = originalView.symbol;
            LinkedList<Order.Order> newBidList = new LinkedList<Order.Order>();
            foreach (Order.Order order in originalView.Bid) {
                Order.Order newOrder = order.Clone();
                newBidList.AddLast(newOrder);
            }
            view.Bid = newBidList;

            LinkedList<Order.Order> newAskList = new LinkedList<Order.Order>();
            foreach (Order.Order order in originalView.Ask)
            {
                Order.Order newOrder = order.Clone();
                newAskList.AddLast(newOrder);
            }
            view.Ask = newAskList;
            return view;
        }

        public LinkedList<Order.Order> SortedBid() {
            return SortedLinkedList(Bid);
        }
        public LinkedList<Order.Order> SortedAsk()
        {
            return SortedLinkedList(Ask);
        }
        public LinkedList<Order.Order> SortedLinkedList(LinkedList<Order.Order> lList)
        {
            //LinkedList < Order.Order > newSortedList = (LinkedList < Order.Order >)IList.
            return new LinkedList<Order.Order>();
        }
    }
}
