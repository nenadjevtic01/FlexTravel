using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class SafetyConfiguration : IEntityTypeConfiguration<Safety>
    {
        public void Configure(EntityTypeBuilder<Safety> builder)
        {
            builder.Property(x => x.SafetyId).UseIdentityColumn(1, 1);
            builder.Property(x => x.SafetyLevel).IsRequired();
            builder.Property(x => x.Advices).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.CountryId).IsRequired();
            builder.HasIndex(x => x.CountryId).IsUnique();
        }
    }
}
