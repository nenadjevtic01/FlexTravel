using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.Property(x => x.CountryId).UseIdentityColumn(1, 1);
            builder.Property(x => x.CountryName).IsRequired().HasMaxLength(60);
            builder.HasIndex(x => x.CountryName).IsUnique();
            builder.Property(x => x.CapitalCity).HasMaxLength(60);
            builder.Property(x => x.Population);
            builder.Property(x => x.TotalArea);
            builder.Property(x => x.Currency).HasMaxLength(50);
            builder.Property(x => x.Language).HasMaxLength(60);

            builder
                .HasOne(x => x.Safety)
                .WithOne(x => x.Country)
                .HasForeignKey<Safety>(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Destination)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.VisaInfo)
                .WithOne(x => x.Country)
                .HasForeignKey<Visa>(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
