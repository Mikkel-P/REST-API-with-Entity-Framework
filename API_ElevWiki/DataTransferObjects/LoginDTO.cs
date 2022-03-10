using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_ElevWiki.DataTransferObjects
{
    public class LoginDTO
    {
        public string email { get; set; }

        public string passwordHash { get; set; }
    }
}
