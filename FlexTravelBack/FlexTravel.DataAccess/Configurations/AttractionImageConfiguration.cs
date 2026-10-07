using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class AttractionImageConfiguration : IEntityTypeConfiguration<AttractionImage>
    {
        public void Configure(EntityTypeBuilder<AttractionImage> builder)
        {
            builder.Property(x => x.AttractionImageId).UseIdentityColumn(1, 1);
            builder.Property(x => x.AttractionId).IsRequired();
            builder.Property(x => x.Src).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Alt).IsRequired().HasMaxLength(100);
        }
    }
}
