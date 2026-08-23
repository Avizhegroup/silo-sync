using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Silo.Domains.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Silo.Identity.Server;
public class DatabaseTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        WmsApiContext context)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        string authorizationHeader = Request.Headers["Authorization"].ToString();

        if (authorizationHeader.HasNoValue() 
            || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        string token = authorizationHeader.Substring("Bearer ".Length).Trim();

        if (token.HasNoValue())
        {
            return AuthenticateResult.Fail("Invalid Token");
        }

        try
        {
            var userToken = await context.UserTokens
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(ut => ut.Value == token && !ut.HasExpired);

            if (userToken is null)
            {
                return AuthenticateResult.Fail("Token not found or expired in database");
            }

            var handler = new JwtSecurityTokenHandler();
           
            JwtSecurityToken jwtToken;

            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("Invalid JWT token format");
            }

            var claims = jwtToken.Claims.ToList();
            
            var identity = new ClaimsIdentity(claims, Scheme.Name);
           
            var principal = new ClaimsPrincipal(identity);
            
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating token from database");
       
            return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
        }
    }
}
