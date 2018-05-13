using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DBO.Common;
using DBO.Common.Helpers;
using DBO.Data.Models;
using DBO.Data.Repositories;
using DBO.Data.Repositories.Contract;
using DBO.Services.Contract;
using DBO.Services.Email;

namespace DBO.Controllers
{
    public class EmailController : BaseController
    {
        private static object obj = new Object();

        private readonly INotificationService _notificationService;
        private readonly IRepository<NotificationSettings> _notificationSettingsRepo;


        public EmailController(INotificationService notificationService, IRepository<NotificationSettings> notificationSettingsRepo)
        {
            _notificationService = notificationService;
            _notificationSettingsRepo = notificationSettingsRepo;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SendIntroMail(int companyId, string subject, string body, string email)
        {
            if (string.IsNullOrEmpty(email)) email = Request.Form["Company.Email"];
            if (!string.IsNullOrEmpty(Request.Form["Send"]))
            {
                IEmailService service = new GoogleEmailService();
#if DEBUG
                service = new SendGridEmailService();
#endif
                service.SendIntroMail(companyId, subject, body, email);
                TempData["Message"] = "Email Sent";
                if (Request?.UrlReferrer != null)
                    return Redirect(Request.UrlReferrer.OriginalString);
                return RedirectToAction("Edit", "Companies", new { id = companyId, message = "Email Sent" });
            }

            ScheduledEmailRepository emailRepository = new ScheduledEmailRepository();
            emailRepository.Add(companyId, subject, body, email);
            TempData["Message"] = "Email Scheduled";
            if (Request?.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.OriginalString);
            return RedirectToAction("Edit", "Companies", new { id = companyId, message = "Email Scheduled" });
        }

        public async Task<ActionResult> RunScheduled()
        {
            await RunNotificationEmails();

            lock (obj)
            {
                DatabaseLogging logging = new DatabaseLogging();
                DateTime now = DateTimeHelper.GetCurrentDateTimeByTimeZone(Constants.CurrentTimeZoneId);

                logging.Add($"Run Scheduled emails. Today is {now.DayOfWeek}. Time: {now.Hour}:{now.Minute}. Full: {now}");
                
                if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                {
                    logging.Add("We don't send emails today " + now.DayOfWeek + " " + now);
                    return Content("We don't send emails today " + now.DayOfWeek + " " + now);
                }

                if (now.Hour != 9)
                {
                    logging.Add("time is not correct " + now.Hour + " Full: " + now);
                    return Content("time is not correct " + now.Hour + " Full: " + now);
                }

                ScheduledEmailRepository emailRepository = new ScheduledEmailRepository();
                IEmailService emailService = new GoogleEmailService();
#if DEBUG
                emailService = new SendGridEmailService();
#endif
                var scheduled = emailRepository.GetScheduled().ToList();
                logging.Add($"Schedule time is fine. Got {scheduled.Count} emails to sent");

                foreach (var email in scheduled)
                {
                    try
                    {
                        emailService.SendIntroMail(email.CompanyId, email.Subject, email.Body, email.Email);
                        emailRepository.UpdateStatus(email.Id, EmailStatus.Sent);
                        logging.Add($"Email {email.Id} is sent");
                    }
                    catch (Exception e)
                    {
                        emailRepository.UpdateStatus(email.Id, EmailStatus.Failed);
                        logging.Add($"Email {email.Id} is failed. Error: {e.Message}");
                    }
                }

                logging.Add($"***Scheduled emails are done***");
            }
            return Content("OK");
        }

        [HttpGet]
        public async Task<ActionResult> RunNotificationEmails()
        {
            var allNotificationUsers = _notificationSettingsRepo.Query().Include(nameof(NotificationSettings.User))
                .AsEnumerable()?.Select(n => n.User)
                .ToList();
            if (allNotificationUsers != null)
                foreach (var user in allNotificationUsers)
                {
                    if (user?.CompanyId != null)
                    {
                        _notificationService.SendNotificationsEmailToUser(user.Id, user.CompanyId.Value);
                    }
                }
            return Content("Successfully sended all emails!");
        }

        public ActionResult TestGbdrm()
        {
            var email1 = new GoogleEmailService("lajdak.ua@gmail.com", "Hello", "Viktor", "Hello, V. How are you?", true, false);
            var res1 = email1.SendMail();

            var email2 = new GoogleEmailService("lajdak.ua@gmail.com", "Hello", "Viktor", "Hello, V2. How are you?", true, true);
            var res2 = email2.SendMail();

            return Content($"{res1}   ****   {res2}");
        }
    }
}