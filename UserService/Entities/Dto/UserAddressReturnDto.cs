using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UserAddressReturnDto
    {
        public Guid id { get; set; }

        public string Name { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }

        public int Pincode { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Type { get; set; }

        public long Phone { get; set; }
    }
}
