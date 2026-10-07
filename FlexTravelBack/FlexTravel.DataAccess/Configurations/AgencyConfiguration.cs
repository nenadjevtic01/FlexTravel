using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
    {
        public void Configure(EntityTypeBuilder<Agency> builder)
        {
            builder.Property(x => x.AgencyId).UseIdentityColumn(1, 1);
            builder.Property(x => x.AgencyName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Description).HasMaxLength(1500);
            builder.Property(x => x.Email).HasMaxLength(100);
            builder.Property(x => x.Website).HasMaxLength(200);
            builder.Property(x => x.Logo).HasMaxLength(200);

            builder
                .HasMany(x => x.Employees)
                .WithOne(x => x.Agency)
                .HasForeignKey(x => x.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Trips)
                .WithOne(x => x.Agency)
                .HasForeignKey(x => x.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
