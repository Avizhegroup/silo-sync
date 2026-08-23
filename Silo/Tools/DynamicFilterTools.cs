namespace Silo.Shared.Tools;
public class DynamicFilterTools
{
    public static List<ReportFilter> AggregateFilterValues(List<ReportFilter> applyFilters)
    {
        List<ReportFilter> filters = new();

        filters = applyFilters.GroupBy(p => p.FieldName)
                              .Select(p => new
                               ReportFilter()
                              {
                                  FieldName = p.Key,
                                  Type = p.First().Type,
                                  Component = p.First().Component,
                                  EqualityType = p.First().EqualityType,
                                  AddType = p.First().AddType,
                                  AdditionalData = p.First().AdditionalData,
                                  Values = p.SelectMany(q => q.Values ?? new List<string>() { q.Value }).Distinct().ToList()
                              }).ToList();

        return filters;
    }

}
