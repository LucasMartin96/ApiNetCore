using System.Collections;
using System.Collections.Generic;

namespace FirstApi2xd.Contracts.v1.Responses
{
    public class AuthFailResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}