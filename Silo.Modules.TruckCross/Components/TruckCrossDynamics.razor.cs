namespace Silo.Modules.TruckCross.Components;

public partial class TruckCrossDynamics : IDisposable
{
    private bool _disposed = false;

    public List<GetAllDynamicFieldVm> SectionDynamicFields { get; private set; } = new();

    [Parameter, EditorRequired] public required int SectionId { get; set; }
    [Parameter, EditorRequired] public required TruckCrossDataDto CrossRequest { get; set; }

    [Parameter]
    public List<string> RowStyles { get; set; } = new List<string>();
    [Parameter] public string RichTextMarginTop { get; set; } = "0px";
    [Parameter] public List<string> LabelMarginRightsFirst { get; set; } = new();
    [Parameter] public List<string> LabelMarginRightsSecond { get; set; } = new();
    [Parameter] public string LabelMarginRightFirst { get; set; } = "5px";
    [Parameter] public string LabelMarginRightSecond { get; set; } = "25px";
    [Parameter] public string FieldWidth { get; set; } = "400px";
    
    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public required RfidConnectApi Api { get; set; }
    [CascadingParameter] public required TruckCrossComponentsContext TruckCrossContext { get; set; }
    public bool IsVisible { get; private set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadDynamicFields();

        SubscribeToEvents();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (CrossRequest is null)
        {
            return;
        }

        await LoadFieldValues();
    }

    private async Task LoadDynamicFields()
    {
        var response = await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>(
            "wms/Document",
            "GetDynamicFieldsBySectionId",
            new KeyValuePair<string, object>("sectionId", SectionId));

        var newFields = response?.Value ?? new List<GetAllDynamicFieldVm>();

        if (SectionDynamicFields.Any())
        {
            foreach (var newField in newFields)
            {
                var existingField = SectionDynamicFields.FirstOrDefault(f => f.Id == newField.Id);
                
                if (existingField != null)
                {
                    var currentValue = existingField.Value;
                    existingField.Title = newField.Title;
                    existingField.ValueType = newField.ValueType;
                    existingField.ValueOptions = newField.ValueOptions;
                    existingField.DefaultValue = newField.DefaultValue;
                    existingField.IsRequired = newField.IsRequired;
                    existingField.IsReadOnly = newField.IsReadOnly;
                    existingField.Order = newField.Order;

                    if (currentValue.HasNoValue()
                    && newField.DefaultValue.HasValue())
                    {
                        existingField.Value = newField.DefaultValue;
                    }
                    else
                    {
                        existingField.Value = currentValue;
                    }
                }
                else
                {
                    SectionDynamicFields.Add(newField);
                }
            }

            var fieldsToRemove = SectionDynamicFields.Where(existing =>
                newFields.Neither(newField => newField.Id == existing.Id)).ToList();

            foreach (var fieldToRemove in fieldsToRemove)
            {
                SectionDynamicFields.Remove(fieldToRemove);
            }
        }
        else
        {
            SectionDynamicFields = newFields;
        }

        SectionDynamicFields = SectionDynamicFields.OrderBy(f => f.Order ?? 0)
                                                   .ThenBy(f => f.Title)
                                                   .ToList();

        IsVisible = SectionDynamicFields.Any();

        StateHasChanged();
    }

    public async Task OnShowCross(TruckCrossDataDto cross)
    {
        await OnTruckCrossDataChanged(cross);
    }

    private async Task LoadFieldValues()
    {
        if (CrossRequest?.DynamicDataDict is null || SectionDynamicFields.Neither())
        {
            return;
        }

        foreach (var dynamicField in SectionDynamicFields)
        {
            if (CrossRequest.DynamicDataDict.TryGetValue(dynamicField.Id, out string? value))
            {
                dynamicField.Value = value;
            }
            else if (string.IsNullOrEmpty(dynamicField.Value) && !string.IsNullOrEmpty(dynamicField.DefaultValue))
            {
                dynamicField.Value = dynamicField.DefaultValue;
            }
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnSaveCrossClick()
    {
        await SaveDynamicFieldValues();
    }

    private void SubscribeToEvents()
    {
        if (TruckCrossContext is null)
        {
            return;
        }

        TruckCrossContext.SaveHasFired += OnSaveCrossClick;

        TruckCrossContext.TruckCrossDataHasChanged += OnTruckCrossDataChanged;
    }

    private void UnsubscribeFromEvents()
    {
        if (TruckCrossContext is null)
        {
            return;
        }

        TruckCrossContext.SaveHasFired -= OnSaveCrossClick;

        TruckCrossContext.TruckCrossDataHasChanged -= OnTruckCrossDataChanged;
    }

    private async Task OnTruckCrossDataChanged(TruckCrossDataDto cross)
    {
        if (cross is null)
        {
            return;
        }

        CrossRequest = cross;

        await LoadFieldValues();
    }

    private async Task SaveDynamicFieldValues()
    {
        if (CrossRequest is null)
        {
            return;
        }

        CrossRequest.DynamicDataDict ??= new Dictionary<int, string>();

        foreach (var field in SectionDynamicFields)
        {
            var value = field.Value ?? string.Empty;

            if (CrossRequest.DynamicDataDict.ContainsKey(field.Id))
            {
                CrossRequest.DynamicDataDict[field.Id] = value;
            }
            else
            {
                CrossRequest.DynamicDataDict.Add(field.Id, value);
            }
        }

        CrossRequest.DynamicDataDict = CrossRequest.DynamicDataDict;

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            UnsubscribeFromEvents();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
