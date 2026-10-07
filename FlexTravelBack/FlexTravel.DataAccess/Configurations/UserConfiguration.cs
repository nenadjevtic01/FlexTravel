using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class UserConfiguration : EntityConfiguration<User>
    {
        protected override void ConfigureTableSpecifics(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.UserId).UseIdentityColumn(1, 1);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(254);
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(100);
            builder.Property(x => x.City).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Avatar).HasMaxLength(200);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.FailedLoginAttempts).IsRequired();

            builder
                .HasDiscriminator<string>("Discriminator")
                .HasValue<User>("User")
                .HasValue<AgencyEmployee>("Agency employee")
                .HasValue<RegularUser>("Regular user")
                .HasValue<Admin>("Admin");

            builder
                .HasMany(x => x.UseCases)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
