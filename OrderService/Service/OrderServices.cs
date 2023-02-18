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

            Guid userid = createProductDto.UserId;
            Guid addressid = createProductDto.AddressId;
            Guid paymentid = createProductDto.PaymentId;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            bool checkId = client.GetFromJsonAsync<bool>($"https://localhost:7070/api/checkid?userid={userid}&addressid={addressid}&paymentid={paymentid}").Result;

            if (checkId == false)
                return new OrderResponse(false, "Id not found", null);

            var context = new StringContent("", Encoding.UTF8, "text/plain");

            var check = client.PatchAsJsonAsync($"https://localhost:7291/api/order/{userid}",context).Result;

            if (check == null)
                return new OrderResponse(false,"Not Exist",null);

            if (check.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new OrderResponse(false, "Not found", null);

            if (check.StatusCode == System.Net.HttpStatusCode.NoContent)
                return new OrderResponse(false, "No Content", null);

            return new OrderResponse(true, "Products are purchased successfully", null);
        }
    }
}
