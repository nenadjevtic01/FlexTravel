using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class ReviewConfiguration : EntityConfiguration<Review>
    {
        protected override void ConfigureTableSpecifics(EntityTypeBuilder<Review> builder)
        {
            builder.Property(x => x.ReviewId).UseIdentityColumn(1, 1);
            builder.Property(x => x.TripRating).IsRequired();
            builder.Property(x => x.HotelRating).IsRequired();
            builder.Property(x => x.Comment).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.AgencyReply).HasMaxLength(1000);
            builder.Property(x => x.ModerationNote).HasMaxLength(300);
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Review_TripRating", "[TripRating] BETWEEN 1 AND 5");
                t.HasCheckConstraint("CK_Review_HotelRating", "[HotelRating] BETWEEN 1 AND 5");
                t.HasCheckConstraint("CK_Review_GuideRating", "[GuideRating] IS NULL OR [GuideRating] BETWEEN 1 AND 5");
                t.HasCheckConstraint("CK_Review_OrganisationRating", "[OrganisationRating] IS NULL OR [OrganisationRating] BETWEEN 1 AND 5");
                t.HasCheckConstraint("CK_Review_ValueRating", "[ValueRating] IS NULL OR [ValueRating] BETWEEN 1 AND 5");
            });

            builder.HasIndex(x => x.ReservationId).IsUnique();

            builder
                .HasOne(x => x.Reservation)
                .WithOne(x => x.Review)
                .HasForeignKey<Review>(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Trip)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Accommodation)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.AccommodationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.AgencyReplyBy)
                .WithMany()
                .HasForeignKey(x => x.AgencyReplyByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.ModeratedBy)
                .WithMany()
                .HasForeignKey(x => x.ModeratedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
