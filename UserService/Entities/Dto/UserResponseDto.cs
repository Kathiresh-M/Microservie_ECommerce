using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public string Role { get; set; }
        public ICollection<UserAddressReturnDto> Addresses { get; set; } = new List<UserAddressReturnDto>();
        public ICollection<UserPaymentReturnDto> Payments { get; set; } = new List<UserPaymentReturnDto>();
    }
}
