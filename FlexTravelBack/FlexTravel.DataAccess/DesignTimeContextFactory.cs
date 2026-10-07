using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlexTravel.DataAccess
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<FlexTravelContext>
    {
        public FlexTravelContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("FLEXTRAVEL_CONNECTION_STRING")
                ?? @"Data Source=localhost\SQLEXPRESS;Initial Catalog=FlexTravel;Integrated Security=True;TrustServerCertificate=True";

            var options = new DbContextOptionsBuilder<FlexTravelContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new FlexTravelContext(options);
        }
    }
}
