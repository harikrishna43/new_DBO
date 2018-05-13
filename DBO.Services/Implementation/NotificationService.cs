using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBO.Common;
using DBO.Common.Helpers;
using DBO.Data.Models;
using DBO.Data.Repositories.Contract;
using DBO.Data.Utilities;
using DBO.Data.ViewModels;
using DBO.Services.Contract;
using DBO.Services.Email;
using System.Text;
using DBO.Data.Repositories;

namespace DBO.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Follower> _followerRepository;
        private readonly IRepository<Connection> _connectionRepository;
        private readonly IRepository<News> _newsRepositoty;
        private readonly IRepository<Comment> _commentRepositoty;
        private readonly IRepository<ProcessedNotifications> _processedNotificationRepository;
        private readonly IRepository<NotificationSettings> _notificationSettingsRepository;
        private readonly DatabaseLogging logging;

        public NotificationService(IRepository<Notification> notificationRepository, IRepository<Company> companyRepository, IRepository<Follower> followerRepository,
            IRepository<Connection> connectionRepository, IRepository<News> newsRepositoty, IRepository<Comment> commentRepositoty,
            IRepository<ProcessedNotifications> processedNotificationRepository, IRepository<NotificationSettings> notificationSettingsRepository)
        {
            _notificationRepository = notificationRepository;
            _companyRepository = companyRepository;
            _followerRepository = followerRepository;
            _connectionRepository = connectionRepository;
            _newsRepositoty = newsRepositoty;
            _commentRepositoty = commentRepositoty;
            _processedNotificationRepository = processedNotificationRepository;
            _notificationSettingsRepository = notificationSettingsRepository;
            logging = new DatabaseLogging();
        }

        public void Create(NotificationType notificationType, EventType eventType, int referenceId)
        {
            var notification = new Notification
            {
                NotificationType = notificationType,
                EventType = eventType,
                ReferenceId = referenceId.ToString(),
                DateTime = DateTime.Now
            };
            _notificationRepository.Create(notification);
            _notificationRepository.SaveChanges();
        }

        public void Remove(int id)
        {
            _notificationRepository.Remove(id);
            _notificationRepository.SaveChanges();
        }

        private List<NotificationViewModel> GetAllEmailNotifications(int currentUserCompanyId, string currentUserId, NotificationSettings notificationSettings)
        {
            var notificationsResult = new List<NotificationViewModel>();

            if (notificationSettings != null)
            {
                var emailNotifications = _notificationRepository.GetAll().Where(notification =>
                    !notification.ProcessedNotifications.Any(pn => pn.NotificationId == notification.Id && pn.UserId.Equals(currentUserId))
                     && notification.NotificationType == NotificationType.Email)?.ToList();

                if (emailNotifications.Count > 0)
                {
                    if (notificationSettings.OnNewFollower)
                        notificationsResult.AddRange(GetAllFollowersNotifications(currentUserCompanyId, emailNotifications.Where(e => e.EventType == EventType.NewFollower)));

                    if (notificationSettings.OnConnectionRequest)
                        notificationsResult.AddRange(GetAllConnectionRequestNotifications(currentUserCompanyId, emailNotifications.Where(e => e.EventType == EventType.ConnectionRequest)));

                    if (notificationSettings.OnConnectionAccepts)
                        notificationsResult.AddRange(GetAllConnectionAcceptsNotifications(currentUserCompanyId, emailNotifications.Where(e => e.EventType == EventType.ConnectionApprove)));
                }
            }

            return notificationsResult;
        }

        private IEnumerable<NotificationViewModel> GetAllConnectionAcceptsNotifications(int currentUserCompanyId, IEnumerable<Notification> notifications)
        {
            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    if (int.TryParse(notification.ReferenceId, out int connectionId))
                    {
                        var connection = _connectionRepository.GetById(connectionId);
                        if (connection.CompanyId2 == currentUserCompanyId)
                        {
                            yield return new NotificationViewModel
                            {
                                Id = notification.Id,
                                Content = "Your connection was accepted", //TODO: This message is temporary. Need to change according to future notification message template
                                DateTime = notification.DateTime
                            };
                        }
                    }
                }
            }
        }

        private IEnumerable<NotificationViewModel> GetAllConnectionRequestNotifications(int currentUserCompanyId, IEnumerable<Notification> notifications)
        {
            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    if (int.TryParse(notification.ReferenceId, out int connectionId))
                    {
                        var connection = _connectionRepository.GetById(connectionId);
                        if (connection.CompanyId1 == currentUserCompanyId)
                        {
                            yield return new NotificationViewModel
                            {
                                Id = notification.Id,
                                Content = "There is a new connection to your company", //TODO: This message is temporary. Need to change according to future notification message template
                                DateTime = notification.DateTime
                            };
                        }
                    }
                }
            }
        }

        private IEnumerable<NotificationViewModel> GetAllFollowersNotifications(int currentUserCompanyId, IEnumerable<Notification> notifications)
        {
            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    if (int.TryParse(notification.ReferenceId, out int followerId))
                    {
                        var follower = _followerRepository.GetById(followerId);
                        if (follower?.CompanyId == currentUserCompanyId)
                        {
                            yield return new NotificationViewModel
                            {
                                Id = notification.Id,
                                Content = "Your company has new follower", //TODO: This message is temporary. Need to change according to future notification message template
                                DateTime = notification.DateTime
                            };
                        }
                    }
                }
            }
        }

        public List<NotificationViewModel> GetAllBellsNotifications(int currentUserCompanyId, string currentUserId)
        {
            var notificationsResult = new List<NotificationViewModel>();
            var bellsNotifications = _notificationRepository.GetAll().Where(notification =>
                    !notification.ProcessedNotifications.Any(pn => pn.NotificationId == notification.Id && pn.UserId.Equals(currentUserId))
                     && notification.NotificationType == NotificationType.Bells)?.ToList();
            var connections = _connectionRepository.GetAll().Where(c => c.Status == ConnectionStatus.Approved);

            var hasConnection = connections?.Any(conn => conn.CompanyId1 == currentUserCompanyId || conn.CompanyId2 == currentUserCompanyId) == true;
            if (bellsNotifications.Count > 0 && currentUserCompanyId != -1)
            {
                notificationsResult.AddRange(GetConnectionNewsCreatedNotifications(currentUserCompanyId, bellsNotifications.Where(b => b.EventType == EventType.NewsCreated), hasConnection));
                notificationsResult.AddRange(GetConnectionsNewsCommented(currentUserCompanyId, currentUserId, bellsNotifications.Where(b => b.EventType == EventType.CommentCreated), hasConnection));
            }

            return notificationsResult;

        }

        private IEnumerable<NotificationViewModel> GetConnectionsNewsCommented(int currentUserCompanyId, string currentUserId, IEnumerable<Notification> notifications, bool hasConnection)
        {
            foreach (var notification in notifications)
            {
                if (int.TryParse(notification.ReferenceId, out int commentId))
                {
                    var comment = _commentRepositoty.GetById(commentId);
                    if (comment != null)
                    {
                        var news = _newsRepositoty.GetById(comment.NewsId);
                        if (hasConnection && news.CompanyId != currentUserCompanyId && !comment.UserId.Equals(currentUserId))
                        {
                            yield return new NotificationViewModel
                            {
                                Id = notification.Id,
                                Content = "New comment on yours connected company was created",//TODO: This message is temporary. Need to change according to future notification message template
                                Path = $@"{Constants.NewsfeedLink}#comment_{news.Id}_{comment.Id}",
                                DateTime = notification.DateTime
                            };
                        }
                        else if (news.CompanyId == currentUserCompanyId && !comment.UserId.Equals(currentUserId))
                        {
                            yield return new NotificationViewModel
                            {
                                Id = notification.Id,
                                Content = "Under your news was created comment",//TODO: This message is temporary. Need to change according to future notification message template
                                Path = $@"{Constants.NewsfeedLink}#comment_{news.Id}_{comment.Id}",//Dont change this part of code : "#comment_{news.Id}_{comment.Id}"
                                DateTime = notification.DateTime
                            };
                        }
                    }
                }
            }
        }

        private IEnumerable<NotificationViewModel> GetConnectionNewsCreatedNotifications(int currentUserCompanyId, IEnumerable<Notification> notifications, bool hasConnection)
        {
            foreach (var notification in notifications)
            {
                if (int.TryParse(notification.ReferenceId, out int newsId))
                {
                    var news = _newsRepositoty.GetById(newsId);
                    if (news != null && news.CompanyId != currentUserCompanyId)
                    {
                        yield return new NotificationViewModel
                        {
                            Id = notification.Id,
                            Content = "New news on yours connected company was created",//TODO: This message is temporary. Need to change according to future notification message template
                            Path = $@"{Constants.NewsfeedLink}#news_{news.Id}",//Dont change this part of code : "#news_{news.Id}"
                            DateTime = notification.DateTime
                        };
                    }
                }
            }
        }

        public void ProcessNotifications(string userId, IEnumerable<int> notificationIds, DateTime? dateTimeOfProcess = null)
        {
            if (notificationIds?.Any() == true)
            {
                foreach (var notificationId in notificationIds)
                {
                    _processedNotificationRepository.Create(new ProcessedNotifications
                    {
                        NotificationId = notificationId,
                        UserId = userId,
                        DateTime = dateTimeOfProcess ?? DateTime.Now
                    });
                }
                _processedNotificationRepository.SaveChanges();
            }
        }

        public void SendNotificationsEmailToUser(string userId, int companyId)
        {
            var notificationSettings = GetNotificationSettings(userId);
            var company = _companyRepository.GetById(companyId);
            if (notificationSettings?.NotificationIteration != NotificationIteration.WithoutNotification)
            {
                var processedNotifications = _processedNotificationRepository.GetAll().Where(c => c.UserId == userId)?.OrderByDescending(c => c.DateTime)?.ToList();
                var lastNotificationDate = processedNotifications.Count > 0 ? processedNotifications?.Select(c => c.DateTime)?.FirstOrDefault() : null;
                var allUsersNotifications = GetAllEmailNotifications(companyId, userId, notificationSettings);
                if (allUsersNotifications.Count > 0)
                {
                    var notificationEmailBody = GetNotificationEmailBody(allUsersNotifications);
                    if (lastNotificationDate.HasValue)
                    {
                        if (notificationSettings.NotificationIteration == NotificationIteration.OnesADay
                            && DateTimeHelper.GetDaysCountFromDateToDate(DateTime.Now, lastNotificationDate.Value) == 1)
                        {
                            SendEmail(notificationEmailBody, company.Email);
                            logging.Add($"Notes were sent to the user {userId} at {DateTime.Now}. One day iteration.");
                            ProcessNotifications(userId, allUsersNotifications.Select(n => n.Id));
                        }
                        else if (notificationSettings.NotificationIteration == NotificationIteration.OnesAWeek
                            && DateTimeHelper.GetDaysCountFromDateToDate(DateTime.Now, lastNotificationDate.Value) == 7)
                        {
                            SendEmail(notificationEmailBody, company.Email);
                            logging.Add($"Notes were sent to the user {userId} at {DateTime.Now}. Week iteration.");
                            ProcessNotifications(userId, allUsersNotifications.Select(n => n.Id));
                        }
                    }
                    else
                    {
                        SendEmail(notificationEmailBody, company.Email);
                        logging.Add($"Notes were sent to the user {userId} at {DateTime.Now} at first time");
                        ProcessNotifications(userId, allUsersNotifications.Select(n => n.Id));
                    }

                }
            }
        }

        private void SendEmail(string notificationEmailBody, string emailToSend)
        {
            IEmailService emailService = new GoogleEmailService(emailToSend, "You have new notifications", string.Empty, notificationEmailBody, true, false);
#if DEBUG
            emailService = new SendGridEmailService(emailToSend, "Test subject", string.Empty, notificationEmailBody, true, true);
#endif
            emailService.SendMail();
        }

        private NotificationSettings GetNotificationSettings(string userId)
        {
            return _notificationSettingsRepository.Query().FirstOrDefault(n => n.UserId.Equals(userId));
        }

        private string GetNotificationEmailBody(List<NotificationViewModel> notifications)
        {
            var notificationBody = string.Empty;
            if (notifications.Count > 0)
            {
                var notificationBodySb = new StringBuilder();
                foreach (var notification in notifications)
                {
                    if (!string.IsNullOrEmpty(notification.Content))
                    {
                        notificationBodySb.Append($"{notification.DateTime} - {notification.Content}");
                        notificationBodySb.Append("<br />");
                    }
                }
                notificationBody = notificationBodySb.ToString();
            }
            return notificationBody;
        }
    }
}
