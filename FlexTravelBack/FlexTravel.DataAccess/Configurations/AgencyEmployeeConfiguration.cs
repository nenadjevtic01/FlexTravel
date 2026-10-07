using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class AgencyEmployeeConfiguration : IEntityTypeConfiguration<AgencyEmployee>
    {
        public void Configure(EntityTypeBuilder<AgencyEmployee> builder)
        {
        }
    }
}
