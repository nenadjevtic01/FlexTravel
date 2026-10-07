using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class DestinationImageConfiguration : IEntityTypeConfiguration<DestinationImage>
    {
        public void Configure(EntityTypeBuilder<DestinationImage> builder)
        {
            builder.Property(x => x.DestinationImageId).UseIdentityColumn(1, 1);
            builder.Property(x => x.DestinationId).IsRequired();
            builder.Property(x => x.Src).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Alt).IsRequired().HasMaxLength(100);
        }
    }
}
