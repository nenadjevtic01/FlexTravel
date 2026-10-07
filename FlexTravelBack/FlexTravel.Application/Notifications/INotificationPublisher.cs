using FlexTravel.Application.Dto;

namespace FlexTravel.Application.Notifications
{
    public interface INotificationPublisher
    {
        void Publish(int userId, NotificationDto notification);
    }
}
