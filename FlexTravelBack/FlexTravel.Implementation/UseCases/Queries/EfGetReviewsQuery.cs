using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetReviewsQuery : EfUseCase, IGetReviewsQuery
    {
        private readonly IUser _user;

        public EfGetReviewsQuery(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetReviews;
        public string UseCaseName => "Get reviews";
        public string UseCaseDescription => "Reviews of trips and hotels";

        public ReviewPageDto Execute(ReviewSearchDto search)
        {
            IQueryable<Review> query = Context.Review;

            var canModerate = _user.UseCaseIds.Contains(UseCaseIds.ModerateReview);

            if (!search.IncludeHidden || !canModerate)
            {
                query = query.Where(r => r.IsActive);
            }
            else if (search.HiddenOnly)
            {
                query = query.Where(r => !r.IsActive);
            }

            if (search.TripId.HasValue)
            {
                var seriesId = Context.Trip
                    .Where(t => t.TripId == search.TripId.Value)
                    .Select(t => (int?)t.TripSeriesId)
                    .FirstOrDefault();

                query = seriesId == null
                    ? query.Where(r => false)
                    : query.Where(r => r.Trip.TripSeriesId == seriesId.Value);
            }

            if (search.DestinationId.HasValue)
            {
                query = query.Where(r => r.Trip.DestinationId == search.DestinationId.Value);
            }

            if (search.AgencyId.HasValue)
            {
                query = query.Where(r => r.Trip.AgencyId == search.AgencyId.Value);
            }

            if (search.AccommodationId.HasValue)
            {
                query = query.Where(r => r.AccommodationId == search.AccommodationId.Value);
            }

            if (search.Unanswered)
            {
                query = query.Where(r => r.AgencyReply == null && r.IsActive);
            }

            var perPage = Math.Clamp(search.PerPage, 1, 50);
            var page = Math.Max(search.Page, 1);
            var visible = query.Where(r => r.IsActive);

            return new ReviewPageDto
            {
                TotalCount = query.Count(),
                CurrentPage = page,
                ItemsPerPage = perPage,
                AverageTripRating = Ratings.Trip(visible).Average,
                AverageHotelRating = Ratings.Hotel(visible).Average,
                Items = query
                    .OrderByDescending(r => r.CreatedAt)
                    .ThenByDescending(r => r.ReviewId)
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .Select(ReviewProjection.ToDto)
                    .ToList()
            };
        }
    }

    public static class ReviewProjection
    {
        public static readonly System.Linq.Expressions.Expression<Func<Review, ReviewDto>> ToDto = r => new ReviewDto
        {
            Id = r.ReviewId,
            ReservationId = r.ReservationId,
            TripId = r.TripId,
            TripName = r.Trip.TripName,
            TripStartDate = r.Trip.StartDate,
            AgencyId = r.Trip.AgencyId,
            Agency = r.Trip.Agency.AgencyName,
            DestinationId = r.Trip.DestinationId,
            Destination = r.Trip.Destination.DestinationName,
            HotelId = r.AccommodationId,
            Hotel = r.Accommodation.AccommodationName,
            Author = r.User.FirstName + " " + r.User.LastName.Substring(0, 1) + ".",
            AuthorAvatar = r.User.Avatar,
            Adults = r.Reservation.NumberOfAdults,
            Children = r.Reservation.NumberOfChildren,
            TripRating = r.TripRating,
            HotelRating = r.HotelRating,
            GuideRating = r.GuideRating,
            OrganisationRating = r.OrganisationRating,
            ValueRating = r.ValueRating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            AgencyReply = r.AgencyReply,
            AgencyReplyBy = r.AgencyReplyBy == null ? null : r.AgencyReplyBy.FirstName + " " + r.AgencyReplyBy.LastName,
            AgencyReplyAt = r.AgencyReplyAt,
            IsHidden = !r.IsActive,
            ModerationNote = r.ModerationNote,
            ModeratedBy = r.ModeratedBy == null ? null : r.ModeratedBy.FirstName + " " + r.ModeratedBy.LastName,
            ModeratedAt = r.ModeratedAt
        };
    }
}
