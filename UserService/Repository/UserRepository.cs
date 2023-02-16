using Contracts.IRepository;
using Entities;
using Entities.Dto;
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _dbcontext;
        public UserRepository(DatabaseContext databaseContext)
        {
            _dbcontext = databaseContext;
        }

        public User GetAddressBookByEmailandPhone(string email, long phone)
        {
            return _dbcontext.Users.SingleOrDefault(user => user.Email == email && user.Phone == phone);
        }

        //Get user by username
        public User GetUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName) + " was null in GetUser from repository");

            return _dbcontext.Users.SingleOrDefault(user => user.Email == userName);
        }

        /// <summary>
        /// Gets the complete user account details for the given id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User GetUserDetails(Guid userId)
        {
            return _dbcontext.Users.Where(a => a.Id == userId && a.IsActive == true).SingleOrDefault();
        }

        /// <summary>
        /// Gets all the delivery addreses of the user
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<Address> GetAllAddress(Guid userId)
        {
            return _dbcontext.Addresses.Where(a => a.UserId == userId && a.IsActive == true).ToList();
        }

        /// <summary>
        /// Gets all the delivery addreses of the user
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<Payment> GetAllPayment(Guid userId)
        {
            return _dbcontext.Payments.Where(a => a.UserId == userId && a.IsActive == true).ToList();
        }

        public List<User> GetAllUserDetails()
        {
            return _dbcontext.Users.Where(a => a.IsActive == true).ToList();
        }

        //Get user by user id
        public User GetUser(Guid userId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId) + " was null in GetUser from repository");

            return _dbcontext.Users.SingleOrDefault(user => user.Id == userId);
        }

        public string GetAccess(Guid userId)
        {
            return _dbcontext.Users.Where(a => a.Id == userId).Select(a => a.Role).SingleOrDefault();
        }

        public void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("User data was null in Deleteuser from repository");

            _dbcontext.Users.Update(user);
        }

        public void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("User data was null in Deleteuser from repository");

            _dbcontext.Users.Remove(user);
        }

        public void CreateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("User data was null in CreateUser from repository");

            _dbcontext.Users.Add(user);
        }

        public void CreateUser(UserDto UserData)
        {
            var addressBook = new User
            {
                FirstName = UserData.FirstName,
                LastName = UserData.LastName,
                Email = UserData.Email,
                Password= UserData.Password,
                Phone= UserData.Phone,
                Role = UserData.role
              
            };

            _dbcontext.Addresses.AddRange(UserData.Addresses);
        }
        public void Save()
        {
            _dbcontext.SaveChanges();
        }

        public bool CheckAddress(Guid userid, Guid addressid)
        {
            return _dbcontext.Addresses.Any(a => a.Id == addressid && a.UserId == userid);
        }

        public bool CheckPayment(Guid userid, Guid paymentid)
        {
            return _dbcontext.Payments.Any(a => a.Id == paymentid && a.UserId == userid);
        }
    }
}
