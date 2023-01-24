using Contracts.IRepository;
using Entities;
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserPaymentRepository : IUserPaymentRepository
    {
        private readonly DatabaseContext _dbcontext;
        public UserPaymentRepository(DatabaseContext databaseContext)
        {
            _dbcontext = databaseContext;
        }

        public Payment GetPaymentByUserId(string paymentvalue)
        {
            if (paymentvalue == null || paymentvalue == string.Empty)
                throw new ArgumentNullException(nameof(paymentvalue) + " was null in GetPayment from UserPaymentRepository");

            return _dbcontext.Payments.SingleOrDefault(a => a.PaymentValue == paymentvalue);
        }

        public void CreatePayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException("Payment data was null in CreatePayment from UserPaymentRepository");

            _dbcontext.Payments.Add(payment);
        }

        public Payment GetPayment(Guid paymentid)
        {
            return _dbcontext.Payments.SingleOrDefault(user => user.Id == paymentid);
        }

        public void UpdatePaymentdata(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException("payment data was null in UpdatePaymentdata from UserPaymentRepository");

            _dbcontext.Payments.Update(payment);
        }

        public void DeletePayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException("Payment data was null in DeletePayment from UserPaymentRepository");

            _dbcontext.Payments.Remove(payment);
        }

        public void Save()
        {
            _dbcontext.SaveChanges();
        }
    }
}
