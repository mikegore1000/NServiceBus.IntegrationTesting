using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Messages
{
    public class PlaceOrder : ICommand
    {
        public PlaceOrder(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }

    public class OrderPlaced : IEvent
    {
        public OrderPlaced(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }

    public class OrderBilled : IEvent
    {
        public OrderBilled(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}
