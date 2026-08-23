using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Silo.Domains.Android;
public static class DomainsAndroidServices
{
    public static void AddDomainsAndroidServices(this IServiceCollection services)
    {
        services.AddDbContext<WmsAndroidContext>(options =>
        {
            options.UseSqlite("Filename = Wms.db;");

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });
    }
}
