using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Silo.Application.Dto;
using Silo.Application.Shared.Contracts;
using Silo.Domains.Services;

namespace Silo.Infrastructure.Shared;

public sealed class TeamYarSecResponseLoggingMiddleware(
    RequestDelegate next,
    ILogger<TeamYarSecResponseLoggingMiddleware> logger)
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);

            var capture = context.Features.Get<TeamYarSecResponseCapture>();
            if (capture is null)
            {
                return;
            }

            await LogResponse(context, capture);

            capture.ResponseBody.Position = 0;
            await capture.ResponseBody.CopyToAsync(capture.OriginalBody, context.RequestAborted);
        }
        finally
        {
            var capture = context.Features.Get<TeamYarSecResponseCapture>();
            if (capture is not null)
            {
                context.Response.Body = capture.OriginalBody;
                context.Features.Set<TeamYarSecResponseCapture>(null);
                capture.Dispose();
            }
        }
    }

    private async Task LogResponse(HttpContext context, TeamYarSecResponseCapture capture)
    {
        try
        {
            capture.ResponseBody.Position = 0;
            using var reader = new StreamReader(capture.ResponseBody, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            var response = JsonSerializer.Deserialize<ApiResponseWithStatusCode<JsonElement>>(
                body, SerializerOptions)
                ?? throw new JsonException("The TeamYar response was null.");
            response.HttpStatusCode = context.Response.StatusCode;

            var json = JsonSerializer.Serialize(response);
            var dbContext = context.RequestServices.GetRequiredService<WmsApiContext>();
            var updated = await dbContext.StructuredExternalConnections
                .Where(connection => capture.ConnectionIds.Contains(connection.Id))
                .ExecuteUpdateAsync(setters => setters.SetProperty(connection => connection.Response, json),
                    CancellationToken.None);

            if (updated != capture.ConnectionIds.Count)
            {
                logger.LogError("Expected to update {ExpectedCount} TeamYar response records, but updated {UpdatedCount}.",
                    capture.ConnectionIds.Count, updated);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Could not save the response for TeamYar connections {ConnectionIds}.",
                capture.ConnectionIds);
        }
    }
}
