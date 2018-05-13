using DBO.Data.Models;

namespace DBO.Data.ViewModels
{
    public class NotificationSettingsViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool OnConnectionRequest { get; set; }
        public bool OnConnectionAccepts { get; set; }
        public bool OnNewFollower { get; set; }
        public NotificationIteration NotificationIteration { get; set; }
    }
}
