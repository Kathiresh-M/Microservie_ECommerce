using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model
{
    public class Cart : Commonmodel
    {
        public Guid User_Id { get; set; }
        public Guid Product_Id { get; set; }
        public int Quantity { get; set; }
        public bool IsPurchase { get; set; } = false;
    }
}
