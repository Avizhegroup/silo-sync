using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Product.Pages;
public partial class Freeze
{
    public bool IsLoading = false;
    public SaveFreezeCommand Command = new();
    public List<GetAllProductBySerialVm> SelectedTags = new();
    public List<TelerikDropDownItemGeneric<bool>> FreezeStatuses = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Status_Freezed,
            Value = true
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Status_Not_Freezed,
            Value = false
        }
    };

    public ProductSerialModal ProductModal { get; set; }
    public TelerikGrid<GetAllProductBySerialVm> Grid { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        if (SelectedTags.Any())
        {
            Command.Serials = SelectedTags.Select(p => p.ProductSerial).ToList();

            int result = (await Api.PostAsync<int>("SSaveFreeze"
                , new KeyValuePair<string, object>("freeze", Command))).Value;
           
            if (result > 0)
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            }
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Serials_Add, "error");
        }

        IsLoading = false;
    }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Command = new();

        SelectedTags = new();
    }

    public async Task OnDeleteSerialProductClick(GetAllProductBySerialVm product)
    {
        SelectedTags.Remove(product);

        Grid.Rebind();
    }

    public async Task OnSelectSerials(List<GetAllProductBySerialVm> products)
    {
        foreach (GetAllProductBySerialVm product in products)
        {
            if (!SelectedTags.Any(p=>p.ProductSerial == product.ProductSerial))
            {
                SelectedTags.Add(product);
            }
        }

        Grid.Rebind();
    }
}
