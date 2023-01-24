using Contracts;
using Contracts.IRepository;
using Contracts.Response;
using Entities.Model;
using Entities.RequestDto;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserPaymentService : IUserPaymentService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILog _log;
        private readonly IUserPaymentRepository _userPaymentRepository;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="userRepository">Communication between respository and controller</param>
        /// <param name="userPaymentRepository"></param>
        /// <returns></returns>
        public UserPaymentService(IUserRepository userRepository, IUserPaymentRepository userPaymentRepository)
        {
            _userRepository = userRepository;
            _log = LogManager.GetLogger(typeof(UserPaymentService));
            _userPaymentRepository = userPaymentRepository;
        }

        /// <summary>
        /// Create payment details
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="tokenUserId"></param>
        /// <param name="paymentdto"></param>
        /// <returns></returns>
        public PaymentResponse AddPaymentDetails(Guid userid, Guid tokenUserId, CreatePaymentDto paymentdto)
        {
            _log.Info("Create payment details under user id");

            try
            {
                var user = _userRepository.GetUser(userid);

                if (user == null)
                    return new PaymentResponse(false, "user not found", null);

                if (!user.Id.Equals(tokenUserId))
                    return new PaymentResponse(false, "User not found", null);

                var userExists = _userPaymentRepository.GetPaymentByUserId(paymentdto.PaymentValue);

                if (userExists != null && userExists.UserId.Equals(tokenUserId))
                    return new PaymentResponse(false, "Payment details is already exists", null);


                var usertosave = new Payment()
                {
                    Type = paymentdto.Type,
                    Name = paymentdto.Name,
                    PaymentValue = paymentdto.PaymentValue,
                    Expiry = paymentdto.Expiry,
                    UserId = tokenUserId
                };

                _userPaymentRepository.CreatePayment(usertosave);
                _userPaymentRepository.Save();

                _log.Info("Payment added successfully");
                return new PaymentResponse(true, null, usertosave);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new PaymentResponse(false, "Something went wrong, please try again", null);
            }
        }

        /// <summary>
        /// Get payment details by using payment id
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="tokenUserId"></param>
        /// <returns></returns>
        public PaymentResponse GetPaymentByPaymentId(Guid paymentId, Guid tokenUserId)
        {
            _log.Info("Get payment details by using payment id" + paymentId);

            try
            {
                var user = _userPaymentRepository.GetPayment(paymentId);

                if (user == null)
                    return new PaymentResponse(false, "payment not found", null);

                return new PaymentResponse(true, null, user);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new PaymentResponse(false, "Something went wrong, please try again", null);
            }
        }

        /// <summary>
        /// Update payment details
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="paymentid"></param>
        /// <param name="tokenUserId"></param>
        /// <param name="paymentupdatedto"></param>
        /// <returns></returns>
        public PaymentResponse UpdatePayment(Guid userid, Guid paymentid, Guid tokenUserId, PaymentUpdateDto paymentupdatedto)
        {
            _log.Info("Update payment details");

            try
            {
                var user = _userRepository.GetUser(userid);

                if (user == null)
                    return new PaymentResponse(false, "user not found", null);

                if (!user.Id.Equals(tokenUserId))
                    return new PaymentResponse(false, "User not found", null);

                var paymentExists = _userPaymentRepository.GetPayment(paymentid);

                if (paymentExists == null)
                    return new PaymentResponse(false, "payment not found", null);

                if (!string.IsNullOrEmpty(paymentExists.Type))
                    paymentExists.Type = paymentupdatedto.Type;

                if (!string.IsNullOrEmpty(paymentExists.Name))
                    paymentExists.Name = paymentupdatedto.Name;

                if (!string.IsNullOrEmpty(paymentExists.PaymentValue))
                    paymentExists.PaymentValue = paymentupdatedto.PaymentValue;

                if (!string.IsNullOrEmpty(paymentExists.Expiry))
                    paymentExists.Expiry = paymentupdatedto.Expiry;

                _userPaymentRepository.UpdatePaymentdata(paymentExists);
                _userPaymentRepository.Save();

                _log.Info("Successfully updated payment details");
                return new PaymentResponse(true, null, paymentExists);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new PaymentResponse(false, "Something went wrong, please try again", null);
            }
        }

        public PaymentResponse DeletePaymentDetails(Guid userId, Guid tokenUserId, Guid paymentid)
        {
            _log.Info($"Delete payment details");

            if (!userId.Equals(tokenUserId))
                return new PaymentResponse(false, "User not found", null);
            try
            {
                User user = _userRepository.GetUser(userId);

                if (user == null)
                    return new PaymentResponse(false, "User not found", null);

                Payment payment = _userPaymentRepository.GetPayment(paymentid);

                if (payment == null)
                    return new PaymentResponse(false, "payment not found", null);

                _userPaymentRepository.DeletePayment(payment);
                _userPaymentRepository.Save();

                _log.Info("Successfully deleted payment id");
                return new PaymentResponse(true, null, payment);
            }
            catch (Exception exception)
            {
                _log.Error("Something went wrong please try again" + exception);
                return new PaymentResponse(false, "Something went wrong, please try again", null);
            }
        }
    }
}
