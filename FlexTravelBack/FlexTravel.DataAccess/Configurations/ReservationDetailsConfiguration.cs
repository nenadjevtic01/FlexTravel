using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ReservationTravelerConfiguration : IEntityTypeConfiguration<ReservationTraveler>
    {
        public void Configure(EntityTypeBuilder<ReservationTraveler> builder)
        {
            builder.Property(x => x.ReservationTravelerId).UseIdentityColumn(1, 1);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.DateOfBirth).IsRequired();
            builder.Property(x => x.IsChild).IsRequired();

            builder
                .HasOne(x => x.Reservation)
                .WithMany(x => x.Travelers)
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(x => x.PaymentId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Amount).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.Method).IsRequired();
            builder.Property(x => x.PaidAt).IsRequired();
            builder.Property(x => x.Note).HasMaxLength(200);

            builder
                .HasOne(x => x.Reservation)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.RecordedBy)
                .WithMany()
                .HasForeignKey(x => x.RecordedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ReservationMessageConfiguration : IEntityTypeConfiguration<ReservationMessage>
    {
        public void Configure(EntityTypeBuilder<ReservationMessage> builder)
        {
            builder.Property(x => x.ReservationMessageId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Text).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.SentAt).IsRequired();
            builder.HasIndex(x => new { x.ReservationId, x.SentAt });

            builder
                .HasOne(x => x.Reservation)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class TripSeriesConfiguration : IEntityTypeConfiguration<TripSeries>
    {
        public void Configure(EntityTypeBuilder<TripSeries> builder)
        {
            builder.Property(x => x.TripSeriesId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder
                .HasOne(x => x.Agency)
                .WithMany(x => x.TripSeries)
                .HasForeignKey(x => x.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Departures)
                .WithOne(x => x.Series)
                .HasForeignKey(x => x.TripSeriesId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
