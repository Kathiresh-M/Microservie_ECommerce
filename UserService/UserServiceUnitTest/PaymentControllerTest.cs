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
using Entities.Model;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using Entities.RequestDto;
using Entities.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UserServiceUnitTest
{
    public class PaymentControllerTest
    {
        private readonly Mock<ILogger<PaymentController>> _logger;
        private readonly PaymentController _controller;
        private readonly IMapper _mapper;
        private readonly IUserPaymentService _userPaymentService;
        private readonly IUserPaymentRepository _userPaymentRepository;
        private readonly IUserRepository _userRepository;

        public PaymentControllerTest()
        {
            _logger = new Mock<ILogger<PaymentController>>();
            _userPaymentRepository = new UserPaymentRepository(GetDbContext());
            _userRepository = new UserRepository(GetDbContext());
            _userPaymentService = new UserPaymentService(_userRepository, _userPaymentRepository);
            _controller = new PaymentController(_userPaymentService, _mapper);

            _mapper = new Mapper(new MapperConfiguration(map =>
            {
                map.CreateMap<CreateUserDto, UserDto>();

                map.CreateMap<CreateAddressDto, Address>();

                map.CreateMap<User, UserReturnDto>();

                map.CreateMap<User, UserResponseDto>();

                map.CreateMap<CreatePaymentDto, Payment>();

                map.CreateMap<Address, UserAddressReturnDto>();

                map.CreateMap<Payment, UserPaymentReturnDto>();

            }));

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
        /// To test Create Payment by using user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateAddress_Valid_ReturnCreatedStatus()
        {
            Guid id = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");

            CreatePaymentDto createaddress = new CreatePaymentDto()
            {
                Type = "Credit",
                Name = "kathiresh",
                PaymentValue = "1234567891234587",
                Expiry = "12-2022"
            };

            var result = _controller.AddPayment(id, createaddress);

            Assert.IsType<CreatedResult>(result);
        }

        /// <summary>
        /// To test Create Payment by using user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void CreateAddress_Valid_ReturnNotFoundStatus()
        {
            Guid id = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed8");

            CreatePaymentDto createaddress = new CreatePaymentDto()
            {
                Type = "Credit",
                Name = "kathiresh",
                PaymentValue = "1234567891234587",
                Expiry = "12-2022"
            };

            var result = _controller.AddPayment(id, createaddress);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("user not found", finalresult.Value);
        }

        /// <summary>
        /// To test Get an Address by using address id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetAnPayment_Valid_ReturnOkStatus()
        {
            Guid paymentId = Guid.Parse("212877aa-a119-440b-838f-d67ef79fe664");

            var result = _controller.GetPaymentByPaymentId(paymentId);

            Assert.IsType<ObjectResult>(result);
        }

        /// <summary>
        /// To test Get an Address by using address id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void GetAnPayment_Valid_ReturnNotFoundStatus()
        {
            Guid paymentid = Guid.Parse("212877aa-a119-440b-838f-d67ef79fe665");

            var result = _controller.GetPaymentByPaymentId(paymentid);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("payment not found", finalresult.Value);
        }

        


        /// <summary>
        /// To test, Update Payment by using user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateAddress_Valid_ReturnOkStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");
            Guid id = Guid.Parse("212877aa-a119-440b-838f-d67ef79fe664");

            PaymentUpdateDto createaddress = new PaymentUpdateDto()
            {
                Type = "Credit",
                Name = "kathiresh",
                PaymentValue = "1234567891234587",
                Expiry = "12-2022"
            };

            var result = _controller.UpdatePaymentDetails(userId, id, createaddress);

            var finalresult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Payment details Updated Successfully", finalresult.Value);
        }

        /// <summary>
        /// To test, Update Payment by using user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateAddress_InValidUserId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed8");
            Guid id = Guid.Parse("212877aa-a119-440b-838f-d67ef79fe664");

            PaymentUpdateDto createaddress = new PaymentUpdateDto()
            {
                Type = "Credit",
                Name = "kathiresh",
                PaymentValue = "1234567891234587",
                Expiry = "12-2022"
            };

            var result = _controller.UpdatePaymentDetails(userId, id, createaddress);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("user not found", finalresult.Value);
        }

        /// <summary>
        /// To test, Update Payment by using user id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void UpdateAddress_InValidPaymentId_ReturnNotFoundStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");
            Guid id = Guid.Parse("212877aa-a119-440b-838f-d67ef79fe665");

            PaymentUpdateDto createaddress = new PaymentUpdateDto()
            {
                Type = "Credit",
                Name = "kathiresh",
                PaymentValue = "1234567891234587",
                Expiry = "12-2022"
            };

            var result = _controller.UpdatePaymentDetails(userId, id, createaddress);

            var finalresult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("payment not found", finalresult.Value);
        }

        /// <summary>
        /// To Test Delete Payment by using PaymentId
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeletePayment_ValidPaymentId_ReturnOkStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");
            Guid id = Guid.Parse("212877aa-a119-440b-838f-d67ef79fe664");

            IActionResult result = _controller.DeletePaymentDetails(userId, id);

            var finaldata = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("Deleted Successsully" + id, finaldata.Value);
        }

        /// <summary>
        /// To Test Delete Payment by using PaymentId
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void DeletePayment_InValidPaymentId_ReturnOkStatus()
        {
            Guid userId = Guid.Parse("1f7bd91d-5443-46c8-8481-84d0e1c69ed7");
            Guid id = Guid.Parse("212877aa-a119-440b-838f-d67ef79fe665");

            IActionResult result = _controller.DeletePaymentDetails(userId, id);

            var finaldata = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal("payment not found", finaldata.Value);
        }
    }
}
