using System.IO;

namespace Silo.Application.Shared.Contracts;

public sealed class TeamYarSecResponseCapture(Stream originalBody) : IDisposable
{
    public Stream OriginalBody { get; } = originalBody;

    public MemoryStream ResponseBody { get; } = new();

    public List<int> ConnectionIds { get; } = [];

    public void Dispose() => ResponseBody.Dispose();
}
