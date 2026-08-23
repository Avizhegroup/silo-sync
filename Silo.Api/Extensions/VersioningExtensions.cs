using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Extensions;

public static class VersioningExtensions
{
    public static IServiceCollection AddSiloApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var controllers = assembly.GetTypes()
                .Where(w => typeof(SiloBaseController).IsAssignableFrom(w) && !w.Namespace.Contains("Base"));
            foreach (Type controller in controllers)
            {
                var version = controller.Namespace.Split('.').Last().Replace("v", "");
                options.Conventions.Controller(controller).HasApiVersion(new ApiVersion(int.Parse(version), 0));
            }
        });

        return services;
    }
}
