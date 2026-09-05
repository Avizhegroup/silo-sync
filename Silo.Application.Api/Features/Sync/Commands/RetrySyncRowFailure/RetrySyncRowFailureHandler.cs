using Microsoft.EntityFrameworkCore;
using Silo.Application.Features;
using Silo.Domains.Services;
using Silo.Sync.Core.Retry;

namespace Silo.Application.Api.Features.Sync;

public class RetrySyncRowFailureHandler(WmsApiContext context, IFailedRowRetryService retryService)
    : IRequestHandler<RetrySyncRowFailureCommand, RetrySyncRowFailureVm>
{
    public async Task<RetrySyncRowFailureVm> Handle(RetrySyncRowFailureCommand request, CancellationToken cancellationToken)
    {
        var failure = await context.SyncRowFailures
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (failure is null)
        {
            return new RetrySyncRowFailureVm
            {
                Success = false,
                ErrorMessage = "Failure record not found."
            };
        }

        if (failure.Status == "Resolved")
        {
            return new RetrySyncRowFailureVm
            {
                Success = false,
                ErrorMessage = "The failure has already been resolved and cannot be retried."
            };
        }

        var result = await retryService.RetryAsync(failure.SourceKey, failure.RowKey, cancellationToken);

        return new RetrySyncRowFailureVm
        {
            Success = result.Success,
            ErrorCategory = result.ErrorCategory,
            ErrorMessage = result.ErrorMessage
        };
    }
}
