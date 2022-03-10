using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_ElevWiki.DataTransfer
{
    public class TokenDTO
    {
        public string accessToken { get; set; }

        public string refreshToken { get; set; }
    }
}
