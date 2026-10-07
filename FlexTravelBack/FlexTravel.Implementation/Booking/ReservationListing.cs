using FlexTravel.Application.Dto;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.Booking
{
    public static class ReservationListing
    {
        public static IQueryable<Reservation> FilterByStatus(IQueryable<Reservation> query, string status)
        {
            return !string.IsNullOrWhiteSpace(status) && Enum.TryParse<ReservationStatus>(status, true, out var parsed)
                ? query.Where(x => x.Status == parsed)
                : query;
        }

        public static PagedResponse<ReservationDto> Page(IQueryable<Reservation> query, PagedSearch search, bool includeCustomer)
        {
            var perPage = Math.Clamp(search.PerPage, 1, 50);
            var page = Math.Max(search.Page, 1);
            var total = query.Count();

            var items = query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(x => new
                {
                    x.ReservationId,
                    x.Code,
                    x.Status,
                    x.CreatedAt,
                    x.StatusChangedAt,
                    x.TripId,
                    x.Trip.TripName,
                    x.Trip.StartDate,
                    x.Trip.EndDate,
                    Destination = x.Trip.Destination.DestinationName,
                    Country = x.Trip.Destination.Country.CountryName,
                    Image = x.Trip.Destination.Images.OrderBy(i => i.DestinationImageId).Select(i => i.Src).FirstOrDefault(),
                    Agency = x.Trip.Agency.AgencyName,
                    Accommodation = x.Accommodation.AccommodationName,
                    x.NumberOfAdults,
                    x.NumberOfChildren,
                    x.PricePerPerson,
                    x.ChildDiscountPercent,
                    x.Subtotal,
                    x.Offer,
                    x.OfferPercent,
                    PromoCode = x.PromoCode == null ? null : x.PromoCode.Code,
                    x.PromoPercent,
                    x.DiscountTotal,
                    x.ExtrasTotal,
                    x.TotalPrice,
                    x.CancellationFee,
                    x.Note,
                    x.UserId,
                    Paid = x.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                    ConfirmedAt = x.StatusChanges
                        .Where(c => c.Status == ReservationStatus.Confirmed)
                        .Min(c => (DateTime?)c.ChangedAt),

                    UnreadMessages = x.Messages.Count(m => m.ReadAt == null
                        && (includeCustomer ? m.SenderUserId == x.UserId : m.SenderUserId != x.UserId)),
                    Review = x.Review == null ? null : new
                    {
                        Id = x.Review.ReviewId,
                        x.Review.TripRating,
                        x.Review.HotelRating,
                        x.Review.GuideRating,
                        x.Review.OrganisationRating,
                        x.Review.ValueRating,
                        x.Review.Comment,
                        x.Review.CreatedAt,
                        x.Review.AgencyReply,
                        IsHidden = !x.Review.IsActive
                    },
                    CustomerName = x.User.FirstName + " " + x.User.LastName,
                    CustomerEmail = x.User.Email,
                    CustomerPhone = x.User.Phone,
                    Handled = x.StatusChanges
                        .Where(c => c.Status == ReservationStatus.Confirmed || c.Status == ReservationStatus.Rejected)
                        .OrderByDescending(c => c.ChangedAt)
                        .Select(c => new { Name = c.ChangedBy.FirstName + " " + c.ChangedBy.LastName, c.ChangedAt })
                        .FirstOrDefault(),
                    Rooms = x.Rooms.Select(r => new
                    {
                        r.RoomId,
                        r.Room.RoomType.RoomTypeName,
                        r.Room.RoomType.Capacity,
                        r.NumberOfRooms,
                        r.PricePerNight
                    }).ToList(),
                    Extras = x.Extras
                        .OrderBy(e => e.ReservationExtraId)
                        .Select(e => new { e.TripExtraId, e.Kind, e.Name, e.UnitPrice, e.Quantity, e.Total })
                        .ToList()
                })
                .ToList()
                .Select(x =>
                {
                    var nights = BookingPricing.Nights(x.StartDate, x.EndDate);
                    return new ReservationDto
                    {
                        Id = x.ReservationId,
                        Code = x.Code,
                        Status = x.Status.ToString(),
                        CreatedAt = x.CreatedAt,
                        StatusChangedAt = x.StatusChangedAt,
                        TripId = x.TripId,
                        TripName = x.TripName,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Destination = x.Destination,
                        Country = x.Country,
                        Image = x.Image,
                        Agency = x.Agency,
                        Accommodation = x.Accommodation,
                        Adults = x.NumberOfAdults,
                        Children = x.NumberOfChildren,
                        PricePerPerson = x.PricePerPerson,
                        ChildDiscountPercent = x.ChildDiscountPercent,
                        Subtotal = x.Subtotal,
                        Offer = BookingPricing.OfferName(x.Offer),
                        OfferPercent = x.OfferPercent,
                        PromoCode = x.PromoCode,
                        PromoPercent = x.PromoPercent,
                        DiscountTotal = x.DiscountTotal,
                        ExtrasTotal = x.ExtrasTotal,
                        TotalPrice = x.TotalPrice,
                        Extras = x.Extras.Select(e => new PriceQuoteExtraDto
                        {
                            ExtraId = e.TripExtraId,
                            Kind = e.Kind.ToString(),
                            Name = e.Name,
                            UnitPrice = e.UnitPrice,
                            Quantity = e.Quantity,
                            Total = e.Total
                        }).ToList(),
                        DecideBy = x.Status == ReservationStatus.Pending ? PendingRules.DecideBy(x.CreatedAt, x.StartDate) : null,
                        CancellationFee = x.CancellationFee,
                        Payment = PaymentRules.Summary(x.Status, x.TotalPrice, x.CancellationFee, x.StartDate,
                            x.ConfirmedAt, x.Paid, DateTime.Today),
                        UnreadMessages = x.UnreadMessages,
                        Review = x.Review == null ? null : new ReservationReviewDto
                        {
                            Id = x.Review.Id,
                            TripRating = x.Review.TripRating,
                            HotelRating = x.Review.HotelRating,
                            GuideRating = x.Review.GuideRating,
                            OrganisationRating = x.Review.OrganisationRating,
                            ValueRating = x.Review.ValueRating,
                            Comment = x.Review.Comment,
                            CreatedAt = x.Review.CreatedAt,
                            AgencyReply = x.Review.AgencyReply,
                            IsHidden = x.Review.IsHidden
                        },
                        CanReview = CanReview(x.Status, x.EndDate) && x.Review == null,
                        Note = x.Note,
                        Rooms = x.Rooms.Select(r => new PriceQuoteRoomDto
                        {
                            RoomId = r.RoomId,
                            RoomType = r.RoomTypeName,
                            Capacity = r.Capacity,
                            Count = r.NumberOfRooms,
                            PricePerNight = r.PricePerNight,
                            Total = BookingPricing.RoomTotal(r.PricePerNight, r.NumberOfRooms, nights)
                        }).ToList(),
                        CanCancel = CancellationRules.CanCancel(x.Status, x.StartDate, DateTime.Today),
                        HandledBy = x.Handled?.Name,
                        HandledAt = x.Handled?.ChangedAt,
                        Customer = includeCustomer
                            ? new ReservationCustomerDto { Name = x.CustomerName, Email = x.CustomerEmail, Phone = x.CustomerPhone }
                            : null
                    };
                })
                .ToList();

            return new PagedResponse<ReservationDto>
            {
                TotalCount = total,
                CurrentPage = page,
                ItemsPerPage = perPage,
                Items = items
            };
        }

        public static bool CanReview(ReservationStatus status, DateTime endDate) =>
            status == ReservationStatus.Confirmed && endDate.Date <= DateTime.Today;
    }
}
