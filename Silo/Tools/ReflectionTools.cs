namespace Silo.Tools;
public static class ReflectionTools
{
    public static T CastSearchObjectFromAppliedFilters<T>(List<ReportFilter> filters)
    {
        var search = (T)Activator.CreateInstance(typeof(T));

        Type type = search.GetType();

        foreach (var property in type.GetProperties())
        {
            var filter = filters.FirstOrDefault(p => p.Type == FilterType.Static && p.FieldName.Equals(property.Name));

            if (property.PropertyType == typeof(string))
            {
                if (filter is not null)
                {
                    property.SetValue(search, filter.Value);
                }
                else
                {
                    property.SetValue(search, "-1");
                }
            }
            else if (property.PropertyType == typeof(int?))
            {
                if (filter is not null)
                {
                    property.SetValue(search, int.Parse(filter.Value));
                }
                else
                {
                    property.SetValue(search, -1);
                }
            }
            else if (property.PropertyType == typeof(bool))
            {
                if (filter is not null)
                {
                    property.SetValue(search, filter.Value.Equals("true"));
                }
                else
                {
                    property.SetValue(search, false);
                }
            }
        }

        return search;
    }
}
