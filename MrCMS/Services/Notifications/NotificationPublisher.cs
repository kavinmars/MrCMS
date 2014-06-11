using System;
using Microsoft.Owin;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services.Notifications
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly ISession _session;
        private readonly IOwinContext _context;

        public NotificationPublisher(ISession session,IOwinContext context)
        {
            _session = session;
            _context = context;
        }

        public void PublishNotification(string message, PublishType publishType = PublishType.Both, NotificationType notificationType = NotificationType.All)
        {
            var notification = new Notification
                                   {
                                       Message = message,
                                       User = CurrentRequestData.CurrentUser,
                                       NotificationType = notificationType
                                   };
            switch (publishType)
            {
                case PublishType.Transient:
                    PushNotification(notification);
                    break;
                case PublishType.Persistent:
                    SaveNotification(notification);
                    break;
                case PublishType.Both:
                    SaveNotification(notification);
                    PushNotification(notification);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("publishType");
            }
        }

        private void SaveNotification(Notification notification)
        {
            _session.Transact(session => session.Save(notification));
            _context.EventContext().Publish<IOnPersistentNotificationPublished, OnPersistentNotificationPublishedEventArgs>(
                            new OnPersistentNotificationPublishedEventArgs(notification));
        }

        private void PushNotification(Notification notification)
        {
            _context.EventContext().Publish<IOnTransientNotificationPublished, OnTransientNotificationPublishedEventArgs>(
                            new OnTransientNotificationPublishedEventArgs(notification));
        }
    }
}