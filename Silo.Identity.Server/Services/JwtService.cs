using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Azure.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Silo.Application.Dto;
using Silo.Application.Exceptions;
using Silo.Identity.Server.Services;
using Silo.Identity.Server.Utilities;
using Claim = System.Security.Claims.Claim;

namespace Silo.Identity.Server;

public partial class JwtService(IdentityBusiness identityBusiness
    , IConfiguration configuration
    , MavadkaranSsoService mavadkaranSsoService) : IJwtService
{
    public async Task<string> AuthenticateAsync(ApiAuthenticateDto request)
    {
        string id = string.Empty;

        if (request.Username.ToLower().NotEquals("admin"))
        {
            await SsoLogin();
        }

        if (id.HasNoValue())
        {
            id = ((IdentityBusiness)identityBusiness).TSLogin(request.Username, request.Password);
        }

        if (id.Equals("0"))
        {
            throw new UserNotFoundException();
        }

        var data = ((IdentityBusiness)identityBusiness).IUserDataForProfileById(id);

        ClaimsIdentity? claimsIdentity;

        if (request.StationMac.HasValue())
        {
            var stationCode = ((IdentityBusiness)identityBusiness).IGetStationCodeByMac(request.StationMac);
            claimsIdentity = await GetClaimsIdentityForStation(request.Username, id, stationCode);
        }
        else
        {
            claimsIdentity = await GetClaimsIdentity(request.Username, id, data.Item1, data.Item2);
        }

        if (request.StationMac.HasValue())
        {
            var stationCode = ((IdentityBusiness)identityBusiness).IGetStationCodeByMac(request.StationMac);
            claimsIdentity = await GetClaimsIdentityForStation(request.Username, id, stationCode);
        }
        else
        {
            claimsIdentity = await GetClaimsIdentity(request.Username, id, data.Item1, data.Item2);
        }

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = "AvizheIdentity",
            Audience = "AvizheTicketIdentityUser",
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(0),
            Expires = DateTime.Now.AddHours(10),
            SigningCredentials = CryptoTools.GetJwtCredential("84311GFT66934ECC86D327R7CF4F2OPC"),
            Subject = claimsIdentity,
            Claims = claimsIdentity.Claims.ToDictionary(claim => claim.Type, claim => (object)claim.Value)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken securityToken = tokenHandler.CreateToken(descriptor);
        string jwt = tokenHandler.WriteToken(securityToken);

        return jwt;

        async Task SsoLogin()
        {
            if (configuration["SSO:Type"] == "Mavadkaran")
            {
                var mavadSsoToken = await mavadkaranSsoService.GetAccessTokenAsync(request);

                if (mavadSsoToken.HasNoValue())
                {
                    throw new UserNotFoundException();
                }

                id = ((IdentityBusiness)identityBusiness).TSLogin(request.Username, request.Password);

                if (id.Equals("0"))
                {
                    id = ((IdentityBusiness)identityBusiness).AddNewUserAndRole(request.Username
                        , request.Password
                        , "SSO"
                        , true
                        , request.Username
                        , "User"
                        , "{}");
                }
            }
        }
    }

    public async Task<string> AuthenticateBySessionTokenAsync(string token)
    {
        var claimsIdentity = await GetClaimsIdentityForAnonymous($"Anon_{token}", token);

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = "AvizheIdentity",
            Audience = "AvizheTicketIdentityUser",
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(0),
            Expires = DateTime.Now.AddMinutes(10),
            SigningCredentials = CryptoTools.GetJwtCredential("84311GFT66934ECC86D327R7CF4F2OPC"),
            Subject = claimsIdentity,
            Claims = claimsIdentity.Claims.ToDictionary(claim => claim.Type, claim => (object)claim.Value)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken securityToken = tokenHandler.CreateToken(descriptor);
        string jwt = tokenHandler.WriteToken(securityToken);

        return jwt;
    }

    private async Task<ClaimsIdentity> GetClaimsIdentity(string username
        , string userId
        , string persianName
        , string imageName)
    {
        List<Claim> claims = new();

        claims.Add(new(ClaimTypes.Name, username));
        claims.Add(new(ClaimTypes.NameIdentifier, userId));

        var roles = identityBusiness.GetUserRoles(userId, userId).Select();

        if (roles.Any(p => p.ItemArray[2].ToString().ToLower() != "user"))
        {
            var roleDt = roles.FirstOrDefault(p => p.ItemArray[2].ToString().ToLower() != "user");
            claims.Add(new(ClaimTypes.Role, roleDt.ItemArray[2].ToString()));
        }
        else
        {
            var roleDt = roles.First();
            claims.Add(new(ClaimTypes.Role, roleDt.ItemArray[2].ToString()));
        }

        claims.Add(new(ClaimTypes.Surname, persianName));
        claims.Add(new(ClaimTypes.Locality, imageName));

        return new ClaimsIdentity(claims);
    }

    private async Task<ClaimsIdentity> GetClaimsIdentityForAnonymous(string username
        , string userId)
    {
        List<Claim> claims = new();

        claims.Add(new(ClaimTypes.Name, username));
        claims.Add(new(ClaimTypes.NameIdentifier, userId));
        claims.Add(new(ClaimTypes.Role, "Anonymous"));

        return new ClaimsIdentity(claims);
    }

    private async Task<ClaimsIdentity> GetClaimsIdentityForStation(string username
        , string userId
        , string stationCode)
    {
        List<Claim> claims = new();

        claims.Add(new(ClaimTypes.Name, username));
        claims.Add(new(ClaimTypes.NameIdentifier, userId));
        claims.Add(new(ClaimTypes.Role, "Station"));
        claims.Add(new(ClaimTypes.SerialNumber, stationCode));

        return new ClaimsIdentity(claims);
    }


}
