using Newtonsoft.Json.Linq;
using Silo.Application.Dto;
using Silo.Application.Dto.DynamicField;

namespace Silo.Components.DynamicField;
public partial class DynamicFieldFillValue
{
    [Parameter][EditorRequired] public List<DynamicFieldWithValueDto> DynamicFieldsDto { get; set; }

    public async Task<string> GetJsonData()
    {
        dynamic exo = new System.Dynamic.ExpandoObject();

        foreach (var field in DynamicFieldsDto)
        {
            ((IDictionary<String, Object>)exo).Add(field.Title, field.Value);
        }

        return Newtonsoft.Json.JsonConvert.SerializeObject(exo);
    }

    public async Task FillJsonData(JToken data)
    {
        foreach (var field in DynamicFieldsDto)
        {
            if (field.Title is not null)
            {
                if (data[field.Title.Trim()] is not null)
                {
                    field.Value = data[field.Title.Trim()].ToString();
                }
                else
                {
                    field.Value = "";
                }
            }
        }

        StateHasChanged();
    }

    public async Task<List<ChoosableKeyValue>> GetKeyValueList()
    {
        return DynamicFieldsDto
            .Select(p => new ChoosableKeyValue()
            {
                Key = p.Title,
                Value = p.Value,
            }).ToList();
    }

    public async Task Clear()
    {
        DynamicFieldsDto.ForEach(p => p.Value = string.Empty);
    }
}
