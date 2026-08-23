using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Silo.Ui.Gate.DML;
using Silo.Ui.Gate.Models;
using Silo.Ui.Gate.Models.DML;

namespace Silo.Ui.Gate.DAL;

public class ApiBusiness
{
    //var resultLogin = await WorkingWithApiConnector.LoadUnDirectAsync("TSLogin", "http://95.217.149.5:2020/RfidCore/v1/Asset/Post",
    //     new KeyValuePair<string, object>("username", "admin"),
    //     new KeyValuePair<string, object>("password", "rfidadmin"));
    //    if ((bool) resultLogin["successful"])
    //    {
    //        Console.WriteLine(resultLogin["value"].ToString());
    //    }
    //    else
    //        Console.WriteLine("0");

    internal string StringKey_Url = "http://"+Properties.Settings.Default.ServerIp+"/RfidCore/v1/Wms/PostObject";


    internal async Task<bool> Login(string username, string password)
    {
        var result = await WorkingWithApiConnector.LoadUnDirectAsync("TSLogin", StringKey_Url,
          new KeyValuePair<string, object>("username", username),
          new KeyValuePair<string, object>("password", password));
        try
        {
            if ((bool)result["successful"] && result["value"].ToString() != "0")
            {
                //  DAL.Items.OnlineUserCode = result["value"].ToString();
                return true;
            }
            else
                return false;
        }
        catch
        {
            new frmSetting().Show();
            return false;
        }
    }

    internal async Task<List<GateResult>> SSaveGateLogAndShowResult(List<Tags> listTags, string actionId,string ActionType)
    {
        if (listTags.Count>0 && actionId!="")
        {
            List<string> _tagStringListForInsert = new List<string>();
            List<string> _tagStringListForUpdateActionStatus = new List<string>();
            foreach (Tags tag in listTags)
            {
                if (tag.TagReedSaveStatus == 0)
                {
                    if (tag.TagActionStatus == 0)
                    {
                        _tagStringListForInsert.Add(tag.TagEPC + "*" + tag.TagPackageId + "*" + tag.DocumentId + "*" + tag.WMUsertId);
                    }
                    else
                    {
                        _tagStringListForUpdateActionStatus.Add(tag.TagEPC + "*" + tag.TagActionStatus);
                    }
                }
            }
            try
            {

                if (_tagStringListForInsert.Count>0 || _tagStringListForUpdateActionStatus.Count>0)
                {
                    var result = await WorkingWithApiConnector.LoadUnDirectAsync("SSaveGateLogAndShowResult", StringKey_Url,

                           new KeyValuePair<string, object>("username", Properties.Settings.Default.GateNumber)
                         , new KeyValuePair<string, object>("actionId", actionId)
                         , new KeyValuePair<string, object>("deviceId", Properties.Settings.Default.GateNumber)
                         , new KeyValuePair<string, object>("listTagsForInsert", _tagStringListForInsert)
                         , new KeyValuePair<string, object>("listTagsForUpdateActionStatus", _tagStringListForUpdateActionStatus)
                         , new KeyValuePair<string, object>("gateType", ActionType));
                    if ((bool)result["successful"])
                    {
                        List<GateResult> _gateResultList = new List<GateResult>();

                        if (result["value"]!=null)
                        {

                            foreach (JToken item in result["value"])
                            {
                                _gateResultList.Add(new GateResult()
                                {
                                    ProductCode = item["productCode"].ToString(),
                                    ProductName = item["productName"].ToString(),
                                    Count = item["count"].ToString(),
                                    ProductTechnicalCode = item["productTechnicalCode"].ToString(),
                                    Row = item["row"].ToString(),
                                    SumValue = item["sumValue"].ToString(),
                                    TagSerial = item["tagSerial"].ToString(),
                                    ProductSerial = item["productSerial"].ToString(),
                                    ProductType = item["productType"].ToString(),
                                    ProductStatus = item["productStatus"].ToString(),
                                    TagStatus = item["tagStatus"].ToString(),
                                    TagInDestinationId = item["tagInDestinationId"].ToString(),
                                    Lock = item["lock"].ToString(),
                                    ProductLine = item["productLine"].ToString(),
                                    ProductShift = item["productShift"].ToString(),
                                    DocumentId=item["documentId"].ToString(),
                                    PMToStoreCode=item["pmToStoreCode"].ToString(),
                                    PMToStoreTitle=item["pmToStoreTitle"].ToString(),
                                    PMToZoneCode=item["pmToZoneCode"].ToString(),
                                    Freeze=item["freeze"].ToString(),
                                    ProductOldSerial=item["productOldSerial"].ToString(),
                                    TagPackageStatus=listTags.FirstOrDefault(p => p.TagEPC==item["tagSerial"].ToString()).TagPackageStatus,
                                    LastInspectResult= item["lastInspectResult"].ToString(),
                                    TagRegisterDateTime=Convert.ToDateTime(item["tagRegisterDateTime"].ToString())
                                    


                                });

                            }
                        }
                        if (listTags.Count>0)
                        {
                            //foreach (string tagEpc in _tagStringListForInsert)
                            //{
                            //    if (listTags.FirstOrDefault(p => p.TagEPC == tagEpc.Split('*')[0]) != null)
                            //        listTags.FirstOrDefault(p => p.TagEPC == tagEpc.Split('*')[0]).TagReedSaveStatus = 1;
                            //}
                        }
                        return _gateResultList;
                    }
                    else

                        return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteExceptionLogs(ex);
                return null;

            }
        }
        else
            return null;
    }




    internal async Task<List<GateResult>> ChangeActionIdGetEpcList(string actionId)
    {
        List<GateResult> _gateResultList = new List<GateResult>();

        var result = await WorkingWithApiConnector.LoadUnDirectAsync("ChangeActionIdGetEpcList", StringKey_Url,

              new KeyValuePair<string, object>("username", Properties.Settings.Default.GateNumber)
            , new KeyValuePair<string, object>("actionId", actionId)
            , new KeyValuePair<string, object>("deviceId", Properties.Settings.Default.GateNumber));
        if ((bool)result["successful"])
        {

            if (result["value"] != null)
            {

                foreach (JToken item in result["value"])
                {
                    if (item["tagSerial"].ToString() != "000")
                    {
                        _gateResultList.Add(new GateResult()
                        {
                            TagSerial = item["tagSerial"].ToString()
                        }
                        );
                    }
                }


            }
        }
        return _gateResultList;

    }



    internal async Task<List<GateResult>> SSaveGateLogAndShowResultForHandHeldTags(string actionId,string ActionType)
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SSaveGateLogAndShowResultForHandHeldTags", StringKey_Url,
new KeyValuePair<string, object>("actionId", actionId)
                 , new KeyValuePair<string, object>("deviceId", Properties.Settings.Default.GateNumber)
                 , new KeyValuePair<string, object>("gateType", ActionType));
            if ((bool)result["successful"])
            {
                List<GateResult> _gateResultList = new List<GateResult>();

                if (result["value"] != null)
                {

                    foreach (JToken item in result["value"])
                    {
                        _gateResultList.Add(new GateResult()
                        {
                            ProductCode = item["productCode"].ToString(),
                            ProductName = item["productName"].ToString(),
                            Count = item["count"].ToString(),
                            ProductTechnicalCode = item["productTechnicalCode"].ToString(),
                            Row = item["row"].ToString(),
                            SumValue = item["sumValue"].ToString(),
                            TagSerial = item["tagSerial"].ToString(),
                            ProductSerial = item["productSerial"].ToString(),
                            ProductType = item["productType"].ToString(),
                            ProductStatus = item["productStatus"].ToString(),
                            TagStatus = item["tagStatus"].ToString(),
                            TagInDestinationId = item["tagInDestinationId"].ToString(),
                            Lock = item["lock"].ToString(),
                            ProductLine = item["productLine"].ToString(),
                            ProductShift = item["productShift"].ToString(),
                            DocumentId = item["documentId"].ToString(),
                            PMToStoreCode = item["pmToStoreCode"].ToString(),
                            PMToStoreTitle = item["pmToStoreTitle"].ToString(),
                            PMToZoneCode = item["pmToZoneCode"].ToString(),
                            Freeze = item["freeze"].ToString(),
                            ProductOldSerial = item["productOldSerial"].ToString(),
                            TagPackageStatus = 0,
                            LastInspectResult = item["lastInspectResult"].ToString(),
                            TagRegisterDateTime = Convert.ToDateTime(item["tagRegisterDateTime"].ToString())



                        });
                    }
                }

                return _gateResultList;
            }
            else

                return null;

        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;

        };
    }

    internal async Task<string> GetInventorySummaryByStoreCode(string StoreCode)
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetInventorySummaryByStoreCode", StringKey_Url, new KeyValuePair<string, object>("StoreCode", StoreCode));
            if ((bool)result["successful"])
            {
                return result["value"].ToString();
            }
            else
                return "";
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;

        }
    }

    internal async Task<string> GetNextIdByGateCode()
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetNextInvIdByGateCode", StringKey_Url, new KeyValuePair<string, object>("GateCode", Properties.Settings.Default.GateNumber));
            if ((bool)result["successful"])
            {
                return (Convert.ToInt32(result["value"].ToString())).ToString();
            }
            else
            {
                result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetNextInvIdByGateCode", StringKey_Url, new KeyValuePair<string, object>("GateCode", Properties.Settings.Default.GateNumber));
                if ((bool)result["successful"])
                {
                    return (Convert.ToInt32(result["value"].ToString())).ToString();
                }
                else
                    return "";
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return "";

        }
    }

     


    internal async Task<string> GetNextPreviousInvIdByCurrentId(bool isNext, string invId)
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetNextPreviousInvIdByCurrentId", StringKey_Url, new KeyValuePair<string, object>("isNext", isNext), new KeyValuePair<string, object>("invId", invId), new KeyValuePair<string, object>("gate", Properties.Settings.Default.GateNumber));
            if ((bool)result["successful"])
            {
                return (Convert.ToInt32(result["value"].ToString())).ToString();
            }
            else
                return "1";
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return "1";

        }
    }


   
    internal async Task<bool> SaveAlarmLog( string AlarmLogType, string AlarmLogTag, string AlarmLogSerial,string AlarmLogActionId,string AlarmLogUserId)
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SSaveAlarmLog", StringKey_Url,


               new KeyValuePair<string, object>("AlarmLogGateNumber", Properties.Settings.Default.GateNumber)
             , new KeyValuePair<string, object>("AlarmLogType", AlarmLogType)
             , new KeyValuePair<string, object>("AlarmLogTag", AlarmLogTag)
             , new KeyValuePair<string, object>("AlarmLogSerial", AlarmLogSerial)
             , new KeyValuePair<string, object>("AlarmLogActionId", AlarmLogActionId)
             , new KeyValuePair<string, object>("AlarmLogUserId", AlarmLogUserId));
            if ((bool)result["successful"])
            {

                if ((bool)result["value"])
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return false;

        }


    }



    internal async Task<string> GetNextId()
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetNextInvId", StringKey_Url
                , new KeyValuePair<string, object>("type", "-1"));
            if ((bool)result["successful"])
            {
                return (Convert.ToInt32(result["value"].ToString())).ToString();
            }
            else
                return "1";
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return "1";

        }
    }



    internal async Task<int> GetActionType(string fromWarehouse, string toWarehouse)
    {
        var result = await WorkingWithApiConnector.LoadUnDirectAsync("GetActionType", StringKey_Url,
            new KeyValuePair<string, object>("fromWarehouse", fromWarehouse)
            , new KeyValuePair<string, object>("toWarehouse", toWarehouse));
        if ((bool)result["successful"])
        {
            return Convert.ToInt32(result["value"].ToString());

        }
        else
            return 0;
    }


    internal async Task<List<DynamicFieldDto>> GetDynamicFieldsByActionTypeId(string actionTypeId)
    {
        var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetDynamicFieldsByActionTypeId", StringKey_Url,
            new KeyValuePair<string, object>("actionTypeId", actionTypeId));
        if ((bool)result["successful"])
        {
            var list = new List<DynamicFieldDto>();
            foreach (JToken items in result["value"])
            {
                list.Add(new DynamicFieldDto()
                {
                    Title = items["title"].ToString(),
                    Id = Convert.ToInt32(items["id"].ToString()),
                    RelatedTitle1 = items["relatedTitle1"].ToString(),
                    FieldType =(DynamicFieldType)Convert.ToInt32(items["fieldType"].ToString()),
                    Value = ""
                });
            }
            return list;
        }
        else
            return null;
    }

    internal async Task<bool> SaveAction(string LogGateActionId, string PMCode, string ActionUser, JToken ActionData,string MovementActionDesc,string MovementActionDocumentId,string MovementActionTruckCrossId)
    {

         
            try
            {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SSaveMovementAction", StringKey_Url,
               new KeyValuePair<string, object>("LogGateActionId", LogGateActionId)
             , new KeyValuePair<string, object>("ActionSourceLocation", Properties.Settings.Default.FromStore.Split(',')[0])
             , new KeyValuePair<string, object>("ActionDestinationLocation", Properties.Settings.Default.ToStore)
             , new KeyValuePair<string, object>("GateCode", Properties.Settings.Default.GateNumber)
             , new KeyValuePair<string, object>("ActionUser", ActionUser)
             , new KeyValuePair<string, object>("PMCode", PMCode)
              , new KeyValuePair<string, object>("ActionData", ActionData)
             , new KeyValuePair<string, object>("ActionDestinationZoneCode", "0")
              , new KeyValuePair<string, object>("MovementActionDesc", MovementActionDesc)
               , new KeyValuePair<string, object>("MovementActionDocumentId", MovementActionDocumentId)
               , new KeyValuePair<string, object>("MovementActionTruckCrossId", MovementActionTruckCrossId));
            if ((bool)result["successful"])
            {

                if ((bool)result["value"])
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return false;

        }
       

    }


    internal async Task<List<GetAllAggDocDto>> GetAggDocsByDocTypeAndStatus(string documentType)
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetDocumentsByDocType", StringKey_Url,
               new KeyValuePair<string, object>("documentType", documentType) );
            if ((bool)result["successful"])
            {

                if (result["value"].ToString()!="")
                {
                    List<GetAllAggDocDto> _listAllAggDocDto = new List<GetAllAggDocDto>();
                    foreach (JToken item in result["value"])
                    {


                        _listAllAggDocDto.Add(new GetAllAggDocDto()
                        {
                            DocumentData = item["headerData"].ToString(), 
                            DocumentKey = item["key"].ToString()

                        });

                    }
                    return _listAllAggDocDto;
                }
                else
                    return null;
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;

        }
        return null;
    }

    //string SGetPlacementMission(    string TypeGetPlacementMission, string ActionDescription, string ActionStatus, bool RecursiveFunction = false)
    internal async Task<List<TruckCargo>> SendCargoTruckSignal(string LogGateActionId, string DriverUserId, string WMId, string ActionDescription, ActionStatus ActionStatus)
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetPlacementMission", StringKey_Url,
               new KeyValuePair<string, object>("ProductSerials", null)
             , new KeyValuePair<string, object>("Epcs", null)
             , new KeyValuePair<string, object>("WMDriverUserId", DriverUserId)
             , new KeyValuePair<string, object>("WMId", WMId)
             , new KeyValuePair<string, object>("ActionId", LogGateActionId)
             , new KeyValuePair<string, object>("GateNumber", Properties.Settings.Default.GateNumber)
             , new KeyValuePair<string, object>("GateTitle", Properties.Settings.Default.GateTitle)
             , new KeyValuePair<string, object>("TypeGetPlacementMission", "2")
             , new KeyValuePair<string, object>("ActionDescription", ActionDescription)
             , new KeyValuePair<string, object>("ActionStatus", ActionStatus)
             , new KeyValuePair<string, object>("RecursiveFunction", false)
             , new KeyValuePair<string, object>("CastResult", true));
            if ((bool)result["Successful"])
            {

                if ((bool)result["value"])
                {
                    List<TruckCargo> _gateTruckCargoList = new List<TruckCargo>();
                    foreach (JToken item in result["value"])
                    {


                        _gateTruckCargoList.Add(new TruckCargo()
                        {
                            TruckNumber = item["TruckNumber"].ToString(),
                            ActionStatus = (ActionStatus)Enum.Parse(typeof(ActionStatus), item["ActionStatus"].ToString()),
                            CargoStatus = (CargoStatus)Enum.Parse(typeof(CargoStatus), item["CargoStatus"].ToString()),
                            DestinationAddress = item["DestinationAddress"].ToString(),
                            DestinationWarehouseCode = item["DestinationWarehouseCode"].ToString(),
                            DestinationWarehouseTitle = item["DestinationWarehouseTitle"].ToString(),
                            DestinationZoneCode = item["DestinationZoneCode"].ToString(),
                            DestinationZoneTitle = item["DestinationZoneTitle"].ToString(),
                            DriverUserId = item["DriverUserId"].ToString(),
                            DriverUsername = item["DriverUsername"].ToString(),
                            FromWarehouseCode = item["FromWarehouseCode"].ToString(),
                            FromWarehouseTitle = item["FromWarehouseTitle"].ToString(),
                            FromZoneCode = item["FromZoneCode"].ToString(),
                            FromZoneTitle = item["FromZoneTitle"].ToString(),
                            GateActionId = item["GateActionId"].ToString(),
                            GateTitle = item["GateTitle"].ToString(),
                            Products = JsonConvert.DeserializeObject<List<CargoProduct>>(item["Products"].ToString())

                        });

                    }
                    return _gateTruckCargoList;
                }
                else
                    return null;
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;

        }
        return null;
    }
    internal async Task<List<WarehouseMachines>> GetAllWarehouseMachines()
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync(methodName: "SPGetListWarehouseMachines", url: StringKey_Url

            );
            if ((bool)result["successful"])
            {
                var list = new List<WarehouseMachines>();
                foreach (JToken item in result["value"])
                {
                    // var dataJson = JToken.Parse(item["ProductProperties"].ToString());
                    list.Add(new WarehouseMachines()
                    {
                        WMCode = Convert.ToInt32(item["fld_WMCode"].ToString()),
                        WMDriverName = item["fld_WMDriverName"].ToString(),
                        WMRFID = item["fld_WMRFID"].ToString(),
                        WMTitle = item["fld_WMTitle"].ToString(),
                        WMDriverUserId = item["WMDriverUserId"].ToString()
                    });


                }
                return list;
            }
            else
                return null;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;

        }
    }



     


    internal bool SChangeDocumentStatus(string documentKey, int documentType)
    {
        try
        {

            SaveDocumentStatusCommand _doc = new SaveDocumentStatusCommand
            {
                DocumentKeyTypes = new List<DocumentKeyTypeDto>
                {
                    new DocumentKeyTypeDto
                    {
                        Key = documentKey,
                        Type = documentType.ToString()
                    }
                },
                User = ""
            };
            var result = WorkingWithApiConnector.LoadUnDirect(methodName: "SChangeDocumentStatus", url: StringKey_Url,
 new KeyValuePair<string, object>("command", _doc) 
            );



            if ((bool)result["successful"])
            {
                return true;

            }
            else
                return false;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return false;

        }
    }



    internal async Task<JToken> GetDocumentData(string documentKey,int documentType)
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync(methodName: "SGetDocumentData", url: StringKey_Url,
 new KeyValuePair<string, object>("documentKey", documentKey),
  new KeyValuePair<string, object>("documentType", documentType)
            );


            if ((bool)result["successful"])
            {
                return result["value"];

            }
            else
                return null;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;

        }
    }

    internal  bool SSaveDynamicApi(string documentKey, int documentType)
    {
        try
        {
        
        var result =  WorkingWithApiConnector.LoadUnDirect(methodName: "SSaveDynamicApi", url: StringKey_Url,
new KeyValuePair<string, object>("documentKey", documentKey),
new KeyValuePair<string, object>("documentType", documentType),
new KeyValuePair<string, object>("userToken", "09")
        );

       

            if ((bool)result["successful"])
            {
                return true;

            }
            else
                return false;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return false;

        }
    }


    internal async Task<List<WarehouseDto>> GetAllWarehouses()
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync(methodName: "SGetAllWarehouses", url: StringKey_Url);
            if ((bool)result["successful"])
            {
                var list = new List<WarehouseDto>();
                foreach (JToken items in result["value"])
                {
                    list.Add(new WarehouseDto()
                    {
                        DestinationCode = items["destinationCode"].ToString(),
                        DestinationTitle = items["destinationTitle"].ToString(),
                        OperationalType = (DestinationOperationalType)Convert.ToInt32(items["operationalType"].ToString()),
                        InventoryType = (DestinationInventoryType)Convert.ToInt32(items["inventoryType"].ToString()),
                        IsDefault = Convert.ToBoolean(items["isDefault"].ToString()),
                        IsActive = Convert.ToBoolean(items["isActive"].ToString()),
                        Id = Convert.ToInt32(items["id"].ToString())
                    });
                }
                return list;
            }
            else
                return null;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;

        }
    }

    internal async Task<List<TruckCross>> GetUnexitedCrosses()
    {
        try
        {
            var result = await WorkingWithApiConnector.LoadUnDirectAsync(methodName: "GetUnExitedTruck", url: StringKey_Url);
            if ((bool)result["successful"])
            {

                List<TruckCross> _listTruckCross = new List<TruckCross>();
                foreach (JToken items in result["value"])
                {

                    _listTruckCross.Add(new TruckCross()
                    {
                        DriverName = items["fld_TruckCrossDriverName"].ToString(),
                        Id = items["fld_TruckCrossId"].ToString(),
                        plaque = items["fld_TruckCrossPlaque"].ToString()
                    });
                    //list.Add(new GetTruckCrossQuery()
                    //{
                    //    DestinationCode = items["destinationCode"].ToString(),
                    //    DestinationTitle = items["destinationTitle"].ToString(),
                    //    OperationalType = (DestinationOperationalType)Convert.ToInt32(items["operationalType"].ToString()),
                    //    InventoryType =  (DestinationInventoryType)Convert.ToInt32(items["inventoryType"].ToString()),
                    //    IsDefault = Convert.ToBoolean(items["isDefault"].ToString()),
                    //    IsActive = Convert.ToBoolean(items["isActive"].ToString()),
                    //    Id = Convert.ToInt32(items["id"].ToString())
                    //});
                }
                return _listTruckCross;
            }
            else
                return null;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;

        }
    }
}


//public class ApiBusiness_old
//{
//   internal string StringKey_Url = "http://"+Properties.Settings.Default.ServerIp+"/RfidConnectApiHandler.ashx";

//   internal async Task<List<GateResult>> SSaveGateLogAndShowResult(List<Tags> listTags, string actionId)
//    {
//        if (listTags.Count>0 && actionId!="")
//        {
//            List<string> _tagStringListForInsert = new List<string>();
//            List<string> _tagStringListForUpdateActionStatus = new List<string>();
//            foreach (Tags tag in listTags)
//            {
//                if (tag.TagActionStatus==0)
//                {
//                    if (tag.TagReedSaveStatus==0)
//                    {
//                        _tagStringListForInsert.Add(tag.TagEPC+"*"+tag.TagPackageId+"*"+tag.DocumentId+"*"+tag.WMUsertId);
//                    }
//                }
//                else if(tag.TagActionStatus==1)
//                {
//                    _tagStringListForUpdateActionStatus.Add(tag.TagEPC);
//                }
//            }
//            try
//            {




//                var result = await WorkingWithApiConnector.LoadUnDirectAsync("SSaveGateLogAndShowResult", StringKey_Url,

//                       new KeyValuePair<string, object>("username", Properties.Settings.Default.GateNumber)
//                     , new KeyValuePair<string, object>("actionId", actionId)
//                     , new KeyValuePair<string, object>("deviceId", Properties.Settings.Default.GateNumber)
//                     , new KeyValuePair<string, object>("listTagsForInsert", _tagStringListForInsert)
//                     , new KeyValuePair<string, object>("listTagsForUpdateActionStatus", _tagStringListForUpdateActionStatus)
//                     , new KeyValuePair<string, object>("gateType", "3"));
//                if ((bool)result["Successful"])
//                {
//                    List<GateResult> _gateResultList = new List<GateResult>();
//                    foreach (JToken item in result["Value"])
//                    {
//                        _gateResultList.Add(new GateResult()
//                        {
//                            ProductCode = item["ProductCode"].ToString(),
//                            ProductName = item["ProductName"].ToString(),
//                            Count = item["Count"].ToString(),
//                            ProductTechnicalCode = item["ProductTechnicalCode"].ToString(),
//                            Row = item["Row"].ToString(),
//                            SumValue = item["SumValue"].ToString(),
//                            TagSerial = item["TagSerial"].ToString(),
//                            ProductSerial = item["ProductSerial"].ToString(),
//                            ProductType = item["ProductType"].ToString(),
//                            ProductStatus = item["ProductStatus"].ToString(),
//                            TagStatus = item["TagStatus"].ToString(),
//                            TagInDestinationId = item["TagInDestinationId"].ToString(),
//                            Lock = item["Lock"].ToString(),
//                            ProductLine = item["ProductLine"].ToString(),
//                            ProductShift = item["ProductShift"].ToString(),
//                            DocumentId=item["DocumentId"].ToString()

//                        });
//                    }

//                    foreach (string tagEpc in _tagStringListForInsert)
//                    {
//                        listTags.FirstOrDefault(p => p.TagEPC==tagEpc.Split('*')[0]).TagReedSaveStatus=1;
//                    }
//                    return _gateResultList;
//                }
//                else
//                    return null;
//            }
//            catch
//            {
//                return null;
//            }
//        }
//        else
//            return null;
//    }


//    internal async Task<string> GetNextId()
//    {
//        var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetNextInvId", StringKey_Url, new KeyValuePair<string, object>("type", "-1"));
//        if ((bool)result["Successful"])
//        {
//            return (Convert.ToInt32(result["Value"].ToString()) ).ToString();
//        }
//        else
//            return "1";
//    }


//    internal async Task<bool> SaveAction(string LogGateActionId,string PMCode, string ActionUser, JToken ActionData)
//    {
//         var result = await WorkingWithApiConnector.LoadUnDirectAsync("SSaveMovementAction", StringKey_Url,


//            new KeyValuePair<string, object>("LogGateActionId", LogGateActionId)
//          , new KeyValuePair<string, object>("ActionSourceLocation", Properties.Settings.Default.FromStore)
//          , new KeyValuePair<string, object>("ActionDestinationLocation", Properties.Settings.Default.ToStore)
//          , new KeyValuePair<string, object>("GateCode", Properties.Settings.Default.GateNumber)
//          , new KeyValuePair<string, object>("ActionUser", ActionUser)
//          , new KeyValuePair<string, object>("PMCode", PMCode)
//           , new KeyValuePair<string, object>("ActionData", ActionData));
//        if ((bool)result["Successful"])
//        {

//            if ((bool)result["Value"])
//            {
//                return true;
//            }
//            else
//                return false;
//        }
//        return false;
//    }

//            internal async Task<List<WarehouseMachines>> GetAllWarehouseMachines()
//    {
//        var result = await WorkingWithApiConnector.LoadUnDirectAsync("SPGetListWarehouseMachines", StringKey_Url,
//            new KeyValuePair<string, object>("userToken", 0)
//        );
//        if ((bool)result["Successful"])
//        {
//            var list = new List<WarehouseMachines>();
//            foreach (JToken item in result["Value"])
//            {
//                // var dataJson = JToken.Parse(item["ProductProperties"].ToString());
//                list.Add(new WarehouseMachines()
//                {
//                    WMCode = Convert.ToInt32(item["fld_WMCode"].ToString()),
//                    WMDriverName = item["fld_WMDriverName"].ToString(),
//                    WMRFID = item["fld_WMRFID"].ToString(),
//                    WMTitle = item["fld_WMTitle"].ToString()
//                });


//            }
//            return list;
//        }
//        else
//            return null;
//    }



//    internal async Task<List<WarehouseDto>> GetAllWarehouses()
//    {
//        var result = await WorkingWithApiConnector.LoadUnDirectAsync("SGetAllWarehouses", StringKey_Url);
//        if ((bool)result["Successful"])
//        {
//            var list = new List<WarehouseDto>();
//            foreach (JToken items in result["Value"])
//            {
//                list.Add(new WarehouseDto()
//                {
//                    DestinationCode = items["DestinationCode"].ToString(),
//                    DestinationTitle = items["DestinationTitle"].ToString(),
//                    OperationalType = (DestinationOperationalType)Convert.ToInt32(items["OperationalType"].ToString()),
//                    InventoryType =  (DestinationInventoryType)Convert.ToInt32(items["InventoryType"].ToString()),
//                    IsDefault = Convert.ToBoolean(items["IsDefault"].ToString()),
//                    IsActive = Convert.ToBoolean(items["IsActive"].ToString()),
//                    Id = Convert.ToInt32(items["Id"].ToString())
//                });
//            }
//            return list;
//        }
//        else
//            return null;
//    }




//}

