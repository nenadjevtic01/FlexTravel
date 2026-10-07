using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class TripExtraConfiguration : IEntityTypeConfiguration<TripExtra>
    {
        public void Configure(EntityTypeBuilder<TripExtra> builder)
        {
            builder.Property(x => x.TripExtraId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Kind).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(300);
            builder.Property(x => x.Price).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.IsActive).IsRequired();

            builder
                .HasOne(x => x.Trip)
                .WithMany(x => x.Extras)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ReservationExtraConfiguration : IEntityTypeConfiguration<ReservationExtra>
    {
        public void Configure(EntityTypeBuilder<ReservationExtra> builder)
        {
            builder.Property(x => x.ReservationExtraId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Kind).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.UnitPrice).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.Total).IsRequired().HasPrecision(10, 2);

            builder
                .HasOne(x => x.Reservation)
                .WithMany(x => x.Extras)
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.TripExtra)
                .WithMany()
                .HasForeignKey(x => x.TripExtraId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class PriceAlertConfiguration : IEntityTypeConfiguration<PriceAlert>
    {
        public void Configure(EntityTypeBuilder<PriceAlert> builder)
        {
            builder.Property(x => x.PriceAlertId).UseIdentityColumn(1, 1);
            builder.Property(x => x.MaxPrice).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.LastNotifiedPrice).HasPrecision(10, 2);
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => new { x.UserId, x.DestinationId }).IsUnique();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.PriceAlerts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Destination)
                .WithMany()
                .HasForeignKey(x => x.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.Property(x => x.ExchangeRateId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);
            builder.HasIndex(x => x.Currency).IsUnique();
            builder.Property(x => x.RatePerEuro).IsRequired().HasPrecision(12, 4);
            builder.Property(x => x.UpdatedAt).IsRequired();

            builder
                .HasOne(x => x.UpdatedBy)
                .WithMany()
                .HasForeignKey(x => x.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(new ExchangeRate
            {
                ExchangeRateId = 1,
                Currency = "RSD",
                RatePerEuro = 117.2m,
                UpdatedAt = new DateTime(2026, 10, 5)
            });
        }
    }

    public class ScheduledJobRunConfiguration : IEntityTypeConfiguration<ScheduledJobRun>
    {
        public void Configure(EntityTypeBuilder<ScheduledJobRun> builder)
        {
            builder.Property(x => x.ScheduledJobRunId).UseIdentityColumn(1, 1);
            builder.Property(x => x.StartedAt).IsRequired();
            builder.Property(x => x.StartedBy).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Error).HasMaxLength(1000);
            builder.HasIndex(x => x.StartedAt);
        }
    }

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.Property(x => x.RefreshTokenId).UseIdentityColumn(1, 1);
            builder.Property(x => x.TokenHash).IsRequired().HasMaxLength(64);
            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
