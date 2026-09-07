namespace Silo.Application.Features;

public class TestSyncSourceQueryCommand : IRequest<TestSyncSourceQueryVm>
{
    public int Id { get; set; }
    public int SampleSize { get; set; } = 10;
}
