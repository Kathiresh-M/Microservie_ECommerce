using AutoMapper;
using Contracts;
using Contracts.IRepository;
using Contracts.Response;
using Entities.Dto;
using Entities.Model;
using Entities.RequestDto;
using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly ILog _log;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="userRepository">Communication between user respository and controller</param>
        /// <param name="userAddressRepository">Communication between address respository and controller</param>
        /// <param name="mapper">used to map dto</param>
        /// <returns></returns>
        public UserAddressService(IUserAddressRepository userAddressRepository, IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _userAddressRepository= userAddressRepository;
            _log = LogManager.GetLogger(typeof(UserAddressService));
            _mapper = mapper;
        }

        /// <summary>
        /// Create Address details
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="tokenUserId"></param>
        /// <param name="addressdto"></param>
        /// <returns></returns>
        public AddressResponse AddAddress(Guid userid, Guid tokenUserId, CreateAddressDto addressdto)
        {
            _log.Info("Create new address by using user id");
            try
            {
                var addressBook = _userRepository.GetUser(userid);

                if (addressBook == null)
                    return new AddressResponse(false, "user not found", null);

                if (!addressBook.Id.Equals(tokenUserId))
                    return new AddressResponse(false, "User not found", null);

                var addressExists = _userAddressRepository.GetAddressByUserId(userid);

                if (addressExists != null && addressExists.UserId.Equals(tokenUserId))
                    return new AddressResponse(false, "Address already exists", null);


                var usertosave = new Address()
                {
                    Name = addressdto.Name,
                    Line1 = addressdto.Line1,
                    Line2 = addressdto.Line2,
                    City = addressdto.City,
                    Pincode = addressdto.Pincode,
                    State = addressdto.State,
                    Country = addressdto.Country,
                    Type = addressdto.Type,
                    Phone = addressdto.Phone,
                    UserId = tokenUserId
                };

                _userAddressRepository.CreateAddress(usertosave);
                _userAddressRepository.Save();

                _log.Info("Successfully added address details under user id");
                return new AddressResponse(true, null, usertosave);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new AddressResponse(false, "Something went wrong, please try again", null);
            }
        }

        /// <summary>
        /// Get Address details by address id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public AddressResponse GetAddressByAddressId(Guid userId, Guid tokenUserId)
        {
            _log.Info("Get addtess details by using address id");

            try
            {
                if (!userId.Equals(tokenUserId))
                    return new AddressResponse(false, "User not found", null);

                var user = _userAddressRepository.GetAddress(userId);

                if (user == null)
                    return new AddressResponse(false, "Address not found", null);

                return new AddressResponse(true, null, user);
            }
            catch(Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new AddressResponse(false, "Something went wrong, please try again", null);
            }
        }

        public AddressResponse UpdateAddress(Guid userid, Guid addressid, Guid tokenUserId, AddressUpdateDto addressupdatedto)
        {
            _log.Info($"Updated address: {addressid}");

            try
            {
                var addressBook = _userRepository.GetUser(userid);

                if (addressBook == null)
                    return new AddressResponse(false, "user not found", null);

                if (!addressBook.Id.Equals(tokenUserId))
                    return new AddressResponse(false, "User not found", null);

                var addressExists = _userAddressRepository.GetAddress(addressid);

                if (addressExists == null)
                    return new AddressResponse(false, "Address not found", null);

                if (!string.IsNullOrEmpty(addressExists.Name))
                    addressExists.Name = addressupdatedto.Name;

                if (!string.IsNullOrEmpty(addressExists.Line1))
                    addressExists.Line1 = addressupdatedto.Line1;

                if (!string.IsNullOrEmpty(addressExists.Line2))
                    addressExists.Line2 = addressupdatedto.Line2;

                if (!string.IsNullOrEmpty(addressExists.City))
                    addressExists.City = addressupdatedto.City;

                if (addressExists.Pincode != null)
                    addressExists.Pincode = addressupdatedto.Pincode;

                if (!string.IsNullOrEmpty(addressExists.State))
                    addressExists.State = addressupdatedto.State;

                if (!string.IsNullOrEmpty(addressExists.Country))
                    addressExists.Country = addressupdatedto.Country;

                if (!string.IsNullOrEmpty(addressExists.Type))
                    addressExists.Type = addressupdatedto.Type;

                if (!long.IsNegative(addressExists.Phone))
                    addressExists.Phone = addressupdatedto.Phone;

                addressExists.DateUpdated = DateTime.UtcNow;
                _userAddressRepository.UpdateAddressdata(addressExists);
                _userAddressRepository.Save();

                _log.Info("Successfully updated address details");
                return new AddressResponse(true, null, addressExists);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new AddressResponse(false, "Something went wrong, please try again", null);
            }
        }

        public AddressResponse DeleteAddress(Guid userId, Guid tokenUserId, Guid addressid)
        {
            _log.Info("Delete address details by using address id");

            if (!userId.Equals(tokenUserId))
                return new AddressResponse(false, "User not found", null);

            try
            {
                User user = _userRepository.GetUser(userId);

                if (user == null)
                    return new AddressResponse(false, "User not found", null);

                Address address = _userAddressRepository.GetAddress(addressid);

                if (address == null)
                    return new AddressResponse(false, "Address not found", null);

                _userAddressRepository.DeleteAddress(address);
                _userAddressRepository.Save();

                return new AddressResponse(true, null, address);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new AddressResponse(false, "Something went wrong, please try again", null);
            }
        }
    }
}
