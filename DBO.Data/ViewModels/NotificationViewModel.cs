using DBO.Data.Models;
using System;

namespace DBO.Data.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Path { get; set; }
        public DateTime DateTime { get; set; }
    }
}
