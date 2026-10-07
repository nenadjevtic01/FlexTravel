namespace FlexTravel.Application.Dto
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        public string Role { get; set; }
        public string Agency { get; set; }
        public DateTime MemberSince { get; set; }
        public string Avatar { get; set; }
    }

    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        public string KeepRefreshToken { get; set; }
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public DateTime PaidAt { get; set; }
        public string RecordedBy { get; set; }
        public string Note { get; set; }
    }

    public class PaymentSummaryDto
    {
        public decimal AmountDue { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public decimal Deposit { get; set; }
        public DateTime? DepositDueDate { get; set; }
        public DateTime? BalanceDueDate { get; set; }
        public decimal RefundDue { get; set; }

        public string Status { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class RecordPaymentDto
    {
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }

        public string Method { get; set; }
        public bool IsRefund { get; set; }
        public string Note { get; set; }
    }

    public class CancellationTierDto
    {
        public int MinDaysBefore { get; set; }
        public int FeePercent { get; set; }
        public DateTime Until { get; set; }
    }

    public class TripManifestDto
    {
        public int TripId { get; set; }
        public string TripName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Destination { get; set; }
        public string Transport { get; set; }
        public int Capacity { get; set; }
        public int Travelers { get; set; }
        public IEnumerable<ManifestReservationDto> Reservations { get; set; } = new List<ManifestReservationDto>();
    }

    public class ManifestReservationDto
    {
        public int ReservationId { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Hotel { get; set; }
        public string Rooms { get; set; }
        public string PaymentStatus { get; set; }
        public string Note { get; set; }
        public IEnumerable<ManifestTravelerDto> Travelers { get; set; } = new List<ManifestTravelerDto>();
    }

    public class ManifestTravelerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int Age { get; set; }
        public bool IsChild { get; set; }
    }

    public class ReservationMessageDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
        public string Sender { get; set; }
        public string SenderAvatar { get; set; }
        public bool FromAgency { get; set; }
        public bool IsMine { get; set; }
        public bool IsRead { get; set; }
    }

    public class SendMessageDto
    {
        public int ReservationId { get; set; }
        public string Text { get; set; }
    }

    public class DepartureDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SeatsLeft { get; set; }
        public decimal PriceFrom { get; set; }
        public string Offer { get; set; }
        public int OfferPercent { get; set; }
    }

    public class SimilarTripsDto
    {
        public IEnumerable<DepartureDto> OtherDepartures { get; set; } = new List<DepartureDto>();

        public IEnumerable<TripListDto> Similar { get; set; } = new List<TripListDto>();
    }

    public class AddDepartureDto
    {
        public int TripId { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class SetActiveDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
