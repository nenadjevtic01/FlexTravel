using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Notifications;

namespace FlexTravel.Implementation.Booking
{
    public class PriceAlertService
    {
        private readonly FlexTravelContext _context;
        private readonly ISearchTripsQuery _search;
        private readonly NotificationService _notifications;

        public PriceAlertService(FlexTravelContext context, ISearchTripsQuery search, NotificationService notifications)
        {
            _context = context;
            _search = search;
            _notifications = notifications;
        }

        public TripListDto Cheapest(int destinationId) =>
            _search.Execute(new TripSearchDto { DestinationId = destinationId, Sort = "price", PerPage = 1 })
                .Items
                .FirstOrDefault();

        public int Check(int destinationId)
        {
            var alerts = _context.PriceAlert
                .Where(a => a.DestinationId == destinationId && a.User.IsActive)
                .ToList();

            if (alerts.Count == 0)
            {
                return 0;
            }

            var cheapest = Cheapest(destinationId);
            var sent = 0;

            foreach (var alert in alerts)
            {
                if (cheapest == null || cheapest.PriceFrom > alert.MaxPrice)
                {
                    alert.LastNotifiedPrice = null;
                    continue;
                }

                if (alert.LastNotifiedPrice.HasValue && cheapest.PriceFrom >= alert.LastNotifiedPrice.Value)
                {
                    continue;
                }

                alert.LastNotifiedPrice = cheapest.PriceFrom;
                alert.LastNotifiedAt = DateTime.UtcNow;
                _notifications.Send(alert.UserId, NotificationType.PriceDrop,
                    $"Price alert: {cheapest.Destination}",
                    $"{cheapest.Name} by {cheapest.Agency} now costs from {BookingPricing.Euro(cheapest.PriceFrom)} per person"
                        + $" (your limit is {BookingPricing.Euro(alert.MaxPrice)}).",
                    $"/trips/{cheapest.Id}");
                sent++;
            }

            _context.SaveChanges();
            return sent;
        }

        public int CheckAll() =>
            _context.PriceAlert
                .Select(a => a.DestinationId)
                .Distinct()
                .ToList()
                .Sum(Check);
    }
}
