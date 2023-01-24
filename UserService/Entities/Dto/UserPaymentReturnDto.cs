using Entities.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UserPaymentReturnDto
    {
        public Guid id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string PaymentValue { get; set; }

        public string? Expiry { get; set; }
    }
}
