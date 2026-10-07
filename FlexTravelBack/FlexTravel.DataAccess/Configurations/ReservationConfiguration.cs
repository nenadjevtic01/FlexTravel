using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ReservationConfiguration : EntityConfiguration<Reservation>
    {
        protected override void ConfigureTableSpecifics(EntityTypeBuilder<Reservation> builder)
        {
            builder.Property(x => x.ReservationId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
            builder.HasIndex(x => x.Code).IsUnique();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.TripId).IsRequired();
            builder.Property(x => x.AccommodationId).IsRequired();
            builder.Property(x => x.NumberOfAdults).IsRequired();
            builder.Property(x => x.NumberOfChildren).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Note).HasMaxLength(500);
            builder.Property(x => x.PricePerPerson).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.ChildDiscountPercent).IsRequired();
            builder.Property(x => x.Subtotal).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.Offer).IsRequired();
            builder.Property(x => x.OfferPercent).IsRequired();
            builder.Property(x => x.PromoPercent).IsRequired();
            builder.Property(x => x.DiscountTotal).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.ExtrasTotal).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.TotalPrice).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.CancellationFee).HasPrecision(10, 2);

            builder
                .HasMany(x => x.Rooms)
                .WithOne(x => x.Reservation)
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Accommodation)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.AccommodationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
