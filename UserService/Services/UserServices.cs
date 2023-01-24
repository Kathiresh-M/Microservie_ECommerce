using AutoMapper;
using Contracts;
using Contracts.IRepository;
using Contracts.Response;
using Entities.Dto;
using Entities.Model;
using Entities.RequestDto;
using log4net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private readonly ILog _log;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="userRepository">Communication between respository and controller</param>
        /// <param name="mapper">used to map dto</param>
        /// <param name="jwtService"></param>
        /// <returns></returns>
        public UserServices(IUserRepository userRepository, IMapper mapper, IJWTService jwtService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _log = LogManager.GetLogger(typeof(UserServices));
            _jwtService = jwtService;
        }

        /// <summary>
        /// get authentication token
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>return token</returns>
        public TokenResponse AuthUser(UserLoginDto userData)
        {
            _log.Info("Get user authentication from service layer");
            try
            {
                var user = _userRepository.GetUser(userData.UserName);

                if (user == null)
                    return new TokenResponse(false, "User not authenticated", null, null);

                if (PasswordMatches(userData.Password, user.Password))
                    return new TokenResponse(true, "", _jwtService.GenerateSecurityToken(user), "bearer");

                return new TokenResponse(false, "User not authenticated", null, null);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please try again"+exception);
                return new TokenResponse(false, "Something went wrong, please try again", null, null);
            }
        }

        /// <summary>
        /// Check while field is null or not
        /// </summary>
        /// <param name="userdata"></param>
        /// <returns>string</returns>
        public string Checkuserdata(CreateUserDto userdata)
        {
            _log.Info("Check user data from service layer");
            try
            {
                if (userdata.FirstName == null && userdata.LastName == null && userdata.Email == null &&
                    userdata.Password == null && userdata.Phone == null && userdata.Role == null)
                {
                    return "badrequest";
                }
                return "true";
            }
            catch (Exception exception)
            {
                _log.Error("Bad request from service layer" + exception);
                return "Bad Request";
            }
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userdata"></param>
        /// <returns></returns>
        public UserResponse CreateUser(CreateUserDto userdata)
        {
            _log.Info("Create new user, service layer");

            try
            {
                var usercheck = _userRepository.GetAddressBookByEmailandPhone(userdata.Email, userdata.Phone);

                if (usercheck != null)
                {
                    return new UserResponse(false, "already exists", null);
                }

                var usertosave = new User()
                {
                    FirstName = userdata.FirstName,
                    LastName = userdata.LastName,
                    Email = userdata.Email,
                    Password = HashPassword(userdata.Password),
                    Phone = userdata.Phone,
                    Role = userdata.Role,
                };

                _userRepository.CreateUser(usertosave);
                _userRepository.Save();

                return new UserResponse(true, "", usertosave);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new UserResponse(false, "Something went wrong, please try again", null);
            }
        }

        /// <summary>
        /// Gets complete all user details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public UserResponseDetails GetUserDetailsById(Guid userId, Guid tokenUserId)
        {
            _log.Info("Get user details by using user id");

            if (!userId.Equals(tokenUserId))
                return new UserResponseDetails(false, "User not found", null);

            try
            {
                User user = _userRepository.GetUserDetails(userId);

                if (user == null)
                    return new UserResponseDetails(false, "User not found", null);

                List<Address> userAddresses = _userRepository.GetAllAddress(userId);
                List<Payment> userPayments = _userRepository.GetAllPayment(userId);

                UserResponseDto userAccount = _mapper.Map<UserResponseDto>(user);
                userAccount.Addresses = _mapper.Map<List<UserAddressReturnDto>>(userAddresses);
                userAccount.Payments = _mapper.Map<List<UserPaymentReturnDto>>(userPayments);

                return new UserResponseDetails(true, null, userAccount);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new UserResponseDetails(false, "Something went wrong, please try again", null);
            }
        }

        /// <summary>
        /// Admin can Gets user Account
        /// </summary>
        /// <returns></returns>
        public List<UserReturnDto> GetAllUserAccount()
        {
            List<UserReturnDto> userAccounts = _mapper.Map<List<UserReturnDto>>(_userRepository.GetAllUserDetails());

            return userAccounts;
        }

        /// <summary>
        /// Update user details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tokenUserId"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public UserResponse UpdateUser(Guid userId, Guid tokenUserId, UserUpdateDto userData)
        {
            if (!userId.Equals(tokenUserId))
                return new UserResponse(false, "User not found", null);

            var user = _userRepository.GetUser(userId);

            if (user == null)
                return new UserResponse(false, "User not found", null);

            if (!string.IsNullOrEmpty(userData.FirstName))
                user.FirstName = userData.FirstName;

            if (!string.IsNullOrEmpty(userData.LastName))
                user.LastName = userData.LastName;

            if (!string.IsNullOrEmpty(userData.Email))
                user.Email = userData.Email;

            if (!string.IsNullOrEmpty(userData.Password))
                user.Password = HashPassword(userData.Password);

            if (!long.IsNegative(userData.Phone))
                user.Phone = userData.Phone;

            if (!string.IsNullOrEmpty(userData.Role))
                user.Role = userData.Role;

            user.DateUpdated = DateTime.UtcNow;
            _userRepository.UpdateUser(user);
            _userRepository.Save();

            return new UserResponse(true, "Updated user data successfully", user);
        }

        /// <summary>
        /// Delete user details
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public UserResponse DeleteUser(Guid userId, Guid tokenUserId)
        {
            if (!userId.Equals(tokenUserId))
                return new UserResponse(false, "User not found", null);

            User user = _userRepository.GetUser(userId);

            if (user == null)
                return new UserResponse(false, "User not found", null);

            _userRepository.DeleteUser(user);
            _userRepository.Save();

            return new UserResponse(true, null, user);
        }

        /// <summary>
        /// Verify a password by using hashing
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        /// <summary>
        /// Check password match the given condition
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tokenUserId"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public bool PasswordMatches(string providedPassword, string passwordHash)
        {
            byte[] buffer4;
            if (passwordHash == null)
            {
                return false;
            }
            if (providedPassword == null)
            {
                throw new ArgumentNullException("providedPassword");
            }
            byte[] src = Convert.FromBase64String(passwordHash);
            if (src.Length != 0x31 || src[0] != 0)
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(providedPassword, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            bool areSame = true;
            for (int i = 0; i < a.Length; i++)
            {
                areSame &= a[i] == b[i];
            }
            return areSame;
        }

    }
}