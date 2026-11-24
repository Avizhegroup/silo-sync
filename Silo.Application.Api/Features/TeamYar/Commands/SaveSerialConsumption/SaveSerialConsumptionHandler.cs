using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>
/// Marks serials as consumed (Tag.Deactivate = true). The inquiry then reports them as Used.
/// The document code and the date are kept in the TeamYar request record (StructuredExternalConnection).
/// </summary>
public class SaveSerialConsumptionHandler(WmsApiContext context
    , IHttpContextAccessor httpContextAccessor
    , ILogger<SaveSerialConsumptionHandler> logger)
    : IRequestHandler<SaveSerialConsumptionCommand, SaveSerialConsumptionVm>, ITeamYarSecHandler
{
    public async Task<SaveSerialConsumptionVm> Handle(SaveSerialConsumptionCommand request, CancellationToken cancellationToken)
    {
        var serials = TeamYarHelper.NormalizeSerials(request.Serials);
        var errors = new List<string>();

        if (serials.Count == 0) errors.Add("Serials must contain at least one serial.");
        if (string.IsNullOrWhiteSpace(request.DocumentCode)) errors.Add("DocumentCode is required.");

        if (!string.IsNullOrWhiteSpace(request.Date) && !TeamYarHelper.TryParsePersianDate(request.Date, out _))
        {
            errors.Add($"Date '{request.Date}' is not a valid Persian date (yyyy/MM/dd).");
        }

        if (errors.Count == 0)
        {
            var found = await TeamYarHelper.GetTagSerialsAsync(context, serials, cancellationToken);
            var missing = serials.Where(s => !found.Contains(s)).ToList();

            if (missing.Count > 0)
            {
                errors.Add($"Serial(s) not found or revoked: {TeamYarHelper.Quote(missing)}.");
            }
        }

        if (errors.Count > 0)
        {
            throw TeamYarHelper.Validation(errors);
        }

        var userId = TeamYarHelper.CurrentUserId(httpContextAccessor);

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.Tags
                         .Where(t => serials.Contains(t.ProductSerial) && t.Deactivate != true)
                         .ExecuteUpdateAsync(setters => setters
                             .SetProperty(t => t.Deactivate, (bool?)true)
                             .SetProperty(t => t.LastModifierUser, userId), cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            logger.LogError(ex, "TeamYar SerialConsumption failed. CommandUniqueId: {CommandUniqueId}", request.CommandUniqueId);
            throw;
        }
    }
}
