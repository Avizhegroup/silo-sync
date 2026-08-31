
namespace Silo.Application.Features;

public class SaveTextResourcesCommand : IRequest<SaveTextResourcesVm>
{
    public List<TextResourceDto> Items { get; set; } = new();

    public List<int> DeletedIds { get; set; } = new();
}

public class TextResourceDto
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string? Value { get; set; }
}
