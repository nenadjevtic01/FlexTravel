using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class PromoCodeConfiguration : EntityConfiguration<PromoCode>
    {
        protected override void ConfigureTableSpecifics(EntityTypeBuilder<PromoCode> builder)
        {
            builder.Property(x => x.PromoCodeId).UseIdentityColumn(1, 1);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Description).HasMaxLength(200);
            builder.Property(x => x.DiscountPercent).IsRequired();
            builder.Property(x => x.ValidFrom).IsRequired();
            builder.Property(x => x.ValidTo).IsRequired();

            builder.HasIndex(x => x.Code).IsUnique();

            builder
                .HasOne(x => x.Agency)
                .WithMany(x => x.PromoCodes)
                .HasForeignKey(x => x.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Reservations)
                .WithOne(x => x.PromoCode)
                .HasForeignKey(x => x.PromoCodeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
