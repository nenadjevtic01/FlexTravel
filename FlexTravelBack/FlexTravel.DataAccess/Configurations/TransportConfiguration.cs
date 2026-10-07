using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class TransportConfiguration : IEntityTypeConfiguration<Transport>
    {
        public void Configure(EntityTypeBuilder<Transport> builder)
        {
            builder.Property(x => x.TransportId).UseIdentityColumn(1, 1);
            builder.Property(x => x.TransportName).IsRequired().HasMaxLength(30);

            builder
                .HasMany(x => x.TripList)
                .WithOne(x => x.Transport)
                .HasForeignKey(x => x.TransportId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
