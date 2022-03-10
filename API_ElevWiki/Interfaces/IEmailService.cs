
using System.Threading.Tasks;
using API_ElevWiki.Models.Configuration;
using API_ElevWiki.Models;

namespace API_ElevWiki.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfirmationLink
            (ConfirmationEmailMessage message, string confirmationLink);

        Task SendEmailWeeklyOverview
            (TimeRecord[] tRec, WeeklyOverviewEmailMessage message);
    }
}
