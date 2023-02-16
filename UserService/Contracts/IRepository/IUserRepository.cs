using Entities.Dto;
using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IRepository
{
    public interface IUserRepository
    {
        User GetAddressBookByEmailandPhone(string email, long phone);
        void CreateUser(User user);
        void CreateUser(UserDto UserData);
        User GetUser(string userName);
        string GetAccess(Guid userId);
        List<Address> GetAllAddress(Guid userId);
        List<Payment> GetAllPayment(Guid userId);
        User GetUserDetails(Guid userId);
        User GetUser(Guid userId);
        List<User> GetAllUserDetails();
        void UpdateUser(User user);
        void DeleteUser(User user);
        void Save();

        bool CheckAddress(Guid userid, Guid addressid);
        bool CheckPayment(Guid userid, Guid paymentid);
    }
}
