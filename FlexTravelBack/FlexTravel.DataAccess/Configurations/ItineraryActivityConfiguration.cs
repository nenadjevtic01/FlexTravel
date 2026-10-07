using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ItineraryActivityConfiguration : IEntityTypeConfiguration<ItineraryActivity>
    {
        public void Configure(EntityTypeBuilder<ItineraryActivity> builder)
        {
            builder.Property(x => x.ItineraryActivityId).UseIdentityColumn(1, 1);
            builder.Property(x => x.ItineraryId).IsRequired();
            builder.Property(x => x.Day).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Activity).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.Meals).IsRequired();
            builder.Property(x => x.Overnight).HasMaxLength(100);
        }
    }
}
