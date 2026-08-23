using MediatR;

namespace Silo.Application.Features;

public class DeleteUserTokenCommand : IRequest<DeleteUserTokenVm>
{
    public int TokenId { get; set; }
}
