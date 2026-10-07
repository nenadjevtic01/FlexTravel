namespace FlexTravel.Domain
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }

        public virtual User User { get; set; }
    }

    public enum NotificationType
    {
        ReservationCreated = 1,
        ReservationConfirmed = 2,
        ReservationRejected = 3,
        ReservationCancelled = 4,
        SeatsAvailable = 5,
        NewTripToFavorite = 6,
        ReviewReceived = 7,
        ReviewReplied = 8,
        ReviewHidden = 9,
        NewMessage = 10,
        PaymentRecorded = 11,
        ReservationExpired = 12,
        PaymentReminder = 13,
        ReviewInvitation = 14,
        PriceDrop = 15
    }
}
