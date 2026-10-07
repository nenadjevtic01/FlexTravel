using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetReservationQuery : EfUseCase, IGetReservationQuery
    {
        private readonly IUser _user;

        public EfGetReservationQuery(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetReservation;
        public string UseCaseName => "Get reservation";
        public string UseCaseDescription => "Reservation details with price breakdown and history";

        public ReservationDetailDto Execute(int id)
        {
            var agencyId = Context.User
                .OfType<AgencyEmployee>()
                .Where(e => e.UserId == _user.Id)
                .Select(e => (int?)e.AgencyId)
                .FirstOrDefault();

            var query = Context.Reservation.Where(x => x.ReservationId == id && x.IsActive);
            query = agencyId.HasValue
                ? query.Where(x => x.Trip.AgencyId == agencyId.Value)
                : query.Where(x => x.UserId == _user.Id);

            var reservation = ReservationListing
                .Page(query, new ReservationSearchDto { Page = 1, PerPage = 1 }, includeCustomer: agencyId.HasValue)
                .Items
                .FirstOrDefault();

            if (reservation == null)
            {
                throw new EntityNotFoundException("Reservation", id);
            }

            var extra = query
                .Select(x => new
                {
                    AgencyAddress = x.Trip.Agency.Address,
                    AgencyPhone = x.Trip.Agency.Phone,
                    Travelers = x.Travelers
                        .OrderBy(t => t.ReservationTravelerId)
                        .Select(t => new TravelerDto
                        {
                            FirstName = t.FirstName,
                            LastName = t.LastName,
                            DateOfBirth = t.DateOfBirth,
                            IsChild = t.IsChild
                        })
                        .ToList(),
                    Payments = x.Payments
                        .OrderBy(p => p.PaidAt)
                        .Select(p => new
                        {
                            p.PaymentId,
                            p.Amount,
                            p.Method,
                            p.PaidAt,
                            RecordedBy = p.RecordedBy.FirstName + " " + p.RecordedBy.LastName,
                            p.Note
                        })
                        .ToList(),
                    History = x.StatusChanges
                        .OrderBy(c => c.ChangedAt)
                        .Select(c => new
                        {
                            c.Status,
                            c.ChangedAt,
                            ChangedBy = c.ChangedByUserId == null ? null : c.ChangedBy.FirstName + " " + c.ChangedBy.LastName,
                            IsSystem = c.ChangedByUserId == null,
                            IsEmployee = c.ChangedBy is AgencyEmployee,
                            IsAdmin = c.ChangedBy is Admin,
                            c.Comment
                        })
                        .ToList()
                })
                .First();

            var nights = BookingPricing.Nights(reservation.StartDate, reservation.EndDate);
            var status = Enum.Parse<ReservationStatus>(reservation.Status);
            var feePercentToday = CancellationRules.FeePercent(status, reservation.StartDate, DateTime.Today);
            var childPrice = BookingPricing.ChildPrice(reservation.PricePerPerson, reservation.ChildDiscountPercent);
            var offerDiscount = BookingPricing.Discount(reservation.Subtotal, reservation.OfferPercent);

            var detail = new ReservationDetailDto
            {
                Nights = nights,
                ChildPrice = childPrice,
                AdultsTotal = reservation.Adults * reservation.PricePerPerson,
                ChildrenTotal = reservation.Children * childPrice,
                RoomsTotal = reservation.Rooms.Sum(r => r.Total),
                OfferDiscount = offerDiscount,
                PromoDiscount = reservation.DiscountTotal - offerDiscount,
                AgencyAddress = extra.AgencyAddress,
                AgencyPhone = extra.AgencyPhone,
                FreeCancellationUntil = CancellationRules.FreeUntil(reservation.StartDate),
                CancellationFeePercentToday = feePercentToday,
                CancellationFeeToday = CancellationRules.Fee(reservation.TotalPrice, feePercentToday),
                CancellationPolicy = CancellationRules.Policy(reservation.StartDate),
                Travelers = extra.Travelers,
                Payments = extra.Payments.Select(p => new PaymentDto
                {
                    Id = p.PaymentId,
                    Amount = p.Amount,
                    Method = p.Method.ToString(),
                    PaidAt = p.PaidAt,
                    RecordedBy = p.RecordedBy,
                    Note = p.Note
                }).ToList(),
                CanDecide = agencyId.HasValue && reservation.Status == ReservationStatus.Pending.ToString(),
                History = extra.History.Select(h => new ReservationHistoryDto
                {
                    Status = h.Status.ToString(),
                    ChangedAt = h.ChangedAt,
                    ChangedBy = h.ChangedBy ?? "Automatic",
                    Role = h.IsSystem ? "System" : h.IsEmployee ? "Agency" : h.IsAdmin ? "Admin" : "Customer",
                    Comment = h.Comment
                }).ToList()
            };

            CopyListFields(reservation, detail);

            detail.CanCancel = !agencyId.HasValue && reservation.CanCancel;
            detail.CanReview = !agencyId.HasValue && reservation.CanReview;
            detail.CanEditTravelers = !agencyId.HasValue && BookingPricing.HoldsCapacity(status)
                && reservation.StartDate.Date > DateTime.Today;
            detail.CanRecordPayment = agencyId.HasValue
                && ((status == ReservationStatus.Confirmed && reservation.Payment.Balance > 0) || reservation.Payment.RefundDue > 0);
            return detail;
        }

        private static void CopyListFields(ReservationDto from, ReservationDetailDto to)
        {
            foreach (var property in typeof(ReservationDto).GetProperties())
            {
                property.SetValue(to, property.GetValue(from));
            }
        }
    }
}
