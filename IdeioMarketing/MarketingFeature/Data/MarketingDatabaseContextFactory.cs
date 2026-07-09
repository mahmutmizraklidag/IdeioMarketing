using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IdeioMarketing.MarketingFeature.Data
{
    public class MarketingDatabaseContextFactory : IDesignTimeDbContextFactory<MarketingDatabaseContext>
    {
        public MarketingDatabaseContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var options = new DbContextOptionsBuilder<MarketingDatabaseContext>()
                .UseSqlServer(GetMarketingConnectionString(configuration))
                .Options;

            return new MarketingDatabaseContext(options);
        }

        private static string GetMarketingConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServer") ?? string.Empty;
            return connectionString.Contains("Encrypt=", StringComparison.OrdinalIgnoreCase)
                ? connectionString
                : connectionString.TrimEnd(';') + ";Encrypt=False;";
        }
    }
}
