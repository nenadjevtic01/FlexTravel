namespace FlexTravel.Domain
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public int Capacity { get; set; }

        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
