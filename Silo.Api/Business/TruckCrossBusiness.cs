using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json.Linq;
using Silo.Application.Contracts;
using Silo.Domains.Entities;
using Silo.Domains.Services;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Silo.Api.Business;
public class TruckCrossBusiness(ILogger<TruckCrossBusiness> logger
                              , IDataAccess dataAccess
                              , IHttpContextAccessor httpContextAccessor
                              , IConfiguration configuration
                              , WmsApiContext apiContext
                              , IMapper mapper) : ProjectBusiness(dataAccess, logger, httpContextAccessor)
{
    #region Truck Cross
    public int SSaveTruckCross(TruckCrossData cross)
    {
        if (cross.Id.Equals(0))
        {
            apiContext.Add(cross);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCross"));
            }
        }
        else
        {
            apiContext.Update(cross);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public int SGetNextTruckCrossTurn()
    {
        string command = "SELECT COALESCE(MAX(fld_TruckCrossPresentTurn),0) FROM tbl_TruckCross WHERE CAST(fld_TruckCrossPresentDateTime AS DATE) = CAST(GETDATE() AS DATE) ";

        int result = (int)dataAccess.SqlDataAdapter(command, 60).Select()[0].ItemArray[0];

        return result + 1;
    }

    public GetTruckCrossVm SSearchTruckCross(GetTruckCrossQuery search)
    {
        var crosses = apiContext.Crosses.Where(p => p.ExitDateTime == null);

        if (search.NationalCode.HasValue())
        {
            crosses = crosses.Where(p => p.NationalCode == search.NationalCode);
        }

        if (search.DriverName.HasValue())
        {
            crosses = crosses.Where(p => p.DriverName == search.DriverName);
        }

        if (search.DriverPhone.HasValue())
        {
            crosses = crosses.Where(p => p.DriverPhone == search.DriverPhone);
        }

        if (search.Id is not null)
        {
            crosses = crosses.Where(p => p.Id == search.Id.Value);
        }

        if (crosses.Any())
        {
            crosses = crosses.Include(p => p.PresentUser)
            .Include(p => p.EnterUser)
                             .Include(p => p.ExitUser)
                             .OrderByDescending(p => p.PresentDateTime);

            return mapper.Map<GetTruckCrossVm>(crosses.First());
        }
        else
        {
            return null;
        }
    }

    public TruckCrossDataDto SSearchAllTruckCross(GetTruckCrossQuery search)
    {
        var crosses = apiContext.Crosses.AsQueryable();

        if (search.NationalCode.HasValue())
        {
            crosses = crosses.Where(p => p.NationalCode == search.NationalCode);
        }

        if (search.DriverName.HasValue())
        {
            crosses = crosses.Where(p => p.DriverName == search.DriverName);
        }

        if (search.DriverPhone.HasValue())
        {
            crosses = crosses.Where(p => p.DriverPhone == search.DriverPhone);
        }

        if (search.Id is not null)
        {
            crosses = crosses.Where(p => p.Id == search.Id.Value);
        }

        if (crosses.Any())
        {
            crosses = crosses.Include(p => p.PresentUser)
                             .Include(p => p.EnterUser)
                             .Include(p => p.ExitUser)
                             .OrderByDescending(p => p.PresentDateTime);

            return mapper.Map<TruckCrossDataDto>(crosses.First());
        }
        else
        {
            return null;
        }
    }

    public GetTruckCrossVm SGetTruckCrossById(int id)
    {
        var cross = apiContext.Crosses
                                .Include(p => p.PresentUser)
                                .Include(p => p.EnterUser)
                                .Include(p => p.ExitUser)
                                .FirstOrDefault(p => p.ExitDateTime == null);

        return mapper.Map<GetTruckCrossVm>(cross);
    }

    public List<TruckCrossDataDto> SReportTruckCrossForm(GetTruckCrossReportQuery search)
    {
        var crosses = apiContext.Crosses.Where(p => p.Id != null);

        if (search.NationalCode.HasValue())
        {
            crosses = crosses.Where(p => p.NationalCode == search.NationalCode);
        }

        if (search.DriverName.HasValue())
        {
            crosses = crosses.Where(p => p.DriverName == search.DriverName);
        }

        if (search.PresentCause != 0)
        {
            crosses = crosses.Where(p => p.PresentCause == search.PresentCause);
        }

        if (search.FromDate.HasValue())
        {
            crosses = crosses.Where(p => p.PresentDateTime.Value.Date >= PersianCalendarTools.PersianToGregorian(search.FromDate));
        }

        if (search.ToDate.HasValue())
        {
            crosses = crosses.Where(p => p.PresentDateTime.Value.Date <= PersianCalendarTools.PersianToGregorian(search.ToDate));
        }

        if (search.Status != TruckCrossStatuses.None)
        {
            crosses = crosses.Where(p => p.TruckCrossStatus == (int)search.Status);
        }

        if (search.PresentOperationTypeId != 0)
        {
            crosses = crosses.Where(p => p.PresentOperationTypeId == search.PresentOperationTypeId);
        }

        if (search.PresentShipmentId != 0)
        {
            crosses = crosses.Where(p => p.PresentShipmentId == search.PresentShipmentId);
        }

        if (search.PresentOperationDestinationId != 0)
        {
            crosses = crosses.Where(p => p.PresentOperationDestinationId == search.PresentOperationDestinationId);
        }

        if (search.PresentCustomerId != 0)
        {
            crosses = crosses.Where(p => p.PresentCustomerId == search.PresentCustomerId);
        }

        if (search.ProductTitle.HasValue())
        {
            crosses = crosses.Include(p => p.TruckCrossItems)
                .Where(p => p.TruckCrossItems.Any(q => q.Title.Contains(search.ProductTitle.Trim())));
        }

        crosses = crosses.Include(p => p.PresentUser)
                         .Include(p => p.EnterUser)
                         .Include(p => p.ExitUser)
                         .Include(p => p.OperationType)
                         .Include(p => p.Type)
                         .Include(p => p.Shipment)
                         .Include(p => p.OperationDestination)
                         .Include(p => p.Customer)
                         .Include(p => p.MovementAction)
                         .OrderByDescending(p => p.PresentDateTime);

        List<TruckCrossData> result = new();

        #region plaque regex

        if ((search.PlaqueFirstPart != 0)
            || (search.PlaqueCharacter.HasValue())
            || (search.PlaqueSecondPart != 0)
            || (search.PlaqueCityPart != 0))
        {

            string pattern = "";

            if (search.PlaqueFirstPart != 0)
            {
                pattern += search.PlaqueFirstPart.ToString();
            }
            else
            {
                pattern += "[0-9][0-9]";
            }

            if (search.PlaqueCharacter.HasValue())
            {
                pattern += search.PlaqueCharacter;
            }
            else
            {
                pattern += ".";
            }

            if (search.PlaqueSecondPart != 0)
            {
                pattern += search.PlaqueSecondPart.ToString();
            }
            else
            {
                pattern += "[0-9][0-9][0-9]";
            }

            if (search.PlaqueCityPart != 0)
            {
                pattern += search.PlaqueCityPart.ToString();
            }
            else
            {
                pattern += "[0-9][0-9]";
            }

            Regex expr = new(pattern);

            result = crosses.ToList().Where(p => expr.Matches(p.Plaque).Any()).ToList();
        }
        else
        {
            result = crosses.ToList();
        }

        #endregion

        var mappedResult = mapper.Map<List<TruckCrossDataDto>>(result.DistinctBy(p => p.Id));

        return mappedResult;
    }

    public DataTable SGetLoadedProducts(int id)
    {
        var cmd = $@"SELECT        
		                    tbl_TagsMovement.ProductCode,
		                    tbl_Products.ProductTitle AS [ProductName],
		                    tbl_TagsMovement.ProductSerial,
		                    tbl_TagsMovement.ProductCount,
		                    tbl_MovementActions.MovementActionCarPlaque,
		                    tbl_TruckCross.fld_TruckCrossPlaque,
		                    [dbo].[GeorgianDateToJalaliDate](tbl_TruckCross.fld_TruckCrossExitDateTime),
		                    tbl_MovementActions.MovementActionDate
		                    FROM  tbl_MovementActions LEFT OUTER JOIN
                                tbl_TagsMovement ON tbl_MovementActions.MovementActionId = tbl_TagsMovement.HMovementActionId LEFT OUTER JOIN
                                tbl_Products ON tbl_TagsMovement.ProductCode = tbl_Products.ProductCode INNER JOIN
			                    tbl_TruckCross ON  tbl_MovementActions.MovementActionCarPlaque = tbl_TruckCross.fld_TruckCrossPlaque
			                WHERE (tbl_MovementActions.MovementActionTp = 2) AND (tbl_MovementActions.MovementActionData NOT LIKE N'%برگشت کالا%') 
									AND [dbo].[GeorgianDateToJalaliDate](tbl_MovementActions.MovementActionDateTime) = [dbo].[GeorgianDateToJalaliDate](tbl_TruckCross.fld_TruckCrossExitDateTime)
									AND tbl_TruckCross.fld_TruckCrossId =N'{id}'
                    ";
        var result = dataAccess.SqlDataAdapter(cmd);
        return result;
    }

    public DataTable SGetPrintableTruckCrossData(int truckCrossId)
    {
        string command =
            $"""
            SELECT COALESCE(tbl_TruckCross.fld_TruckCrossId, 0) AS Id,
                   tbl_TruckCross.fld_TruckCrossPresentDateTime AS PresentDateTime,
                   CAST(CAST(tbl_TruckCross.fld_TruckCrossPresentDateTime as time) as nvarchar(5)) as PresentTime,
                   COALESCE(tbl_User_1.Name, '') AS PresentUser,
                   COALESCE(tbl_TruckCross.fld_TruckCrossNationalCode, '') AS NationalCode,
                   COALESCE(tbl_TruckCross.fld_TruckCrossDriverName, '') AS DriverName,
                   COALESCE(tbl_TruckCompany.fld_TruckCompanyTitle, '') AS CompanyTitle,
                   COALESCE(tbl_TruckCross.fld_TruckCrossPassportCode, '') AS PassportCode,
                   COALESCE(tbl_TruckCross.fld_TruckCrossDriverPhone, '') AS DriverPhone,
                   COALESCE(tbl_TruckCross.fld_TruckCrossLicenseCode, '') AS LicenseCode,
                   COALESCE(tbl_TruckCross.fld_TruckCrossPlaque, '') AS Plaque,
                   COALESCE(tbl_TruckCross.fld_TruckCrossInternationalPlaque, '') AS InternationalPlaque,
                   COALESCE(tbl_TruckType.fld_TruckTypeTitle, '') AS TruckTypeTitle,
                   COALESCE(tbl_TruckCross.fld_TruckCrossTypeDesc, '') AS TypeDesc,
                   COALESCE(tbl_TruckCrossCause.fld_TruckCrossCauseTitle, '') AS PresentCauseTitle,
                   COALESCE(tbl_TruckCrossOperationType.fld_TruckCrossOperationTypeTitle, '') AS OperationTypeTitle,
                   COALESCE(tbl_TruckCrossShipment.fld_TruckCrossShipmentTitle, '') AS PresentShipmentTitle,
                   COALESCE(tbl_TruckCross.fld_TruckCrossPresentShipmentNumber, '') AS PresentShipmentNumber,
                   COALESCE(tbl_TruckCrossCustomer.fld_TruckCrossCustomerTitle, '') AS PresentCustomerTitle,
                   COALESCE(tbl_TruckCrossOperationDestination.fld_TruckCrossOperationDestinationTitle, '') AS OperationDestinationTitle,
                   COALESCE(tbl_TruckCross.fld_TruckCrossPresentDesc, '') AS PresentDesc,
                   COALESCE(tbl_TruckCross.fld_TruckCrossPresentTurn, '') AS PresentTurn,
                   tbl_TruckCross.fld_TruckCrossEnterDateTime AS EnterDateTime,
                   CAST(CAST(tbl_TruckCross.fld_TruckCrossEnterDateTime as time) as nvarchar(5)) as EnterTime,
                   COALESCE(tbl_User_2.Name, '') AS EnterUser,
                   COALESCE(tbl_TruckCross.fld_TruckCrossEnterWeightTonage, '') AS EnterWeightTonage,
                   COALESCE(tbl_TruckCrossAcceptPlace.fld_TruckCrossAcceptPlaceTitle, '') AS AcceptPlaceTitle,
                   COALESCE(tbl_TruckCross.fld_TruckCrossEnterAcceptor, '') AS EnterAcceptor,
                   tbl_TruckCross.fld_TruckCrossExitDateTime AS ExitDateTime,
                   CAST(CAST(tbl_TruckCross.fld_TruckCrossExitDateTime as time) as nvarchar(5)) as ExitTime,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitWeightTonage, '') AS ExitWeightTonage,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitPureWeightCargo, 0) AS ExitPureWeightCargo,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitWeightbridgeReceiptNumber, '') AS ExitWeightbridgeReceiptNumber,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitCargoOwnerName, '') AS ExitCargoOwnerName,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitDeliveryAddress, '') AS ExitDeliveryAddress,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitCargoOwnerPhone, '') AS ExitCargoOwnerPhone,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitDesc, '') AS ExitDesc,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitDestination, '') AS ExitDestination,
                   CASE WHEN COALESCE(tbl_TruckCross.fld_TruckCrossExitPaymentType, 0) = 1 THEN N'با فرستنده'
                        WHEN COALESCE(tbl_TruckCross.fld_TruckCrossExitPaymentType, 0) = 2 THEN N'با گیرنده' 
                        WHEN COALESCE(tbl_TruckCross.fld_TruckCrossExitPaymentType, 0) = 3 THEN N'با شرکت' 
                        ELSE N'مشخص نشده' END as ExitPaymentTypeTitle,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitTotalCost, '') AS ExitTotalCost,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitUnitPrice, '') AS ExitUnitPrice,
                   COALESCE(tbl_TruckCross.fld_TruckCrossExitDistance, '') AS ExitDistance,
                   COALESCE(tbl_User.Name, '') AS ExitUser,
            	   CAST(COALESCE((SELECT TOP(1) tbl_MovementActions.MovementActionId FROM tbl_MovementActions 
            	   WHERE tbl_MovementActions.MovementActionTruckCrossId = tbl_TruckCross.fld_TruckCrossId),0) as nvarchar(100)) as GateOpCode,
            	   COALESCE((SELECT TOP(1) tbl_MovementActions.MovementActionDocumentId FROM tbl_MovementActions 
            	   WHERE tbl_MovementActions.MovementActionTruckCrossId = tbl_TruckCross.fld_TruckCrossId),'') as DocumentId,
            	   COALESCE((SELECT TOP(1) tbl_MovementActions.MovementActionUHFLogGate FROM tbl_MovementActions 
            	   WHERE tbl_MovementActions.MovementActionTruckCrossId = tbl_TruckCross.fld_TruckCrossId),'') as GateCode
            FROM tbl_TruckCross
            LEFT OUTER JOIN tbl_User ON tbl_TruckCross.fld_TruckCrossExitUserId = tbl_User.Id
            LEFT OUTER JOIN tbl_User AS tbl_User_2 ON tbl_TruckCross.fld_TruckCrossEnterUserId = tbl_User_2.Id
            LEFT OUTER JOIN tbl_TruckCompany ON tbl_TruckCross.fld_TruckCrossCompany = tbl_TruckCompany.fld_TruckCompanyId
            LEFT OUTER JOIN tbl_User AS tbl_User_1 ON tbl_TruckCross.fld_TruckCrossPresentUserId = tbl_User_1.Id
            LEFT OUTER JOIN tbl_TruckCrossOperationDestination ON tbl_TruckCross.fld_TruckCrossPresentOperationDestination = tbl_TruckCrossOperationDestination.fld_TruckCrossOperationDestinationId
            LEFT OUTER JOIN tbl_TruckCrossShipment ON tbl_TruckCross.fld_TruckCrossPresentShipment = tbl_TruckCrossShipment.fld_TruckCrossShipmentId
            LEFT OUTER JOIN tbl_TruckCrossOperationType ON tbl_TruckCross.fld_TruckCrossPresentOperationType = tbl_TruckCrossOperationType.fld_TruckCrossOperationTypeId
            LEFT OUTER JOIN tbl_TruckType ON tbl_TruckCross.fld_TruckCrossType = tbl_TruckType.fld_TruckTypeId
            LEFT OUTER JOIN tbl_TruckCrossCustomer ON tbl_TruckCross.fld_TruckCrossPresentCustomer = tbl_TruckCrossCustomer.fld_TruckCrossCustomerId
            LEFT OUTER JOIN tbl_TruckCrossCause ON tbl_TruckCross.fld_TruckCrossPresentCause = tbl_TruckCrossCause.fld_TruckCrossCauseId
            LEFT OUTER JOIN tbl_TruckCrossAcceptPlace ON tbl_TruckCross.fld_TruckCrossEnterAcceptPlace = tbl_TruckCrossAcceptPlace.fld_TruckCrossAcceptPlaceId
            WHERE tbl_TruckCross.fld_TruckCrossId = {truckCrossId}
            """;

        var result = dataAccess.SqlDataAdapter(command);
        return result;
    }

    public DataTable SGetAllPrintableTruckCrossDatas(List<int> truckCrossIds)
    {
        string command = $"""
        SELECT 
            COALESCE(tc.fld_TruckCrossId, 0) AS Id,
            tc.fld_TruckCrossPresentDateTime AS PresentDateTime,
            CAST(CAST(tc.fld_TruckCrossPresentDateTime AS time) AS nvarchar(5)) AS PresentTime,
            dbo.GeorgianDateToJalaliDate(tc.fld_TruckCrossPresentDateTime) AS PresentDate,
            COALESCE(u1.Name, '') AS PresentUser,
            COALESCE(tc.fld_TruckCrossNationalCode, '') AS NationalCode,
            COALESCE(tc.fld_TruckCrossDriverName, '') AS DriverName,
            COALESCE(tcomp.fld_TruckCompanyTitle, '') AS CompanyTitle,
            COALESCE(tc.fld_TruckCrossPassportCode, '') AS PassportCode,
            COALESCE(tc.fld_TruckCrossDriverPhone, '') AS DriverPhone,
            COALESCE(tc.fld_TruckCrossLicenseCode, '') AS LicenseCode,
            COALESCE(tc.fld_TruckCrossPlaque, '') AS Plaque,
            COALESCE(tc.fld_TruckCrossInternationalPlaque, '') AS InternationalPlaque,
            COALESCE(tt.fld_TruckTypeTitle, '') AS TruckTypeTitle,
            COALESCE(tc.fld_TruckCrossTypeDesc, '') AS TypeDesc,
            COALESCE(tcc.fld_TruckCrossCauseTitle, '') AS PresentCauseTitle,
            COALESCE(tcot.fld_TruckCrossOperationTypeTitle, '') AS OperationTypeTitle,
            COALESCE(tcs.fld_TruckCrossShipmentTitle, '') AS PresentShipmentTitle,
            COALESCE(tc.fld_TruckCrossPresentShipmentNumber, '') AS PresentShipmentNumber,
            COALESCE(tccust.fld_TruckCrossCustomerTitle, '') AS PresentCustomerTitle,
            COALESCE(tcod.fld_TruckCrossOperationDestinationTitle, '') AS OperationDestinationTitle,
            COALESCE(tc.fld_TruckCrossPresentDesc, '') AS PresentDesc,
            COALESCE(tc.fld_TruckCrossPresentTurn, '') AS PresentTurn,
            tc.fld_TruckCrossEnterDateTime AS EnterDateTime,
            CAST(CAST(tc.fld_TruckCrossEnterDateTime AS time) AS nvarchar(5)) AS EnterTime,
            COALESCE(u2.Name, '') AS EnterUser,
            COALESCE(tcap.fld_TruckCrossAcceptPlaceTitle, '') AS AcceptPlaceTitle,
            COALESCE(tc.fld_TruckCrossEnterAcceptor, '') AS EnterAcceptor,
            tc.fld_TruckCrossExitDateTime AS ExitDateTime,
            CAST(CAST(tc.fld_TruckCrossExitDateTime AS time) AS nvarchar(5)) AS ExitTime,
            COALESCE(CAST(tc.fld_TruckCrossExitWeightTonage AS nvarchar(50)), '0') AS ExitWeightTonage,
            COALESCE(CAST(tc.fld_TruckCrossExitPureWeightCargo AS nvarchar(50)), '0') AS ExitPureWeightCargo,
            COALESCE(tc.fld_TruckCrossExitCargoOwnerName, '') AS ExitCargoOwnerName,
            COALESCE(tc.fld_TruckCrossExitDeliveryAddress, '') AS ExitDeliveryAddress,
            COALESCE(tc.fld_TruckCrossExitCargoOwnerPhone, '') AS ExitCargoOwnerPhone,
            COALESCE(tc.fld_TruckCrossExitDesc, '') AS ExitDesc,
            COALESCE(tc.fld_TruckCrossExitDestination, '') AS ExitDestination,
            CASE 
                WHEN COALESCE(tc.fld_TruckCrossExitPaymentType, 0) = 1 THEN N'با فرستنده'
                WHEN COALESCE(tc.fld_TruckCrossExitPaymentType, 0) = 2 THEN N'با گیرنده'
                WHEN COALESCE(tc.fld_TruckCrossExitPaymentType, 0) = 3 THEN N'با شرکت'
                ELSE N'مشخص نشده'
            END AS ExitPaymentTypeTitle,
            COALESCE(CAST(tc.fld_TruckCrossExitTotalCost AS nvarchar(50)), '') AS ExitTotalCost,
            COALESCE(CAST(tc.fld_TruckCrossExitUnitPrice AS nvarchar(50)), '') AS ExitUnitPrice,
            COALESCE(CAST(tc.fld_TruckCrossExitDistance AS nvarchar(50)), '') AS ExitDistance,
            COALESCE(u3.Name, '') AS ExitUser,
            CAST(COALESCE(ma.MovementActionId, 0) AS nvarchar(100)) AS GateOpCode,
            COALESCE(ma.MovementActionDocumentId, '') AS DocumentId,
            COALESCE(ma.MovementActionUHFLogGate, '') AS GateCode,
            COALESCE(tc.fld_TruckCrossDynamicFields, '') AS DynamicData
        FROM tbl_TruckCross tc WITH (NOLOCK)
        LEFT JOIN tbl_User u3 WITH (NOLOCK) ON tc.fld_TruckCrossExitUserId = u3.Id
        LEFT JOIN tbl_User u2 WITH (NOLOCK) ON tc.fld_TruckCrossEnterUserId = u2.Id
        LEFT JOIN tbl_TruckCompany tcomp WITH (NOLOCK) ON tc.fld_TruckCrossCompany = tcomp.fld_TruckCompanyId
        LEFT JOIN tbl_User u1 WITH (NOLOCK) ON tc.fld_TruckCrossPresentUserId = u1.Id
        LEFT JOIN tbl_TruckCrossOperationDestination tcod WITH (NOLOCK) ON tc.fld_TruckCrossPresentOperationDestination = tcod.fld_TruckCrossOperationDestinationId
        LEFT JOIN tbl_TruckCrossShipment tcs WITH (NOLOCK) ON tc.fld_TruckCrossPresentShipment = tcs.fld_TruckCrossShipmentId
        LEFT JOIN tbl_TruckCrossOperationType tcot WITH (NOLOCK) ON tc.fld_TruckCrossPresentOperationType = tcot.fld_TruckCrossOperationTypeId
        LEFT JOIN tbl_TruckType tt WITH (NOLOCK) ON tc.fld_TruckCrossType = tt.fld_TruckTypeId
        LEFT JOIN tbl_TruckCrossCustomer tccust WITH (NOLOCK) ON tc.fld_TruckCrossPresentCustomer = tccust.fld_TruckCrossCustomerId
        LEFT JOIN tbl_TruckCrossCause tcc WITH (NOLOCK) ON tc.fld_TruckCrossPresentCause = tcc.fld_TruckCrossCauseId
        LEFT JOIN tbl_TruckCrossAcceptPlace tcap WITH (NOLOCK) ON tc.fld_TruckCrossEnterAcceptPlace = tcap.fld_TruckCrossAcceptPlaceId
        OUTER APPLY (
            SELECT TOP(1) 
                MovementActionId,
                MovementActionDocumentId,
                MovementActionUHFLogGate
            FROM tbl_MovementActions WITH (NOLOCK)
            WHERE MovementActionTruckCrossId = tc.fld_TruckCrossId
        ) ma
        WHERE tc.fld_TruckCrossId IN ({string.Join(',', truckCrossIds)})
        """;

        var result = dataAccess.SqlDataAdapter(command, 180);
        return result;
    }

    public bool SGetTruckCrossSecurityGatePhysicalStatus()
    {
        return bool.Parse(configuration["ProjectConfigs:WmsConfigs:TruckCrossGate:IsPhysical"]);
    }

    public GetTruckCrossPriceExitVm SCalculatePriceExit(TruckCrossData cross)
    {
        GetTruckCrossPriceExitVm result = new()
        {
            FinalPrice = 0,
            Fee = 0
        };

        try
        {
            var List_TruckCrossShippingFeeByProductType = dataAccess.SqlDataAdapter("SELECT        fld_TruckCrossShippingFeeProductTypeId, fld_TruckCrossShippingFeeAmount FROM            tbl_TruckCrossShippingFee");
            var List_TruckCrossShippingFeeByCompany = dataAccess.SqlDataAdapter("SELECT        fld_TruckCrossShippingFeeCompanyId, fld_TruckCrossShippingFeeAmount FROM            tbl_TruckCrossShippingFee");
            decimal Fee = 0;
            foreach (DataRow dr in List_TruckCrossShippingFeeByCompany.Rows)
            {
                if (dr["fld_TruckCrossShippingFeeCompanyId"].ToString() == cross.TruckCrossCompanyId.ToString())
                {
                    Fee = Convert.ToDecimal(dr["fld_TruckCrossShippingFeeAmount"].ToString());
                    break;
                }
            }
            result.FinalPrice = Fee * cross.ExitPureWeightCargo;

            result.Fee = Fee;

            return result;
        }
        catch
        {
            return result;
        }
    }
    #endregion

    #region Truck Company
    public List<TruckCrossCompany> SGetAllTruckCompany()
    => apiContext.TruckCompanies.ToList();

    public int SSaveTruckCompany(TruckCrossCompany truckCrossCompany)
    {
        if (truckCrossCompany.Id.Equals(0))
        {
            apiContext.Add(truckCrossCompany);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCompany"));
            }
        }
        else
        {
            apiContext.Update(truckCrossCompany);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteTruckCompany(int id)
    {
        apiContext.TruckCompanies.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }

    public List<TruckCrossDataDto> SGetTruckCrossByNc(string nationalCode)
    {
        var query = apiContext.Crosses
                              .Where(p => p.NationalCode == nationalCode)
                              .Include(p => p.PresentUser)
                              .ToList()
                              .DistinctBy(p => p.Plaque);

        var dynamicFieldIds = apiContext.DynamicFields
                  .Where(p => p.FieldType == (int)DynamicFieldType.TruckCrossPresent)
                  .Select(p => p.Id.ToString())
                  .ToHashSet();

        var result = new List<TruckCrossData>();

        foreach (var cross in query)
        {
            if (cross.DynamicData.HasNoValue())
            {
                result.Add(cross);

                continue;
            }

            try
            {
                var token = JToken.Parse(cross.DynamicData);

                if (token is JObject jsonObject)
                {
                    var filteredJson = new JObject();

                    foreach (var property in jsonObject.Properties())
                    {
                        if (dynamicFieldIds.Contains(property.Name))
                        {
                            filteredJson[property.Name] = property.Value;
                        }
                    }

                    cross.DynamicData = filteredJson.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, ex.Message);

                cross.DynamicData = "{}";
            }

            result.Add(cross);
        }

        return mapper.Map<List<TruckCrossDataDto>>(result);
    }
  
    public List<TruckCrossDataDto> SGetUnexitedCrosses()
    {
        var data = mapper.Map<List<TruckCrossDataDto>>(apiContext.Crosses
                     .Where(p => p.TruckCrossStatus == (int)TruckCrossStatuses.Present ||
                                 p.TruckCrossStatus == (int)TruckCrossStatuses.Enter)
                     .Include(p => p.PresentUser)
                     .Include(p => p.EnterUser)
                     .Include(p => p.ExitUser)
                     .OrderBy(p => p.PresentTurn))
                     .ToList();

        return data;
    }
    #endregion

    #region TruckCross OperationType
    public List<TruckCrossOperationType> SGetAllTruckCrossOperationType()
    => apiContext.TruckCrossOperationTypes.ToList();

    public List<TruckCrossOperationType> SGetTruckCrossOperationTypesByCause(int presentCauseId)
    => apiContext.TruckCrossOperationTypes.Where(p => p.TruckCrossCauseId.Equals(presentCauseId)).ToList();

    public int SSaveTruckCrossOperationType(TruckCrossOperationType truckCrossOperationType)
    {
        if (truckCrossOperationType.Id.Equals(0))
        {
            apiContext.Add(truckCrossOperationType);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCrossOperationType"));
            }
        }
        else
        {
            apiContext.Update(truckCrossOperationType);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteTruckCrossOperationType(int id)
    {
        apiContext.TruckCrossOperationTypes.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion

    #region TruckCross OperationDestination
    public List<TruckCrossOperationDestination> SGetAllTruckCrossOperationDestination()
    => apiContext.TruckCrossOperationDestinations.ToList();

    public int SSaveTruckCrossOperationDestination(TruckCrossOperationDestination truckCrossOperationDestination)
    {
        if (truckCrossOperationDestination.Id.Equals(0))
        {
            apiContext.Add(truckCrossOperationDestination);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCrossOperationDestination"));
            }
        }
        else
        {
            apiContext.Update(truckCrossOperationDestination);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteTruckCrossOperationDestination(int id)
    {
        apiContext.TruckCrossOperationDestinations.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion

    #region TruckCross Shipment
    public List<TruckCrossShipment> SGetAllTruckCrossShipment()
    => apiContext.TruckCrossShipments.ToList();

    public int SSaveTruckCrossShipment(TruckCrossShipment truckCrossShipment)
    {
        if (truckCrossShipment.Id.Equals(0))
        {
            apiContext.Add(truckCrossShipment);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCrossShipment"));
            }
        }
        else
        {
            apiContext.Update(truckCrossShipment);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteTruckCrossShipment(int id)
    {
        apiContext.TruckCrossShipments.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion

    #region TruckCross Customer
    public List<TruckCrossCustomer> SGetAllTruckCrossCustomer()
    => apiContext.TruckCrossCustomers.ToList();

    public int SSaveTruckCrossCustomer(TruckCrossCustomer truckCrossCustomer)
    {
        if (truckCrossCustomer.Id.Equals(0))
        {
            apiContext.Add(truckCrossCustomer);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCrossCustomer"));
            }
        }
        else
        {
            apiContext.Update(truckCrossCustomer);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteTruckCrossCustomer(int id)
    {
        apiContext.TruckCrossCustomers.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion

    #region TruckCross ProductType
    public List<TruckCrossProductType> SGetAllTruckCrossProductType()
    => apiContext.TruckCrossProductTypes.ToList();

    public List<TruckCrossProductType> SGetTruckCrossProductTypesByCause(int presentCauseId)
    {

        return apiContext.TruckCrossProductTypes
                          .ToList()
                          .Where(p => p.TruckCrossCauseIdsArray != null && p.TruckCrossCauseIds.Any(p => p == presentCauseId))
                          .ToList();

    }


    public List<TruckCrossProductType> SGetTruckCrossProductTypesByCauses(List<int> presentCauseIds)
    => apiContext.TruckCrossProductTypes.ToList()
            .Where(p => p.TruckCrossCauseIdsArray is not null &&
                       p.TruckCrossCauseIds.ToList().Intersect(presentCauseIds).Count().Equals(presentCauseIds.Count)
            ).ToList();

    public int SSaveTruckCrossProductType(TruckCrossProductType truckCrossProductType)
    {
        if (truckCrossProductType.Id.Equals(0))
        {
            apiContext.Add(truckCrossProductType);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCrossProductType"));
            }
        }
        else
        {
            apiContext.Update(truckCrossProductType);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteTruckCrossProductType(int id)
    {
        apiContext.TruckCrossProductTypes.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion

    #region TruckCross AcceptPlace
    public List<TruckCrossAcceptPlace> SGetAllTruckCrossAcceptPlace()
    => apiContext.TruckCrossAcceptPlaces.ToList();

    public int SSaveTruckCrossAcceptPlace(TruckCrossAcceptPlace truckCrossAcceptPlace)
    {
        if (truckCrossAcceptPlace.Id.Equals(0))
        {
            apiContext.Add(truckCrossAcceptPlace);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCrossAcceptPlace"));
            }
        }
        else
        {
            apiContext.Update(truckCrossAcceptPlace);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteTruckCrossAcceptPlace(int id)
    {
        apiContext.TruckCrossAcceptPlaces.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion

    #region Truck Cross Cause
    public int SSaveTruckPresentCause(TruckCrossCause truckCrossCause)
    {
        if (truckCrossCause.Id.Equals(0))
        {
            apiContext.Add(truckCrossCause);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCrossCause"));
            }
        }
        else
        {
            apiContext.Update(truckCrossCause);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public List<TruckCrossCause> SGetTruckPresentCause()
    => apiContext.TruckCrossCauses.ToList();

    public bool SDeleteTruckPresentCause(int id)
    {
        apiContext.TruckCrossCauses.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion

    #region Truck Type
    public List<TruckType> SGetTruckType()
    => apiContext.TruckTypes.ToList();

    public bool SDeleteTruckType(int id)
    {
        apiContext.TruckTypes.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }

    public int SSaveTruckType(TruckType truckType)
    {
        if (truckType.Id.Equals(0))
        {
            apiContext.Add(truckType);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckType"));
            }
        }
        else
        {
            apiContext.Update(truckType);

            return apiContext.SaveChanges();
        }

        return 0;
    }
    #endregion

    #region WeighbridgeLogs
    public WeighBridgeLog SGetLastWeighbridgeLog()
    {
        if (apiContext.WeighbridgeLogs.Any())
        {
            return apiContext.WeighbridgeLogs.OrderByDescending(p => p.DateTime).First();
        }

        return new()
        {
            DateTime = DateTime.Now,
            ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
            Weight = 0
        };
    }

    public int SSaveWeighbridgeLog(WeighBridgeLogDto log)
    {
        apiContext.Add(new WeighBridgeLog()
        {
            DateTime = DateTime.Now,
            ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
            Weight = log.Weight,
            WeighbridgeCode = log.WeighbridgeCode
        });

        if (apiContext.SaveChanges() == 1)
        {
            return int.Parse(GetLatestIdOfIdentityTable("tbl_WeighbridgeLog"));
        }

        return 0;
    }
    #endregion

    #region TruckCross Item
    public List<GetTruckCrossItemsVm> SGetTruckCrossItemsByTruckCrossId(long truckCrossId)
    => mapper.Map<List<GetTruckCrossItemsVm>>(apiContext.TruckCrossItems
                         .Where(p => p.TruckCrossId == truckCrossId)
                         .Include(p => p.TruckCrossProductType));

    public List<GetTruckCrossItemsVm> SGetTruckCrossItemsByTruckCrossIdAndType(long truckCrossId, Domains.Entities.TruckCrossItemTypes truckCrossItemType)
    => mapper.Map<List<GetTruckCrossItemsVm>>(
                          apiContext.TruckCrossItems
                                    .Where(p => p.Type == truckCrossItemType &&
                                                p.TruckCrossId == truckCrossId)
                                    .Include(p => p.TruckCrossProductType));

    public List<GetTruckCrossItemsVm> SSaveTruckCrossItem(List<TruckCrossItem> items)
    {
        if (!items.Any())
        {
            return null;
        }

        var truckCrossId = items.First().TruckCrossId;

        var truckCrossItemType = items.First().Type;

        apiContext.TruckCrossItems.Where(b => b.Type == truckCrossItemType && b.TruckCrossId == truckCrossId).ExecuteDelete();

        apiContext.TruckCrossItems.AddRange(items);

        apiContext.SaveChanges();

        return SGetTruckCrossItemsByTruckCrossIdAndType(truckCrossId.Value, truckCrossItemType.Value);
    }

    public bool SDeleteTruckCrossItem(int id)
    => apiContext.TruckCrossItems.Where(p => p.Id == id).ExecuteDelete() > 0;
    #endregion

    #region Revoke
    public bool SRevokePresentTruckCross(int id)
    {
        List<Expression<Func<SetPropertyCalls<TruckCrossData>, SetPropertyCalls<TruckCrossData>>>> parameters = new()
        {
            { sett => sett.SetProperty(x => x.PresentRevokeDateTime, DateTime.Now) },
            { sett => sett.SetProperty(x => x.PresentRevokeUserId, httpContext.User.GetUserId()) },
            { sett => sett.SetProperty(x => x.TruckCrossStatus, (int)TruckCrossStatuses.Revoke) }
        };

        return apiContext.Crosses
                         .Where(p => p.Id == id)
                         .ExecuteUpdate(sett => sett.SetProperty(x => x.PresentRevokeDateTime, DateTime.Now)
                                                                             .SetProperty(x => x.PresentRevokeUserId, httpContext.User.GetUserId())
                                                                             .SetProperty(x => x.TruckCrossStatus, (int)TruckCrossStatuses.Revoke)) > 0;
    }
    #endregion

    #region Convertion
    public int SConvertTruckCrossStatus()
    {
        string command = """
            UPDATE tbl_TruckCross  
            SET     fld_TruckCrossStatus =  CASE  
                                    WHEN fld_TruckCrossPresentRevokeDateTime IS NOT NULL THEN 4
                                    WHEN fld_TruckCrossExitDateTime IS NOT NULL 
            						 AND fld_TruckCrossPresentRevokeDateTime IS NULL THEN 3
                                    WHEN fld_TruckCrossEnterDateTime IS NOT NULL 
            						 AND fld_TruckCrossExitDateTime IS NULL 
            						 AND fld_TruckCrossPresentRevokeDateTime IS NULL THEN 2
                                    WHEN fld_TruckCrossPresentDateTime IS NOT NULL 
            						 AND fld_TruckCrossEnterDateTime IS NULL 
            						 AND fld_TruckCrossExitDateTime IS NULL 
            						 AND fld_TruckCrossPresentRevokeDateTime IS NULL THEN 1
                                    ELSE 0
                                END 
            WHERE fld_TruckCrossStatus is null
            """;

        return dataAccess.CmdSqlExecuteNonQuery(command);
    }
    #endregion

    #region TruckCross Shipment
    public List<TruckCrossShipmentFee> SGetAllTruckCrossShipmentFee()
    => apiContext.TruckCrossShipmentFees.ToList();

    public int SSaveTruckCrossShipmentFee(TruckCrossShipmentFee truckCrossShipmentFee)
    {
        if (truckCrossShipmentFee.Id.Equals(0))
        {
            apiContext.Add(truckCrossShipmentFee);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_TruckCrossShippingFee"));
            }
        }
        else
        {
            apiContext.Update(truckCrossShipmentFee);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteTruckCrossShipmentFee(int id)
    {
        apiContext.TruckCrossShipmentFees.Remove(new()
        {
            Id = id
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion
}
