using Microsoft.Extensions.DependencyInjection;

namespace Silo.Modules.Ai;
public static class AiModuleServices
{
    public static void AddAiModuleServices(this IServiceCollection services)
    {
        // ChatAgentService is an API-layer concern and is registered in Silo.Api.
        // No AI agent services need to be registered in the Blazor UI module.
    }
}
