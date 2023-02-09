using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses
{
    public class CartResponses : MessageResponse
    {
        public Cart Cartdto { get; protected set; }
        public CartResponses(bool isSuccess, string message, Cart user) : base(isSuccess, message)
        {
            this.Cartdto = user;
        }
    }
}
