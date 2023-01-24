using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public long Phone { get; set; }

        public string role { get; set; }

        public IEnumerable<Address> Addresses { get; set; }

        public IEnumerable<Payment> Payments { get; set; }
    }
}
