using Silo.Application.Dto;

namespace Silo.Identity.Server;
public interface IJwtService
{
    Task<string> AuthenticateAsync(ApiAuthenticateDto request);
    Task<string> AuthenticateBySessionTokenAsync(string token);
}
