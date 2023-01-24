using AutoMapper;
using Entities.Dto;
using Entities.Model;
using Entities.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Profiles
{
    public class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<CreateUserDto,UserDto>();
            CreateMap<CreateAddressDto, Address>();
            CreateMap<User, UserReturnDto>();
            CreateMap<User, UserResponseDto>();
            CreateMap<CreatePaymentDto, Payment>();
            CreateMap<Address, UserAddressReturnDto>();
            CreateMap<Payment, UserPaymentReturnDto>();
        }
    }
}
