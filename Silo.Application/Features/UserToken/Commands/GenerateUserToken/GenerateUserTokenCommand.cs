using MediatR;

namespace Silo.Application.Features;

public class GenerateUserTokenCommand : IRequest<GenerateUserTokenVm>
{
    public string UserId { get; set; }
}
