using AutoMapper;
using Entities.Dto;
using Entities.Model;
using Entities.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Profiles
{
    public class ProductProfiles : Profile
    {
        public ProductProfiles()
        {
            CreateMap<CreateProductDto, ProductModel>();
            CreateMap<ProductModel, ProductReturnDto>().ReverseMap();
            CreateMap<Cart, CartReturnDto>();
        }
    }
}
