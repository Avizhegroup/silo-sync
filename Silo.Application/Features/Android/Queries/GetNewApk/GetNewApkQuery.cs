namespace Silo.Application.Features;

public class GetNewApkQuery : IRequest<GetNewApkVm>
{
    public string CurrentVersion { get; set; }
    public string? StationCode { get; set; }
}
