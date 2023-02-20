using AutoMapper;
using Contracts.IRepository;
using Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Controllers;
using Entities.Profiles;
using Repository;
using Services;
using Entities.Model;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Entities.RequestDto;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UserServiceUnitTest
{
    public class AddressControllerTest
    {
        private readonly Mock<ILogger<AddressController>> _logger;
        private readonly AddressController _controller;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IUserAddressService _userAddressService;

        public AddressControllerTest()
        {
            MapperConfiguration mapconfig = new MapperConfiguration(map =>
            {
                map.AddProfile(new UserProfiles());
            });
            IMapper mapper = mapconfig.CreateMapper();
            _mapper = mapper;
            _logger = new Mock<ILogger<AddressController>>();
            var dbcontext = GetDbContext();
            _userAddressRepository = new UserAddressRepository(dbcontext);
            _userRepository = new UserRepository(dbcontext);
            _userAddressService = new UserAddressService(_userAddressRepository, _userRepository, _mapper);
            _controller = new AddressController(_userAddressService, _mapper);

            string userId = "1f7bd91d-5443-46c8-8481-84d0e1c69ed7";
            Mock<HttpContext> contextMock = new Mock<HttpContext>();
            contextMock.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("user_id",userId)
                                        // other required and custom claims
                           }, "TestAuthentication")));
            _controller.ControllerContext.HttpContext = contextMock.Object;
        }

        public DatabaseContext GetDbContext()
        {
            var option = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var context = new DatabaseContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            User user = new User
            {
                Id = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7"),
                FirstName = "Rampra",
                LastName = "R",
                Email = "ramprash@gmail.com",
                Password = "Rampra@123",
                Phone = 1234567984,
                Role = "Customer",
                DateCreated = DateTime.Now
            };

            context.Users.Add(user);

            Address address = new Address
            {
                Id = Guid.Parse("aafae32c-d5dd-43c6-b398-b0026903bbfc"),
                Name = "kathir",
                Line1 = "12/12 Ram Nagar",
                Line2 = "5th street",
                City = "Cbe",
                Pincode = 123456,
                State = "TN",
                Country = "India",
                Type = "Home",
                Phone = 6516165165,
                UserId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7"),
                DateCreated = DateTime.Now
            };

            context.Addresses.Add(address);

            Payment payment = new Payment
            {
                Id = Guid.Parse("212877aa-a119-440b-838f-d67ef79fe664"),
                Type = "Credit",
                Name = "kathir",
                PaymentValue = "1234567891234567",
                Expiry = "12-2023",
                UserId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7"),
                IsActive = true,
                DateCreated = DateTime.Now
            };

            context.Payments.Add(payment);
            context.SaveChanges();
            return context;
        }

        /// <summary>
        /// To test Create Address by using user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateAddress_Valid_ReturnCreatedStatus()
        {
            Guid id = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");

            CreateAddressDto createaddress = new CreateAddressDto()
            {
                Name = "kathiresh",
                Line1 = "12/12 Sai Nagar",
                Line2 = "6th street",
                City = "Erode",
                Pincode = 987654,
                State = "TN",
                Country = "India",
                Type = "Home",
                Phone = 6516165265
            };

            var result = _controller.AddAddress(id, createaddress);

            Assert.IsType<CreatedResult>(result);
        }

        /// <summary>
        /// To test Create Address by using user id (Not Found Exception)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateAddress_Valid_ReturnNotFoundStatus()
        {
            Guid id = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed8");

            CreateAddressDto createaddress = new CreateAddressDto()
            {
                Name = "kathiresh",
                Line1 = "12/12 Sai Nagar",
                Line2 = "6th street",
                City = "Erode",
                Pincode = 987654,
                State = "TN",
                Country = "India",
                Type = "Home",
                Phone = 6516165265
            };

            var result = _controller.AddAddress(id, createaddress);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("user not found", finalresult.Value);
        }

        /// <summary>
        /// To test Get an Address by using address id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetAnAddress_Valid_ReturnOkStatus()
        {
            Guid addressId = Guid.Parse("aafae32c-d5dd-43c6-b398-b0026903bbfc");

            var result = _controller.GetAddressByAddressId(addressId);

            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// To test Get an Address by using address id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetAnAddress_Valid_ReturnAddressNotFoundStatus()
        {
            Guid addressId = Guid.Parse("aafae32c-d5dd-43c6-b398-b0026903bbf8");

            var result = _controller.GetAddressByAddressId(addressId);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Address not found", finalresult.Value);
        }

        /// <summary>
        /// To test, Update Address by using user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateAddress_Valid_ReturnOkStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");
            Guid id = Guid.Parse("aafae32c-d5dd-43c6-b398-b0026903bbfc");

            AddressUpdateDto createaddress = new AddressUpdateDto()
            {
                Name = "kathiresh",
                Line1 = "12/12 Sai Nagar",
                Line2 = "6th street",
                City = "Erode",
                Pincode = 987654,
                State = "TN",
                Country = "India",
                Type = "Home",
                Phone = 6516165265
            };

            var result = _controller.UpdateAddress(userId, id, createaddress);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Address Updated Successfully", finalresult.Value);
        }

        /// <summary>
        /// Update Address by using user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateAddress_InValidUserId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed8");
            Guid id = Guid.Parse("aafae32c-d5dd-43c6-b398-b0026903bbfc");

            AddressUpdateDto createaddress = new AddressUpdateDto()
            {
                Name = "kathiresh",
                Line1 = "12/12 Sai Nagar",
                Line2 = "6th street",
                City = "Erode",
                Pincode = 987654,
                State = "TN",
                Country = "India",
                Type = "Home",
                Phone = 6516165265
            };

            var result = _controller.UpdateAddress(userId, id, createaddress);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("user not found", finalresult.Value);
        }

        /// <summary>
        /// To Test Delete Address by using AddressId
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteAddress_CheckAddressBookId_ReturnOkStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");
            Guid id = Guid.Parse("aafae32c-d5dd-43c6-b398-b0026903bbfc");

            IActionResult result = _controller.DeleteAddress(userId, id);

            var finaldata = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("Deleted Successsully" + id, finaldata.Value);
        }

        /// <summary>
        /// To Test Delete Address by using AddressId
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteAddress_InValidUserId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed8");
            Guid id = Guid.Parse("aafae32c-d5dd-43c6-b398-b0026903bbfc");

            IActionResult result = _controller.DeleteAddress(userId, id);

            var finaldata = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal("User not found", finaldata.Value);
        }

        /// <summary>
        /// To Test Delete Address by using AddressId
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteAddress_InValidAddressId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");
            Guid id = Guid.Parse("aafae32c-d5dd-43c6-b398-b0026903bbff");

            IActionResult result = _controller.DeleteAddress(userId, id);

            var finaldata = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal("Address not found", finaldata.Value);
        }
    }
}
