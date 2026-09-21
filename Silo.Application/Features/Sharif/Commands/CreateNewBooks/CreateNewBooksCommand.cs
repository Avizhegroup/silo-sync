namespace Silo.Application.Features;

public class CreateNewBooksCommand : IRequest<CreateNewBooksVm>
{
    public List<CreateNewBookItemDto> Data { get; set; } = new();


}
