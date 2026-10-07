using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfReplyToReviewCommand : EfUseCase, IReplyToReviewCommand
    {
        private readonly CurrentAgency _agency;
        private readonly IUser _user;
        private readonly ReplyToReviewValidator _validator;
        private readonly NotificationService _notifications;

        public EfReplyToReviewCommand(FlexTravelContext context, CurrentAgency agency, IUser user,
            ReplyToReviewValidator validator, NotificationService notifications) : base(context)
        {
            _agency = agency;
            _user = user;
            _validator = validator;
            _notifications = notifications;
        }

        public int UseCaseId => UseCaseIds.ReplyToReview;
        public string UseCaseName => "Reply to review";
        public string UseCaseDescription => "Agency replies to a review of its trip";

        public void Execute(ReplyToReviewDto request)
        {
            var agencyId = _agency.Id(UseCaseName);
            _validator.ValidateAndThrow(request);

            var review = Context.Review
                .Include(r => r.Trip).ThenInclude(t => t.Agency)
                .FirstOrDefault(r => r.ReviewId == request.ReviewId && r.Trip.AgencyId == agencyId && r.IsActive);

            if (review == null)
            {
                throw new EntityNotFoundException("Review", request.ReviewId);
            }

            if (review.AgencyReply != null)
            {
                throw new ConflictException("This review already has a reply.");
            }

            review.AgencyReply = request.Reply.Trim();
            review.AgencyReplyAt = DateTime.UtcNow;
            review.AgencyReplyByUserId = _user.Id;

            _notifications.Send(review.UserId, NotificationType.ReviewReplied,
                $"{review.Trip.Agency.AgencyName} replied to your review",
                review.AgencyReply.Length <= 140 ? review.AgencyReply : review.AgencyReply[..139] + "…",
                $"/reservations/{review.ReservationId}");
            Context.SaveChanges();
            _notifications.Push();
        }
    }
}
