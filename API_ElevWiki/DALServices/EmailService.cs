using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using API_ElevWiki.Models.Configuration;
using API_ElevWiki.Interfaces;
using API_ElevWiki.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace API_ElevWiki.Repository
{
    public class EmailService : IEmailService
    {
        private readonly AppSettingsEmail _emailConfig;

        // Remember to put HTML Templates at the specified path
        private readonly string filePathConfirmEmail = Path.GetFullPath
            (@"..\HTMLTemplates\ConfirmationEmail.html"); 
        private readonly string filePathWeeklyUpdate = Path.GetFullPath
            (@"..\HTMLTemplates\WeeklyUpdateEmail.html"); 

        public EmailService(AppSettingsEmail emailConfig)
        {
            _emailConfig = emailConfig;
        }

        #region  Confirm email builder
        public async Task SendEmailConfirmationLink
            (ConfirmationEmailMessage message, string confirmationLink)
        {
            MimeMessage emailMessage = await CreateEmailConfirmationLink(confirmationLink, message);

            await Send(emailMessage);
        }

        private async Task<MimeMessage> CreateEmailConfirmationLink
            (string confirmationLink, ConfirmationEmailMessage message)
        {
            BodyBuilder bodyBuilder = new()
            { 
                HtmlBody = await CreateConfirmationLink(confirmationLink) 
            };

            MimeMessage emailMessage = new()
            {
                Subject = message.Subject,
                Body = bodyBuilder.ToMessageBody()
            };

            emailMessage.From.Add(new MailboxAddress(_emailConfig.from));
            emailMessage.To.AddRange(message.To);

            return emailMessage;
        }

        private async Task<string> CreateConfirmationLink(string confirmationLink)
        {
            string body;

            using (StreamReader reader = new(filePathConfirmEmail))
            {
                body = await reader.ReadToEndAsync();
            }

            body = body.Replace("{link}", confirmationLink);

            return body;
        }
        #endregion

        #region Weekly overview email builder
        public async Task SendEmailWeeklyOverview(TimeRecord[] tRec, WeeklyOverviewEmailMessage message)
        {
            MimeMessage emailMessage = await CreateEmailWeeklyOverview(tRec, message);

            await Send(emailMessage);
        }

        private async Task<MimeMessage> CreateEmailWeeklyOverview
            (TimeRecord[] tRec, WeeklyOverviewEmailMessage message)
        {
            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = await CreateWeeklyOverview(tRec) 
            };

            MimeMessage emailMessage = new()
            {
                Subject = message.Subject,
                Body = bodyBuilder.ToMessageBody()
            };

            emailMessage.From.Add(new MailboxAddress(_emailConfig.from));
            emailMessage.To.AddRange(message.To);

            return emailMessage;
        }

        private async Task<string> CreateWeeklyOverview(TimeRecord[] tRec)
        {
            string body;

            using (StreamReader reader = new(filePathWeeklyUpdate))
            {
                body = await reader.ReadToEndAsync();
            }

            #region Timestamp stringbuilder
            StringBuilder[] timestamps = new StringBuilder[5];

            timestamps[0].AppendLine(tRec[0].timestamp.ToString());
            timestamps[0].Append(tRec[1].timestamp.ToString());
            timestamps[1].AppendLine(tRec[2].timestamp.ToString());
            timestamps[1].Append(tRec[3].timestamp.ToString());
            timestamps[2].AppendLine(tRec[4].timestamp.ToString());
            timestamps[2].Append(tRec[5].timestamp.ToString());
            timestamps[3].AppendLine(tRec[6].timestamp.ToString());
            timestamps[3].Append(tRec[7].timestamp.ToString());
            timestamps[4].AppendLine(tRec[8].timestamp.ToString());
            timestamps[4].Append(tRec[9].timestamp.ToString());
            timestamps[5].AppendLine(tRec[10].timestamp.ToString());
            timestamps[5].Append(tRec[11].timestamp.ToString());
            #endregion

            #region CheckStatus stringbuilder
            StringBuilder[] checkStatus = new StringBuilder[5];

            checkStatus[0].AppendLine(tRec[0].checkStatus);
            checkStatus[0].Append(tRec[1].checkStatus);
            checkStatus[1].AppendLine(tRec[2].checkStatus);
            checkStatus[1].Append(tRec[3].checkStatus);
            checkStatus[2].AppendLine(tRec[4].checkStatus);
            checkStatus[2].Append(tRec[5].checkStatus);
            checkStatus[3].AppendLine(tRec[6].checkStatus);
            checkStatus[3].Append(tRec[7].checkStatus);
            checkStatus[4].AppendLine(tRec[8].checkStatus);
            checkStatus[4].Append(tRec[9].checkStatus);
            checkStatus[5].AppendLine(tRec[10].checkStatus);
            checkStatus[5].Append(tRec[11].checkStatus);
            #endregion

            #region Find week number
            DateTime dt = DateTime.Now;
            Calendar cal = new CultureInfo("da-DK").Calendar;
            string weekNumber = cal.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
            #endregion

            #region Hours from check in to check out
            string[] hourAmount = new string[5];

            hourAmount[0] = (tRec[1].timestamp - tRec[0].timestamp).TotalHours.ToString();
            hourAmount[1] = (tRec[3].timestamp - tRec[2].timestamp).TotalHours.ToString();
            hourAmount[2] = (tRec[5].timestamp - tRec[4].timestamp).TotalHours.ToString();
            hourAmount[3] = (tRec[7].timestamp - tRec[6].timestamp).TotalHours.ToString();
            hourAmount[4] = (tRec[9].timestamp - tRec[8].timestamp).TotalHours.ToString();
            hourAmount[5] = (tRec[11].timestamp - tRec[10].timestamp).TotalHours.ToString();
            #endregion

            #region Total weekly hour amount
            int weeklyHourAmount = Convert.ToInt32
                (hourAmount[0] + hourAmount[1] + hourAmount[2] + hourAmount[3] + hourAmount[4] + hourAmount[5]);
            #endregion

            #region Replaces elements in the body of the WeeklyOverviewEmailTemplate.html
            body = body.Replace("{weekNumber}", weekNumber);
            body = body.Replace("{name}", tRec[0].Student.name);
            body = body.Replace("{weeklyHourAmount}", weeklyHourAmount.ToString());

            body = body.Replace("{timestampsDay1}", timestamps[0].ToString());
            body = body.Replace("{checkStatusDay1}", checkStatus[0].ToString());
            body = body.Replace("hourAmountDay1", hourAmount[0]);

            body = body.Replace("{timestampsDay2}", timestamps[1].ToString());
            body = body.Replace("{checkStatusDay2}", checkStatus[1].ToString());
            body = body.Replace("hourAmountDay2", hourAmount[1]);

            body = body.Replace("{timestampsDay3}", timestamps[2].ToString());
            body = body.Replace("{checkStatusDay3}", checkStatus[2].ToString());
            body = body.Replace("hourAmountDay3", hourAmount[2]);

            body = body.Replace("{timestampsDay4}", timestamps[3].ToString());
            body = body.Replace("{checkStatusDay4}", checkStatus[3].ToString());
            body = body.Replace("hourAmountDay4", hourAmount[3]);

            body = body.Replace("{timestampsDay5}", timestamps[4].ToString());
            body = body.Replace("{checkStatusDay5}", checkStatus[4].ToString());
            body = body.Replace("hourAmountDay5", hourAmount[4]);

            body = body.Replace("{timestampsDay6}", timestamps[5].ToString());
            body = body.Replace("{checkStatusDay6}", checkStatus[5].ToString());
            body = body.Replace("hourAmountDay6", hourAmount[5]);
            #endregion

            return body;
        }
        #endregion

        private async Task Send(MimeMessage emailMessage)
        {
            using (SmtpClient client = new())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.smtpServer, _emailConfig.port, true /*MailKit.Security.SecureSocketOptions.SslOnConnect*/);
                    client.AuthenticationMechanisms.Remove("X0AUTH2");
                    await client.AuthenticateAsync(_emailConfig.email, _emailConfig.password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    // Error message here
                    Console.WriteLine("\n" + ex + "\n");
                    Debug.WriteLine("\n" + ex + "\n");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
