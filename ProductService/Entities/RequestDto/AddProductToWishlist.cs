using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestDto
{
    public class AddProductToWishlist
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public Guid Product { get; set; }
    }
}
