namespace Silo.Application.Features;
public class SaveBookOpsCommand : IRequest<SaveBookOpsVm>   
{
    public int OpCode { get; set; }
    public List<string> Serials { get; set; }
    public bool IsBorrow { get; set; }
}
