using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Identity.Client;
using Silo.Shared.Components;

namespace Silo.Modules.Inspect.Pages;
public partial class DefineInspectElements
{
    public bool IsLoading = true;
    public bool IsSelectAllTypes = false;
    public string UserToken;
    public string Option = string.Empty;
    public List<string> Options = new();
    public SaveInspectElementCommand Element = new();
    public List<GetAllInspectElementVm> InspectElements;
    public List<ChoosableKeyValue> Types;
    public List<TelerikDropDownItemGeneric<InspectElementType>> ElementTypes = new()
    {
        new()
        {
            Name= TextResources.APP_StringKeys_Inspect_Type_Checkbox,
            Value = InspectElementType.MultiOption
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Inspect_Type_Combobox,
            Value = InspectElementType.OneOption
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Inspect_Type_Int,
            Value = InspectElementType.Int
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Inspect_Type_String,
            Value = InspectElementType.String
        }
    };
    public List<TelerikDropDownItem> OneOptionValues = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Verified,
            Value = "true"
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Unverified,
            Value = "false"
        }
    };

    [Inject] public SiloAuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }


    public Modal ModalResult { get; set; }
    public Modal ModalRemove { get; set; }
    public Modal ModalInspects { get; set; }
    public TelerikDropDownList<string, string> ComboMultiOptions { get; set; }

    protected override async Task SiloInitializer()
    {
        UserToken = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        await RefreshElementData();
       var type= await FormalCache.GetTypes();

        Types = type
                          .Select(p => new ChoosableKeyValue()
                          {
                              IsChoosed = false,
                              Key = p.Code,
                              Value = p.Title.ToString()
                          }).ToList();

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        Option = string.Empty;

        Options = new();

        Element = new();

        IsSelectAllTypes = false;

        Types.ForEach(p => p.IsChoosed = false);
    }

    public async Task OnSubmitClick(EditContext context)
    {
        Element.ProductTypes = new();

        Element.Options = new();

        if (IsSelectAllTypes)
        {
            Element.ProductTypes.Add("ALL");
        }
        else
        {
            Types.ForEach(p =>
            {
                if (p.IsChoosed)
                {
                    Element.ProductTypes.Add(p.Key);
                }
            });
        }

        if (Element.InspectElementType == InspectElementType.MultiOption)
        {
            Options.ForEach(p => Element.Options.Add(p));
        }

        if (!CheckIsFormValid())
        {
            return;
        }

        int result = (await Api.PostAsync<int>("SSaveElement",
            new KeyValuePair<string, object>("inspect", Element))).Value;

        if (Element.Id == 0)
        {
            Element.Id = result;
        }

        await RefreshElementData();

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        StateHasChanged();
    }

    public async Task OnRemoveModalClick(MouseEventArgs e)
    {
        if (Element.Id == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await ModalRemove.Open(e);
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        bool result = (await Api.PostAsync<bool>("SDeleteElement",
                          new KeyValuePair<string, object>("elementId", Element.Id))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        await RefreshElementData();

        await OnClearClick(e);
    }

    public async Task OnSelectElement(GetAllInspectElementVm element)
    {
        Element = Mapper.Map<SaveInspectElementCommand>(element);

        if (Element.ProductTypes.First().Equals("ALL"))
        {
            IsSelectAllTypes = true;

            Types.ForEach(p => p.IsChoosed = true);
        }
        else
        {
            Types.ForEach(p => p.IsChoosed = Element.ProductTypes.Any(et => et.Equals(p.Key)));
        }

        Options = element.Options;

        await ModalInspects.Close(new());
    }

    public void OnElementTypeChanged()
    {
        Element.Value = string.Empty;
    }

    public async Task OnTypeSelectAllChange(ChangeEventArgs e)
    {
        Types.ForEach(p => p.IsChoosed = (bool)e.Value);

        IsSelectAllTypes = (bool)e.Value;

        StateHasChanged();
    }

    public async Task OnTypeSelectChange(bool status)
    {
        if (IsSelectAllTypes && status)
        {
            IsSelectAllTypes = false;
        }

        StateHasChanged();
    }

    public async Task OnAddOptionClick(MouseEventArgs e)
    {
        if (Option.HasValue())
        {
            Options.Add(Option);

            Option = string.Empty;

            ComboMultiOptions.Rebind();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");

            await ModalResult.Open(e);
        }
    }

    public async Task OnRemoveOption(string option)
    {
        Options.Remove(option);

        ComboMultiOptions.Rebind();
    }

    public async Task RefreshElementData()
    {
        IsLoading = true;

        InspectElements = (await Api.PostAsync<List<GetAllInspectElementVm>>("SGetAllElements")).Value;

        IsLoading = false;
    }

    private bool CheckIsFormValid()
    {
        if (Element.InspectElementType == InspectElementType.NotSpecified)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_ElementType, "error");

            return false;
        }

        if (Element.InspectElementType == InspectElementType.MultiOption)
        {
            if (!Element.Options.Any())
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Options_Add, "error");

                return false;
            }
        }

        if (!Element.ProductTypes.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_ProductType_Add, "error");

            return false;
        }

        if (Element.Id.Equals(0))
        {
            if (InspectElements.Any(p => p.Row.Equals(Element.Row)))
            {
                Notification.Show(
                    string.Format(TextResources.APP_StringKeys_Validation_Remote_Uniqueness, TextResources.APP_StringKeys_InspectElement_Row)
                    , "error");

                return false;
            }
        }
        else
        {
            if (InspectElements.Any(p => p.Id.NotEquals(Element.Id) && p.Row.Equals(Element.Row)))
            {
                Notification.Show(
                    string.Format(TextResources.APP_StringKeys_Validation_Remote_Uniqueness, TextResources.APP_StringKeys_InspectElement_Row)
                    , "error");

                return false;
            }
        }

        return true;
    }
}
