using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class TripAttractionConfiguration : IEntityTypeConfiguration<TripAttraction>
    {
        public void Configure(EntityTypeBuilder<TripAttraction> builder)
        {
            builder.HasKey(x => new { x.TripId, x.AttractionId });
        }
    }
}
