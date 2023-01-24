using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IRepository
{
    public interface IUserAddressRepository
    {
        Address GetAddressByUserId(Guid UserId);
        void CreateAddress(Address address);
        List<Address> GetAllAddressDetails();
        void UpdateAddressdata(Address address);
        Address GetAddress(Guid addressid);
        void DeleteAddress(Address address);
        void Save();
    }
}
