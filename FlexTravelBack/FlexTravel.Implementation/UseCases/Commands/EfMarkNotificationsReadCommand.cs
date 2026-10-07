using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfMarkNotificationsReadCommand : EfUseCase, IMarkNotificationsReadCommand, ISkipAuditLog
    {
        private readonly IUser _user;

        public EfMarkNotificationsReadCommand(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.MarkNotificationsRead;
        public string UseCaseName => "Mark notifications read";
        public string UseCaseDescription => "Mark own notifications as read";

        public void Execute(MarkNotificationsReadDto request)
        {
            var ids = request?.Ids ?? new List<int>();
            var unread = Context.Notification.Where(n => n.UserId == _user.Id && n.ReadAt == null);

            if (ids.Count > 0)
            {
                unread = unread.Where(n => ids.Contains(n.NotificationId));
            }

            var now = DateTime.UtcNow;
            foreach (var notification in unread.ToList())
            {
                notification.ReadAt = now;
            }

            Context.SaveChanges();
        }
    }
}
