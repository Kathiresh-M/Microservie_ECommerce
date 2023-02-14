using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model
{
    public class OrderDetails : CommonForAll
    {
        public Guid OrderId { get; set; }
        public bool PaymentStatus { get; set; } = true;
    }
}
