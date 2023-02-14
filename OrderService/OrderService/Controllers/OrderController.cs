using AutoMapper;
using Contract;
using Contract.Responses;
using Entities.RequestDto;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Data;

namespace OrderService.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public OrderController(IMapper mapper, IOrderService orderService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(OrderController));
        }

        /// <Summary>
        /// Process to payment
        /// </Summary>
        /// <param name="adminid"></param>
        [Authorize]
        [HttpPost]
        [Route("api/payment")]
        public IActionResult ProductPayment([FromBody] PaymentDto payment )
        {
            _log.Info("Get in to Payment Process");

            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer", "");

                var checkGivenDetails = _orderService.CheckDetails(payment, token);

                if (!checkGivenDetails.IsSuccess && checkGivenDetails.Message.Contains("found"))
                    return NotFound(checkGivenDetails.Message);

                return Ok();
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }
    }
}
