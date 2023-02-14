using Contract.Responses;
using Entities.RequestDto;
using Entities.ServiceDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public interface IOrderService
    {
        OrderResponse CheckDetails(PaymentDto createProductDto, string token);
    }
}
