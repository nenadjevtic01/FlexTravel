using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfModerateReviewCommand : EfUseCase, IModerateReviewCommand
    {
        private readonly IUser _user;
        private readonly ModerateReviewValidator _validator;
        private readonly NotificationService _notifications;

        public EfModerateReviewCommand(FlexTravelContext context, IUser user, ModerateReviewValidator validator,
            NotificationService notifications) : base(context)
        {
            _user = user;
            _validator = validator;
            _notifications = notifications;
        }

        public int UseCaseId => UseCaseIds.ModerateReview;
        public string UseCaseName => "Moderate review";
        public string UseCaseDescription => "Hide or show a review";

        public void Execute(ModerateReviewDto request)
        {
            _validator.ValidateAndThrow(request);

            var review = Context.Review
                .Include(r => r.Trip)
                .FirstOrDefault(r => r.ReviewId == request.ReviewId);

            if (review == null)
            {
                throw new EntityNotFoundException("Review", request.ReviewId);
            }

            if (review.IsActive == request.Visible)
            {
                return;
            }

            review.IsActive = request.Visible;
            review.ModerationNote = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim();
            review.ModeratedAt = DateTime.UtcNow;
            review.ModeratedByUserId = _user.Id;

            if (!request.Visible)
            {
                _notifications.Send(review.UserId, NotificationType.ReviewHidden,
                    "Your review was hidden",
                    $"Your review of {review.Trip.TripName} was hidden by a moderator: {review.ModerationNote}",
                    $"/reservations/{review.ReservationId}");
            }

            Context.SaveChanges();
            _notifications.Push();
        }
    }
}
