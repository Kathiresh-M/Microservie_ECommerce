using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Model
{
    public class ProductModel : Commonmodel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Visibility { get; set; }
        public byte[] Image { get; set; }
        public decimal Price { get; set; }
        public int ProductCount { get; set; }
        public string Category { get; set; }
    }
}
