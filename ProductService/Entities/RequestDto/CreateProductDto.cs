using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestDto
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public bool Visibility { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public byte[] Image { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int ProductCount { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^(Shirts|Footwear and Shoes|Toys|Electronics and Gadgets|Sports)$",
        ErrorMessage = "The field can only be any one of the following, Clothes , Footwear and Shoes, Toys, Electronics and Gadgets, Sports")]
        public string Category {  get; set; }

    }
}
