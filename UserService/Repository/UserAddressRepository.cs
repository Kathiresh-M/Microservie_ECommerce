using Contracts.IRepository;
using Entities;
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly DatabaseContext _dbcontext;
        public UserAddressRepository(DatabaseContext databaseContext)
        {
            _dbcontext = databaseContext;
        }

        public Address GetAddressByUserId(Guid UserId)
        {
            if (UserId == null || UserId == Guid.Empty)
                throw new ArgumentNullException(nameof(UserId) + " was null in GetAddress from UserAddressRepository");

            return _dbcontext.Addresses.SingleOrDefault(a => a.UserId == UserId);
        }

        public void CreateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("User data was null in CreateAddress from UserAddressRepository");

            _dbcontext.Addresses.Add(address);
        }

        public List<Address> GetAllAddressDetails()
        {
            return _dbcontext.Addresses.Where(a => a.IsActive == true).ToList();
        }

        public void UpdateAddressdata(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("User data was null in UpdateAddressdata from UserAddressRepository");

            _dbcontext.Addresses.Update(address);
        }

        public Address GetAddress(Guid addressid)
        {
            return _dbcontext.Addresses.SingleOrDefault(user => user.Id == addressid);
        }

        public void DeleteAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("Address data was null in DeleteAddress from UserAddressRepository");

            _dbcontext.Addresses.Remove(address);
        }

        public void Save()
        {
            _dbcontext.SaveChanges();
        }
    }
}
