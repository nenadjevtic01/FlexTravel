using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetMyWaitlistQuery : EfUseCase, IGetMyWaitlistQuery
    {
        private readonly IUser _user;

        public EfGetMyWaitlistQuery(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetMyWaitlist;
        public string UseCaseName => "Get my waitlist";
        public string UseCaseDescription => "Trips the customer is waiting for";

        public IEnumerable<WaitlistEntryDto> Execute(int request)
        {
            var today = DateTime.Today;

            return Context.WaitlistEntry
                .Where(w => w.UserId == _user.Id && w.IsActive && w.Trip.IsActive && w.Trip.StartDate > today)
                .OrderBy(w => w.Trip.StartDate)
                .Select(w => new
                {
                    w.WaitlistEntryId,
                    w.TripId,
                    w.Trip.TripName,
                    Destination = w.Trip.Destination.DestinationName,
                    Image = w.Trip.Destination.Images.OrderBy(i => i.DestinationImageId).Select(i => i.Src).FirstOrDefault(),
                    w.Trip.StartDate,
                    w.Trip.EndDate,
                    w.Adults,
                    w.Children,
                    w.CreatedAt,
                    w.NotifiedAt,
                    Ahead = w.Trip.Waitlist.Count(o => o.IsActive && o.CreatedAt < w.CreatedAt),
                    w.Trip.Capacity,
                    Taken = w.Trip.Reservations
                        .Where(r => r.IsActive && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                        .Sum(r => (int?)(r.NumberOfAdults + r.NumberOfChildren)) ?? 0
                })
                .ToList()
                .Select(w =>
                {
                    var seatsLeft = Math.Max(0, w.Capacity - w.Taken);
                    return new WaitlistEntryDto
                    {
                        Id = w.WaitlistEntryId,
                        TripId = w.TripId,
                        TripName = w.TripName,
                        Destination = w.Destination,
                        Image = w.Image,
                        StartDate = w.StartDate,
                        EndDate = w.EndDate,
                        Adults = w.Adults,
                        Children = w.Children,
                        JoinedAt = w.CreatedAt,
                        Position = w.Ahead + 1,
                        SeatsLeft = seatsLeft,
                        CanBookNow = seatsLeft >= w.Adults + w.Children,
                        NotifiedAt = w.NotifiedAt
                    };
                })
                .ToList();
        }
    }
}
