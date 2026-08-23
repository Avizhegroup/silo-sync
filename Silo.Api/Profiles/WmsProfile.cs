using AutoMapper;
using Newtonsoft.Json;
using Silo.Application.Shared.Features;
using Silo.Domains.Entities;
using Silo.Domains.Entities.Api;
using Station = Silo.Domains.Entities.Api.Station;

namespace Silo.Api;
public class WmsProfile : Profile
{
	public WmsProfile()
	{
		CreateMap<Domains.Entities.TruckCrossData, GetTruckCrossVm>()
            .ForMember(dest => dest.PresentUsername, opt => opt.MapFrom(src => src.PresentUser.Username))
            .ForMember(dest => dest.EnterUsername, opt => opt.MapFrom(src => src.EnterUser.Username))
            .ForMember(dest => dest.ExitUsername, opt => opt.MapFrom(src => src.ExitUser.Username))
            .ForMember(dest => dest.PresentUserId, opt => opt.MapFrom(src => src.PresentUser.Id))
            .ForMember(dest => dest.EnterUserId, opt => opt.MapFrom(src => src.EnterUser.Id))
            .ForMember(dest => dest.ExitUserId, opt => opt.MapFrom(src => src.ExitUser.Id))
            .ForMember(dest => dest.PresentRevokeUserId, opt => opt.MapFrom(src => src.PresentRevokeUser.Id))
            .ForMember(dest => dest.PresentRevokeUsername, opt => opt.MapFrom(src => src.PresentRevokeUser.Name))
            .ReverseMap();

        CreateMap<GallerySaveDto,Gallery>();

        CreateMap<Gallery,GetGalleryDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

        CreateMap<DynamicField, DynamicFieldDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id));
        
        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(dest => dest.DestinationCode, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.DestinationTitle, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.InventoryType, opt => opt.MapFrom(src => src.InventoryType == "1" ? DestinationInventoryType.Physical : DestinationInventoryType.Virtual))
            .ForMember(dest => dest.OperationalType, opt => opt.MapFrom(src => (DestinationOperationalType)src.OperationalType))
            .ReverseMap();

        CreateMap<ZoneDto, Zone>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ZoneCode))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.RowIndex, opt => opt.MapFrom(src => src.RowIndex))
            .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
            .ForMember(dest => dest.MinCapacity, opt => opt.MapFrom(src => src.MinCapacity))
            .ForMember(dest => dest.ParentLayer, opt => opt.MapFrom(src => src.ParentLayer))
            .ForMember(dest => dest.ParentCode, opt => opt.MapFrom(src => src.ParentCode))
            .ForMember(dest => dest.Dimention, opt => opt.MapFrom(src => src.Dimention))
            .ForMember(dest => dest.CountPixle, opt => opt.MapFrom(src => src.ZoneCountPixle))
            .ForMember(dest => dest.WarehouseCode, opt => opt.MapFrom(src => src.StoreCode))
            .ReverseMap();

        CreateMap<FreezeHeader, GetFreezeHeaderBySerialDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ReverseMap();

        CreateMap<TruckCrossItem, TruckCrossItemDto>();

        CreateMap<Domains.Entities.TruckCrossData, TruckCrossDataDto>()
            .ForMember(dest => dest.PresentUsername, opt => opt.MapFrom(src => src.PresentUser.Username))
            .ForMember(dest => dest.EnterUsername, opt => opt.MapFrom(src => src.EnterUser.Username))
            .ForMember(dest => dest.ExitUsername, opt => opt.MapFrom(src => src.ExitUser.Username))
            .ForMember(dest => dest.PresentUserId, opt => opt.MapFrom(src => src.PresentUser.Id))
            .ForMember(dest => dest.EnterUserId, opt => opt.MapFrom(src => src.EnterUser.Id))
            .ForMember(dest => dest.ExitUserId, opt => opt.MapFrom(src => src.ExitUser.Id))
            .ForMember(dest => dest.PresentRevokeUserId, opt => opt.MapFrom(src => src.PresentRevokeUser.Id))
            .ForMember(dest => dest.PresentRevokeUsername, opt => opt.MapFrom(src => src.PresentRevokeUser.Username))
            .ForMember(dest => dest.PresentOperationTypeTitle, opt => opt.MapFrom(src => src.OperationType.Title))
            .ForMember(dest => dest.TruckTypeTitle, opt => opt.MapFrom(src => src.Type.Title))
            .ForMember(dest => dest.PresentShipmentTitle, opt => opt.MapFrom(src => src.Shipment.Title))
            .ForMember(dest => dest.PresentOperationDestinationTitle, opt => opt.MapFrom(src => src.OperationDestination.Title))
            .ForMember(dest => dest.PresentCustomerTitle, opt => opt.MapFrom(src => src.Customer.Title))
            .ForMember(dest => dest.GateOperationCode, opt => opt.MapFrom(src => src.MovementAction.MovementActionUHFLogId.ToString()))
            .ForMember(dest => dest.MovementActionId, opt => opt.MapFrom(src => src.MovementAction.MovementActionId.ToString()))
            .ReverseMap();

        CreateMap<CreateReportFormatCommand, ReportFormat>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Details)));

        CreateMap<ReportFormat, GetReportFormatsByPathVm>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Name));

        CreateMap<ReportFormat, GetReportFormatByIdVm>();

        CreateMap<MenuLink, GetAllMenuLinksVm>();

        CreateMap<ActionType, GetAllDocumentTypesVm>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.ToString()));
        
        CreateMap<SaveProductSizeCommand, ProductSize>()
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.SizeData)));

        CreateMap<Zone, ZoneExcelDto>().ReverseMap();

        CreateMap<ZoneExcelDto, Zone>().ReverseMap();

        CreateMap<GetDividableDocumentItemVm, DocumentItem>().ReverseMap();

        CreateMap<GetTruckCrossItemsVm, TruckCrossItem>().ReverseMap();

        CreateMap<ProductSubGroup, GetAllProductSubGroupVm>()
            .ForMember(dest => dest.ProductGroupTitle, opt => opt.MapFrom(src => src.ProductGroup.Title))
            .ReverseMap();

        CreateMap<NotificationQueue, GetAllNotificationQueueVm>()
            .ForMember(dest => dest.OrderTitle, opt => opt.MapFrom(src => src.NotificationOrder.Title))
            .ReverseMap();

        CreateMap<City, GetCitiesVm>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.ProvinceId.ToString()))
            .ReverseMap();

        CreateMap<Province, GetAllProvinceVm>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ReverseMap();

        CreateMap<Product, GetProductModelsVm>();

        CreateMap<SalesShopExcelDto, SalesShop>();

        CreateMap<SalesShop, GetSalesShopByShopCodeVm>();

        CreateMap<SalesInstaller, GetSalesInstallerByCodeVm>();

        CreateMap<TransferProductDto, SaveProductCommand>();

        CreateMap<Station, GetAllStationsVm>()
            .ForMember(dest => dest.StationActionType, opt => opt.MapFrom(src => src.ActionType.ToString()));

        CreateMap<Line, Silo.Domains.Android.AndroidLine>();

        CreateMap<Shift, Silo.Domains.Android.AndroidShift>();

        CreateMap<DynamicField, DynamicFieldDto>();

        CreateMap<Gallery, Domains.Android.AndroidGallery>();

        CreateMap<SaveGalleryMediaWithFileCommand, Gallery>();

        CreateMap<Domains.Entities.Gallery, SaveGalleryMediaWithFileVm>();

        CreateMap<ActionTypeControls, GetAllActionTypeControlsDto>();

        CreateMap<CreateNewActionTypeCommand, ActionType>();

        CreateMap<WarehouseType, GetAllWarehouseTypesDto>();

        CreateMap<PrintFormat, GetPrintFormatsByPageTitleDto>();

        CreateMap<PrintFormat, GetAllPrintFormatDto>();

        CreateMap<DynamicFieldSection, GetAllDynamicFieldSectionsVm>();

        CreateMap<CreatePreparedReportCommand, PreparedReport>();

        CreateMap<PreparedReport, GetPreparedReportByIdVm>();

        CreateMap<Zone, GetAllZonesVm>().ForMember(dest => dest.ZoneCode, opt => opt.MapFrom(src => src.Code))
       .ForMember(dest => dest.StoreCode, opt => opt.MapFrom(src => src.WarehouseCode));

        CreateMap<ActionType, GetAllActionTypesDto>().ReverseMap();
        CreateMap<GetAllActionTypesDto, CreateNewActionTypeCommand>();
        CreateMap<GetAllDestinationTypeDto, CreateNewDestinationTypeCommand>().ReverseMap();
        CreateMap<WarehouseType, GetAllDestinationTypeDto>().ReverseMap();
        CreateMap<CreateNewDestinationTypeCommand, WarehouseType>().ReverseMap();
        CreateMap<GetAllChatSessionsDto, GetAllChatSessionsVm>().ReverseMap();
        CreateMap<CreateNewChatSessionsCommand, ChatSessions>().ReverseMap();
        CreateMap<ChatSessions,GetAllChatSessionsDto>().ReverseMap();
        CreateMap<GetAllChatSessionsDto,CreateNewChatSessionsCommand>().ReverseMap();

        CreateMap<TablesChangeLog, GetTablesChangeLogDto>().ReverseMap();
        CreateMap<GetTablesChangeLogVm, GetTablesChangeLogDto>().ReverseMap();
        CreateMap<Print, PrintReportVm>()
            .ForMember(dest => dest.SoftDeleteUser, opt => opt.MapFrom(src => src.SoftDeleteUser.Name));
        CreateMap<GPSLogs , GetGpsLogDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
    }
}

