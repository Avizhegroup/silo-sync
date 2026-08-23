
namespace Silo.Api.Tools;
public static class DynamicFilteringTools
{
    public static string GetStaticWhere(ReportFilter filter)
    {
        string where = string.Empty;

        if (filter.EqualityType == FilterEqualityType.Equals)
        {
            where += $" {filter.SqlWhereCommand} IN('{string.Join("','", filter.Values)}')  ";
        }
        else if (filter.EqualityType == FilterEqualityType.Like)
        {
            foreach (var value in filter.Values)
            {
                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} LIKE N'%{value}%') ";
            }
        }
        else
        {
            foreach (var value in filter.Values)
            {
                string operatorString = filter.EqualityType switch
                {
                    FilterEqualityType.SmallerThan => "<=",
                    FilterEqualityType.BiggerThan => ">=",
                };

                if (where.HasValue())
                {
                    where += " OR ";
                }

                if (value.StartsWith("dbo.JalaliDateToGeorgianDate"))
                {
                    where += $" ({filter.SqlWhereCommand} {operatorString} {value}) ";
                }
                else
                {
                    where += $" ({filter.SqlWhereCommand} {operatorString} N'{value}') ";
                }
            }
        }

        return "(" + where + ")";
    }
    public static string GetDynamicWhere(ReportFilter filter)
    {
        string where = string.Empty;

        if (filter.EqualityType == FilterEqualityType.Equals)
        {
            where = $" {filter.SqlWhereCommand} IN(N'{string.Join("',N'", filter.Values)}')  ";
        }
        else if (filter.EqualityType == FilterEqualityType.Like)
        {

            foreach (var value in filter.Values)
            {
                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} LIKE N'%{value}%') ";
            }
        }
        else
        {
            foreach (var value in filter.Values)
            {
                string operatorString = filter.EqualityType switch
                {
                    FilterEqualityType.SmallerThan => "<=",
                    FilterEqualityType.BiggerThan => ">=",
                };

                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} {operatorString} N'{value}') ";
            }
        }

        return "(" + where + ")";
    }
    public static string GetTechnicalInfoWhere(ReportFilter filter)
    {
        string where = string.Empty;

        if (filter.EqualityType == FilterEqualityType.Equals)
        {
            where = $" {filter.SqlWhereCommand} IN('{string.Join("','", filter.Values)}')  ";
        }
        else if (filter.EqualityType == FilterEqualityType.Like)
        {
            foreach (var value in filter.Values)
            {
                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} LIKE N'%{value}%') ";
            }
        }
        else
        {
            foreach (var value in filter.Values)
            {
                string operatorString = filter.EqualityType switch
                {
                    FilterEqualityType.SmallerThan => "<=",
                    FilterEqualityType.BiggerThan => ">=",
                };

                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} {operatorString} N'{value}') ";
            }
        }

        return "(" + where + ")";
    }
}
