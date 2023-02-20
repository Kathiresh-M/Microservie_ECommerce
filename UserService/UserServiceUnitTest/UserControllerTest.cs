using AutoMapper;
using Castle.Core.Configuration;
using Contracts;
using Contracts.IRepository;
using Entities.Model;
using Entities;
using Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Repository;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Controllers;
using Entities.RequestDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UserServiceUnitTest
{
    public class UserControllerTest
    {
        private readonly Mock<ILogger<UserController>> _logger;
        private readonly UserController _controller;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public UserControllerTest()
        {
            Microsoft.Extensions.Configuration.IConfiguration config = new ConfigurationBuilder().Build();
            _config = config;
            MapperConfiguration mapconfig = new MapperConfiguration(map =>
            {
                map.AddProfile(new UserProfiles());
            });
            IMapper mapper = mapconfig.CreateMapper();
            _mapper = mapper;
            _logger = new Mock<ILogger<UserController>>();
            _userRepository = new UserRepository(GetDbContext());
            _jwtService = new JWTService(_config);
            _userService = new UserServices(_userRepository, _mapper, _jwtService);
            _controller = new UserController(_userService, _mapper);

            string userId = "97088ab0-6c00-432d-81ce-87828a674b55";
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

            context.Users.AddRange(new User
            {
                Id = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55"),
                FirstName = "Kathiresh",
                LastName = "M",
                Email = "Kathiresh@gmail.com",
                Password = "Kathiresh@123",
                Phone = 1234567984,
                Role = "Customer",
                DateCreated = DateTime.Now
            },
            new User
            {
                Id = Guid.Parse("3039a302-5cdd-48a7-b112-7b3d32b8b48d"),
                FirstName = "Kathir",
                LastName = "M",
                Email = "Kathir@gmail.com",
                Password = "Kathir@123",
                Phone = 1234567984,
                Role = "Admin",
                DateCreated = DateTime.Now
            });

            //context.Users.Add(user);

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
        /// Create a new User
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateUser_Valid_ReturnCreatedStatus()
        {

            CreateUserDto createuser = new CreateUserDto()
            {
                FirstName = "Sam",
                LastName = "S",
                Email = "sam@gmail.com",
                Password = "Sam@12345",
                Phone = 1569874562,
                Role = "Customer"
            };

            var result = _controller.CreateUser(createuser);

            Assert.IsType<CreatedResult>(result);
        }

        /// <summary>
        /// Create a new User with already exist data
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateUser_ValidAlreadyExistEmailandPhone_ReturnConflictStatus()
        {

            CreateUserDto createuser = new CreateUserDto()
            {
                FirstName = "Kathiresh",
                LastName = "M",
                Email = "Kathiresh@gmail.com",
                Password = "Kathir@123",
                Phone = 1234567984,
                Role = "Customer",
            };

            var result = _controller.CreateUser(createuser);

            Assert.IsType<ConflictObjectResult>(result);
        }

        /// <summary>
        /// Get user details by Id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetUserDetails_Valid_ReturnOkStatus()
        {

            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            var result = _controller.GetAllUserDetailsById(userId);

            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// Get user details by Id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetUserDetails_InValidUserId_ReturnOkStatus()
        {

            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b54");

            var result = _controller.GetAllUserDetailsById(userId);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", finalresult.Value);
        }

        /// <summary>
        /// Get user details by Id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetUserbyadmin_Valid_ReturnOkStatus()
        {

            Guid userId = Guid.Parse("3039a302-5cdd-48a7-b112-7b3d32b8b48d");

            var result = _controller.GetAllUserAccount();

            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// Update user Details
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateUser_Valid_ReturnOkStatus()
        {

            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");
            UserUpdateDto userupdate = new UserUpdateDto()
            {
                FirstName = "Kathiresh",
                LastName = "M",
                Email = "Kathiresh@gmail.com",
                Password = "Kathiresh@123",
                Phone = 1234567984,
                Role = "Customer",
            };

            var result = _controller.UpdateUser(userId, userupdate);

            Assert.IsType<OkObjectResult>(result);
        }

        // <summary>
        /// Update user Details
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateUser_InValidUserId_ReturnOkStatus()
        {

            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b54");
            UserUpdateDto userupdate = new UserUpdateDto()
            {
                FirstName = "Kathiresh",
                LastName = "M",
                Email = "Kathiresh@gmail.com",
                Password = "Kathiresh@123",
                Phone = 1234567984,
                Role = "Customer",
            };

            var result = _controller.UpdateUser(userId, userupdate);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", finalresult.Value);
        }

        /// <summary>
        /// Delete user Details
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteUser_Valid_ReturnOkStatus()
        {

            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b55");

            var result = _controller.DeleteUser(userId);

            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// Delete user Details
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeleteUser_InValidUserId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("97088ab0-6c00-432d-81ce-87828a674b54");

            var result = _controller.DeleteUser(userId);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", finalresult.Value);
        }
    }
}
