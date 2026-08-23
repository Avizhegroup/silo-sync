using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Silo.Application.Dto;
using Silo.Identity.Server.Services;
using Silo.Identity.Server.Utilities;
using System.Net;
using System.Text.Json;

namespace Silo.Identity.Server;

public static class SiloServerIdentityServices
{
    public static IServiceCollection AddIdentityServerServices(this IServiceCollection services)
    {
        services.AddTransient<IJwtService, JwtService>();

        services.AddTransient<MavadkaranSsoService>();

        services.AddScoped<IdentityBusiness>();

        services.AddAuthentication();

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
          
            options.SaveToken = true;
           
            options.TokenValidationParameters = new()
            {
                ClockSkew = TimeSpan.Zero,// not before & expires tolerance
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = CryptoTools.GetSymmetricKey("84311GFT66934ECC86D327R7CF4F2OPC"),
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidAudience = "AvizheTicketIdentityUser",
                ValidateIssuer = true,
                ValidIssuer = "AvizheIdentity",
                TokenDecryptionKey = CryptoTools.GetSymmetricKey("84311GFT66934ECC")
            };

            options.Events = new()
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ApiResponse<bool>()
                    {
                        Successful = false,
                        Value = false,
                        Messages = new string[]
                        {
                            TextResources.APP_StringKeys_Error_TokenInvalid
                        }
                    }));
                },

                OnAuthenticationFailed = async context =>
                {
                    context.NoResult();
                   
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ApiResponse<bool>()
                    {
                        Successful = false,
                        Value = false,
                        Messages = new string[]
                        {
                            context.Exception.ToString()
                        }
                    }));
                },

                OnMessageReceived = context =>
                {
                    context.HttpContext.RequestServices
                           .GetRequiredService<ILogger<JwtBearerEvents>>().LogInformation("Jwt is validating ...");

                    return Task.CompletedTask;
                },

                OnForbidden = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new ApiResponse<bool>()
                    {
                        Successful = false,
                        Value = false,
                        Messages = new string[]
                        {
                            TextResources.APP_StringKeys_Validation_LoginFail
                        }
                    });
                    return context.Response.WriteAsync(result);
                }
            };
        })
        .AddScheme<AuthenticationSchemeOptions, DatabaseTokenAuthenticationHandler>(
            "DatabaseToken",
            options => { });

        return services;
    }
}
