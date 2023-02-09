using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses
{
    public class ProductResponse : MessageResponse
    {
        public ProductModel Productdto { get; protected set; }
        public ProductResponse(bool isSuccess, string message, ProductModel user) : base(isSuccess, message)
        {
            this.Productdto = user;
        }
    }
}
