using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Silo.Application.Api.Features;
public class CreatePreparedReportHandler(
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    ILogger<CreatePreparedReportHandler> logger,
    WmsApiContext context)
    : IRequestHandler<CreatePreparedReportCommand, CreatePreparedReportVm>
{
    public async Task<CreatePreparedReportVm> Handle(CreatePreparedReportCommand request, CancellationToken cancellationToken)
    {
        JsonSerializerOptions? options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };

        PreparedReport? preparedReport = new()
        {
            Title = request.Title,
            ReportFileName = request.ReportFileName,
            Variables = JsonSerializer.Serialize(request.Variables, options),
            DataSources = JsonSerializer.Serialize(request.DataSources, options),
            Images = JsonSerializer.Serialize(request.Images, options),
            UserId = httpContextAccessor.HttpContext?.User?.GetUserId() ?? string.Empty
        };

        await context.PreparedReports.AddAsync(preparedReport, cancellationToken);

        var result = await context.SaveChangesAsync(cancellationToken) > 0;

        if (!result)
        {
            return new()
            {
                Result = -1
            };
        }

        return new()
        {
            Result = preparedReport.Id
        };
    }
}
