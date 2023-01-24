using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Response
{
    public class AddressResponse : MessageResponse
    {
        public Address Addresses { get; protected set; }

        public AddressResponse(bool isSuccess, string message, Address addresses) : base(isSuccess, message)
        {
            Addresses = addresses;
        }
    }
}
