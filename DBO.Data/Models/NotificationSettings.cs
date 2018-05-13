using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public enum NotificationIteration
    {
        OnesADay,
        OnesAWeek,
        WithoutNotification
    }

    public class NotificationSettings
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public bool OnConnectionRequest { get; set; }
        public bool OnConnectionAccepts { get; set; }
        public bool OnNewFollower { get; set; }
        public NotificationIteration NotificationIteration { get; set; }
    }
}
