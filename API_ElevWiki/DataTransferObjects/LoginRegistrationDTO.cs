using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_ElevWiki.DataTransfer
{
    public class LoginRegistrationDTO
    {
        public string name { get; set; }

        public string phone { get; set; }

        public string email { get; set; }

        public string passwordHash { get; set; }

        public string clientURI { get; set; }
    }
}
