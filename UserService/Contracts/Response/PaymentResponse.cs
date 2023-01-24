using Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Response
{
    public class PaymentResponse : MessageResponse
    {
        public Payment Payment { get; protected set; }

        public PaymentResponse(bool isSuccess, string message, Payment payment) : base(isSuccess, message)
        {
            Payment = payment;
        }
    }
}
