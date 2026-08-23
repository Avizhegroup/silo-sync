using Silo.Application.Dto.Filter;

namespace Silo.Application.Features;
public class GetTruckCrossDynamicSearchQuery : IRequest<GetTruckCrossDynamicSearchVm>
{
    public List<ReportFilterGeneric<TruckCrossReportFilterType>> Filters { get; set; }
    public List<ReportColumnGeneric<TruckCrossReportColumnsType>> SelectColumns { get; set; }
    public List<ReportCalculatingColumn<TruckCrossReportColumnsType>> Calculating { get; set; }
    public ReportColumnGeneric<TruckCrossReportColumnsType> Pivot { get; set; }
    public List<ReportColumnGeneric<TruckCrossReportColumnsType>> DataMiningElements { get; set; }
}
