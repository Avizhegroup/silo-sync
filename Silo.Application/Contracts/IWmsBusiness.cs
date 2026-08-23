using Silo.Application.Features;
using ProductBrand = Silo.Domains.Entities.ProductBrand;
using Rule = Silo.Application.Dto.Rule;
using SearchApiSyncSto = Silo.Application.Dto.SearchApiSyncSto;

namespace Silo.Application;
public interface IWmsBusiness
{
    bool AddUserClaims(string userId, List<Dto.Claim> claims, string userToken);
    DataTable AssetGetAllProductStatuss(string userToken);
    List<GateResult> ChangeActionIdGetEpcList(string username, string actionId, string deviceId);
    bool ChangeActionStatusUHFReaderlog(string username, string actionId, List<string> Epc, string NewActionStatus);
    bool ChangeTagReadStatus(int tagId, string userToken);
    bool ChangeTagReadStatusByList(List<int> ids);
    int CheckFieldUniqueness(string tableName, string fieldName, string value, string userToken);
    int GetActionType(string fromWarehouse, string toWarehouse);
    List<GetCitiesVm> GetAllCities();
    DataTable GetAllProductPropertyAData(string userToken);
    DataTable GetAllProductPropertyBData(string userToken);
    DataTable GetAllProductPropertyCData(string userToken);
    List<GetAllProvinceVm> GetAllProvinces();
    DataTable GetAllUserByRole(string role, string userToken);
    string GetLatestInvCodeByGateCode(string gateCode);
    DataTable GetPlaceByActionId(string actionId);
    object GetUnExitedTruck();
    DataTable GoldGetAllProductPropertyBData(string userToken);
    List<string> IGetAndroidClaims(string username);
    bool SAddUserToRole(string username, string rolename);
    int SaveProduct(SaveProductCommand product);
    DataTable SCalAggByEpc(List<string> epcs);
    DataTable SCalculateAccontingConflicts(InventoryRequest search);
    bool SCancelRegisterTag(string TagEpc, string username, string deviceId, string deviceIp);
    bool SCastOnline(string driver, string data, string userToken);
    int SChangeDocumentStatus(SaveDocumentStatusCommand command);
    bool SChangeProductStatusBySerial(string ProductTagEPC, int statusCode, int actionId, string destId, string userToken, string productLocationId);
    bool SChangeRegisterDate(string ProductSerial, string NewDate, string NewTime);
    bool SCheckBatchProductUniqueness(List<SaveProductCommand> commands);
    bool SCheckBrandUniqueness(string value);
    bool SCheckClassUniqueness(string value);
    bool SCheckCustomerDocumentAdd(string doc);
    int SCheckDeleteZone(string zoneCode);
    List<CeramicAggregateResult> SCheckDirectPlaceBySecurityGate(string gate, int gateOperationCode, string sourceWarehouse);
    bool SCheckExistNewSerial(string productSerial, string productCode, string productProductionLine = "", string productProductionShift = "");
    bool SCheckGroupUniqueness(string value);
    bool SCheckLineUniqueness(string value);
    Dictionary<int, DateTime> SCheckMovementInRange(string from, string to);
    bool SCheckProductSerial(string ProductSerial);
    bool SCheckProductUniqueness(SaveProductCommand command);
    bool SCheckQcUniqueness(string value);
    bool SCheckSizeUniqueness(string value);
    bool SCheckSubGroupUniqueness(string value);
    bool SCheckTagEPC(string TagEpc);
    string SCheckTagEPCReturnProductSerial(string TagEpc);
    bool SCheckUniqueFileName(string originalFileName, string type);
    int SConvertDataFromJArrayToJToken(string documentType);
    int SCountProductCode(string productCode);
    string SCreateEpc(string serial, string productCode, string refCode, string epc, string type );
    string SCreateNewSerial(string serial, string productCode, string refCode, string epc, string type, string productProductionLine = "", string productProductionShift = "");
    string SCreateProductCode(SaveProductCommand command);
    string SCreateProductTitle(SaveProductCommand command);
    bool SDeleteElement(int elementId);
    bool SDeleteProduct(string ProductCode);
    bool SDeleteReportCargoByActionId(int actionId);
    bool SDeleteWarehouse(int warehouseId);
    int SDivideDocument(SaveDocumentDivideCommand division);
    bool SDocumentDataFromInputFile(string fileName, string documentType, int docCheckType = 0);
    bool SEditTag(string productSerial, decimal productCount, string userToken);
    DataTable SFindTagsProduct(string productSerial, string productCode, string productTechCode, string statusCode, string fromDate, string toDate, string ProductSize, string ProductType, string InspectStatus, string DestinationCode, string DynamicField, string userToken);
    bool SFixInventoryConflicts(SaveFixedConflictsCommand command);
    List<ActionType> SGetActionTypes();
    DataTable SGetAllDocAggSuggestDetailByAggCode(string aggCode, string documentType, int documentStatus);
    List<DocumentItemDto> SGetAllDocItems(string docKey, string docType);
    DataTable SGetAllDocumentByCurrentAndNextStatus(GetAllDocumentByStatusQuery request);
    DataTable SGetAllDocumentItemByStatusVm(GetAllDocumentByStatusQuery request);
    List<DocumentStatus> SGetAllDocumentStatus();
    List<InspectElementDto> SGetAllElements();
    DataTable SGetAllElementsDataTable();
    List<NotificationEventType> SGetAllEventTypes();
    List<GetAllLinesVm> SGetAllLines();
    int SSaveLine(CreateLineCommand line);
    int SRemoveLine(string line);

    DataTable SGetAllMainDocuments(GetAllDocumentByStatusQuery request);
    List<GetAllMenuLinksVm> SGetAllMenuLinks();
    List<string> SGetAllNotFoundSerials(List<string> serials);
    bool SGetSerialDeleteButtonVisibilityInGateMode();
    DataTable SGetAllPlaces();
    List<ProductBrand> SGetAllProductBrands();
    List<ProductClass> SGetAllProductClasses();
    List<ProductGroup> SGetAllProductGroups();
 
    DataTable SGetAllProductPropertyC(string userToken, bool haveNotSelect);
    DataTable SGetAllProductsDefined(string userToken);
    DataTable SGetAllProductSize(string userToken, bool haveNotSelect);
    DataTable SGetAllProductSizes(string userToken);
    DataTable SGetAllProductStatus(string userToken, bool haveNotSelect);
    List<GetAllProductSubGroupVm> SGetAllProductSubGroups();
    DataTable SGetAllProductType(string userToken, bool haveNotSelect);
    List<ProductQc> SGetAllQcs();
    DataTable SGetAllShifts();
    List<ProductSize> SGetAllSizes();
    DataTable SGetAllTag();
    List<string> SGetAllUhfLogReadedSerials(string uhfLogId);
    List<WarehouseDto> SGetAllWarehouses();
    DataTable SGetCapacityByProductCode(string productCode, string from, string destination);
    List<Dto.Rule> SGetcheckingrules(string CheckingRulesId, string CheckingRulesTitle, string CheckingRulesType, string CheckingRulesStationCode);
    DataTable SGetCollectProductList(string documentKey, string documentType, string WarehouseCode = "1");
    DataTable SGetCollectProuductsOrderFromStore(string documentKey, string documentType);
    DataTable SGetCusProducts();
    DataTable SGetCusProductsForWeb(string SearchText);
    GetCustomerGuaranteeVm SGetCustomerGuarantee(CheckCustomerGuaranteeQuery search);
    List<ExitStats> SGetDailyExitStats();
    List<RegisterStats> SGetDailyRegisterStats();
    DataTable SGetDataFromCus(string code);
    string SGetDataFromCusDocumentDesc(string code);
    List<DataElement> SGetDataMiningElements(string DataMiningElementsId, string DataMiningElementsTitle);
    DocumentCheckType SGetDefaultDocumentCheckType();
    List<GetAllDividedDocumentItemVm> SGetDividedDocumentDetails(GetDividableDocumentQuery request);
    List<GetAllDividedDocumentHeaderVM> SGetDividedDocuments(GetDividableDocumentQuery request);
    DataTable SGetDivisionSuggestItems(GetDividableDocumentQuery request);
    List<GetDocProductDataByDocKeyVm> SGetDocProductDataByDocKey(string documentKey);
    DocumentHeader SGetDocumentData(string documentKey, string documentType);
    DataTable SGetDocumentDataFromCustomerDB(string documentId, string doc);
    List<string> SGetDocumentGroupFields();
    DocumentHeader SGetDocumentHeaderAndItems(string documentKey, string documentType);
    List<GetDocProductDataByDocKeyVm> SGetDocumentItemData(string documentKey, string sourceWarehouse, string destinationWarehouse);
    List<GetDocProductDataByDocKeyVm> SGetDocumentItemDataByDocType(string documentKey, string documentType);
    decimal SGetDocumentItemUsedCountByDocKey(string key, int type);
    decimal SGetDocumentItemUsedCountByDocKeyAndProductCode(string key, int type, string productCode);
    List<DocumentHeader> SGetDocumentsByDocType(string documentType, int? limit = null);
    List<string> SGetDuplicatedSalesInstallerCodes(List<string> salesInstallerCodes);
    List<string> SGetDuplicatedSalesStoreCodes(List<string> salesStoreCodes);
    List<DynamicFieldDto> SGetDynamicFieldsByActionTypeId(int actionTypeId);
    List<DynamicFieldDto> SGetDynamicFieldsBySourceAndDestination(string sourceWarehouseCode, string destinationWarehouseCode);
    List<DynamicField> SGetDynamicFieldsForAndroid();
    DataTable SGetEpcListByUHFLogActionId(string UHFLogActionId, string userToken);
    DataTable SGetEpcOtherDeviceUHFLogByUHFLogActionCode(string UHFLogActionCode);
    int SGetFirstDocumentStatus();
    GetInventoryProductsListsVm SGetFreezedInventoryProductsBeforeMovements(GetInventoryConflictsQuery search);
    GetWarehouseProductsListsVm SGetFreezedTagProductsBeforeMovements(GetInventoryConflictsQuery search);
    DataTable SGetFreezeHeaderReport(GetFreezeHeaderReportDto search);
    List<GetFreezeHeaderBySerialDto> SGetFreezeHeadersBySerial(string serial);
    DataTable SGetFreezeItemReport(GetFreezeItemReportDto search);
    DataTable SGetFreezeProducts(List<ReportFilter> filters);
    DataTable SReportPrint(List<ReportFilter> reportFilters);
    DataTable SGetGateAlertsBySerial(string serial);
    DataTable SGetGateProductsByTruckCrossId(int truckCrossId);
    DataTable SGetCargoByTruckCrossId(List<int> truckCrossIds);
    DataTable SGetInStoreProductsByProductCode(List<ReportFilter> reportFilters);
    string SGetInventorySummaryByStoreCode(string StoreCode);
    List<CustomerAccountingData> SGetLastCAD();
    DataTable SGetLastInventoryTags();
    string SGetLastReadedTagForHistory();
    DataTable SGetLastReadedTagsForRevoke();
    DataTable SGetLastUhfGateActionsById(GetAllUhfReaderLogByActionIdQuery action);
    WeighBridgeLog SGetLastWeighbridgeLog();
    int SGetMaxActionId(int Type);
    int SGetMaxCheckingRulesId();
    int SGetMaxIdByMovementActionTp(int MovementActionTp);
    int SGetMaxInvIdByGate(string gate);
    string SGetMaxPOCode();
    string SGetMaxPOOperationCode();
    int SGetMaxTSId();
    DataTable SGetMovementActionByUhfId(int uhfId, string userToken);
    string SGetNewPrintActionId(string userToken);
    string SGetNewProductSerial(string userToken);
    int SGetNextDocumentStatus(int status);
    int SGetNextInventoryHeaderId();
    int SGetNextInvId(int type = -1);
    int SGetNextInvIdByGateCode(string GateCode);
    int SCreateNewUHFReaderLogHeader(SaveUHFReaderLogHeaderCommand command);
    int SGetNextPreviousInvIdByCurrentId(bool isNext, string invId, string gate);
    DataTable SGetNotification();
    string SGetNotif_WinFormService(string DateTimeLastCheck, string User);
    DataTable SGetPlaceItems(string placeId);
    TruckCargo SGetPlacementMission(string[] ProductSerials, string[] Epcs, string WMDriverUserId, string WMId, string ActionId, string GateNumber, string GateTitle, string TypeGetPlacementMission, string ActionDescription, string ActionStatus, bool RecursiveFunction, bool CastResult);
    DataTable SGetPlacementOrders();
    DataTable SGetPlacementOrdersByOperationCode(string operationCode);
    List<CeramicAggregateResult> SGetPlaceProductDataBySerials(PlaceProductRequest request);
    int SGetPreviousDocumentStatus(int status);
    DataTable SGetPrintDataByProductSerial(string ProductSerial, string userToken);
    DataTable SGetPrintListByPrintActionId(string PrintActionId, string userToken);
    DataTable SGetPrintListBySerial(string serial);
    DataTable SGetProduct(string epc);
    DataTable SGetProductBySerial(string serial);
    DataTable SGetProductDetails(string code, string zone, string desticationCode);
    Dictionary<string, object> SGetProductHistory(string code);
    DataTable SGetProductHistoryLight(List<string> codes);
    DataTable SGetProductListInZoneByDate(string code, string zone, string Date, string desticationCode);
    DataTable SGetProductOfWarehouse(InventoryRequest search);
    string SGetProductPlacementMissionsToZoneId(string PPMId);
    DataTable SGetProductsByEpcs(List<string> epcList, string userToken);
    DataTable SGetProductsByEpcsInAndroidAction(List<string> epcList, string userToken, string ActionId, string ActionType, string DeviceId);
    DataTable SGetQueuesForSend();
    DataTable SGetRemainDividableDocumentItem(GetDividableDocumentQuery request);
    int SGetRemainZoneCapacity(string storeCode, string zoneCode);
    int SGetRemainZoneCapacityMulti(string storeCode, string zones);
    DataTable SGetReproductsByProductCode(StoreRequest search);
    GetSalesInstallerByCodeVm SGetSalesInstallerByCode(string installerCode);
    GetSalesShopByShopCodeVm SGetSalesShopByShopCode(string shopCode);
    DataTable SGetSerialDataForPlace(List<string> serials);
    SaveProductCommandEnabilityCheck SGetSettingsAddProduct();
    DataTable SGetStationTypes();
    DataTable SGetStores();
    DataTable SGetTagHistoryMovement(string serial);
    DataTable SGetTagHistoryProductInfo(string serial);
    DataTable SGetTagHistoryReadByGate(string serial);
    DataTable SGetTagHistoryTimeLine(string serial);
    DataTable SGetTagRelatedTags(string serial);
    DataTable SGetTagInventoryHistory(string serial);
    DataTable SGetTagMovementHistory(string serial);
    DataTable SGetTagPlacementHistory(string serial);
    DataTable SGetTagSalesHistory(string serial);
    DataTable SGetTruckCrossByMovementActionId(string movementActionId);
    string SGetUserLogin(string userId);
    List<GetGalleryDto> SGetUserMediasByUsage(int usageType, string usageId, string userId);
    List<GetGalleryDto> SGetUserMediasByUserId(int usageType, string userId);
    List<WarehouseDto> SGetWarehousesDestinationListByActionType(int ActionType);
    bool SIdentifyPallets
    (
        string deviceId,
        List<string> listTags,
        string desc,
        string GateType,
        string invCod,
        string doc,
        string DestinationCode,
        string userToken = "",
        JToken ActionDynamicData = null,
        string? ActionActiveControls = "",
         string? TruckCrossId = "",
         DateTime? saveDateTime = null,
         string? gpsLog = ""
    );
    bool SInsertQueues(List<Dto.Queue> queues);
    int SInsertUpdateNotification(Notification notif);
    int SInsertUpdateZone(ZoneDto zone);
    int SInsertZoneExcel(SaveZoneCommand saveZoneCommand);
    DataTable SInStoreReport(List<ReportFilter> reportFilters);
    DataTable SInStoreReproductReport(StoreRequest search);
    bool SIsDocumentUpdateAllowed(string documentKey, string documentType);
    string SIsProductSerialExist(string ProductSerial, string username, string deviceIp, string deviceId);
    DataTable SLatestUHFLogActionByActionTypeANDDeviceId(string ActionType, string DeviceId, string userToken);
    int SPChangeTagStatus(string epc, int statusCode, int actionId, string destId, string userToken, string TagZone);
    int SPDefineZone(string ZoneCode, string ZoneTitle, string ZoneDimention, string ZoneParentCode, string ZoneParentLayer, string ZoneStoreCode, string ZoneCountPixle, string MinZoneCapacity, string MaxZoneCapacity, string ZoneRowIndex, string UserCode);
    int SPDeleteZone(string ZoneCode, string UserCode);
    List<ZoneDto> SPGetAllZones();
    DataTable SPGetListWarehouseMachines();
    DataTable SPGetPlacementOrdersList(int type = -1);
    Dictionary<string, object> SPGetProductAllDataAndHistory(string code);
    DataTable SPGetProductInfo(string tagEpc);
    string SPGetShift(string Time);
    string SPGetShiftStartEND(string ShiftCode);
    DataTable SPGetShiftStartENDList();
    string SpGetSummaryEnterGateInDate(string Date, string GateNumber);
    string SpGetSummaryEnterGateInShift(string Date, string GateNumber, string Shift);
    string SpGetZoneCodeInEnterAction(string ProductCode, string ProductLine, string ProductShift);
    List<ZoneDto> SPGetZonesByWarehouse(string code);
    bool SPlacementMissionResult(string[] PMCode, string Status, string ToStoreCode, string ToZoneId);
    DataTable SPositionSearch(PositionSearchRequest search);
    bool SProductRegisterOffline(List<RegisterOfflineTagsCommand> commands);
    List<Enter> SPSaveEnterGateUHFLog(string tags, string gate, string gateType, string WMCode);
    bool SPSaveEnterProducts(string actionId, string deviceId, string actionDesc, string actionStore, string LocationCode, List<string> listTags);
    bool SPSaveExitProducts(string username, string actionId, string deviceId, string CarPlaque, string DriverName, string DriverMobile, string actionDesc, string[] listTags, string TagStatus = "2");
    int SPSavePlacementOrders(string userToken, string productCode, string productLine, string productShift, string packCount, string storeCode, List<string> zoneList, string pOCode, int truck, string fromZoneCode = "0", string type = "1");
    int SPSavePlacementOrdersBySerials(string userToken, string productCode, string productLine, string productShift, string packCount, string storeCode, List<string> zoneList, string pOCode, int truck, string fromZoneCode, string type, string[] serials = null);
    int SPSavePlacementOrdersBySerialsFromCollect(SavePlacementOrderCollectCommand order);
    List<CeramicAggregateResult> SPSaveSecurityGateUHFLog(List<string> tags, string code, string gate, string gateType);
    DataTable SPSearchProduct(string name, string reg, string code, string st);
    DataTable SPSearchProductBySerial(GetAllProductBySerialDto search);
    DataTable SPSearchProductPlace(ProductPlaceFilterRequest request);
    DataTable SPSearchProductTypeWeb(ProductTypeRequest search);
    DataTable SPSearchProductWeb(GetAllProductQuery search);
    List<CeramicAggregateResult> SPSecurityGateByTruckCrossByIdReport(string truckCrossId);
    List<CeramicAggregateResult> SPSecurityGateByTruckCrossByIdReportDetails(string truckCrossId, string ProductCode);
    List<CeramicAggregateResult> SPSecurityGateByTruckCrossReport(string gate, int gateOperationCode);
    List<CeramicAggregateResult> SPSecurityGateByTruckCrossReportDetails(int gateOperationCode, string ProductCode);
    List<CeramicAggregateResult> SPSecurityGateReport(string gate, int gateOperationCode);
    List<CeramicAggregateResult> SPSecurityGateReportDetails(int gateOperationCode, string ProductCode);
    int SPSetTagZone(string ProductSerial, string TagZone, string userToken);
    bool SRecalMovementActionType();
    int SRegisterByCodeCount(string[] epcs
     , string productCode
     , string refCode
     , string count
     , string zone
     , string userToken
     , string line = "0"
     , string shift = "0"
     , string DestinationCode = "0"
     , JToken properties = null);

    int SRegisterByCodeSerialCount(string serial
     , string productCode
     , string refCode
     , string count
     , string epc
     , string zone
     , string userToken
     , string line = "0"
     , string shift = "0"
     , string DestinationCode = "0"
     , JToken properties = null);

    bool SRegisterTag(string productSerial, string productCode, string productCount, string[] epcList, string username, string deviceId, string deviceIp, string RegisterProperties);
    int SRemoveBrand(string brandCode);
    bool SRemoveCheckingRule(int id);
    int SRemoveClass(string classCode);
    int SRemoveDocuments(RemoveDocumentCommand command);
    bool SRemoveGalleryMedia(int mediaId);
    int SRemoveGroup(string groupCode);
    bool SRemoveNotification(int id);
    int SRemoveProduct(string productCode, string userToken);
    int SRemoveQc(string qc);
    int SRemoveSize(string sizeCode);
    int SRemoveSubGroup(string subGroupCode);
    bool SRemoveUser(string username);
    DataTable SRepExistTag(string ProductCode, string FromDate, string ToDate, string TechnicalCode, string ProductStatus, string ProductSerial, string TagZone);
    DataTable SRepExistTagTajamoeeProductCode(string ProductCode, string FromDate, string ToDate, string TechnicalCode, string ProductStatus, string ProductSerial, string TagZone);
    DataTable SRepInventoryDetails(InventoryRequest search);
    DataTable SRepInventoryProductList(EnterRequest search);
    DataTable SReportBackEnterProductList(EnterRequest search);
    DataTable SReportBackEnterTajamoeeProductCode(EnterRequest search);
    DataTable SReportCargoByType(int tp);
    DataTable SReportEnter(EnterRequest search);
    DataTable SReportEnterAction(List<ReportFilter> reportFilters);
    DataTable SReportEnterActionByOpCode(List<ReportFilter> reportFilters);
    DataTable SReportEnterActionFull(List<ReportFilter> reportFilters);
    DataTable SReportEnterActionTajamoeeProductCode(List<ReportFilter> reportFilters);
    DataTable SReportEnterProductList(EnterRequest search);
    DataTable SReportEnterProductsByProductCode(EnterRequest search);
    DataTable SReportExit(EnterRequest search);
    DataTable SReportExitActionByOpCode(List<ReportFilter> reportFilters);
    DataTable SReportExitActionFull(List<ReportFilter> reportFilters);
    DataTable SReportExitActions(List<ReportFilter> reportFilters);
    DataTable SReportExitActionTajamoeeProductCode(List<ReportFilter> reportFilters);
    DataTable SReportExitProductList(EnterRequest search);
    DataTable SReportHMovementActions(EnterRequest search);
    DataTable SReportHMovementOnAgeAnalyse(IndexSearch search);
    DataTable SReportHMovementOnDestination(IndexSearch search);
    DataTable SReportHMovementOnProductSize(IndexSearch search);
    DataTable SReportHMovementOnProductStatus(IndexSearch search);
    DataTable SReportHMovementOnProductType(IndexSearch search);
    List<InspectProductHeader> SReportInspectProducts(InspectProductReportQuery request);
    List<InspectReportResponse> SReportInspects(InspectReportRequest request);
    DataTable SReportInventory(InventoryReportRequest request);
    DataTable SReportInventoryBySerial(InventoryReportRequest request);
    List<string> SReportInventorySerials(InventoryReportRequest request);
    DataTable SReportOnBrief();
    DataTable SReportOnDetails(IndexSearch search);
    DataTable SReportOnProductCode(IndexSearch search);
    DataTable SReportOnProductDate(IndexSearch search);
    DataTable SReportOnProductLine(IndexSearch search);
    DataTable SReportOnProductSize(IndexSearch search);
    DataTable SReportOnProductType(IndexSearch search);
    DataTable SReportOnQc(IndexSearch search);
    DataTable SReportOnRegcode(IndexSearch search);
    DataTable SReportOnShift(IndexSearch search);
    DataTable SReportSecurityTags(string gate, string destinationCode);
    DataTable SReportStoreOn10MaxProduct();
    DataTable SReportStoreOnAgeAnalyse();
    DataTable SReportStoreOnProductSize();
    DataTable SReportStoreOnProductType();
    DataTable SReportStoreOnZoneCode();
    DataTable SReportUhfLog(List<ReportFilter> reportFilters);
    DataTable SReportUhfLogByProducts(List<ReportFilter> reportFilters);
    DataTable SReportUhfLogBySerials(List<ReportFilter> reportFilters);
    DataTable SRepPlacementMissions(string ProductCode, string TechnicalCode, string ProductSerial, string FromDate, string ToDate, string FromZoneId, string ToZoneId, string DriverUserId, string PlacementMissionType, string PPMActionId, string TagStatus, string PlacementMissionStatus);
    DataTable SRepRegister(string ProductCode, string FromDate, string ToDate, string ProductLine, string ProductShift, string User, string TechnicalCode = "");
    DataTable SRepRegisterTag(string ProductCode, string FromDate, string ToDate, string ProductLine, string ProductShift, string User, string TechnicalCode, string ProductStatus, string ProductSerial);
    DataTable SRepRegisterTagByTagStatus0(string FromRegisterShamsiUnixDate);
    DataTable SRepRegisterTagDetails(List<ReportFilter> reportFilters);
    DataTable SRepRegisterTagDetailsForWinApp(GetRegisterRequestFilter filter);
    DataTable SRepRegisterTagSummary(List<ReportFilter> reportFilters);
    DataTable SRepRegisterTagSummaryForRegisterWinApp(GetRegisterRequestFilter filter);
    DataTable SRepRegisterTagSummaryGroupBySizeANDRegCode(GetRegisterRequestFilter filter);
    DataTable SRepRegisterTagWithOutEPC();
    DataTable SRepRegisterTajamoeeProductCode(string ProductCode, string FromDate, string ToDate, string ProductLine, string ProductShift, string User, string TechnicalCode, string ProductStatus, string ProductSerial);
    DataTable SRepSCancelRegisterTag(string ProductSerial, string FromDate, string ToDate, string User);
    DataTable SRepSummaryExitProductsForSMS(string fromDate, string toDate);
    DataTable SRepSummaryRegister(string ShamsiUnixFromDate, string ShamsiUnixDateToFrom, string TypeTimeFilter);
    DataTable SRepSummaryRegisterForSMS(string fromDate, string toDate);
    DataTable SRepSummaryStoreInfo();
    int SRevokeDocumentAgg(RevokeDocumentAggregateCommand revoke);
    int SRevokeDocumentDivision(GetDividableDocumentQuery request);
    int SRevokeDocumentStatus(SaveDocumentStatusCommand revoke);
    int SRevokePlacementOrder(string operationCode);
    bool SRevokeTags(List<string> epcs);
    bool SSaveAlarmLog(string AlarmLogGateNumber, string AlarmLogType, string AlarmLogTag, string AlarmLogSerial, string AlarmLogActionId, string AlarmLogUserId);
    int SSaveBrand(ProductBrand brand);
    int SSavecheckingrules(Rule rule);
    int SSaveClass(ProductClass productClass);
    int SSaveDirectPlace(SaveMovementActionDirectCommand place);
    bool SSaveDynamicApi(string documentKey, string doc, string userToken);
    bool SSaveDynamicExcel(string path, string fileName, string originalFileName, string type, string documentType, int docCheckType);
    bool SSaveDynamicManual(SaveDocumentCommand command);
    int SSaveElement(InspectElementDto inspect);
    bool SSaveEnumerationPallets(string username, string actionId, string deviceId, List<string> listTags, string gateType, bool logOnUhf = false);
    int SSaveFreeze(FreezeSaveDto freeze);
    bool SSaveGateLog(string username, string actionId, string deviceId, List<string> listTagsForInsert, List<string> listSerialForInsert, string gateType, List<PlaceSerialDto> listSerialWithInsertType = null);
    List<GateResult> SSaveGateLogAndShowResult(string username, string actionId, string deviceId, List<string> listTagsForInsert, List<string> listTagsForUpdateActionStatus, string gateType);
    List<GateResult> SSaveGateLogAndShowResultForHandHeldTags(string actionId, string deviceId, string gateType);
    int SSaveGroup(ProductGroup group);
    bool SSaveInspect(InspectSaveDto inspect);
    bool SSaveInventory(int header, string store, string deviceId, string[] data);
    bool SSaveInventoryBySerials(SaveInventoryByEpcsCommand command);
    bool SSaveInventoryWithDesc(int header, string store, string deviceId, string[] data, string desc, DateTime? saveDateTime = null);
    int SSaveMediaInGallery(List<GallerySaveDto> medias);
    bool SSaveMovementAction(string LogGateActionId
        , string ActionSourceLocation
        , string ActionDestinationLocation
        , string GateCode
        , string ActionUser
        , string PMCode
        , JToken ActionData
        , string ActionDestinationZoneCode = "0"
        , string MovementActionDesc = ""
        , string MovementActionDocumentId = ""
        , string MovementActionTruckCrossId = "0"
        , List<string> epcs = null);
    bool SSaveMovementData(UpdateActionDataCommand command);
    bool SSaveNdfLog(List<SaveNonDocFileCommand> commands);
    bool SSaveOtherDeviceUHFLog(List<string> EpcList, string DeviceType, string UserId);
    bool SSavePlacementMissions(string userId, string productCode, string productSerial, string productEPC, string type, string wCode, string fromZoneId, string toZoneId, string storeCode, string actionCode, string actionRemainCount);
    bool SSavePlacementMissionsByCount(string userId, string productCode, string productSerial, string productEPC, string type, string wCode, string fromZoneId, string toZoneId, string storeCode, int count, string actionCode = "0");
    bool SSavePlaceMissionsByCountSerials(List<string> codes, List<string> serials, List<string> epcs, List<string> counts, string type, string truck, string fromZoneId, string toZoneId, string destWarehouse, string userToken, string actionCode = "0");
    bool SSavePlaceMissionsBySerial(string serial, string Epc, string ProductCode, string type, string WMDriverUserId, string WMId, string FromStoreCode, string fromZoneId, string ToStoreCode, string toZoneId, string userToken, string POCode, string actionCode = "0");
    bool SSavePlaceOrdersBySerialsCount(List<string> serials, string destWarehouse, string destZone, string pOCode, int truck, string type, string userToken);
    bool SSavePrint(string ProductCode, string ProductRegCode, string ProductTitle, string ProductENTitle, string ProductSize, string ProductItemCount, string ProductUnit, string ProductCountInPack, string ProductCount, string ProductVolume, string ProductWeight, string ProductStatus, string PrintActionId, string ProductProductionLine, string ProductProductionShift, string PrintCount, string ProductType, string ProductContractType);
    bool SSavePrintByNewSerialANDOldSerialInTransferData(string NewTarnsferProductSerial, string OldTransferProductSerial, string ProductCount, string UserToken, string Type);
    string SSavePrintBySerial(string ProductSerial,
                                  string ProductCode,
                                  string ProductCount,
                                  string PrintActionId,
                                  string ProductProductionLine,
                                  string ProductProductionShift,
                                  string PrintCount,
                                  string ProductContractType,
                                  string ProductOldSerial,
                                  string Location,
                                  string DocumentId,
                                  string PrintUser,
                                  string WareHouseCode,
                                  JToken ProductProperties) ;
    int SSaveProductBatch(List<SaveProductCommand> products);
    int SSaveProductByQc(SaveProductCommand command);
    bool SSaveProductEnterAction(string[] tagEPCList, string storeCode, string locationCode, string desc, string userToken);
    bool SSaveProductExitAction(SaveExitActionCommand exitAction);
    bool SSaveProductType(string ProductTypeTitle, string ProductTypeParentId, string ProductTypeParentsId, string ProductTypeCode, string userToken);
    int SSaveQc(ProductQc brand);
    int SSaveSalesInstallers(List<SalesInstallerExcelDto> salesInstallers);
    int SSaveSalesStores(List<SalesShopExcelDto> salesStores);
    bool SSaveSecurityLog(List<string> tags, string code, string gate, string gateType);
    int SSaveSize(SaveProductSizeCommand size);
    int SSaveSubGroup(ProductSubGroup subGroup);
    bool SSaveTechnicalInformation(JArray data);
    bool SSaveTechnicalInformationUsingJsonArray(List<SaveProductTerchnicalDataCommand> commands);
    bool SSaveUserLogin(string token, string userId);
    int SSaveWarehouse(WarehouseDto warehouse);
    int SSaveWeighbridgeLog(WeighBridgeLogDto log);
    DataTable SSearchApiSync(SearchApiSyncSto request);
    DataTable SSearchApiSyncDetails(SearchApiSyncSto request);
    DataTable SSearchInventoryTags(InventoryRequest search);
    DataTable SSearchLastCAD(GetInventoryConflictsQuery search);
    DataTable SSearchOtherDeviceUHFLog(string UserToken);
    DataTable SSearchProductForLocate(List<ReportFilter> reportFilters);
    List<WarehouseDto> SSearchWarehouses(string code);
    DataTable SSearchZone(string productCode, int minCap, int maxCap, string zoneCode, string location, DestinationOperationalType warehouseType, bool zoneCodeLike = false);
    DataTable SSearchZoneByCodes(string[] codes);
    DataTable SSearchZoneProducts(string zoneCode, string desticationCode);
    DataTable SSearchZones(string productCode, int minCap, int maxCap, string zoneCode, string RegCode, string ProductSerial, string ZoneLayer);
    DataTable SSearchZonesLocationProductList();
    DataTable SSearchZonesProducts(StoreProductRequest search);
    DataTable SSelectPlacementMissions(string pPMId = "");
    DataTable SSelectPlacementMissionsByProductSerial(string ProductSerial);
    bool SSendActionToApi(SearchApiSyncSto request, SendActionToApiDto save);
    Task SSendSignalR(TruckCargo cargo);
    bool SSetMissionStatus(string pId, string pStatus, string pDateTime, string pDriver, string pTruck, string PSerial);
    string SSetPlacementMission(string ProductSerial, string WMDriverUserId, string WMId, string Type, string UserId);
    int SSetProductImage(int productGalleryId, string productCode, string userToken);
    bool SSetQueuesStatus(Dictionary<int, int> queueStatus);
    bool SSetTruckNumber(int truck, string user);
    DataTable SSMS_ActiveWMMCount(string FromDateTime);
    DataTable SSMS_AllStoreCapacity();
    DataTable SSMS_ControlFifoInExitRepDaily(string DateTimeForm, string DateTimeTo, string HActionId);
    DataTable SSMS_EmptyZoneInStore();
    DataTable SSMS_ExitAnalyseAGe(string DateTimeForm, string DateTimeTo, string HActionId);
    DataTable SSMS_ForoshAVG();
    DataTable SSMS_ForoshAVGDate(string DateTimeForm, string DateTimeTo);
    DataTable SSMS_ForoshCountActionData(string DateTimeForm, string DateTimeTo);
    DataTable SSMS_ForoshDestinationTypeSumProductCount(string DateTimeForm, string DateTimeTo);
    DataTable SSMS_ForoshMainData(string DateTimeForm, string DateTimeTo);
    DataTable SSMS_ForoshMax();
    DataTable SSMS_HActionAVGDate(string HActionId);
    DataTable SSMS_LastHMovementData(string DateTimeLastCheck);
    DataTable SSMS_mojodiMainData();
    DataTable SSMS_TolidAVG();
    DataTable SSMS_TolidData(string FromDate, string ToDate);
    DataTable SSMS_TolidMax();
    void STest();
    void STestError();
    string STransferProductCountToAnotherTag(string FromSerial, string ToEPC, string Count, string DestinationCode, string userToken);
    bool SUpdateMovementActionTruckCross(int actionId, int newTruckCrossId);
    bool SUpdatePlacementMissions(string productSerial, string productEPC, string wCode, string wDriverUserId, string pStatus);
    bool SUpdateProductCodeForSendToApi(SearchApiSyncSto request, string newCode);
    bool SUpdateUserLogin(string token, string userId);
    bool SUpdateZoneOccupiedCapacity(string StoreCode, string ZoneCode, string ProductSerial);
    bool UpdateUserData(UpdateUserCommand command);
    List<ProductType> SGetAllProductTypes();
    DataTable SGetUserDataForOfflineLoginAndroid();
    List<DataElement> SGetDataMiningElementsByIds(List<string> ids);
    List<GetAllZonesVm> SGetAllZones();
}
