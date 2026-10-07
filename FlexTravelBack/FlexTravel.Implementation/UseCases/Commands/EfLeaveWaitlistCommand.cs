using FlexTravel.Application.Constants;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfLeaveWaitlistCommand : EfUseCase, ILeaveWaitlistCommand
    {
        private readonly IUser _user;

        public EfLeaveWaitlistCommand(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.LeaveWaitlist;
        public string UseCaseName => "Leave waitlist";
        public string UseCaseDescription => "Stop waiting for seats on a trip";

        public void Execute(int entryId)
        {
            var entry = Context.WaitlistEntry.FirstOrDefault(w => w.WaitlistEntryId == entryId && w.UserId == _user.Id && w.IsActive);

            if (entry == null)
            {
                throw new EntityNotFoundException("Waitlist entry", entryId);
            }

            entry.IsActive = false;
            Context.SaveChanges();
        }
    }
}
