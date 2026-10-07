using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class AccommodationConfiguration : IEntityTypeConfiguration<Accommodation>
    {
        public void Configure(EntityTypeBuilder<Accommodation> builder)
        {
            builder.Property(x => x.AccommodationId).UseIdentityColumn(1, 1);
            builder.Property(x => x.AccommodationName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.AccommodationStars).IsRequired().HasPrecision(2, 1);
            builder.Property(x => x.DestinationId).IsRequired();
            builder.Property(x => x.AccommodationTypeId).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            builder.HasMany(x => x.Trips)
                .WithOne(x => x.Accommodation)
                .HasForeignKey(x => x.AccommodationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Rooms)
                .WithOne(x => x.Accommodation)
                .HasForeignKey(x => x.AccommodationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
