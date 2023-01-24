using AutoMapper;
using Contracts;
using Contracts.Response;
using Entities.Dto;
using Entities.Helper;
using Entities.Model;
using Entities.RequestDto;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserService.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILog _log;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="userService">Communication between respository and controller</param>
        /// <param name="mapper">used to map dto</param>
        /// <returns></returns>
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(UserController));
        }

        /// <summary>
        /// Create a New User as a Admin/Customer
        /// </summary>
        /// <param name="userdata"></param>
        /// <response code="201">User Id</response>
        /// <response code="409">User is already exist</response>
        [HttpPost]
        [Route("api/createuser")]
        public IActionResult CreateUser([FromBody] CreateUserDto userdata)
        {
            _log.Info("Create User ");

            try
            {
                string userdatacheck = _userService.Checkuserdata(userdata);

                if(userdatacheck == "badrequest")
                {
                    return BadRequest("User Data is Invalid, please check and give correct Details");
                }

                UserResponse response = _userService.CreateUser(userdata);

                if (!response.IsSuccess && response.Message.Contains("already exists"))
                {
                    _log.Debug("Data Conflict");
                    return Conflict("Error messages like email address already exists and invalid value for a field");
                }

                _log.Info("User Data  was created" + response.Userdto.Id);

                return Created("User created with ID: ", response.Userdto.Id);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please correct the Data and try again");
                return StatusCode(StatusCodes.Status500InternalServerError, exception);
            }
        }

        /// <summary>
        /// Get User authentication and return token
        /// </summary>
        /// <param name="userlogin"></param>
        /// <response code="200">Bearer token</response>
        /// <response code="401">User was Unauthorized</response>
        [HttpPost]
        [Route("api/userauth")]
        public IActionResult Authentication([FromBody] UserLoginDto userlogin)
        {
            _log.Info("Get Authorization");

            if (!ModelState.IsValid)
            {
                _log.Debug("Invalid login details used.");
                return BadRequest("Enter valid user data");
            }

            try
            {
                TokenResponse response = _userService.AuthUser(userlogin);

                if (response.AccessToken == null && response.TokenType == null)
                {
                    return Unauthorized("Please check username and Password");
                }
                _log.Info(userlogin.UserName + " user logged in.");

                var token = new Token(response.AccessToken, response.TokenType);

                return Ok(token);
            }
            catch (Exception ex)
            {
                _log.Error("Something went wrong", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get the complete user details by user id
        /// </summary>
        /// <param name="userid"></param>
        /// <response code="200">get all user details Dto</response>
        /// <response code="404">User Not Found</response>
        [HttpGet]
        [Authorize]
        [Route("api/user/GetAllUserDetails/{userid}")]
        public IActionResult GetAllUserDetailsById(Guid userid)
        {
            _log.Info("Get all User Details by using User Id");

            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            try
            {
                UserResponseDetails response = _userService.GetUserDetailsById(userid, tokenUserId);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }
                _log.Info("Successfully get all user details");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.Error("Something went wrong", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Admin can get user details
        /// </summary>
        /// <response code="200">get user details dto</response>
        /// <response code="204">Admin id not found</response>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/user")]
        public IActionResult GetAllUserAccount()
        {
            _log.Info("Admin can Get all User Details");

            try
            {
                List<UserReturnDto> userAccounts = _userService.GetAllUserAccount();

                if (userAccounts.Count == 0)
                {
                    return NoContent();
                }

                return Ok(userAccounts);
            }
            catch (Exception ex)
            {
                _log.Error("Something went wrong", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update user details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userData"></param>
        /// <response code="200">Updated details dto</response>
        /// <response code="204">User not found</response>
        [HttpPut]
        [Authorize]
        [Route("api/user/{userId}")]
        public IActionResult UpdateUser(Guid userId, [FromBody] UserUpdateDto userData)
        {
            _log.Info("Update User Details");

            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (userId == null || userId == Guid.Empty)
            {
                _log.Debug("Trying to update user data with not a valid user id by user: " + tokenUserId);
                return BadRequest("Not a valid user ID.");
            }

            try
            {
                UserResponse response = _userService.UpdateUser(userId, tokenUserId, userData);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }

                var user = _mapper.Map<UserReturnDto>(response.Userdto);

                return Ok(user);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete User Details
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">User Id</response>
        /// <response code="204">User not found</response>
        [HttpDelete]
        [Authorize]
        [Route("api/user/{id}")]
        public IActionResult DeleteUser(Guid userId)
        {
            _log.Info("Delete User");

            Guid tokenUserId;
            var isValidToken = Guid.TryParse(User.FindFirstValue("user_id"), out tokenUserId);

            if (userId == null || userId == Guid.Empty)
            {
                _log.Debug("Trying to delete user data with not a valid user ID by user Id: " + tokenUserId);
                return BadRequest("Not a valid user ID.");
            }

            try
            {
                UserResponse response = _userService.DeleteUser(userId, tokenUserId);

                if (!response.IsSuccess)
                {
                    return NotFound(response.Message);
                }

                _log.Info($"User with ID: {response.Userdto.Id}, deleted successfully.");

                return Ok("Deleted Successsully " + userId);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
