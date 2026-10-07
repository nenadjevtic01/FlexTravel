using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetMyNotificationsQuery : EfUseCase, IGetMyNotificationsQuery, ISkipAuditLog
    {
        private const int Shown = 20;
        private readonly IUser _user;

        public EfGetMyNotificationsQuery(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetMyNotifications;
        public string UseCaseName => "Get my notifications";
        public string UseCaseDescription => "Latest notifications of the signed-in user";

        public NotificationsDto Execute(int request)
        {
            var mine = Context.Notification.Where(n => n.UserId == _user.Id);

            return new NotificationsDto
            {
                UnreadCount = mine.Count(n => n.ReadAt == null),
                Items = mine
                    .OrderByDescending(n => n.CreatedAt)
                    .ThenByDescending(n => n.NotificationId)
                    .Take(Shown)
                    .Select(n => new NotificationDto
                    {
                        Id = n.NotificationId,
                        Type = n.Type.ToString(),
                        Title = n.Title,
                        Message = n.Message,
                        Link = n.Link,
                        CreatedAt = n.CreatedAt,
                        IsRead = n.ReadAt != null
                    })
                    .ToList()
            };
        }
    }
}
