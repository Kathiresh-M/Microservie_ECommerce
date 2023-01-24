using Entities.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestDto
{
    public class PaymentUpdateDto
    {
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^(Debit|Credit|UPI)$", ErrorMessage = "This field can be either 'Credit type' or 'Debit type' or 'UPI type'")]
        public string Type { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [CheckStringOrInteger("Type")]
        public string PaymentValue { get; set; } = string.Empty;


        [CheckCreditdebit("Type", "Credit", "Debit")]
        [Checkcardexpiry]
        public string? Expiry { get; set; }
    }
}
