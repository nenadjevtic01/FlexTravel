using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(x => x.RoomId).UseIdentityColumn(1, 1);
            builder.Property(x => x.AccommodationId).IsRequired();
            builder.Property(x => x.RoomTypeId).IsRequired();
            builder.Property(x => x.NumberOfRooms).IsRequired();
            builder.Property(x => x.AC).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Balcony).IsRequired().HasDefaultValue(false);

            builder
                .HasMany(x => x.Reservations)
                .WithOne(x => x.Room)
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Prices)
                .WithOne(x => x.Room)
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
