using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class UserFavoritesConfiguration : IEntityTypeConfiguration<UserFavorites>
    {
        public void Configure(EntityTypeBuilder<UserFavorites> builder)
        {
            builder.HasKey(x => new { x.UserId, x.DestinationId });
        }
    }
}
