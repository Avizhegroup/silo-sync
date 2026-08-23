using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Silo.Application.Dto;
using Silo.Modules.TruckCross.Components;
using Silo.Shared.Components;
using Telerik.Blazor;

namespace Silo.Modules.TruckCross.Pages;

public partial class TruckCross
{
    public bool IsLoading = true;
    public TruckCrossItemModes Mode = new();
    public TruckCrossDataDto CrossRequest = new();
    public GetTruckCrossQuery Search = new();
    public List<TruckCrossDataDto> NotExitedCrosses;
    public List<TruckCrossDataDto> ReviewPlaqueCrosses;
    public List<GetAllTruckCrossPresentCauseVm> Causes = new();
    public List<GetAllTruckTypesVm> TruckTypes = new();
    public List<GetAllTruckCompaniesVm> TruckCompanies = new();
    public List<GetAllTruckCrossOperationTypesVm> OperationTypes = new();
    public List<GetAllTruckCrossOperationDestinationsVm> OperationDestinations = new();
    public List<GetAllTruckCrossShipmentVm> Shipments = new();
    public List<GetAllTruckCrossCustomerVm> Customers = new();
    public List<GetAllAcceptPlacesVm> AcceptPlaces = new();
    public string UserId;
    public string Username;
    public List<TelerikDropDownItem> Destinations = new();
    public GetAllTruckCrossPresentCauseVm CauseRequest = new();
    public List<GetAllTruckCrossProductTypeVm> TruckCrossProductTypes = new();
    public TruckCrossItemDto TruckCrossItemRequest = new();
    public List<GetTruckCrossItemsByTruckCrossIdVm> Items;
    public List<TelerikDropDownItemGeneric<int>> PaymentTypes;
    public List<TelerikDropDownItemGeneric<TruckCrossItemModes>> TruckCrossItemTypes;

    public TruckCrossComponentsContext TruckCrossContext { get; set; } = new();

    public TruckCrossPresent TruckCrossPresent { get; set; }
    public TruckCrossEnter TruckCrossEnter { get; set; }
    public TruckCrossExit TruckCrossExit { get; set; }
    public Gallery Gallery { get; set; }
    public Modal ModalReviewPlaque { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    [Parameter] public int? Id { get; set; }

    [CascadingParameter] public DialogFactory Dialog { get; set; }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        Username = (await AuthState.GetAuthenticationStateAsync()).User.GetUsername();

        Causes = (await Api.PostAsyncByUri<List<GetAllTruckCrossPresentCauseVm>>("wms/TruckCross", "SGetTruckPresentCause")).Value;

        TruckTypes = (await Api.PostAsyncByUri<List<GetAllTruckTypesVm>>("wms/TruckCross", "SGetTruckType")).Value;

        TruckCompanies = (await Api.PostAsyncByUri<List<GetAllTruckCompaniesVm>>("wms/TruckCross", "SGetAllTruckCompany")).Value;

        OperationDestinations = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationDestinationsVm>>("wms/TruckCross", "SGetAllTruckCrossOperationDestination")).Value;

        Shipments = (await Api.PostAsyncByUri<List<GetAllTruckCrossShipmentVm>>("wms/TruckCross", "SGetAllTruckCrossShipment")).Value;

        Customers = (await Api.PostAsyncByUri<List<GetAllTruckCrossCustomerVm>>("wms/TruckCross", "SGetAllTruckCrossCustomer")).Value;

        AcceptPlaces = (await Api.PostAsyncByUri<List<GetAllAcceptPlacesVm>>("wms/truckcross", "SGetAllTruckCrossAcceptPlace")).Value;

        PaymentTypes = new()
        {
            new ()
            {
                Value = 1,
                Name = TextResources.APP_StringKeys_Payment_BySender
            },
            new ()
            {
                Value = 2,
                Name = TextResources.APP_StringKeys_Payment_ByReciever
            },
            new ()
            {
                Value = 3,
                Name = TextResources.APP_StringKeys_Payment_ByCompany
            }
        };

        TruckCrossItemTypes = new()
        {
            new()
            {
                Value =TruckCrossItemModes.Enter,
                Name = TextResources.APP_StringKeys_TruckCross_Steps_Enter
            },
            new()
            {
                Value = TruckCrossItemModes.Exit,
                Name = TextResources.APP_StringKeys_TruckCross_Steps_Exit
            }
        };

        await RefreshNotExited();

        SetDefaultValueOfDropDowns();

        if (Id is not null)
        {
            IsLoading = true;

            TruckCrossDataDto cross = (await Api.PostAsyncByUri<TruckCrossDataDto>("wms/TruckCross", "SSearchAllTruckCross"
                              , new KeyValuePair<string, object>("search", new GetTruckCrossQuery()
                              {
                                  Id = Id
                              }))).Value;

            if (cross is not null)
            {
                await ShowCross(cross);

                if (CrossRequest.TruckCrossStatus == TruckCrossStatuses.Revoke)
                {
                    Mode = TruckCrossItemModes.Present;
                }
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_TruckCross_Notfound, "error");
            }
        }

        IsLoading = false;
    }

    #region Clear Button
    public async Task OnClearClick(MouseEventArgs e)
    {
        CrossRequest = new();

        Mode = TruckCrossItemModes.Present;

        SetDefaultValueOfDropDowns();
    }

    public async Task OnRevokeSuccess()
    {
        CrossRequest = new();

        Mode = TruckCrossItemModes.Present;

        SetDefaultValueOfDropDowns();

        await RefreshNotExited();
    }

    public async Task OnSearchClearClick(MouseEventArgs e)
    {
        Search = new();
    }
    #endregion

    #region Unexited
    public async Task OnUnxitedCrossClick(GridRowClickEventArgs e)
    {
        TruckCrossDataDto cross = e.Item as TruckCrossDataDto;

        await ShowCross(cross);
    }

    public async Task OnUnxitedCrossRefreshDataClick(MouseEventArgs e)
    {
        await RefreshNotExited();
    }
    #endregion

    #region Event
    public async Task OnSearchValidSubmit(EditContext context)
    {
        if (Search.NationalCode.HasNoValue()
         && Search.DriverName.HasNoValue()
         && Search.DriverPhone.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");

            return;
        }

        IsLoading = true;

        TruckCrossDataDto cross = (await Api.PostAsyncByUri<TruckCrossDataDto>("wms/TruckCross", "SSearchTruckCross"
                          , new KeyValuePair<string, object>("search", Search))).Value;

        if (cross is not null)
        {
            await ShowCross(cross);
        }

        IsLoading = false;
    }

    public async Task OnStepperChange(int targetindex)
    {
        if (CrossRequest.TruckCrossStatus == TruckCrossStatuses.Revoke)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_TruckCross_Status, "error");

            return;
        }

        if (targetindex > (int)Mode)
        {
            if (targetindex == 1)
            {
                if (Mode == TruckCrossItemModes.Present)
                {
                    if (!CrossRequest.PresentIsSaved)
                    {
                        Notification.Show(
                            string.Format(TextResources.APP_StringKeys_TruckCross_Validation_Step
                            , TextResources.APP_StringKeys_TruckCross_Steps_Present)
                            , "error");

                        return;
                    }
                }
                else
                {
                    if (!CrossRequest.ExitIsSaved)
                    {
                        Notification.Show(
                            string.Format(TextResources.APP_StringKeys_TruckCross_Validation_Step
                            , TextResources.APP_StringKeys_TruckCross_Steps_Exit)
                            , "error");

                        return;
                    }
                }
            }

            if (targetindex == 2)
            {
                if (!CrossRequest.PresentIsSaved
                 || !CrossRequest.EnterIsSaved)
                {
                    Notification.Show(
                        string.Format(TextResources.APP_StringKeys_TruckCross_Validation_Step
                        , TextResources.APP_StringKeys_TruckCross_Steps_Enter)
                        , "error");

                    return;
                }
            }
        }

        Mode = (TruckCrossItemModes)targetindex;
    }

    public async Task OnChooseReviewTruckCross(TruckCrossDataDto cross)
    {
        await OnClearClick(new());

        CrossRequest.NationalCode = cross.NationalCode;

        CrossRequest.DriverName = cross.DriverName;

        CrossRequest.DriverPhone = cross.DriverPhone;

        CrossRequest.TruckCrossCompanyId = cross.TruckCrossCompanyId;

        CrossRequest.Plaque = cross.Plaque;

        SetPlaqueParts(cross.Plaque);

        CrossRequest.TypeId = cross.TypeId;

        CrossRequest.DynamicData = cross.DynamicData;

        await ModalReviewPlaque.Close(new());
    }

    public async Task OnGalleryOcrExecuted(GalleryOcrExtractedTextDto result)
    {
        try
        {
            if (result.OcrType == GalleryOcrTypes.Plaque)
            {
                var cleanedText = result.ExtractedText
                     .Replace("```", "")
                     .Trim();

                var plaqueParts = cleanedText.Split("-");

                var isConfirmed = await Dialog.ConfirmAsync($"متن استخراج شده برابر با \n {cleanedText.Replace("-", "")} \n است ", "", "استفاده کن", TextResources.APP_StringKeys_Disconfirm);

                if (!isConfirmed)
                {
                    return;
                }

                CrossRequest.FirstPart = plaqueParts[0];

                CrossRequest.Character = plaqueParts[1];

                CrossRequest.SecondPart = plaqueParts[2];

                CrossRequest.CityPart = plaqueParts[4];
            }

            if (result.OcrType == GalleryOcrTypes.NationalCard)
            {

                var cleanedText = result.ExtractedText
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                using var jsonDoc = JsonDocument.Parse(cleanedText);
                var root = jsonDoc.RootElement;

                if (root.ValueKind != JsonValueKind.Object)
                {
                    return;
                }

                var driverName = root.TryGetProperty("fullName", out var nameElement)
                    ? nameElement.GetString()
                    : null;

                var nationalCode = root.TryGetProperty("nationalId", out var idElement)
                    ? idElement.GetString()
                    : null;

                string ncExtractedText = $"نام و نام خانوادگی: {driverName} , کدملی: {nationalCode}";

                IsLoading = false;

                StateHasChanged();

                var isConfirmed = await Dialog.ConfirmAsync($"متن استخراج شده برابر با \n {ncExtractedText} \n است ", "", "استفاده کن", TextResources.APP_StringKeys_Disconfirm);

                if (!isConfirmed)
                {
                    return;
                }

                CrossRequest.DriverName = driverName;

                CrossRequest.NationalCode = nationalCode;
            }
        }
        catch (JsonException ex)
        {
            Notification.Show($"خطا در پردازش داده‌ها: {ex.Message}", "error");
        }
        finally
        {
            IsLoading = false;
        }
    }
    #endregion

    #region Private
    private async Task ShowCross(TruckCrossDataDto cross)
    {
        CrossRequest = new();

        Search = Mapper.Map<GetTruckCrossQuery>(cross);

        if (cross is null)
        {
            Mode = TruckCrossItemModes.Present;

            Notification.Show(TextResources.APP_StringKeys_Validation_NotFound, "error");

            return;
        }

        if (cross.PresentCause is not null)
        {
            OperationTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationTypesVm>>("wms/TruckCross", "SGetTruckCrossOperationTypesByCause",
                new KeyValuePair<string, object>("presentCauseId", cross.PresentCause))).Value;

            TruckCrossProductTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossProductTypeVm>>("wms/TruckCross"
                , "SGetTruckCrossProductTypesByCause",
                new KeyValuePair<string, object>("presentCauseId", cross.PresentCause))).Value;
        }

        Items = (await Api.PostAsyncByUriAndContext<List<GetTruckCrossItemsByTruckCrossIdVm>>("wms/TruckCross", "SGetTruckCrossItemsByTruckCrossId"
                , new GetTruckCrossItemsByTruckCrossIdVmContext()
                , new KeyValuePair<string, object>("truckCrossId", cross.Id))).Value;

        CrossRequest = cross;

        SetPlaqueParts(CrossRequest.Plaque);

        CrossRequest.PresentIsSaved = true;

        Mode = TruckCrossItemModes.Empty;

        if (CrossRequest.TruckCrossStatus == TruckCrossStatuses.Exit)
        {
            Mode = TruckCrossItemModes.Exit;

            CrossRequest.ExitIsSaved = true;

            CrossRequest.EnterIsSaved = true;
        }
        else if (CrossRequest.TruckCrossStatus == TruckCrossStatuses.Enter)
        {
            Mode = TruckCrossItemModes.Exit;

            CrossRequest.EnterIsSaved = true;
        }
        else
        {
            Mode = TruckCrossItemModes.Enter;
        }

        await TruckCrossContext.SetTabCross(CrossRequest);
    }

    private async Task RefreshNotExited()
    {
        IsLoading = true;

        NotExitedCrosses = (await Api.PostAsyncByUri<List<TruckCrossDataDto>>("wms/TruckCross", "SGetUnexitedCrosses")).Value;

        IsLoading = false;
    }

    private void SetPlaqueParts(string plaque)
    {
        try
        {
            CrossRequest.FirstPart = plaque.Substring(0, 2);

            CrossRequest.Character = plaque.Substring(2, 1);

            CrossRequest.SecondPart = plaque.Substring(3, 3);

            CrossRequest.CityPart = plaque.Substring(6, 2);
        }
        catch (Exception) { }
    }

    private void SetDefaultValueOfDropDowns()
    {
        CrossRequest.PresentCause = Causes.Count.Equals(1) ? Causes.FirstOrDefault().Id : 0;

        CrossRequest.TypeId = TruckTypes.Count.Equals(1) ? TruckTypes.FirstOrDefault().Id : 0;

        CrossRequest.TruckCrossCompanyId = TruckCompanies.Count.Equals(1) ? TruckCompanies.FirstOrDefault().Id : 0;

        CrossRequest.PresentOperationDestinationId = OperationDestinations.Count.Equals(1) ? OperationDestinations.FirstOrDefault().Id : 0;

        CrossRequest.PresentShipmentId = Shipments.Count.Equals(1) ? Shipments.FirstOrDefault().Id : 0;

        CrossRequest.PresentCustomerId = Customers.Count.Equals(1) ? Customers.FirstOrDefault().Id : 0;
    }
    #endregion
}
