using Entities.Dto;
using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Response
{
    public class UserResponse : MessageResponse
    {
        public User Userdto { get; protected set; }
        public UserResponse(bool isSuccess, string message, User user) : base(isSuccess, message)
        {
            this.Userdto = user;  
        }
    }
}
