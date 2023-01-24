using Contracts.Response;
using Entities.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserPaymentService
    {
        PaymentResponse AddPaymentDetails(Guid userid, Guid tokenUserId, CreatePaymentDto paymentdto);
        PaymentResponse GetPaymentByPaymentId(Guid paymentId, Guid tokenUserId);
        PaymentResponse UpdatePayment(Guid userid, Guid paymentid, Guid tokenUserId, PaymentUpdateDto paymentupdatedto);
        PaymentResponse DeletePaymentDetails(Guid userId, Guid tokenUserId, Guid paymentid);
    }
}
