using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestDto
{
    public class PaymentDto
    {
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public Guid PaymentId { get; set; }
    }
}
