using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Notifications;

namespace FlexTravel.Implementation.Booking
{
    public class WaitlistService
    {
        private readonly FlexTravelContext _context;
        private readonly BookingCalculator _calculator;
        private readonly NotificationService _notifications;

        public WaitlistService(FlexTravelContext context, BookingCalculator calculator, NotificationService notifications)
        {
            _context = context;
            _calculator = calculator;
            _notifications = notifications;
        }

        public int SeatsReleased(int tripId)
        {
            var trip = _context.Trip
                .Where(t => t.TripId == tripId)
                .Select(t => new { t.TripName, t.Capacity, t.StartDate, t.IsActive })
                .FirstOrDefault();

            if (trip == null || !trip.IsActive || trip.StartDate.Date <= DateTime.Today)
            {
                return 0;
            }

            var seatsLeft = trip.Capacity - _calculator.SeatsTaken(tripId);
            if (seatsLeft <= 0)
            {
                return 0;
            }

            var waiting = _context.WaitlistEntry
                .Where(w => w.TripId == tripId && w.IsActive && w.Adults + w.Children <= seatsLeft)
                .OrderBy(w => w.CreatedAt)
                .ToList();

            foreach (var entry in waiting)
            {
                entry.NotifiedAt = DateTime.UtcNow;
                _notifications.Send(entry.UserId, NotificationType.SeatsAvailable,
                    "Seats are available",
                    $"{seatsLeft} {(seatsLeft == 1 ? "seat is" : "seats are")} free on {trip.TripName}. Seats are not held, so book soon.",
                    $"/trips/{tripId}");
            }

            if (waiting.Count > 0)
            {
                _context.SaveChanges();
            }

            return waiting.Count;
        }
    }
}
