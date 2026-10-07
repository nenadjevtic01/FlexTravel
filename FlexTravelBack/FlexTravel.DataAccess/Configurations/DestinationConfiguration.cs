using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class DestinationConfiguration : EntityConfiguration<Destination>
    {
        protected override void ConfigureTableSpecifics(EntityTypeBuilder<Destination> builder)
        {
            builder.Property(x => x.DestinationId).UseIdentityColumn(1, 1);
            builder.Property(x => x.CountryId).IsRequired();
            builder.Property(x => x.DestinationName).IsRequired().HasMaxLength(60);
            builder.Property(x => x.DestinationDescription).IsRequired().HasMaxLength(1000);

            builder
                .HasOne(x => x.Climate)
                .WithOne(x => x.Destination)
                .HasForeignKey<Climate>(x => x.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Trips)
                .WithOne(x => x.Destination)
                .HasForeignKey(x => x.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Images)
                .WithOne(x => x.Destination)
                .HasForeignKey(x => x.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Accommodations)
                .WithOne(x => x.Destination)
                .HasForeignKey(x => x.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Attractions)
                .WithOne(x => x.Destination)
                .HasForeignKey(x => x.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.UserFavorites)
                .WithOne(x => x.Destination)
                .HasForeignKey(x => x.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
