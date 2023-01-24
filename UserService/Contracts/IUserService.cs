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
    public interface IUserService
    {
        UserResponse CreateUser(CreateUserDto userdata);
        TokenResponse AuthUser(UserLoginDto userData);
        string Checkuserdata(CreateUserDto userdata);
        List<UserReturnDto> GetAllUserAccount();
        UserResponseDetails GetUserDetailsById(Guid userId, Guid tokenUserId);
        UserResponse UpdateUser(Guid userId, Guid tokenUserId, UserUpdateDto userData);
        UserResponse DeleteUser(Guid userId, Guid tokenUserId);
    }
}
