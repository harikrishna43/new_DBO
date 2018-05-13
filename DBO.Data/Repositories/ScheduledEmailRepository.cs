using System;
using DBO.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace DBO.Data.Repositories
{
    public class ScheduledEmailRepository
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public IEnumerable<ScheduledEmail> GetScheduled()
        {
            return _db.ScheduledEmails.Where(e => e.Status == EmailStatus.Scheduled);
        }

        public void Add(int companyId, string subject, string body, string emailAddress)
        {
            ScheduledEmail email = new ScheduledEmail
            {
                CompanyId = companyId,
                Subject = subject,
                Body = body,
                Email = emailAddress,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.ScheduledEmails.Add(email);
            _db.SaveChanges();
        }

        public void UpdateStatus(int id, EmailStatus newStatus)
        {
            var email = _db.ScheduledEmails.Find(id);
            if (email == null) return;

            email.Status = newStatus;
            email.UpdatedAt = DateTime.Now;

            _db.SaveChanges();
        }
    }
}
