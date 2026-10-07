using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class AccommodationTypeConfiguration : IEntityTypeConfiguration<AccommodationType>
    {
        public void Configure(EntityTypeBuilder<AccommodationType> builder)
        {
            builder.Property(x => x.AccommodationTypeId).UseIdentityColumn(1, 1);
            builder.Property(x => x.AccommodationTypeName).IsRequired().HasMaxLength(30);

            builder
                .HasMany(x => x.Accommodations)
                .WithOne(x => x.AccommodationType)
                .HasForeignKey(x => x.AccommodationTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
