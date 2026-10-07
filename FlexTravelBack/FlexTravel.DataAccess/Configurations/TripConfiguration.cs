using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class TripConfiguration : EntityConfiguration<Trip>
    {

        protected override void ConfigureTableSpecifics(EntityTypeBuilder<Trip> builder)
        {
            builder.Property(x => x.TripId).UseIdentityColumn(1, 1);
            builder.Property(x => x.TransportId).IsRequired();
            builder.Property(x => x.ItineraryId).IsRequired();
            builder.Property(x => x.DestinationId).IsRequired();
            builder.Property(x => x.AgencyId).IsRequired();
            builder.Property(x => x.TripSeriesId).IsRequired();
            builder.Property(x => x.TripName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.TripDescription).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();
            builder.Property(x => x.Capacity).IsRequired();
            builder.Property(x => x.ChildDiscountPercent).IsRequired();
            builder.Property(x => x.EarlyBookingPercent).IsRequired();
            builder.Property(x => x.LastMinutePercent).IsRequired();
            builder.Property(x => x.LastMinuteDays).IsRequired();

            builder
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.UpdatedBy)
                .WithMany()
                .HasForeignKey(x => x.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Prices)
                .WithOne(x => x.Trip)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Reservations)
                .WithOne(x => x.Trip)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Accommodations)
                .WithOne(x => x.Trip)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Attractions)
                .WithOne(x => x.Trip)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
