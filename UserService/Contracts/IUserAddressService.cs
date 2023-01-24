using Contracts.Response;
using Entities.Dto;
using Entities.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserAddressService
    {
        AddressResponse AddAddress(Guid userid, Guid tokenid, CreateAddressDto addressdto);
        AddressResponse GetAddressByAddressId(Guid userId, Guid tokenUserId);
        AddressResponse UpdateAddress(Guid userid, Guid addressid, Guid tokenUserId, AddressUpdateDto addressupdatedto);
        AddressResponse DeleteAddress(Guid userId, Guid tokenUserId, Guid addressid);
    }
}
