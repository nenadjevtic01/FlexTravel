using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(x => x.NotificationId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Message).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Link).HasMaxLength(200);
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => new { x.UserId, x.CreatedAt });

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
