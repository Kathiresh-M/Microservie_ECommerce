using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IRepository
{
    public interface IUserPaymentRepository
    {
        Payment GetPaymentByUserId(string paymentvalue);
        void CreatePayment(Payment payment);
        Payment GetPayment(Guid paymentid);
        void UpdatePaymentdata(Payment payment);
        void DeletePayment(Payment payment);
        void Save();
    }
}
