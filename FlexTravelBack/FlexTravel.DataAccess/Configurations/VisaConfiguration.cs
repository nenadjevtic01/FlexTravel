using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class VisaConfiguration : IEntityTypeConfiguration<Visa>
    {
        public void Configure(EntityTypeBuilder<Visa> builder)
        {
            builder.Property(x => x.VisaId).UseIdentityColumn(1, 1);
            builder.Property(x => x.VisaType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Duration).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Requirements).IsRequired();
        }
    }
}
