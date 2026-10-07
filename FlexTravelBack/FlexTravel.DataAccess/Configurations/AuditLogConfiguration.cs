using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.Property(x => x.AuditLogId).UseIdentityColumn(1, 1);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ExecutionDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.UseCaseName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Data).IsRequired(false);
            builder.Property(x => x.IsAuthorized).IsRequired();
        }
    }
}
