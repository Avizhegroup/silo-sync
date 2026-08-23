using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silo.Domains.Services;

namespace Silo.Domains;
public static class SiloDomainsServices
{
    public static void AddDomainsServices(this IServiceCollection services
        , IConfiguration configuration)
    {
        services.AddDbContext<WmsApiContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlDefaultConnectionString"));
#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });
    }
}
