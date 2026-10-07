using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class TripAccommodationConfiguration : IEntityTypeConfiguration<TripAccommodation>
    {
        public void Configure(EntityTypeBuilder<TripAccommodation> builder)
        {
            builder.HasKey(x => new { x.TripId, x.AccommodationId });
        }
    }
}
