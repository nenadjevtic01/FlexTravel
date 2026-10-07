using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfCreateReviewCommand : EfUseCase, ICreateReviewCommand
    {
        private readonly IUser _user;
        private readonly CreateReviewValidator _validator;
        private readonly NotificationService _notifications;

        public EfCreateReviewCommand(FlexTravelContext context, IUser user, CreateReviewValidator validator,
            NotificationService notifications) : base(context)
        {
            _user = user;
            _validator = validator;
            _notifications = notifications;
        }

        public int UseCaseId => UseCaseIds.CreateReview;
        public string UseCaseName => "Create review";
        public string UseCaseDescription => "Customer reviews a finished trip";

        public int Execute(CreateReviewDto request)
        {
            _validator.ValidateAndThrow(request);

            var reservation = Context.Reservation
                .Include(x => x.Trip)
                .FirstOrDefault(x => x.ReservationId == request.ReservationId && x.UserId == _user.Id && x.IsActive);

            if (reservation == null)
            {
                throw new EntityNotFoundException("Reservation", request.ReservationId);
            }

            if (reservation.Status != ReservationStatus.Confirmed)
            {
                throw new ConflictException("Only confirmed reservations can be reviewed.");
            }

            if (reservation.Trip.EndDate.Date > DateTime.Today)
            {
                throw new ConflictException(
                    $"You can review the trip after it ends on {reservation.Trip.EndDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture)}.");
            }

            if (Context.Review.Any(r => r.ReservationId == reservation.ReservationId))
            {
                throw new ConflictException("You have already reviewed this trip.");
            }

            var review = new Review
            {
                ReservationId = reservation.ReservationId,
                UserId = _user.Id,
                TripId = reservation.TripId,
                AccommodationId = reservation.AccommodationId,
                TripRating = request.TripRating,
                HotelRating = request.HotelRating,
                GuideRating = request.GuideRating,
                OrganisationRating = request.OrganisationRating,
                ValueRating = request.ValueRating,
                Comment = request.Comment.Trim()
            };

            Context.Review.Add(review);
            _notifications.SendToAgency(reservation.Trip.AgencyId, NotificationType.ReviewReceived,
                $"New review: {request.TripRating}★ for {reservation.Trip.TripName}",
                Excerpt(review.Comment),
                "/agency?tab=reviews");
            Context.SaveChanges();
            _notifications.Push();

            return review.ReviewId;
        }

        private static string Excerpt(string text) => text.Length <= 140 ? text : text[..139] + "…";
    }
}
