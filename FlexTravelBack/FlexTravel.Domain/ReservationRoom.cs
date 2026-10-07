namespace FlexTravel.Domain
{
    public class ReservationRoom : Entity
    {
        public int ReservationRoomId { get; set; }
        public int ReservationId { get; set; }
        public int RoomId { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal PricePerNight { get; set; }

        public virtual Room Room { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
