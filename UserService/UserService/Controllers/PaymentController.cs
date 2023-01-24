using AutoMapper;
using Contracts;
using Contracts.Response;
using Entities.Dto;
using Entities.RequestDto;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;

namespace UserService.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IUserPaymentService _userPaymentService;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="userPaymentService">Communication between respository and controller</param>
        /// <param name="mapper">used to map dto</param>
        /// <returns></returns>
        public PaymentController(IUserPaymentService userPaymentService, IMapper mapper)
        {
            _userPaymentService = userPaymentService;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(PaymentController));
        }

        /// <summary>
        /// Create payment details
        /// </summary>
        /// <param name="Userid">User Id</param>
        /// <param name="paymentdto">New Payment details</param>
        /// <response code="201">Payment Id</response>
        /// <response code="404">user not found</response>
        /// <response code="409">User is already exist</response>
        [Authorize]
        [HttpPost]
        [Route("api/payment/{userid}")]
        public IActionResult AddPayment(Guid Userid, [FromBody] CreatePaymentDto paymentdto)
        {
            _log.Info("Create payment details by using user id");

            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (!isValidToken)
            {
                _log.Debug("User with invalid token");
                return Unauthorized();
            }

            if (Userid == null || Userid == Guid.Empty)
            {
                _log.Debug("Trying to add payment details with not a valid User Id by user: " + tokenUserId);
                return BadRequest("Not a valid payment details.");
            }

            try
            {
                PaymentResponse response = _userPaymentService.AddPaymentDetails(Userid, tokenUserId, paymentdto);

                if (!response.IsSuccess && response.Message.Contains("not found"))
                {
                    return NotFound(response.Message);
                }

                if (!response.IsSuccess && response.Message.Contains("exists"))
                {
                    return Conflict(response.Message);
                }

                return Created("Payment created with ID: ", response.Payment.Id);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }

        /// <summary>
        /// Get payment details By Using Payment Id
        /// </summary>
        /// <param name="paymentid">User Id</param>
        /// <response code="200">payment details dto</response>
        /// <response code="404">User Not Found</response>
        [Authorize]
        [HttpGet]
        [Route("api/payment/{paymentid}")]
        public IActionResult GetPaymentByPaymentId(Guid paymentid)
        {
            _log.Info("Get Payment by using payment Id");
            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (!isValidToken)
            {
                _log.Debug("User with invalid token, trying to Get payment details");
                return Unauthorized();
            }
            try
            {
                PaymentResponse response = _userPaymentService.GetPaymentByPaymentId(paymentid, tokenUserId);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }

                _log.Info("Payment with ID: " + paymentid + ", viewed the data.");
                var user = _mapper.Map<UserPaymentReturnDto>(response.Payment);

                return Ok(user);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }

        }

        /// <summary>
        /// Update payment details
        /// </summary>
        /// <param name="Userid">User Id</param>
        /// <param name="Paymentid">Payment Id</param>
        /// <param name="paymentupdatedto">Update Payment Data</param>
        /// <response code="200">Payment details Updated Successfully</response>
        /// <response code="404">User Not Found</response>
        [Authorize]
        [HttpPut]
        [Route("api/payment/{userid}/{paymentid}")]
        public IActionResult UpdatePaymentDetails(Guid Userid, Guid Paymentid, [FromBody] PaymentUpdateDto paymentupdatedto)
        {

            _log.Info("Update payment Data by using user Id");
            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (!isValidToken)
            {
                _log.Debug("User with invalid token, trying to update payment data");
                return Unauthorized();
            }

            try
            {
                PaymentResponse response = _userPaymentService.UpdatePayment(Userid, Paymentid, tokenUserId, paymentupdatedto);

                if (!response.IsSuccess && response.Message.Contains("not found"))
                {
                    return NotFound(response.Message);
                }

                _log.Info("Payment details Updated Successfully");

                return Ok("Payment details Updated Successfully");
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }

        /// <summary>
        /// Delete payment details by using payment id
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="paymentid"></param>
        /// <response code="200">Delete Successfully with deleted Payment Id</response>
        /// <response code="404">Payment Not Found</response>
        [Authorize]
        [HttpDelete]
        [Route("api/payment/{paymentid}")]
        public IActionResult DeletePaymentDetails(Guid userid, Guid paymentid)
        {
            _log.Info("Delete Payment details by using payment id");
            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (!isValidToken)
            {
                _log.Warn("User with invalid token, trying to upload user data");
                return Unauthorized();
            }

            if (userid == null || userid == Guid.Empty || paymentid == null || paymentid == Guid.Empty)
            {
                _log.Error("Trying to delete payment data with not a valid Id by user: " + tokenUserId);
                return BadRequest("Not a valid payment.");
            }

            try
            {
                PaymentResponse response = _userPaymentService.DeletePaymentDetails(userid, tokenUserId, paymentid);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }

                _log.Info($"Payment Id is deleted successfully.");

                return Ok("Deleted Successsully" + paymentid);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }
    }
}
