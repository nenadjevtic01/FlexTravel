using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfUpdateTravelersCommand : EfUseCase, IUpdateTravelersCommand
    {
        private readonly IUser _user;

        public EfUpdateTravelersCommand(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.UpdateTravelers;
        public string UseCaseName => "Update travelers";
        public string UseCaseDescription => "Correct the names of travelers on a reservation";

        public void Execute(UpdateTravelersDto request)
        {
            var reservation = Context.Reservation
                .Include(r => r.Trip)
                .Include(r => r.Travelers)
                .FirstOrDefault(r => r.ReservationId == request.ReservationId && r.UserId == _user.Id && r.IsActive)
                ?? throw new EntityNotFoundException("Reservation", request.ReservationId);

            if (!BookingPricing.HoldsCapacity(reservation.Status) || reservation.Trip.StartDate.Date <= DateTime.Today)
            {
                throw new ConflictException("Travelers can be changed only on active reservations before departure.");
            }

            var problems = TravelerRules.Check(request.Travelers, reservation.NumberOfAdults, reservation.NumberOfChildren,
                reservation.Trip.StartDate, DateTime.Today);
            if (problems.Count > 0)
            {
                throw new ValidationException(problems);
            }

            Context.RemoveRange(reservation.Travelers);
            foreach (var traveler in TravelerRules.ToEntities(request.Travelers, reservation.Trip.StartDate))
            {
                reservation.Travelers.Add(traveler);
            }

            Context.SaveChanges();
        }
    }

    public class EfRecordPaymentCommand : EfUseCase, IRecordPaymentCommand
    {
        private readonly CurrentAgency _agency;
        private readonly IUser _user;
        private readonly NotificationService _notifications;

        public EfRecordPaymentCommand(FlexTravelContext context, CurrentAgency agency, IUser user, NotificationService notifications)
            : base(context)
        {
            _agency = agency;
            _user = user;
            _notifications = notifications;
        }

        public int UseCaseId => UseCaseIds.RecordPayment;
        public string UseCaseName => "Record payment";
        public string UseCaseDescription => "Agency records a payment or refund";

        public int Execute(RecordPaymentDto request)
        {
            var agencyId = _agency.Id(UseCaseName);

            if (!Enum.TryParse<PaymentMethod>(request.Method, true, out var method) || !Enum.IsDefined(method))
            {
                throw new ValidationException(new[] { new ValidationFailure("Method", "Choose cash, card or bank transfer.") });
            }

            if (request.Amount <= 0 || request.Amount != Math.Round(request.Amount, 2))
            {
                throw new ValidationException(new[] { new ValidationFailure("Amount", "Enter an amount greater than 0, in euros and cents.") });
            }

            var reservation = Context.Reservation
                .Include(r => r.Trip)
                .Include(r => r.Payments)
                .Include(r => r.StatusChanges)
                .FirstOrDefault(r => r.ReservationId == request.ReservationId && r.Trip.AgencyId == agencyId && r.IsActive)
                ?? throw new EntityNotFoundException("Reservation", request.ReservationId);

            var summary = PaymentRules.Summary(reservation.Status, reservation.TotalPrice, reservation.CancellationFee,
                reservation.Trip.StartDate,
                reservation.StatusChanges.Where(c => c.Status == ReservationStatus.Confirmed).Select(c => (DateTime?)c.ChangedAt).Min(),
                reservation.Payments.Sum(p => p.Amount), DateTime.Today);

            if (request.IsRefund)
            {
                if (summary.RefundDue <= 0)
                {
                    throw new ConflictException("Nothing is due back to the customer.");
                }

                if (request.Amount > summary.RefundDue)
                {
                    throw new ConflictException($"At most {BookingPricing.Euro(summary.RefundDue)} is due back to the customer.");
                }
            }
            else
            {
                if (reservation.Status == ReservationStatus.Pending)
                {
                    throw new ConflictException("Confirm the reservation before taking a payment.");
                }

                if (summary.Balance <= 0)
                {
                    throw new ConflictException("This reservation is fully paid.");
                }

                if (request.Amount > summary.Balance)
                {
                    throw new ConflictException($"Only {BookingPricing.Euro(summary.Balance)} is still owed.");
                }
            }

            var payment = new Payment
            {
                Amount = request.IsRefund ? -request.Amount : request.Amount,
                Method = method,
                PaidAt = DateTime.UtcNow,
                RecordedByUserId = _user.Id,
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim()[..Math.Min(200, request.Note.Trim().Length)]
            };
            reservation.Payments.Add(payment);

            _notifications.Send(reservation.UserId, NotificationType.PaymentRecorded,
                request.IsRefund ? $"Refund for {reservation.Code}" : $"Payment received for {reservation.Code}",
                request.IsRefund
                    ? $"{BookingPricing.Euro(request.Amount)} has been returned to you."
                    : $"We received {BookingPricing.Euro(request.Amount)} for {reservation.Trip.TripName}. Thank you!",
                $"/reservations/{reservation.ReservationId}");
            Context.SaveChanges();
            _notifications.Push();

            return payment.PaymentId;
        }
    }

    public class EfSendReservationMessageCommand : EfUseCase, ISendReservationMessageCommand
    {
        private readonly ReservationAccess _access;
        private readonly IUser _user;
        private readonly NotificationService _notifications;

        public EfSendReservationMessageCommand(FlexTravelContext context, ReservationAccess access, IUser user,
            NotificationService notifications) : base(context)
        {
            _access = access;
            _user = user;
            _notifications = notifications;
        }

        public int UseCaseId => UseCaseIds.SendReservationMessage;
        public string UseCaseName => "Send reservation message";
        public string UseCaseDescription => "Write to the customer or the agency about a reservation";

        public int Execute(SendMessageDto request)
        {
            var text = request.Text?.Trim();
            if (string.IsNullOrEmpty(text) || text.Length > 2000)
            {
                throw new ValidationException(new[] { new ValidationFailure("Text", "Write a message of up to 2000 characters.") });
            }

            var reservation = _access.Visible()
                .Where(r => r.ReservationId == request.ReservationId)
                .Select(r => new { r.ReservationId, r.Code, r.UserId, r.Trip.AgencyId, AgencyName = r.Trip.Agency.AgencyName })
                .FirstOrDefault()
                ?? throw new EntityNotFoundException("Reservation", request.ReservationId);

            var message = new ReservationMessage
            {
                ReservationId = reservation.ReservationId,
                SenderUserId = _user.Id,
                Text = text,
                SentAt = DateTime.UtcNow
            };
            Context.ReservationMessage.Add(message);

            var preview = text.Length <= 140 ? text : text[..139] + "…";
            var link = $"/reservations/{reservation.ReservationId}";
            if (_access.IsAgency)
            {
                _notifications.Send(reservation.UserId, NotificationType.NewMessage,
                    $"Message from {reservation.AgencyName}", preview, link);
            }
            else
            {
                _notifications.SendToAgency(reservation.AgencyId, NotificationType.NewMessage,
                    $"Message about {reservation.Code} from {_user.Identity}", preview, link);
            }

            Context.SaveChanges();
            _notifications.Push();
            return message.ReservationMessageId;
        }
    }
}

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetReservationMessagesQuery : EfUseCase, IGetReservationMessagesQuery, ISkipAuditLog
    {
        private readonly ReservationAccess _access;
        private readonly IUser _user;

        public EfGetReservationMessagesQuery(FlexTravelContext context, ReservationAccess access, IUser user) : base(context)
        {
            _access = access;
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetReservationMessages;
        public string UseCaseName => "Get reservation messages";
        public string UseCaseDescription => "Messages between the customer and the agency";

        public IEnumerable<ReservationMessageDto> Execute(int reservationId)
        {
            var customerId = _access.Visible()
                .Where(r => r.ReservationId == reservationId)
                .Select(r => (int?)r.UserId)
                .FirstOrDefault()
                ?? throw new EntityNotFoundException("Reservation", reservationId);

            var messages = Context.ReservationMessage
                .Include(m => m.Sender)
                .Where(m => m.ReservationId == reservationId)
                .OrderBy(m => m.SentAt)
                .ToList();

            var now = DateTime.UtcNow;
            var unreadForMe = messages.Where(m => m.ReadAt == null
                && (_access.IsAgency ? m.SenderUserId == customerId : m.SenderUserId != customerId)).ToList();
            foreach (var message in unreadForMe)
            {
                message.ReadAt = now;
            }

            if (unreadForMe.Count > 0)
            {
                Context.SaveChanges();
            }

            return messages.Select(m => new ReservationMessageDto
            {
                Id = m.ReservationMessageId,
                Text = m.Text,
                SentAt = m.SentAt,
                Sender = m.Sender.FirstName + " " + m.Sender.LastName,
                SenderAvatar = m.Sender.Avatar,
                FromAgency = m.SenderUserId != customerId,
                IsMine = m.SenderUserId == _user.Id,
                IsRead = m.ReadAt != null
            }).ToList();
        }
    }

    public class EfGetTripManifestQuery : EfUseCase, IGetTripManifestQuery
    {
        private readonly CurrentAgency _agency;

        public EfGetTripManifestQuery(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.GetTripManifest;
        public string UseCaseName => "Get trip manifest";
        public string UseCaseDescription => "Passenger list of a departure";

        public TripManifestDto Execute(int tripId)
        {
            var agencyId = _agency.Id(UseCaseName);
            var trip = Context.Trip
                .Where(t => t.TripId == tripId && t.AgencyId == agencyId)
                .Select(t => new
                {
                    t.TripId,
                    t.TripName,
                    t.StartDate,
                    t.EndDate,
                    t.Capacity,
                    Destination = t.Destination.DestinationName + ", " + t.Destination.Country.CountryName,
                    Transport = t.Transport.TransportName
                })
                .FirstOrDefault()
                ?? throw new EntityNotFoundException("Trip", tripId);

            var reservations = Context.Reservation
                .Where(r => r.TripId == tripId && r.IsActive
                    && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                .OrderBy(r => r.User.LastName).ThenBy(r => r.Code)
                .Select(r => new
                {
                    r.ReservationId,
                    r.Code,
                    r.Status,
                    r.TotalPrice,
                    r.CancellationFee,
                    r.Note,
                    Customer = r.User.FirstName + " " + r.User.LastName,
                    r.User.Phone,
                    r.User.Email,
                    Hotel = r.Accommodation.AccommodationName,
                    Rooms = r.Rooms.Select(x => new { x.NumberOfRooms, x.Room.RoomType.RoomTypeName }).ToList(),
                    Paid = r.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                    ConfirmedAt = r.StatusChanges.Where(c => c.Status == ReservationStatus.Confirmed).Min(c => (DateTime?)c.ChangedAt),
                    Travelers = r.Travelers.OrderBy(t => t.ReservationTravelerId)
                        .Select(t => new { t.FirstName, t.LastName, t.DateOfBirth, t.IsChild }).ToList()
                })
                .ToList();

            return new TripManifestDto
            {
                TripId = trip.TripId,
                TripName = trip.TripName,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Destination = trip.Destination,
                Transport = trip.Transport,
                Capacity = trip.Capacity,
                Travelers = reservations.Sum(r => r.Travelers.Count),
                Reservations = reservations.Select(r => new ManifestReservationDto
                {
                    ReservationId = r.ReservationId,
                    Code = r.Code,
                    Status = r.Status.ToString(),
                    Customer = r.Customer,
                    Phone = r.Phone,
                    Email = r.Email,
                    Hotel = r.Hotel,
                    Rooms = string.Join(", ", r.Rooms.Select(x => $"{x.NumberOfRooms} × {x.RoomTypeName}")),
                    Note = r.Note,
                    PaymentStatus = PaymentRules.Summary(r.Status, r.TotalPrice, r.CancellationFee, trip.StartDate,
                        r.ConfirmedAt, r.Paid, DateTime.Today).Status,
                    Travelers = r.Travelers.Select(t => new ManifestTravelerDto
                    {
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        DateOfBirth = t.DateOfBirth,
                        Age = TravelerRules.AgeOn(t.DateOfBirth, trip.StartDate),
                        IsChild = t.IsChild
                    }).ToList()
                }).ToList()
            };
        }
    }
}
