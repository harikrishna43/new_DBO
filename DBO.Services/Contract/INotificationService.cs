using DBO.Data.Models;
using DBO.Data.ViewModels;
using System;
using System.Collections.Generic;

namespace DBO.Services.Contract
{
    public interface INotificationService
    {
        void Create(NotificationType notificationType, EventType eventType, int referenceId);
        void Remove(int id);
        List<NotificationViewModel> GetAllBellsNotifications(int currentUserCompanyId, string currentUserId);
        void ProcessNotifications(string userId, IEnumerable<int> notificationIds, DateTime? processDateTime = null);
        void SendNotificationsEmailToUser(string id, int companyId);
    }
}
