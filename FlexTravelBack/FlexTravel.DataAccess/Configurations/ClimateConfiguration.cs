using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ClimateConfiguration : IEntityTypeConfiguration<Climate>
    {
        public void Configure(EntityTypeBuilder<Climate> builder)
        {
            builder.Property(x => x.ClimateId).UseIdentityColumn(1, 1);
            builder.Property(x => x.DestinationId).IsRequired();
            builder.HasIndex(x => x.DestinationId).IsUnique();
            builder.Property(x => x.ClimateInfo).IsRequired().HasMaxLength(1000);
        }
    }
}
