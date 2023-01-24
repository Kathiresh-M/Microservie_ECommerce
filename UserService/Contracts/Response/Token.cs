using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Response
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }

        public Token(string accessToken, string tokenType)
        {
            AccessToken = accessToken;
            TokenType = tokenType;
        }
    }
}
