namespace Silo.Application.Features;
public class GetUserTokensQuery : IRequest<GetUserTokensVm>
{
    public string UserId { get; set; }
}
