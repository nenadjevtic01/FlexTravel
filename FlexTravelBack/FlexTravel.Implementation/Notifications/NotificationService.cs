using FlexTravel.Application.Dto;
using FlexTravel.Application.Notifications;
using FlexTravel.DataAccess;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.Notifications
{
    public class NotificationService
    {
        private readonly FlexTravelContext _context;
        private readonly INotificationPublisher _publisher;
        private readonly List<Notification> _pending = new List<Notification>();

        public NotificationService(FlexTravelContext context, INotificationPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public void Send(int userId, NotificationType type, string title, string message, string link)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = Truncate(title, 100),
                Message = Truncate(message, 500),
                Link = link,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notification.Add(notification);
            _pending.Add(notification);
        }

        public void SendToAgency(int agencyId, NotificationType type, string title, string message, string link)
        {
            var employees = _context.User
                .OfType<AgencyEmployee>()
                .Where(e => e.AgencyId == agencyId && e.IsActive)
                .Select(e => e.UserId)
                .ToList();

            foreach (var userId in employees)
            {
                Send(userId, type, title, message, link);
            }
        }

        public void Push()
        {
            foreach (var notification in _pending.Where(n => n.NotificationId != 0))
            {
                _publisher.Publish(notification.UserId, ToDto(notification));
            }

            _pending.Clear();
        }

        public static NotificationDto ToDto(Notification notification) => new NotificationDto
        {
            Id = notification.NotificationId,
            Type = notification.Type.ToString(),
            Title = notification.Title,
            Message = notification.Message,
            Link = notification.Link,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.ReadAt.HasValue
        };

        private static string Truncate(string text, int length) =>
            text == null || text.Length <= length ? text : text[..(length - 1)] + "…";
    }
}
