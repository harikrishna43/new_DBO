using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace DBO.Services
{
    public class EmailService
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
        public string SmtpServer { get; set; }

        public Attachment[] Attachments { get; private set; }

        //private EmailService() {
        //    FromEmail = string.Empty;
        //    ToEmail = string.Empty;
        //    this.Subject = string.Empty;
        //    this.FromName = string.Empty;
        //    this.ToName = string.Empty;
        //    this.Body = string.Empty;
        //    this.IsBodyHtml = true;
        //    SmtpServer = string.Empty;
        //}

        public EmailService(
                    string toEmail,
                    string subject,
                    string toName,
                    string body,
                    bool isBodyHtml,
                    bool fromKim,
                    params Attachment[] attachments
                   )
        {
            FromEmail = fromKim ? "kd@discoverbusinessopportunities.com" : ConfigurationManager.AppSettings["FromEmail"];
            ToEmail = toEmail;
            this.Subject = subject;
            this.FromName = ConfigurationManager.AppSettings["FromName"];
            this.ToName = toName;
            this.Body = body;
            this.IsBodyHtml = isBodyHtml;
            this.Attachments = attachments;
        }

        public void Send()
        {
            Task.Run(async () => { await SendEmail(); });
        }

        private async Task<Response> SendEmail()
        {
            var apiKey = ConfigurationManager.AppSettings["SENDGRID_API_KEY"];
            var client = new SendGridClient(apiKey: apiKey);

            var from = new EmailAddress(this.FromEmail, this.FromName);
            var subject = this.Subject;
            var to = new EmailAddress(this.ToEmail, this.ToName);
            SendGridMessage message = null;
            if (IsBodyHtml)
            {
                message = MailHelper.CreateSingleEmail(from, to, subject, "", this.Body);
            }
            else
            {
                message = MailHelper.CreateSingleEmail(from, to, subject, this.Body, "");
            }

            if(Attachments.Length > 0)
            {
                message.AddAttachments(Attachments.ToList());
            }

            var response = await client.SendEmailAsync(message);
            return response;
        }
    }
}