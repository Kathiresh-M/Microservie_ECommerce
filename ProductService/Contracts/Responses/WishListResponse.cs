using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Responses
{
    public class WishListResponse : MessageResponse
    {
        public Wishlist Wishlistdto { get; protected set; }
        public WishListResponse(bool isSuccess, string message, Wishlist wishlist) : base(isSuccess, message)
        {
            this.Wishlistdto = wishlist;
        }
    }
}
