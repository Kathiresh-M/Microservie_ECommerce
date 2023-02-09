using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestDto
{
    public class AddToCartDto
    {
        [Required(ErrorMessage = "Please enter your Product Id")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Please enter your product quantity")]
        [Range(1, 10, ErrorMessage = "Please give range between 1 to 10")]
        public int Quantity { get; set; }
    }
}
