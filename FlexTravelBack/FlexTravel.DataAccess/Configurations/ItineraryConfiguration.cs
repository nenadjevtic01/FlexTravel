using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ItineraryConfiguration : IEntityTypeConfiguration<Itinerary>
    {
        public void Configure(EntityTypeBuilder<Itinerary> builder)
        {
            builder.Property(x => x.ItineraryId).UseIdentityColumn(1, 1);
            builder.Property(x => x.ItineraryName).IsRequired().HasMaxLength(100);

            builder
                .HasMany(x => x.Trips)
                .WithOne(x => x.Itinerary)
                .HasForeignKey(x => x.ItineraryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.ItineraryActivities)
                .WithOne(x => x.Itinerary)
                .HasForeignKey(x => x.ItineraryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
