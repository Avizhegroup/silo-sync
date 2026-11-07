namespace Silo.Application.Features;

public class SaveBooksCommand : IRequest<SaveBooksVm>
{
    public List<SaveBooksDto> Data { get; set; }
}
