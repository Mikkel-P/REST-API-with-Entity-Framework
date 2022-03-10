using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_ElevWiki
{
    public class AppSettingsEmail
    {
        public string from { get; set; }

        public string smtpServer { get; set; }

        public int port { get; set; }

        public string email { get; set; }

        public string password { get; set; }
    }
}
