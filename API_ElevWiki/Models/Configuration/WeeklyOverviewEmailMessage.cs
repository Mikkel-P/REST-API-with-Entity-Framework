using System.Collections.Generic;
using System.Linq;
using MimeKit;

namespace API_ElevWiki
{
    public class WeeklyOverviewEmailMessage
    {
        public List<MailboxAddress> To { get; set; }

        public string Subject { get; set; }
        
        public WeeklyOverviewEmailMessage(IEnumerable<string> to, string subject)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(s => new MailboxAddress(s)));
            Subject = subject;
        }
    }
}
