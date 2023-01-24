using Entities.Dto;
using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Response
{
    public class UserResponseDetails : MessageResponse
    {
        public UserResponseDto Userdto { get; protected set; }
        public UserResponseDetails(bool isSuccess, string message, UserResponseDto user) : base(isSuccess, message)
        {
            this.Userdto = user;
        }
    }
}
