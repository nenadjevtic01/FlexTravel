using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.Property(x => x.RoomTypeId).UseIdentityColumn(1, 1);
            builder.Property(x => x.RoomTypeName).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Capacity).IsRequired();

            builder
                .HasMany(x => x.Rooms)
                .WithOne(x => x.RoomType)
                .HasForeignKey(x => x.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
