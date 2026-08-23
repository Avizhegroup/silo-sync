using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.TruckCross.Pages;
public partial class TruckCrossConfigs
{
    public bool IsLoading = true;
    public TruckCrossConfig Request = new();
    public TruckCrossConfigWithCause RequestWithCause = new();
    public List<TruckCrossConfig> Configs;
    public List<TruckCrossConfigWithCause> ConfigsWithCause;
    public int OperationTypeCauseIdFilter;
    public string ModeTitle = TextResources.APP_StringKeys_TruckCross_TypeTruck;
    public string ProductTypeCauseTitlesFilter;
    public List<GetAllTruckCrossPresentCauseVm> Causes = new();
    public List<GetAllTruckCrossPresentCauseVm> ProductTypeCausesFilters = new();
    public TruckCrossConfigModes Mode;
    public List<TelerikDropDownItemGeneric<TruckCrossConfigModes>> Modes = new();

    public TelerikGrid<TruckCrossConfig> GridConfigs { get; set; }
    public TelerikGrid<TruckCrossConfigWithCause> GridConfigsWithCause { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    public Modal ModalDelete { get; set; }
    public Modal ModalWithClauseDelete { get; set; }
    public Modal ModalTruckCrossCauses { get; set; }
    public Modal ModalTruckCrossCausesFilter { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Init();

        Causes = (await Api.PostAsyncByUri<List<GetAllTruckCrossPresentCauseVm>>("wms/TruckCross", "SGetTruckPresentCause")).Value;
       
        ProductTypeCausesFilters = Causes;

        await GetAllConfigs();

        IsLoading = false;
    }

#region Init
    public async Task OnModeChangeClick(TruckCrossConfigModes newMode)
    {
        IsLoading = true;

        if (newMode.NotEquals(Mode))
        {
            Mode = newMode;

            await OnRefreshClick(new());

            Configs = null;

            ConfigsWithCause = null;

            await GetAllConfigs();

            Causes = (await Api.PostAsyncByUri<List<GetAllTruckCrossPresentCauseVm>>("wms/TruckCross", "SGetTruckPresentCause")).Value;

            ModeTitle = Mode switch
            {
                TruckCrossConfigModes.TruckCrossProductType => TextResources.APP_StringKeys_Field_ProductType,
                TruckCrossConfigModes.TruckCrossPresentCause => TextResources.APP_StringKeys_TruckCross_Present_Cause,
                TruckCrossConfigModes.TruckCrossCustomer => TextResources.APP_StringKeys_TruckCross_Customer,
                TruckCrossConfigModes.TruckCrossShipment => TextResources.APP_StringKeys_TruckCross_Shipment,
                TruckCrossConfigModes.TruckCrossAcceptPlace => TextResources.APP_StringKeys_TruckCross_Enter_AcceptPlace,
                TruckCrossConfigModes.TruckCompany => TextResources.APP_StringKeys_Company,
                TruckCrossConfigModes.TruckCrossOperationDestination => TextResources.APP_StringKeys_TruckCross_Operation_Destination,
                TruckCrossConfigModes.TruckType => TextResources.APP_StringKeys_TruckCross_TypeTruck,
                TruckCrossConfigModes.TruckCrossOperationType => TextResources.APP_StringKeys_TruckCross_Operation_Type,
                TruckCrossConfigModes.NotChoosed => ""
            };
        }

        IsLoading = false;
    }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        IsLoading = true;

        Request = new();

        RequestWithCause = new();

        OperationTypeCauseIdFilter = new();

        ProductTypeCauseTitlesFilter = string.Empty;

        Causes.ForEach(p => p.IsChoosed = false);

        ProductTypeCausesFilters.ForEach(p=>p.IsChoosed = false);

        await GetAllConfigs();

        IsLoading = false;
    }
    #endregion

    #region Choose
    public void OnChooseConfig(TruckCrossConfig config)
    {
        Request = config;
    }

    public void OnChooseConfigWithCause(TruckCrossConfigWithCause config)
    {
        RequestWithCause = config;

        SetChoosedCausesStatusByCauseIds(RequestWithCause.TruckCrossCauseIds);
    }

    public void OnChooseTruckCrossProductTypeCauses()
    {
        RequestWithCause.TruckCrossCauseIds = Causes.Where(p => p.IsChoosed).Select(p => p.Id).ToList();

        RequestWithCause.TruckCrossCauseId = RequestWithCause.TruckCrossCauseIds.FirstOrDefault();

        if (RequestWithCause.TruckCrossCauseIds.Any())
        {
            RequestWithCause.TruckCrossCauseTitles = Causes.Where(p => p.IsChoosed)
                                                            .Select(p => p.Title)
                                                            .Aggregate((p, q) => p + ", " + q);
        }
        else
        {
            RequestWithCause.TruckCrossCauseTitles = string.Empty;
        }

        StateHasChanged();
    }

    public void OnChooseTruckCrossProductTypeCausesFilters()
    {
        if (ProductTypeCausesFilters.Count(p=>p.IsChoosed).Equals(0))
        {
            ProductTypeCauseTitlesFilter = string.Empty;
        }
        else
        {
            ProductTypeCauseTitlesFilter = ProductTypeCausesFilters.Where(p => p.IsChoosed)
                                                                  .Select(p => p.Title)
                                                                  .Aggregate((p, q) => p + ", " + q);
        }

        StateHasChanged();
    }
    #endregion

    #region Submit
    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        var result = await SaveConfig();

        IsLoading = false;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            if (Request.Id == 0)
            {
                RequestWithCause = null;

                Request.Id = result;

                Configs.Add(Request);

                GridConfigs.Rebind();
            }
            else
            {
                Configs.FirstOrDefault(p => p.Id.Equals(Request.Id)).Title = Request.Title;
            }
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    public async Task OnWithCauseValidSubmit(EditContext context)
    {
        IsLoading = true;

        if (!IsValidWithCause())
        {
            IsLoading = false;

            return;
        }

        var result = await SaveConfig();

        IsLoading = false;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            if (RequestWithCause.Id == 0)
            {
                Request = null;

                RequestWithCause.Id = result;

                ConfigsWithCause.Add(RequestWithCause);

                GridConfigsWithCause.Rebind();
            }
            else
            {
                ConfigsWithCause.FirstOrDefault(p => p.Id.Equals(RequestWithCause.Id)).Title = RequestWithCause.Title;
            }
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    public async Task OnProductTypeByCausesSearchClick()
    {
        IsLoading = true;

        if (ProductTypeCausesFilters.Count(p=>p.IsChoosed).Equals(0))
        {
            await GetAllConfigs();
        }
        else
        {
            ConfigsWithCause = Mapper.Map<List<TruckCrossConfigWithCause>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossProductTypeVm>>("wms/TruckCross", "SGetTruckCrossProductTypesByCauses"
                            , new KeyValuePair<string, object>("presentCauseId", ProductTypeCausesFilters.Where(p=>p.IsChoosed).Select(p=>p.Id).ToList()))).Value);
        }

        IsLoading = false;
    }

    public async Task OnOperationTypeByCauseSearchClick()
    {
        IsLoading = true;

        if (OperationTypeCauseIdFilter.Equals(0))
        {
            await GetAllConfigs();
        }
        else
        {
            ConfigsWithCause = Mapper.Map<List<TruckCrossConfigWithCause>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossProductTypeVm>>("wms/TruckCross", "SGetTruckCrossOperationTypesByCause"
                            , new KeyValuePair<string, object>("presentCauseId", OperationTypeCauseIdFilter))).Value);
        }

        IsLoading = false;
    }
    #endregion

    #region Remove
    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (Request.Id == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await ModalDelete.Open(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        var result = await RemoveConfig();

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            Configs.Remove(Request);

            Request = new();

            GridConfigs.Rebind();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async Task OnWithClauseRemoveClick(MouseEventArgs e)
    {
        if (RequestWithCause.Id == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await ModalWithClauseDelete.Open(new());
    }

    public async Task OnWithClauseConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        var result = await RemoveConfig();

        if (result)
        {
            ConfigsWithCause.Remove(RequestWithCause);

            RequestWithCause = new();

            Causes.ForEach(p => p.IsChoosed = false);

            GridConfigsWithCause.Rebind();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }
    #endregion

    #region private
    private void Init()
    {
        Modes = new()
        {
            new()
            {
                Value = TruckCrossConfigModes.TruckType,
                Name = TextResources.APP_StringKeys_TruckCross_TypeTruck
            },
            new()
            {
                Value = TruckCrossConfigModes.TruckCompany,
                Name = TextResources.APP_StringKeys_Company
            },
            new()
            {
                Value = TruckCrossConfigModes.TruckCrossCustomer,
                Name = TextResources.APP_StringKeys_TruckCross_Customer
            },
            new ()
            {
                Value = TruckCrossConfigModes.TruckCrossOperationType,
                Name = TextResources.APP_StringKeys_TruckCross_Operation_Type
            },
            new ()
            {
                Value = TruckCrossConfigModes.TruckCrossOperationDestination,
                Name = TextResources.APP_StringKeys_TruckCross_Operation_Destination
            },
            new ()
            {
                Value = TruckCrossConfigModes.TruckCrossPresentCause,
                Name = TextResources.APP_StringKeys_TruckCross_Present_Cause
            },
            new ()
            {
                Value = TruckCrossConfigModes.TruckCrossShipment,
                Name = TextResources.APP_StringKeys_TruckCross_Shipment
            },
            new ()
            {
                Value = TruckCrossConfigModes.TruckCrossProductType,
                Name = TextResources.APP_StringKeys_Field_ProductType
            },
            new ()
            {
                Value = TruckCrossConfigModes.TruckCrossAcceptPlace,
                Name = TextResources.APP_StringKeys_TruckCross_Enter_AcceptPlace
            }
        };

        Mode = TruckCrossConfigModes.TruckType;
    }

    private async Task GetAllConfigs()
    {
        if (Mode.Equals(TruckCrossConfigModes.TruckCrossOperationType) ||
        Mode.Equals(TruckCrossConfigModes.TruckCrossProductType))
        {
            ConfigsWithCause = Mode switch
            {
                TruckCrossConfigModes.TruckCrossOperationType =>
                    Mapper.Map<List<TruckCrossConfigWithCause>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationTypesVm>>("wms/TruckCross", "SGetAllTruckCrossOperationType")).Value),

                TruckCrossConfigModes.TruckCrossProductType =>
                    Mapper.Map<List<TruckCrossConfigWithCause>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossProductTypeVm>>("wms/TruckCross", "SGetAllTruckCrossProductType")).Value),

                _ => new()
            };
        }
        else
        {
            Configs = Mode switch
            {
                TruckCrossConfigModes.TruckType =>
                    Mapper.Map<List<TruckCrossConfig>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckTypesVm>>("wms/TruckCross", "SGetTruckType")).Value),

                TruckCrossConfigModes.TruckCompany =>
                    Mapper.Map<List<TruckCrossConfig>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCompaniesVm>>("wms/TruckCross", "SGetAllTruckCompany")).Value),

                TruckCrossConfigModes.TruckCrossCustomer =>
                    Mapper.Map<List<TruckCrossConfig>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossCustomerVm>>("wms/TruckCross", "SGetAllTruckCrossCustomer")).Value),

                TruckCrossConfigModes.TruckCrossOperationDestination =>
                    Mapper.Map<List<TruckCrossConfig>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationDestinationsVm>>("wms/TruckCross", "SGetAllTruckCrossOperationDestination")).Value),

                TruckCrossConfigModes.TruckCrossPresentCause =>
                    Mapper.Map<List<TruckCrossConfig>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossPresentCauseVm>>("wms/TruckCross", "SGetTruckPresentCause")).Value),

                TruckCrossConfigModes.TruckCrossShipment =>
                    Mapper.Map<List<TruckCrossConfig>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossShipmentVm>>("wms/TruckCross", "SGetAllTruckCrossShipment")).Value),

                TruckCrossConfigModes.TruckCrossAcceptPlace =>
                    Mapper.Map<List<TruckCrossConfig>>(
                        (await Api.PostAsyncByUri<List<GetAllTruckCrossShipmentVm>>("wms/TruckCross", "SGetAllTruckCrossAcceptPlace")).Value),

                _ => new()
            };
        }
    }

    private async Task<int> SaveConfig()
    {
        int result = Mode switch
        {
            TruckCrossConfigModes.TruckType =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckType"
                    , new KeyValuePair<string, object>("truckType", Request))).Value,

            TruckCrossConfigModes.TruckCompany =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCompany"
                    , new KeyValuePair<string, object>("truckCrossCompany", Request))).Value,

            TruckCrossConfigModes.TruckCrossCustomer =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCrossCustomer"
                    , new KeyValuePair<string, object>("truckCrossCustomer", Request))).Value,

            TruckCrossConfigModes.TruckCrossOperationType =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCrossOperationType"
                    , new KeyValuePair<string, object>("truckCrossOperationType", RequestWithCause))).Value,

            TruckCrossConfigModes.TruckCrossOperationDestination =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCrossOperationDestination"
                    , new KeyValuePair<string, object>("truckCrossOperationDestination", Request))).Value,

            TruckCrossConfigModes.TruckCrossPresentCause =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckPresentCause"
                    , new KeyValuePair<string, object>("id", Request))).Value,

            TruckCrossConfigModes.TruckCrossShipment =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCrossShipment"
                    , new KeyValuePair<string, object>("truckCrossShipment", Request))).Value,

            TruckCrossConfigModes.TruckCrossProductType =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCrossProductType"
                    , new KeyValuePair<string, object>("truckCrossProductType", RequestWithCause))).Value,

            TruckCrossConfigModes.TruckCrossAcceptPlace =>
                (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCrossAcceptPlace"
                    , new KeyValuePair<string, object>("truckCrossProductType", Request))).Value,

            _ => new()
        };

        return result;
    }

    private async Task<bool> RemoveConfig()
    {
        var result = Mode switch
        {
            TruckCrossConfigModes.TruckType =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckType"
                    , new KeyValuePair<string, object>("id", Request.Id))).Value,

            TruckCrossConfigModes.TruckCompany =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCompany"
                    , new KeyValuePair<string, object>("id", Request.Id))).Value,

            TruckCrossConfigModes.TruckCrossCustomer =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossCustomer"
                    , new KeyValuePair<string, object>("id", Request.Id))).Value,

            TruckCrossConfigModes.TruckCrossOperationType =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossOperationType"
                    , new KeyValuePair<string, object>("id", RequestWithCause.Id))).Value,

            TruckCrossConfigModes.TruckCrossOperationDestination =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossOperationDestination"
                                    , new KeyValuePair<string, object>("id", Request.Id))).Value,

            TruckCrossConfigModes.TruckCrossPresentCause =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckPresentCause"
                    , new KeyValuePair<string, object>("id", Request.Id))).Value,

            TruckCrossConfigModes.TruckCrossShipment =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossShipment"
                    , new KeyValuePair<string, object>("id", Request.Id))).Value,

            TruckCrossConfigModes.TruckCrossProductType =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossProductType"
                    , new KeyValuePair<string, object>("id", RequestWithCause.Id))).Value,

            TruckCrossConfigModes.TruckCrossAcceptPlace =>
                (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossAcceptPlace"
                    , new KeyValuePair<string, object>("id", Request.Id))).Value,
        };

        return result;
    }

    private string AggregateCauseTitlesFromCauseIds(List<int> causeIds)
    {
        if (causeIds is not null && causeIds.Count > 0)
        {
            return Causes.IntersectBy(causeIds, p => p.Id)
                         .Select(p => p.Title)
                         .Aggregate((p, q) => p + ", " + q);
        }
        return string.Empty;
    }

    private void SetChoosedCausesStatusByCauseIds(List<int> causeIds)
    {
        Causes.ForEach(p => p.IsChoosed = false);

        if (causeIds is not null && causeIds.Count > 0)
        {
            Causes.Where(p => causeIds.Any(q => p.Id.Equals(q)))
                                  .ToList()
                                  .ForEach(p => p.IsChoosed = true);
        }
    }

    private bool IsValidWithCause()
    {
        if (Mode.Equals(TruckCrossConfigModes.TruckCrossProductType))
        {
            if (RequestWithCause.TruckCrossCauseIds.Any())
            {
                return true;
            }
            else
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required,
                                                TextResources.APP_StringKeys_TruckCross_Present_Cause), "error");
                return false;
            }
        }
        else
        {
            if (RequestWithCause.TruckCrossCauseId > 0)
            {
                return true;
            }
            else
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required,
                                                TextResources.APP_StringKeys_TruckCross_Present_Cause), "error");
                return false;
            }
        }
    }
    #endregion
}
