using AutoMapper;
using Contract;
using Contract.Responses;
using Entities.RequestDto;
using Entities.ServiceDto;
using log4net;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class OrderServices : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly ILog _log;
        public OrderServices(IMapper mapper)
        {
            _log = LogManager.GetLogger(typeof(OrderServices));
            _mapper = mapper;
        }

        /// <summary>
        /// Check Payment details
        /// </summary>
        /// <returns></returns>
        public OrderResponse CheckDetails(PaymentDto createProductDto, string token)
        {
            _log.Info("Check given payment details");

            Guid userId = createProductDto.UserId;
            Guid addressId = createProductDto.AddressId;
            Guid paymentId = createProductDto.PaymentId;

            var client = new HttpClient();
           /* client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var checkuserId = client.GetFromJsonAsync<bool>($"https://localhost:7291/api/checkuserid/{userId}").Result;

            if (checkuserId == false)
                return new OrderResponse(false, "User Id not found", null);*/

           /* var checkproductId = client.GetFromJsonAsync<bool>($"https://localhost:7291/api/checkproductid/{productId}").Result;

            if (checkproductId == false)
                return new OrderResponse(false, "Product Id not found", null);*/

            var checkId = client.GetFromJsonAsync<bool>($"https://localhost:7070/api/checkid/{userId}/{addressId}/{paymentId}").Result;

            if (checkId == false)
                return new OrderResponse(false, "Id not found", null);

                var check = client.GetFromJsonAsync<bool>($"https://localhost:7291/api/order/{userId}").Result;

                return new OrderResponse(true, "Products are purchased successfully", null);

            /*var checkPaymentId = client.GetFromJsonAsync<bool>($"https://localhost:7070/api/checkpaymentid/{paymentId}").Result;

            if (checkPaymentId == false)
                return new OrderResponse(false, "Payment Id not found", null);*/

           /* var checkpayemnttype = client.GetFromJsonAsync<bool>($"https://localhost:7070/api/checkpaymenttype/{paymentType}").Result;

            if (checkpayemnttype == false)
                return new OrderResponse(false, "Payment Type was not found",null);*/

           /* var checktotalamount = client.GetFromJsonAsync<bool>($"https://localhost:7070/api/checktotalamount/{productId}/{totalprice}").Result;

            if (checktotalamount == false)
                return new OrderResponse(false, "Given amount is mismatch, please give correct amount", null);*/
        }
    }
}
