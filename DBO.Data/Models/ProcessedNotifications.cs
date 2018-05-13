using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class ProcessedNotifications
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(Notification))]
        public int NotificationId { get; set; }

        public DateTime DateTime { get; set; }

        public ApplicationUser User { get; set; }
        public Notification Notification { get; set; }
    }
}
