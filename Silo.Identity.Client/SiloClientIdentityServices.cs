using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Silo.Identity.Client;

public static class SiloClientIdentityServices
{
    public static void AddSiloClientIdentityServices(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, SiloAuthenticationStateProvider>();
       
        services.AddScoped(sp=> (SiloAuthenticationStateProvider) sp.GetRequiredService<AuthenticationStateProvider>());
        
        services.AddScoped<IAuthenticationService, AuthenticationService>();
       
        services.AddScoped<IClaimManager, ClaimManager>();
    }
}
