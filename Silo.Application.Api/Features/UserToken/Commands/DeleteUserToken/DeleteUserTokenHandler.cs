using MediatR;
using Microsoft.EntityFrameworkCore;
using Silo.Domains.Services;

namespace Silo.Application.Features;

public class DeleteUserTokenHandler(WmsApiContext context)
    : IRequestHandler<DeleteUserTokenCommand, DeleteUserTokenVm>
{
    public async Task<DeleteUserTokenVm> Handle(
        DeleteUserTokenCommand request,
        CancellationToken cancellationToken)
    {
        var rowsAffected = await context.UserTokens
            .Where(x => x.Id == request.TokenId)
            .ExecuteDeleteAsync(cancellationToken);

        return new()
        {
            Result = rowsAffected > 0
        };
    }
}
