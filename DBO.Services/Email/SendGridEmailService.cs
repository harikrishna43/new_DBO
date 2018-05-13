using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DBO.Data.Repositories;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DBO.Services.Email
{
    [Obsolete("Use google service instead")]
    public class SendGridEmailService : IEmailService
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

        public SendGridEmailService()
        {
        }

        public SendGridEmailService(
            string toEmail,
            string subject,
            string toName,
            string body,
            bool isBodyHtml,
            bool fromKim
        )
        {
            FromEmail = fromKim ? ConfigurationManager.AppSettings["GmailKDL"] : ConfigurationManager.AppSettings["FromEmail"];
            ToEmail = toEmail;
            this.Subject = subject;
            this.FromName = ConfigurationManager.AppSettings["FromName"];
            this.ToName = toName;
            this.Body = body;
            this.IsBodyHtml = isBodyHtml;
        }

        private SendGridEmailService(
                    string toEmail,
                    string subject,
                    string toName,
                    string body,
                    bool isBodyHtml,
                    bool fromKim,
                    params Attachment[] attachments
                   )
        {
            FromEmail = fromKim ? ConfigurationManager.AppSettings["GmailKDL"] : ConfigurationManager.AppSettings["FromEmail"];
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

        public void SendIntroMail(int companyId, string subject, string body, string email)
        {
            var header = new Attachment
            {
                Content = Convert.ToBase64String(System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("/img/bg-email.jpg"))),
                ContentId = "header",
                Filename = "header",
                Disposition = "inline",
                Type = "image/jpg",
            };
            var banner = new Attachment
            {
                Content = Convert.ToBase64String(System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("/img/logo.png"))),
                ContentId = "banner",
                Filename = "banner",
                Disposition = "inline",
                Type = "image/png",
            };

            var logo = new Attachment
            {
                Content = Convert.ToBase64String(System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("/img/dbo.png"))),
                ContentId = "logo",
                Filename = "logo",
                Disposition = "inline",
                Type = "image/png",
            };

            var registrationRepository = new RegistrationRepository();
            body = body.Replace("http://discoverbusinessopportunities.com",
                "http://discoverbusinessopportunities.com/home/register?token=" +
                registrationRepository.GetRegistrationCode(companyId));


            var service = new SendGridEmailService(
                email,
                subject,
                email,
                body,
                true,
                true,
                banner,
                logo
            );

            service.Send();
        }

        public string SendMail()
        {
            var service = new SendGridEmailService(
                "filippvoloshin@gmail.com",
                "test email",
                "filippvoloshin@gmail.com",
                Body,
                true,
                true
            );

            service.Send();

            return "";
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

            if (Attachments != null && Attachments.Length > 0)
            {
                message.AddAttachments(Attachments.ToList());
            }

            var response = await client.SendEmailAsync(message);
            return response;
        }
    }
}