using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ReservationRoomConfiguration : EntityConfiguration<ReservationRoom>
    {

        protected override void ConfigureTableSpecifics(EntityTypeBuilder<ReservationRoom> builder)
        {
            builder.Property(x => x.ReservationRoomId).UseIdentityColumn(1, 1);
            builder.Property(x => x.ReservationId).IsRequired();
            builder.Property(x => x.RoomId).IsRequired();
            builder.Property(x => x.NumberOfRooms).IsRequired();
            builder.Property(x => x.PricePerNight).IsRequired().HasPrecision(10, 2);
            builder.HasIndex(x => new { x.ReservationId, x.RoomId }).IsUnique();
        }
    }
}
