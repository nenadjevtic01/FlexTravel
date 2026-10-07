using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Booking;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetDestinationQuery : EfUseCase, IGetDestinationQuery
    {
        public EfGetDestinationQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetDestination;
        public string UseCaseName => "Get destination";
        public string UseCaseDescription => "Destination details with country info and accommodations";

        public DestinationDetailDto Execute(int id)
        {
            var destination = Context.Destination
                .Where(x => x.DestinationId == id && x.IsActive)
                .Select(x => new DestinationDetailDto
                {
                    Id = x.DestinationId,
                    Name = x.DestinationName,
                    Description = x.DestinationDescription,
                    Climate = x.Climate == null ? null : x.Climate.ClimateInfo,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Images = x.Images
                        .OrderBy(i => i.DestinationImageId)
                        .Select(i => new ImageDto { Src = i.Src, Alt = i.Alt })
                        .ToList(),
                    Country = new CountryDto
                    {
                        Id = x.Country.CountryId,
                        Name = x.Country.CountryName,
                        ContinentId = x.Country.ContinentId,
                        Continent = x.Country.Continent.ContinentName,
                        CapitalCity = x.Country.CapitalCity,
                        Currency = x.Country.Currency,
                        Language = x.Country.Language,
                        Population = x.Country.Population,
                        TotalArea = x.Country.TotalArea,
                        Visa = x.Country.VisaInfo == null ? null : new VisaDto
                        {
                            Type = x.Country.VisaInfo.VisaType,
                            Duration = x.Country.VisaInfo.Duration,
                            Requirements = x.Country.VisaInfo.Requirements
                        },
                        Safety = x.Country.Safety == null ? null : new SafetyDto
                        {
                            Level = x.Country.Safety.SafetyLevel,
                            Advices = x.Country.Safety.Advices
                        }
                    },
                    Accommodations = x.Accommodations
                        .Where(a => a.IsActive)
                        .OrderByDescending(a => a.AccommodationStars)
                        .Select(a => new AccommodationDto
                        {
                            Id = a.AccommodationId,
                            Name = a.AccommodationName,
                            Type = a.AccommodationType.AccommodationTypeName,
                            Stars = a.AccommodationStars,
                            Rooms = a.Rooms
                                .OrderBy(r => r.RoomType.Capacity)
                                .Select(r => new RoomDto
                                {
                                    Id = r.RoomId,
                                    RoomType = r.RoomType.RoomTypeName,
                                    Capacity = r.RoomType.Capacity,
                                    NumberOfRooms = r.NumberOfRooms,
                                    AC = r.AC,
                                    Balcony = r.Balcony
                                })
                                .ToList()
                        })
                        .ToList(),
                    Attractions = x.Attractions
                        .OrderBy(a => a.AttractionCategory.AttractionCategoryName)
                        .ThenBy(a => a.AttractionName)
                        .Select(a => new AttractionDto
                        {
                            Id = a.AttractionId,
                            Name = a.AttractionName,
                            Description = a.AttractionDescription,
                            Category = a.AttractionCategory.AttractionCategoryName
                        })
                        .ToList()
                })
                .AsSplitQuery()
                .FirstOrDefault();

            if (destination == null)
            {
                throw new EntityNotFoundException("Destination", id);
            }

            destination.Rating = Ratings.Trip(Context.Review.Where(r => r.Trip.DestinationId == id));

            var hotelRatings = Context.Review
                .Where(r => r.IsActive && r.Accommodation.DestinationId == id)
                .GroupBy(r => r.AccommodationId)
                .Select(g => new { Id = g.Key, Average = g.Average(r => (double)r.HotelRating), Count = g.Count() })
                .ToList();

            foreach (var hotel in destination.Accommodations)
            {
                var rating = hotelRatings.FirstOrDefault(h => h.Id == hotel.Id);
                hotel.Rating = Ratings.Create(rating?.Average, rating?.Count ?? 0);
            }

            return destination;
        }
    }
}
