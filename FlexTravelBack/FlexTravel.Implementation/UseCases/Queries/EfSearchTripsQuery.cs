using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfSearchTripsQuery : EfUseCase, ISearchTripsQuery
    {
        public EfSearchTripsQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.SearchTrips;
        public string UseCaseName => "Search trips";
        public string UseCaseDescription => "Search upcoming trips";

        public PagedResponse<TripListDto> Execute(TripSearchDto search)
        {
            var today = DateTime.Today;
            var query = Context.Trip.Where(x => x.IsActive && x.StartDate > today);

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var keyword = search.Keyword.Trim();
                query = query.Where(x => x.TripName.Contains(keyword)
                                      || x.Destination.DestinationName.Contains(keyword)
                                      || x.Destination.Country.CountryName.Contains(keyword)
                                      || x.Agency.AgencyName.Contains(keyword));
            }

            if (search.DestinationId.HasValue)
            {
                query = query.Where(x => x.DestinationId == search.DestinationId.Value);
            }

            if (search.AgencyId.HasValue)
            {
                query = query.Where(x => x.AgencyId == search.AgencyId.Value);
            }

            if (search.CountryId.HasValue)
            {
                query = query.Where(x => x.Destination.CountryId == search.CountryId.Value);
            }

            if (search.ContinentId.HasValue)
            {
                query = query.Where(x => x.Destination.Country.ContinentId == search.ContinentId.Value);
            }

            if (search.TransportId.HasValue)
            {
                query = query.Where(x => x.TransportId == search.TransportId.Value);
            }

            if (search.DateFrom.HasValue)
            {
                query = query.Where(x => x.StartDate >= search.DateFrom.Value.Date);
            }

            if (search.DateTo.HasValue)
            {
                query = query.Where(x => x.StartDate <= search.DateTo.Value.Date);
            }

            var trips = query
                .Select(x => new
                {
                    x.TripId,
                    x.TripName,
                    x.DestinationId,
                    x.AgencyId,
                    x.TripSeriesId,
                    x.EarlyBookingPercent,
                    x.EarlyBookingUntil,
                    x.LastMinutePercent,
                    x.LastMinuteDays,
                    Destination = x.Destination.DestinationName,
                    Country = x.Destination.Country.CountryName,
                    Image = x.Destination.Images.OrderBy(i => i.DestinationImageId).Select(i => i.Src).FirstOrDefault(),
                    Agency = x.Agency.AgencyName,
                    Transport = x.Transport.TransportName,
                    x.StartDate,
                    x.EndDate,
                    x.Capacity,
                    SeatsTaken = x.Reservations
                        .Where(r => r.IsActive && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                        .Sum(r => (int?)(r.NumberOfAdults + r.NumberOfChildren)) ?? 0,
                    Prices = x.Prices.Select(p => new { p.ValidFrom, p.Price }).ToList(),
                    Rooms = x.Accommodations
                        .SelectMany(a => a.Accommodation.Rooms)
                        .Select(r => new
                        {
                            r.RoomType.Capacity,
                            Prices = r.Prices.Select(p => new { p.ValidFrom, p.Price }).ToList()
                        })
                        .ToList()
                })
                .ToList();

            var ratings = Context.Review
                .Where(r => r.IsActive)
                .GroupBy(r => r.Trip.TripSeriesId)
                .Select(g => new { SeriesId = g.Key, Average = g.Average(r => (double)r.TripRating), Count = g.Count() })
                .ToDictionary(x => x.SeriesId);

            var departures = Context.Trip
                .Where(t => t.IsActive && t.StartDate > today)
                .GroupBy(t => t.TripSeriesId)
                .Select(g => new { SeriesId = g.Key, Count = g.Count() })
                .ToDictionary(x => x.SeriesId, x => x.Count);

            var list = trips
                .Select(x =>
                {
                    var nights = BookingPricing.Nights(x.StartDate, x.EndDate);
                    var price = BookingPricing.PriceAt(x.Prices.Select(p => (p.ValidFrom, p.Price)), DateTime.Now);
                    var cheapestBed = x.Rooms
                        .Where(r => r.Capacity > 0)
                        .Select(r => BookingPricing.PriceAt(r.Prices.Select(p => (p.ValidFrom, p.Price)), x.StartDate) / r.Capacity)
                        .DefaultIfEmpty(0)
                        .Min();
                    var (offer, offerPercent) = BookingPricing.OfferAt(x.StartDate, x.EarlyBookingPercent, x.EarlyBookingUntil,
                        x.LastMinutePercent, x.LastMinuteDays, DateTime.Today);
                    var priceFrom = Math.Round(price + cheapestBed * nights, 0);
                    var rating = ratings.GetValueOrDefault(x.TripSeriesId);

                    return new TripListDto
                    {
                        Id = x.TripId,
                        Name = x.TripName,
                        DestinationId = x.DestinationId,
                        Destination = x.Destination,
                        Country = x.Country,
                        Image = x.Image,

                        Agency = x.Agency,
                        Transport = x.Transport,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Nights = nights,
                        Capacity = x.Capacity,
                        SeatsLeft = Math.Max(0, x.Capacity - x.SeatsTaken),
                        PricePerPerson = price,
                        PriceFromBeforeDiscount = priceFrom,
                        PriceFrom = Math.Round(priceFrom - BookingPricing.Discount(priceFrom, offerPercent), 0),
                        Offer = BookingPricing.OfferName(offer),
                        OfferPercent = offerPercent,
                        Rating = Ratings.Round(rating?.Average),
                        ReviewCount = rating?.Count ?? 0,
                        OtherDepartures = Math.Max(0, departures.GetValueOrDefault(x.TripSeriesId) - 1)
                    };
                });

            if (search.Travelers.HasValue)
            {
                list = list.Where(x => x.SeatsLeft >= search.Travelers.Value);
            }

            if (search.MaxPrice.HasValue)
            {
                list = list.Where(x => x.PriceFrom <= search.MaxPrice.Value);
            }

            if (search.Deals)
            {
                list = list.Where(x => x.Offer != null);
            }

            list = search.Sort switch
            {
                "price" => list.OrderBy(x => x.PriceFrom).ThenBy(x => x.StartDate),
                "priceDesc" => list.OrderByDescending(x => x.PriceFrom).ThenBy(x => x.StartDate),
                "rating" => list.OrderByDescending(x => x.Rating ?? 0).ThenByDescending(x => x.ReviewCount).ThenBy(x => x.StartDate),
                _ => list.OrderBy(x => x.StartDate).ThenBy(x => x.Name)
            };

            var all = list.ToList();
            var perPage = Math.Clamp(search.PerPage, 1, 48);
            var page = Math.Max(search.Page, 1);

            return new PagedResponse<TripListDto>
            {
                TotalCount = all.Count,
                CurrentPage = page,
                ItemsPerPage = perPage,
                Items = all.Skip((page - 1) * perPage).Take(perPage).ToList()
            };
        }
    }
}
