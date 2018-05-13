using System;
using System.Collections.Generic;

namespace DBO.Data.Models
{
    public enum NotificationType
    {
        Email,
        Bells
    }

    public enum EventType
    {
        ConnectionRequest,
        ConnectionApprove,
        NewFollower,
        NewsCreated,
        CommentCreated
    }

    public class Notification
    {
        public int Id { get; set; }
        public NotificationType NotificationType { get; set; }
        public EventType EventType { get; set; }
        public string ReferenceId { get; set; }
        public DateTime DateTime { get; set; }

        public virtual ICollection<ProcessedNotifications> ProcessedNotifications { get; set; }
    }
}
