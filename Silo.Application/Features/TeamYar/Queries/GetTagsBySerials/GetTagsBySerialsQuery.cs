namespace Silo.Application.Features;

public class GetTagsBySerialsQuery : IRequest<GetTagsBySerialsVm>
{
    /// <summary>
    /// لیست سری ساخت
    /// </summary>
    public List<string>? Serials { get; set; }

    /// <summary>
    /// از تاریخ (مثلا 1405/07/01)
    /// </summary>
    public string? FromDate { get; set; }

    /// <summary>
    /// از تاریخ (مثلا 1405/07/30)
    /// </summary>
    public string? ToDate { get; set; }
}
