using AutoMapper;
using Contracts;
using Contracts.Response;
using Entities.Dto;
using Entities.Model;
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
    public class AddressController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="userAddressService">Communication between respository and controller</param>
        /// <param name="mapper">used to map dto</param>
        /// <returns></returns>
        public AddressController(IUserAddressService userAddressService, IMapper mapper)
        {
            _userAddressService = userAddressService;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(UserController));
        }

        /// <summary>
        /// Create new address under user id
        /// </summary>
        /// <param name="Userid">User Id</param>
        /// <param name="addressdto">New Address Data</param>
        /// <response code="200">Guid Address Id</response>
        /// <response code="404">User Not Found</response>
        /// <response code="409">Address already exists</response>
        [Authorize]
        [HttpPost]
        [Route("api/Address/{userid}")]
        public IActionResult AddAddress(Guid Userid, [FromBody] CreateAddressDto addressdto)
        {
            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (!isValidToken)
            {
                _log.Warn("User with invalid token, trying to upload user data");
                return Unauthorized();
            }

            if (Userid == null || Userid == Guid.Empty)
            {
                _log.Error("Trying to update address data with not a valid User Id by user: " + tokenUserId);
                return BadRequest("Not a valid address.");
            }
            try
            {
                AddressResponse response = _userAddressService.AddAddress(Userid, tokenUserId, addressdto);

                if (!response.IsSuccess && response.Message.Contains("not found"))
                {
                    return NotFound(response.Message);
                }

                if (!response.IsSuccess && response.Message.Contains("exists"))
                {
                    return Conflict(response.Message);
                }

                return Created("Address created with ID: ", response.Addresses.Id);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }

        /// <summary>
        /// Get All Address details By Using Address Id
        /// </summary>
        /// <param name="addressid">User Id</param>
        /// <response code="200">Guid Address Id</response>
        /// <response code="400">Address Not Found</response>
        [Authorize]
        [HttpGet]
        [Route("api/Address/{addressid}")]
        public IActionResult GetAddressByAddressId(Guid addressid)
        {
            _log.Info("Get Address by using address Id");
            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (!isValidToken)
            {
                _log.Debug("User with invalid token, trying to Get address details");
                return Unauthorized();
            }
            try
            {

                AddressResponse response = _userAddressService.GetAddressByAddressId(addressid, tokenUserId);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }

                _log.Info("Address with ID: " + addressid + ", viewed the data.");
                var user = _mapper.Map<UserAddressReturnDto>(response.Addresses);

                return Ok(user);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }

        }

        /// <summary>
        /// Update User details
        /// </summary>
        /// <param name="Userid">User Id</param>
        /// <param name="Addressid">Address Id</param>
        /// <param name="addressupdatedto">Update Address Data</param>
        /// <response code="200">Address Update Successfully</response>
        /// <response code="404">User Not Found</response>
        [Authorize]
        [HttpPut]
        [Route("api/Address/{userid}/{addressid}")]
        public IActionResult UpdateAddress(Guid Userid, Guid Addressid, [FromBody] AddressUpdateDto addressupdatedto)
        {

            _log.Info("Update Address Data by using user Id");
            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (!isValidToken)
            {
                _log.Debug("User with invalid token, trying to update address data");
                return Unauthorized();
            }

            if (Userid == null || Userid == Guid.Empty || Addressid == null || Addressid == Guid.Empty)
            {
                _log.Debug("Trying to update address data with not a valid Id by user: " + tokenUserId);
                return BadRequest("Not a valid address.");
            }

            try
            {
                AddressResponse response = _userAddressService.UpdateAddress(Userid, Addressid, tokenUserId, addressupdatedto);

                if (!response.IsSuccess && response.Message.Contains("not found"))
                {
                    return NotFound(response.Message);
                }

                _log.Info("Address Updated Successfully");

                return Ok("Address Updated Successfully");
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }

        /// <summary>
        /// Delete User details
        /// </summary>
        /// <param name="userid">User Id</param>
        /// <param name="addressid">New Address Data</param>
        /// <response code="200">Delete Successfully with deleted Address Id</response>
        /// <response code="404">Address Not Found</response>
        [Authorize]
        [HttpDelete]
        [Route("api/Address/{userid}/{addressid}")]
        public IActionResult DeleteAddress(Guid userid,Guid addressid)
        {
            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (!isValidToken)
            {
                _log.Warn("User with invalid token, trying to upload user data");
                return Unauthorized();
            }

            if (userid == null || userid == Guid.Empty || addressid == null || addressid == Guid.Empty)
            {
                _log.Error("Trying to delete address data with not a valid Id by user: " + tokenUserId);
                return BadRequest("Not a valid address.");
            }

            try
            {
                AddressResponse response = _userAddressService.DeleteAddress(userid,tokenUserId,addressid);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }

                _log.Info($"Address Id is deleted successfully.");

                return Ok("Deleted Successsully" + addressid);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }
    }
}
