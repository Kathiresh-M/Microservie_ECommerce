using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model
{
    public class Wishlist : Commonmodel
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }   
        public Guid Product {  get; set; }
    }
}
