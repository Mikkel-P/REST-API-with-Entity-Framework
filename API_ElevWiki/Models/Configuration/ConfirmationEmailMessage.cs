using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_ElevWiki.Models.Configuration
{
    public class ConfirmationEmailMessage
    {
        public List<MailboxAddress> To { get; set; }

        public string Subject { get; set; }

        public ConfirmationEmailMessage(IEnumerable<string> to, string subject)
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(x => new MailboxAddress(x)));
            Subject = subject;
        }
    }
}
