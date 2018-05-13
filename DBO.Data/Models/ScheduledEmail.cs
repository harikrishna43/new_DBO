using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class ScheduledEmail
    {
        public int Id { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [MaxLength(63)]
        public string Subject { get; set; }

        [MaxLength(63)]
        public string Email { get; set; }

        public string Body { get; set; }

        public EmailStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public enum EmailStatus
    {
        Scheduled,
        Sent,
        Failed
    }
}
