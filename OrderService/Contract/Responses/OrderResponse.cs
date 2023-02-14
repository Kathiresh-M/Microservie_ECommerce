using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Responses
{
    public class OrderResponse : MessageResponse
    {
        public Order orderdto { get; protected set; }
        public OrderResponse(bool isSuccess, string message, Order order) : base(isSuccess, message)
        {
            this.orderdto = order;
        }
    }
}
