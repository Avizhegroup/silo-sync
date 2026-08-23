
using Silo.Application.Features;

namespace Silo.Modules.Document.Components;

public partial class DocumentItemGridHierarchy
{
    public bool IsLoading = true;
    public List<DocumentItemDto> DocumentItems = new();

    [EditorRequired][Parameter] public string DocumentKey { get; set; }
    [EditorRequired][Parameter] public string DocumentType { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        IsLoading = true;

        DocumentItems =  (await Api.PostAsyncByUri<List<DocumentItemDto>>("wms/Document","SGetAllDocItems"
                            , new KeyValuePair<string, object>("documentKey", DocumentKey)
                            , new KeyValuePair<string, object>("documentType", DocumentType)
                            )).Value;

        IsLoading = false;
    }
}
