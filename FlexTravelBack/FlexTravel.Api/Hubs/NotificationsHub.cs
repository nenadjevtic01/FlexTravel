using FlexTravel.Application.Dto;
using FlexTravel.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FlexTravel.Api.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub
    {
        public const string Path = "/hubs/notifications";
        public const string NotificationEvent = "notification";
    }

    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection) => connection.User?.FindFirst("UserId")?.Value;
    }

    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<NotificationsHub> _hub;
        private readonly ILogger<SignalRNotificationPublisher> _logger;

        public SignalRNotificationPublisher(IHubContext<NotificationsHub> hub, ILogger<SignalRNotificationPublisher> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        public void Publish(int userId, NotificationDto notification)
        {
            _hub.Clients.User(userId.ToString())
                .SendAsync(NotificationsHub.NotificationEvent, notification)
                .ContinueWith(
                    task => _logger.LogWarning(task.Exception, "Live notification to user {UserId} failed.", userId),
                    TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
