using AutoMapper;
using Newtonsoft.Json;
using Silo.Application.Shared.Features;
using Silo.Domains.Entities;

namespace Silo.Profiles;
public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<ApplicationUser, LoginCommand>();

        CreateMap<AddAccountCommand, ApplicationUser>().ReverseMap();

        CreateMap<EditAccountCommand, ApplicationUser>().ReverseMap();

        CreateMap<UserDropDownableDto, ApplicationUser>().ReverseMap();

        CreateMap<GetUserByUsernameVm, UpdateUserByIdCommand>().ReverseMap();

        CreateMap<SaveProductCommand, GetAllProductVm>()
            .ForMember(response => response.ProductName, input => input.MapFrom(i => i.ProductTitle))
            .ForMember(response => response.TechnicalCode, input => input.MapFrom(i => i.ProductTechnicalCode))
            .ReverseMap();

        CreateMap<SaveProductCommand, PositionProductResponse>()
            .ForMember(response => response.ProductName, input => input.MapFrom(i => i.ProductTitle))
            .ForMember(response => response.TechnicalCode, input => input.MapFrom(i => i.ProductTechnicalCode))
            .ReverseMap();

        CreateMap<SaveProductCommand, GetCusProductBySearchTextVm>().ReverseMap();

        CreateMap<SaveProductCommand, ExcelProductDto>().ReverseMap();

        CreateMap<SaveGalleryMediaCommand, GetGalleryMediasDto>();

        CreateMap<TruckCrossDataDto, GetTruckCrossQuery>();

        CreateMap<GetAllAggDocVm, GetAllDocAggSuggestDetailVm>().ReverseMap();

        CreateMap<DocumentItemDto, DocumentItemPrintDto>();

        CreateMap<GetAllDocumentHeaderVm,DocumentHeaderDto>()
            .ForMember(response => response.DocumentItems, input => input.MapFrom(i => i.DocumentItems))
            .ReverseMap();

        CreateMap<DocumentItemDto, GetAllDocumentItemVm>().ReverseMap();

        CreateMap<GateProductErrorDto, GateProductErrorDto>().ReverseMap();

        CreateMap<GetAllPlaceHeadersVm, MovementActionDirectDto>().ReverseMap();

        CreateMap<PlaceProductAggDto, GetPlaceProductBySerialWithAggResultVm>().ReverseMap();

        CreateMap<PlaceProductBySerialDto, GetPlaceProductBySerialWithAggResultVm>().ReverseMap();

        CreateMap<SaveDynamicFieldCommand, GetAllDynamicFieldVm>().ReverseMap();

        CreateMap<SaveInspectElementCommand, GetAllInspectElementVm>().ReverseMap();

        CreateMap<SaveNotificationOrderCommand, GetAllNotificationOrderQuery>().ReverseMap();

        CreateMap<SaveProductSizeCommand, GetAllProductSizeVm>().ReverseMap()
            .ForMember(response => response.SizeData, input => input.MapFrom(i => JsonConvert.DeserializeObject<GetProductSizeDataVm>(i.Data)));

        CreateMap<SaveProductTypeCommand, GetAllProductTypeVm>().ReverseMap();

        CreateMap<GateProductDto, GateProductErrorDto>().ReverseMap();

        CreateMap<SaveNotificationOrderCommand, GetAllNotificationOrderVm>().ReverseMap();

        CreateMap<GetAllTruckCrossPresentCauseVm, TruckCrossConfig>().ReverseMap();
     
        CreateMap<GetAllTruckTypesVm, TruckCrossConfig>().ReverseMap();
        
        CreateMap<GetAllTruckCompaniesVm, TruckCrossConfig>().ReverseMap();
        
        CreateMap<GetAllTruckCrossOperationTypesVm, TruckCrossConfigWithCause>().ReverseMap();
        
        CreateMap<GetAllTruckCrossOperationDestinationsVm, TruckCrossConfig>().ReverseMap();
        
        CreateMap<GetAllTruckCrossShipmentVm, TruckCrossConfig>().ReverseMap();
        
        CreateMap<GetAllTruckCrossCustomerVm, TruckCrossConfig>().ReverseMap();
        
        CreateMap<GetAllTruckCrossProductTypeVm, TruckCrossConfigWithCause>().ReverseMap();

        CreateMap<GetAllGateProductVm, GateProductErrorDto>().ReverseMap();

        CreateMap<GetExitActionByUhfIdVm, SaveExitActionCommand>().ReverseMap();

        CreateMap<GetTruckCrossItemsByTruckCrossIdVm, TruckCrossItemDto>().ReverseMap();

        CreateMap<GetAllActionTypesDto, GetAllDocumentTypesVm>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.ToString()));

        CreateMap<GetAllActionTypesVm, CreateNewActionTypeCommand>().ReverseMap();

        CreateMap<ActionType, GetAllActionTypesVm>().ReverseMap();

        CreateMap<GetAllActionTypesDto, GetAllActionTypesVm>().ReverseMap();

        CreateMap<GetAllExitActionVm, MovementActionPrintDto>().ReverseMap();

        CreateMap<GetAllEnterActionsVm, MovementActionPrintDto>().ReverseMap();
        CreateMap<PrintableOrderDto, GateProductDto>().ReverseMap();

        CreateMap<TruckCrossHeaderPrintDto, TruckCrossDataDto>().ReverseMap();

        CreateMap<SaveProuctClassCommand, GetAllProductClassVm>().ReverseMap();

        CreateMap<SaveProuctSubGroupCommand, GetAllProductSubGroupVm>().ReverseMap();

        CreateMap<PrintProductGuaranteesDto, GetProductGuaranteesVm>().ReverseMap();

        CreateMap<SaveTruckCrossShipmentFeeConfigsCommand, GetAllTruckCrossShipmentFeeVm>().ReverseMap();

        CreateMap<ApplicationUser, UserChoosableDto>().ReverseMap();
        CreateMap<GetAllUsersVm, ApplicationUser>().ReverseMap();
        CreateMap<GetGalleryMediasDto, SaveGalleryMediaWithFileVm>().ReverseMap();
        CreateMap<GetAllActionTypesDto, CreateNewActionTypeCommand>();
        CreateMap<GetAllDestinationTypeDto, CreateNewDestinationTypeCommand>().ReverseMap();
        CreateMap<CreateNewDestinationTypeCommand, WarehouseType>().ReverseMap();
    }
}
