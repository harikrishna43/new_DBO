using System;
using System.Net.Mail;
using System.Configuration;
using System.Linq;
using System.Web;
using DBO.Data.Repositories;

namespace DBO.Services.Email
{
    public class GoogleEmailService : IEmailService
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

        public GoogleEmailService()
        {
        }

        public GoogleEmailService(
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

        private GoogleEmailService(
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

        public string SendMail()
        {
            MailMessage msg = new MailMessage();
            var client = new SmtpClient();
            try
            {
                msg.Subject = this.Subject;
                msg.Body = this.Body;
                msg.From = new MailAddress(FromEmail);
                msg.To.Add(ToEmail);
                msg.IsBodyHtml = IsBodyHtml;
                client.Host = "smtp.gmail.com";
                System.Net.NetworkCredential basicauthenticationinfo
                    = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["GmailKDL"], ConfigurationManager.AppSettings["GmailKDP"]);
                client.Port = int.Parse("587");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicauthenticationinfo;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                foreach (var item in Attachments ?? Enumerable.Empty<Attachment>())
                {
                    msg.Attachments.Add(item);
                }

                client.Send(msg);
            }
            catch (Exception ex)
            {
                //log.Error(ex.Message);
                return ex.Message;
            }

            return "";
        }

        public void SendIntroMail(int companyId, string subject, string body, string email)
        {
            var header = new Attachment(HttpContext.Current.Server.MapPath("/img/bg-email.jpg"));
            header.ContentId = "header";
            header.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
            header.ContentDisposition.Inline = true;

            var banner = new Attachment(HttpContext.Current.Server.MapPath("/img/logo.png"));
            banner.ContentId = "banner";
            banner.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
            banner.ContentDisposition.Inline = true;
            //attachment.ContentType = "image/jpg";
            //attachment.ContentDisposition = "inline";

            //{
            //    Content = Convert.ToBase64String(System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("/img/logo.png"))),

            //    Filename = "banner",
            //    Disposition = "inline",
            //    Type = "image/jpg",
            //};

            var logo = new Attachment(HttpContext.Current.Server.MapPath("/img/dbo.png"));
            logo.ContentId = "logo";
            logo.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
            logo.ContentDisposition.Inline = true;
            //{
            //    Content = Convert.ToBase64String(System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("/img/dbo.png"))),
            //    ContentId = "logo",
            //    Filename = "logo",
            //    Disposition = "inline",
            //    Type = "image/png",
            //};

            var registrationRepository = new RegistrationRepository();
            body = body.Replace("http://discoverbusinessopportunities.com",
                "http://discoverbusinessopportunities.com/home/register?token=" +
                registrationRepository.GetRegistrationCode(companyId));

            var service = new GoogleEmailService(
                email,
                subject,
                email,
                body,
                true,
                true,
                header,
                banner,
                logo
            );

            service.SendMail();
        }
    }
}
