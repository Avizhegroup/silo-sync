using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Silo.Domains.Services;
using Silo.Identity.Server.Services;
using Silo.Identity.Server.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Silo.Application.Features;
public class GenerateUserTokenHandler(
    WmsApiContext context,
    IdentityBusiness identityBusiness)
    : IRequestHandler<GenerateUserTokenCommand, GenerateUserTokenVm>
{
    public async Task<GenerateUserTokenVm> Handle(
        GenerateUserTokenCommand request,
        CancellationToken cancellationToken)
    {
        var userData = identityBusiness.IUserDataForProfileById(request.UserId);

        var user = await context.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            cancellationToken);

        if (user is null)
        {
            return new()
            {
                Result = false,
                Token = null
            };
        }

        var claimsIdentity = GetClaimsIdentity(
            user.Username,
            request.UserId,
            userData.Item1,
            userData.Item2);

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = "AvizheIdentity",
            Audience = "AvizheTicketIdentityUser",
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(0),
            Expires = DateTime.Now.AddYears(1),
            SigningCredentials = CryptoTools.GetJwtCredential("84311GFT66934ECC86D327R7CF4F2OPC"),
            Subject = claimsIdentity,
            Claims = claimsIdentity.Claims.ToDictionary(claim => claim.Type, claim => (object)claim.Value)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken securityToken = tokenHandler.CreateToken(descriptor);
        string jwt = tokenHandler.WriteToken(securityToken);

        var userToken = new Domains.Entities.UserToken
        {
            Value = jwt,
            UserId = request.UserId,
            HasExpired = false,
        };

        await context.UserTokens.AddAsync(userToken, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;

        return new()
        {
            Result = result,
            Token = jwt
        };
    }

    private ClaimsIdentity GetClaimsIdentity(
        string username,
        string userId,
        string persianName,
        string imageName)
    {
        List<System.Security.Claims.Claim> claims = new();

        claims.Add(new System.Security.Claims.Claim(ClaimTypes.Name, username));
        claims.Add(new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, userId));

        var roles = identityBusiness.GetUserRoles(userId, userId).Select();

        if (roles.Any(p => p.ItemArray[2].ToString().ToLower() != "user"))
        {
            var roleDt = roles.FirstOrDefault(p => p.ItemArray[2].ToString().ToLower() != "user");
            claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, roleDt.ItemArray[2].ToString()));
        }
        else
        {
            var roleDt = roles.First();
            claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, roleDt.ItemArray[2].ToString()));
        }

        claims.Add(new System.Security.Claims.Claim(ClaimTypes.Surname, persianName));
        claims.Add(new System.Security.Claims.Claim(ClaimTypes.Locality, imageName));

        return new ClaimsIdentity(claims);
    }
}
