using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model
{
    public class Order : CommonForAll
    {
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public Guid PaymentId { get; set; }
        public string PaymentType { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
