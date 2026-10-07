using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class WaitlistEntryConfiguration : EntityConfiguration<WaitlistEntry>
    {
        protected override void ConfigureTableSpecifics(EntityTypeBuilder<WaitlistEntry> builder)
        {
            builder.Property(x => x.WaitlistEntryId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Adults).IsRequired();
            builder.Property(x => x.Children).IsRequired();
            builder.HasIndex(x => new { x.TripId, x.UserId });

            builder
                .HasOne(x => x.Trip)
                .WithMany(x => x.Waitlist)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Waitlist)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
