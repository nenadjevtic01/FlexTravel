using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ContinentConfiguration : IEntityTypeConfiguration<Continent>
    {
        public void Configure(EntityTypeBuilder<Continent> builder)
        {
            builder.Property(x => x.ContinentId).UseIdentityColumn(1, 1);
            builder.Property(x => x.ContinentName).IsRequired().HasMaxLength(30);
            builder.HasIndex(x => x.ContinentName).IsUnique();

            builder
                .HasMany(x => x.Countries)
                .WithOne(x => x.Continent)
                .HasForeignKey(x => x.ContinentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
