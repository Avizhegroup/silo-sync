namespace Silo.Application.Features;

public class GetAllFreezeHeaderVm
{
    public int HeaderId { get; set; }
    public string FreezeStatus { get; set; }
    public DateTime DateTime { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int Count { get; set; }
}
