using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ReservationStatusChangeConfiguration : IEntityTypeConfiguration<ReservationStatusChange>
    {
        public void Configure(EntityTypeBuilder<ReservationStatusChange> builder)
        {
            builder.Property(x => x.ReservationStatusChangeId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.ChangedAt).IsRequired();
            builder.Property(x => x.Comment).HasMaxLength(500);

            builder
                .HasOne(x => x.Reservation)
                .WithMany(x => x.StatusChanges)
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.ChangedBy)
                .WithMany()
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
