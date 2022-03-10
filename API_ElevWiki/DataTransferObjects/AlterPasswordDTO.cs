using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_ElevWiki.DataTransfer
{
    public class AlterPasswordDTO
    {
        public string email { get; set; }

        public string oldPasswordHash { get; set; }

        public string newPasswordHash { get; set; }

        public int studentId { get; set; }
    }
}
