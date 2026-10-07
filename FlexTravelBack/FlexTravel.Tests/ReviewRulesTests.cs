using FlexTravel.Application.Dto;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.UseCases.Commands;
using FlexTravel.Implementation.Validators;
using FlexTravel.Tests.Infrastructure;
using FluentValidation;

namespace FlexTravel.Tests
{
    public class ReviewRulesTests : IDisposable
    {
        private const string Comment = "A well organised trip with a great guide and a lovely hotel.";

        private readonly TestDatabase _db = new TestDatabase();
        private readonly RecordingPublisher _publisher = new RecordingPublisher();
        private World World => _db.World;

        public void Dispose() => _db.Dispose();

        private int Review(User customer, Reservation reservation, int tripRating = 5, int hotelRating = 4, string comment = Comment) =>
            new EfCreateReviewCommand(_db.Context, new TestUser(customer), new CreateReviewValidator(),
                    new NotificationService(_db.Context, _publisher))
                .Execute(new CreateReviewDto
                {
                    ReservationId = reservation.ReservationId,
                    TripRating = tripRating,
                    HotelRating = hotelRating,
                    Comment = comment
                });

        private Reservation FinishedTrip(ReservationStatus status = ReservationStatus.Confirmed)
        {
            var trip = World.Trip(startsInDays: -10, nights: 5);
            return World.Reserve(trip, World.Customer, 2, status: status);
        }

        [Fact]
        public void A_confirmed_finished_trip_can_be_reviewed_once()
        {
            var reservation = FinishedTrip();

            Review(World.Customer, reservation);

            Assert.Throws<ConflictException>(() => Review(World.Customer, reservation));
            Assert.Single(_db.NewContext().Review);
        }

        [Fact]
        public void A_trip_that_has_not_ended_cannot_be_reviewed()
        {
            var trip = World.Trip(startsInDays: -2, nights: 5);
            var reservation = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);

            var error = Assert.Throws<ConflictException>(() => Review(World.Customer, reservation));
            Assert.StartsWith("You can review the trip after it ends", error.Message);
        }

        [Theory]
        [InlineData(ReservationStatus.Pending)]
        [InlineData(ReservationStatus.Rejected)]
        [InlineData(ReservationStatus.Cancelled)]
        public void Only_confirmed_reservations_can_be_reviewed(ReservationStatus status)
        {
            var reservation = FinishedTrip(status);

            Assert.Throws<ConflictException>(() => Review(World.Customer, reservation));
        }

        [Fact]
        public void Nobody_reviews_someone_elses_trip()
        {
            var reservation = FinishedTrip();

            Assert.Throws<FlexTravel.Application.Exceptions.EntityNotFoundException>(() => Review(World.OtherCustomer, reservation));
        }

        [Theory]
        [InlineData(0, 3, Comment)]
        [InlineData(6, 3, Comment)]
        [InlineData(4, 3, "Too short.")]
        public void Ratings_are_one_to_five_stars_with_a_real_comment(int tripRating, int hotelRating, string comment)
        {
            var reservation = FinishedTrip();

            Assert.Throws<ValidationException>(() => Review(World.Customer, reservation, tripRating, hotelRating, comment));
        }

        [Fact]
        public void The_agency_is_notified_about_a_new_review()
        {
            var reservation = FinishedTrip();

            Review(World.Customer, reservation, tripRating: 4);

            var pushed = Assert.Single(_publisher.Published);
            Assert.Equal(World.Employee.UserId, pushed.UserId);
            Assert.Equal("ReviewReceived", pushed.Notification.Type);
        }

        [Fact]
        public void Hidden_reviews_do_not_count_in_the_rating()
        {
            var first = FinishedTrip();
            var second = World.Reserve(first.Trip, World.OtherCustomer, 2, status: ReservationStatus.Confirmed);
            Review(World.Customer, first, tripRating: 5);
            var hiddenId = Review(World.OtherCustomer, second, tripRating: 1);

            var context = _db.NewContext();
            context.Review.Single(r => r.ReviewId == hiddenId).IsActive = false;
            context.SaveChanges();

            var rating = Ratings.Trip(_db.NewContext().Review);
            Assert.Equal(1, rating.Count);
            Assert.Equal(5, rating.Average);
        }
    }
}
