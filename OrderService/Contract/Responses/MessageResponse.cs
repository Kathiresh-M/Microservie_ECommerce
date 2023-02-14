using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Responses
{
    public class MessageResponse
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; }

        public MessageResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
