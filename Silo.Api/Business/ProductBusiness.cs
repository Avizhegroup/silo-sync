using System.Globalization;
using AutoMapper;
using Silo.Api.Tools;
using Silo.Application;
using Silo.Application.Contracts;
using Silo.Domains.Services;

namespace Silo.Api.Business;

public class ProductBusiness(ILogger<ProductBusiness> logger
        , IDataAccess dataAccess
        , IHttpContextAccessor httpContextAccessor
        , IConfiguration configuration
        , WmsApiContext apiContext
        , IMapper mapper) : ProjectBusiness(dataAccess, logger, httpContextAccessor)
{
    public List<string> SGetAllTechnicalInfoDataKeys()
    {
        var cmd = $"""
            SELECT DISTINCT [key] as Titles
            FROM tbl_Products
            CROSS APPLY OPENJSON(ProductTechnicalData)
            WHERE tbl_Products.ProductTechnicalData != ''
            """;

        List<string?> keys = dataAccess.SqlDataAdapter(cmd, 120).AsEnumerable().Select(p => p.Field<string>("Titles")).ToList();

        return keys;
    }

    #region Guarantee and Expire
    public bool SSaveExpireAndGuaranteeByProductCode(SaveExpireGuaranteeByProductCodesCommand command)
    {
        string userId = httpContext.User.GetUserId();

        List<string> commands = new();

        string updateCommandSetPart = string.Empty;

        if (command.GuaranteeType != GuaranteeTypes.None)
        {
            if (command.GuaranteeType != GuaranteeTypes.Date)
            {
                updateCommandSetPart += $" fld_ProductGuaranteeType = {(int)command.GuaranteeType} , fld_ProductGuaranteeMonths = {command.GuaranteeMonths} ";
            }
            else
            {
                updateCommandSetPart += $" fld_ProductGuaranteeType = {(int)command.GuaranteeType} , fld_ProductGuaranteeEndDate = N'{command.GuaranteeDate}' ";
            }
        }

        if (command.ExpireType != GuaranteeTypes.None)
        {
            if (updateCommandSetPart.HasValue())
            {
                updateCommandSetPart += " , ";
            }

            if (command.ExpireType != GuaranteeTypes.Date)
            {
                updateCommandSetPart += $" fld_ProductExpireType = {(int)command.ExpireType} , fld_ProductExpireMonths = {command.ExpireMonths} ";
            }
            else
            {
                updateCommandSetPart += $" fld_ProductExpireType = {(int)command.ExpireType} , fld_ProductExpireEndDate = N'{command.ExpireDate}' ";
            }
        }

        if (updateCommandSetPart.HasValue())
        {
            commands.Add(
                $"""
                UPDATE tbl_Products 
                SET {updateCommandSetPart}
                WHERE ProductCode IN ('{string.Join("','", command.ProductCodes)}')
                """);
        }

        if (commands.Any())
        {
            foreach (var productCode in command.ProductCodes)
            {
                commands.Add(
                $"""
                INSERT INTO tbl_ExpireGuaranteeLog 
                (
                fld_ExpireGuaranteeProductCode,
                fld_ExpireGuaranteeGuaranteeType,
                fld_ExpireGuaranteeGuaranteeMonths,
                fld_ExpireGuaranteeExpireType, 
                fld_ExpireGuaranteeExpireMonths, 
                fld_ExpireGuaranteeUserId,
                fld_ExpireGuaranteeDateTime, 
                fld_ExpireGuaranteeDate, 
                fld_ExpireGuaranteeTime,
                fld_ExpireGuaranteeGuaranteeEndDate,
                fld_ExpireGuaranteeExpireEndDate
                )
                VALUES (
                N'{productCode}', 
                N'{(int)command.GuaranteeType}' ,
                N'{command.GuaranteeMonths}',
                N'{(int)command.ExpireType}', 
                N'{command.ExpireMonths}', 
                N'{userId}',
                N'{DateTime.Now}', 
                N'{PersianCalendarTools.GregorianToPersian(DateTime.Now)}',
                N'{DateTime.Now.ToString("HH:mm")}',
                N'{command.GuaranteeDate}',
                N'{command.ExpireDate}')
                """);
            }
        }

        return dataAccess.CmdSqlExecuteNonQueryWithTransaction(commands) > 0;
    }

    public DataTable SGetProductsForGuaranteeExpire(List<ReportFilter> reportFilters)
    {
        List<string> whereList = new();

        foreach (ReportFilter filter in reportFilters)
        {
            if (filter.SqlWhereCommand.HasValue())
            {
                continue;
            }

            if (filter.Type.Equals(FilterType.Static))
            {
                switch (filter.FieldName)
                {
                    case "ProductCode":
                        filter.SqlWhereCommand = "tbl_Products.ProductCode";
                        break;

                    case "ProductName":
                        filter.SqlWhereCommand = "tbl_Products.ProductTitle";
                        break;

                    case "ProductType":
                        filter.SqlWhereCommand = $"tbl_Products.ProductType";
                        break;

                    case "ProductGroup":
                        filter.SqlWhereCommand = $"tbl_Products.fld_ProductGroup";
                        break;

                    case "ProductBrand":
                        filter.SqlWhereCommand = "tbl_Products.fld_ProductBrand";
                        break;

                    case "ProductSize":
                        filter.SqlWhereCommand = $"tbl_Products.ProductSize";
                        break;

                    case "Qc":
                        filter.SqlWhereCommand = "tbl_Products.ProductStatus";
                        break;

                    case "TechnicalCode":
                        filter.SqlWhereCommand = $"tbl_Products.ProductTechnicalCode";
                        break;

                    case "ProductClass":
                        filter.SqlWhereCommand = $"tbl_Products.fld_ProductClass";
                        break;

                    case "ProductSubGroup":
                        filter.SqlWhereCommand = $"tbl_Products.fld_ProductSubGroup";
                        break;
                }

                if (filter.SqlWhereCommand.HasValue())
                {
                    whereList.Add(DynamicFilteringTools.GetStaticWhere(filter));
                }
            }
        }

        var cmd =
            $"""
            SELECT COALESCE(tbl_Products.ProductCode, N'') AS ProductCode,
            COALESCE(tbl_Products.ProductTitle, N'') AS ProductName,
            COALESCE(tbl_ProductType.ProductTypeTitle, N'') AS ProductType,
            COALESCE(tbl_ProductGroup.fld_ProductGroupTitle, N'') AS ProductGroup,
            COALESCE(tbl_ProductBrand.fld_ProductBrandTitle, N'') AS ProductBrand,
            COALESCE([Size].fld_ProductPropertyCTitle, N'') AS Size,
            COALESCE([Qc].ProductStatusTitle, N'') AS Qc,
            COALESCE(tbl_Products.ProductTechnicalCode, N'') AS TechnicalCode,
            COALESCE((SELECT fld_ProductClassTitle FROM tbl_ProductClass WHERE fld_ProductClassCode = tbl_Products.fld_ProductClass),N'') AS ProductClassTitle,
            COALESCE((SELECT fld_ProductSubGroupTitle FROM tbl_ProductSubGroup WHERE fld_ProductSubGroupCode = tbl_Products.fld_ProductSubGroup ),N'') AS ProductSubGroupTitle,
            CASE tbl_Products.fld_ProductGuaranteeType 
            WHEN {(int)GuaranteeTypes.EnterToWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Enter}' 
            WHEN {(int)GuaranteeTypes.ExitFromWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Exit}' 
            WHEN {(int)GuaranteeTypes.AcceptInspect} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Inspect}' 
            WHEN {(int)GuaranteeTypes.ExitFromFactory} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Factory}'
            WHEN {(int)GuaranteeTypes.Sell} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Sell}'
            WHEN {(int)GuaranteeTypes.Install} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Install}'
            WHEN {(int)GuaranteeTypes.Customer} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Customer}'
            WHEN {(int)GuaranteeTypes.Date} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Date}'
            ELSE N'{TextResources.APP_StringKeys_NotChoosed}'
            END AS [GuaranteeStatus],
            CASE WHEN tbl_Products.fld_ProductGuaranteeType = 8 THEN tbl_Products.fld_ProductGuaranteeEndDate
            ELSE COALESCE(CAST(tbl_Products.fld_ProductGuaranteeMonths AS nvarchar(10)) + N' ماه',N'') END AS [GuaranteeDuration],
            CASE tbl_Products.fld_ProductExpireType 
            WHEN {(int)GuaranteeTypes.EnterToWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Enter}' 
            WHEN {(int)GuaranteeTypes.ExitFromWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Exit}' 
            WHEN {(int)GuaranteeTypes.AcceptInspect} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Inspect}' 
            WHEN {(int)GuaranteeTypes.ExitFromFactory} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Factory}'
            WHEN {(int)GuaranteeTypes.Sell} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Sell}'
            WHEN {(int)GuaranteeTypes.Install} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Install}'
            WHEN {(int)GuaranteeTypes.Customer} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Customer}'
            WHEN {(int)GuaranteeTypes.Date} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Date}'
            ELSE N'{TextResources.APP_StringKeys_NotChoosed}' END AS [ExpireStatus],
            CASE WHEN tbl_Products.fld_ProductExpireType = 8 THEN tbl_Products.fld_ProductExpireEndDate
            ELSE COALESCE(CAST(tbl_Products.fld_ProductExpireMonths AS nvarchar(10)) + N' ماه',N'') END AS [ExpireDuration]
            FROM tbl_Products
            LEFT JOIN tbl_ProductType ON tbl_ProductType.ProductTypeCode = tbl_Products.ProductType 
            LEFT JOIN tbl_ProductGroup ON tbl_ProductGroup.fld_ProductGroupCode = tbl_Products.fld_ProductGroup
            LEFT JOIN tbl_ProductBrand ON tbl_ProductBrand.fld_ProductBrandCode = tbl_Products.fld_ProductBrand
            LEFT JOIN tbl_ProductPropertyC AS [Size] ON [Size].fld_ProductPropertyCId = tbl_Products.ProductSize
            LEFT JOIN tbl_ProductStatus AS [Qc] ON [Qc].ProductStatusCode = tbl_Products.ProductStatus
            {(whereList.Any() ? " WHERE " + string.Join(" AND ", whereList) : "")}
            """;

        return dataAccess.SqlDataAdapter(cmd);
    }

    public bool SSaveProductGuarantees(SaveProductGuaranteesCommand command)
    {
        string userId = httpContext.User.GetUserId();

        List<string> commands = new();

        commands.Add(
            $"""
                UPDATE tbl_ProductGuarantee
                SET fld_ProductGuaranteeStatus = N'{command.GuaranteeStatus}',fld_ProductGuaranteeStartDate = N'{command.GuaranteeStartDate}',
                fld_ProductGuaranteeEndDate = N'{command.GuaranteedEndDate}', fld_ProductGuaranteeActivationType = N'{(int)command.GuaranteeActivationType}',
                fld_ProductGuaranteeLastModifiedDateTime = GETDATE() ,fld_ProductGuaranteeLastModifiedUserId = N'{userId}'
                WHERE  tbl_ProductGuarantee.fld_ProductGuaranteeProductSerial IN ('{string.Join("','", command.ProductSerials)}')
                """);

        return dataAccess.CmdSqlExecuteNonQueryWithTransaction(commands) > 0;
    }

    public DataTable SGetProductGuarantees(List<ReportFilter> reportFilters)
    {
        List<string> whereList = new();

        List<string> subWhereList = new();

        foreach (ReportFilter filter in reportFilters)
        {
            if (filter.SqlWhereCommand.HasValue())
            {
                continue;
            }

            if (filter.Type.Equals(FilterType.Static))
            {
                switch (filter.FieldName)
                {
                    case "ProductType":
                        filter.SqlWhereCommand = $"Tags.ProductType";
                        break;

                    case "ProductGroup":
                        filter.SqlWhereCommand = $"Tags.fld_ProductGroup";
                        break;

                    case "ProductSubGroup":
                        filter.SqlWhereCommand = $"Tags.fld_ProductSubGroup";
                        break;

                    case "ProductClass":
                        filter.SqlWhereCommand = $"Tags.fld_ProductClass";
                        break;

                    case "TechnicalCode":
                        filter.SqlWhereCommand = $"Tags.RegCode";
                        break;

                    case "ProductName":
                        filter.SqlWhereCommand = "Tags.ProductName";
                        break;

                    case "ProductCode":
                        filter.SqlWhereCommand = "Tags.ProductCode";
                        break;

                    case "ProductBrand":
                        filter.SqlWhereCommand = "Tags.fld_ProductBrand";
                        break;

                    case "ProductSerial":
                        filter.SqlWhereCommand = $"Tags.ProductSerial";
                        break;

                    case "GuaranteeStatus":
                        filter.SqlWhereCommand = $"Guarantee.fld_ProductGuaranteeStatus";
                        break;

                    case "ExitActionDateTime":
                        filter.SqlWhereCommand = "TagsMovement.RTagsMovementDate";
                        whereList.Add("TagsMovement.RMovementActionType = N'6'");
                        subWhereList.Add(
                        $"""
                            tbl_TagsMovement.RTagsMovementDate IN('{string.Join("','", filter.Values)}')
                        """);
                        break;

                    case "ExitActionDocumentCode":
                        filter.SqlWhereCommand = "TagsMovement.RMovementActionDocumentId";
                        whereList.Add("TagsMovement.RMovementActionType = N'6'");
                        break;

                    case "ExitActionCode":
                        filter.SqlWhereCommand = "TagsMovement.RMovementActionId";
                        whereList.Add("TagsMovement.RMovementActionType = N'6'");
                        break;

                    case "MovementActionData":
                        filter.SqlWhereCommand = "";
                        break;
                }

                if (filter.SqlWhereCommand.HasValue())
                {
                    whereList.Add(DynamicFilteringTools.GetStaticWhere(filter));
                }
            }
        }

        var cmd =
            $"""
            SELECT DISTINCT
            [NESTED].ProductSerial,[NESTED].OldSerial,[NESTED].ProductCode,[NESTED].RegCode,[NESTED].ProductTitle,
            [NESTED].ProductType,[NESTED].ProductGroup,[NESTED].ProductSubGroup,[NESTED].ProductClass,[NESTED].ProductBrand,
            [NESTED].ProductStatus,[NESTED].RegisterShamsiDate,[NESTED].InspectDate,[NESTED].InspectStatusTitle,
            [NESTED].EnterActionDate,[NESTED].ExitActionDate, [NESTED].ActivationType,
            [NESTED].GuaranteeStatus,[NESTED].GuaranteeStartDate,[NESTED].GuaranteeEndDate
            FROM(
            	SELECT  TOP 100 PERCENT
            	COALESCE(Tags.ProductSerial,N'') AS ProductSerial,
            	COALESCE(Tags.DeviceIp,N'') AS OldSerial,
            	COALESCE(Tags.ProductCode,N'') AS ProductCode,
            	COALESCE(Tags.RegCode,N'') AS RegCode,
            	COALESCE(Tags.ProductName,N'') AS ProductTitle,
            	COALESCE((SELECT ProductTypeTitle FROM tbl_ProductType WHERE ProductTypeCode = Tags.ProductType ),N'') AS ProductType,
            	COALESCE((SELECT  fld_ProductGroupTitle FROM tbl_ProductGroup WHERE fld_ProductGroupCode = Tags.fld_ProductGroup),N'') AS ProductGroup,
            	COALESCE((SELECT tbl_ProductSubGroup.fld_ProductSubGroupTitle FROM tbl_ProductSubGroup 
            	WHERE tbl_ProductSubGroup.fld_ProductSubGroupCode = Tags.fld_ProductSubGroup),N'') AS ProductSubGroup,
            	COALESCE((SELECT tbl_ProductClass.fld_ProductClassTitle FROM tbl_ProductClass 
            	WHERE tbl_ProductClass.fld_ProductClassCode = Tags.fld_ProductClass),N'') AS ProductClass,
            	COALESCE((SELECT fld_ProductBrandTitle FROM tbl_ProductBrand WHERE fld_ProductBrandCode = Tags.fld_ProductBrand),N'') AS ProductBrand,
            	COALESCE((SELECT tbl_ProductStatus.ProductStatusTitle FROM tbl_ProductStatus 
            	WHERE  tbl_ProductStatus.ProductStatusCode = Tags.ProductStatus),N'') AS ProductStatus,
            	COALESCE(Tags.TagRegisterShamsiUnixDate,N'') AS RegisterShamsiDate,
            	COALESCE(
            	(SELECT TOP(1) tbl_Inspect.fld_InspectShamsiDate FROM tbl_Inspect 
            	WHERE tbl_Inspect.fld_InspectId = Tags.fld_InspectActionId 
            	AND tbl_Inspect.fld_InspectSerial = Tags.ProductSerial 
            	ORDER BY tbl_Inspect.fld_InspectDateTime DESC), N'') AS InspectDate,
            	CASE WHEN (Tags.Lock = 0 AND fld_LastInspectResult = N'[]') THEN N'بازرسی نشده' 
            	WHEN (Tags.Lock = 0 AND fld_LastInspectResult <> N'[]') THEN N'بازرسی تأیید' 
            	WHEN (Tags.Lock = 1 AND fld_LastInspectResult <> N'[]') THEN N'بازرسی مردود' ELSE N'-' END AS InspectStatusTitle,

            	COALESCE((SELECT TOP(1) tbl_TagsMovement.RTagsMovementDate FROM tbl_TagsMovement 
            	WHERE tbl_TagsMovement.ProductSerial = Tags.ProductSerial AND tbl_TagsMovement.RMovementActionType = N'4'
                {(subWhereList.Any() ? " AND " + string.Join(" AND ", subWhereList) : "")}
            	ORDER BY tbl_TagsMovement.RTagsMovementDate DESC),N'') AS EnterActionDate,
                
            	COALESCE((SELECT TOP(1) tbl_TagsMovement.RTagsMovementDate FROM tbl_TagsMovement 
            	WHERE tbl_TagsMovement.ProductSerial = Tags.ProductSerial AND tbl_TagsMovement.RMovementActionType = N'6'
                {(subWhereList.Any() ? " AND " + string.Join(" AND ", subWhereList) : "")}
            	ORDER BY tbl_TagsMovement.RTagsMovementDate DESC),N'') AS ExitActionDate,

            	CASE WHEN Guarantee.fld_ProductGuaranteeStatus = 0 THEN N'شروع نشده' 
            	WHEN Guarantee.fld_ProductGuaranteeStatus = 1 THEN N'فعال' 
            	WHEN Guarantee.fld_ProductGuaranteeStatus = 2 THEN N'اتمام یافته' 
            	ELSE N'' END AS GuaranteeStatus,
                (CASE WHEN Guarantee.fld_ProductGuaranteeActivationType = 1 THEN N'لحظه ورود به انبار محصول' 
                WHEN Guarantee.fld_ProductGuaranteeActivationType = 2 THEN N'لحظه خروج از انبار محصول' 
                WHEN Guarantee.fld_ProductGuaranteeActivationType = 3 THEN N'لحظه تأیید بازرسی' 
                WHEN Guarantee.fld_ProductGuaranteeActivationType = 4 THEN N'لحظه خروج از کارخانه' 
                WHEN Guarantee.fld_ProductGuaranteeActivationType = 5 THEN N'لحظه ثبت فروش توسط نمایندگی فروش' 
                WHEN Guarantee.fld_ProductGuaranteeActivationType = 6 THEN N'لحظه نصب کالا توسط مأمور نصب' 
                WHEN Guarantee.fld_ProductGuaranteeActivationType = 7 THEN N'لحظه کنترل اصالت کالا توسط مصرف کننده' 
                WHEN Guarantee.fld_ProductGuaranteeActivationType = 8 THEN N'تاریخ مشخص' 
                ELSE N'مشخص نشده' END) AS ActivationType,

            	COALESCE(Guarantee.fld_ProductGuaranteeStartDate,N'') AS GuaranteeStartDate,
            	COALESCE(Guarantee.fld_ProductGuaranteeEndDate,N'') AS GuaranteeEndDate,
            	TagsMovement.RTagsMovementDate AS MovementDate,
            	TagsMovement.RMovementActionType AS ActionType,
            	COALESCE(TagsMovement.RMovementActionDocumentId,N'') AS DocumentId

            	FROM tbl_Tags AS Tags 
            	INNER JOIN tbl_ProductGuarantee AS Guarantee  ON Tags.ProductSerial = Guarantee.fld_ProductGuaranteeProductSerial 
            	LEFT OUTER JOIN tbl_TagsMovement AS TagsMovement ON Tags.ProductSerial = TagsMovement.ProductSerial

                {(whereList.Any() ? " WHERE " + string.Join(" AND ", whereList) : "")}
            	
                ORDER BY TagsMovement.RTagsMovementDate
            	) AS [NESTED]
            
            """;

        return dataAccess.SqlDataAdapter(cmd, 120);
    }

    public int SCopyTagsToGuarantee()
    {
        string userId = httpContext.User.GetUserId();

        string command =
            $"""
            INSERT INTO tbl_ProductGuarantee 
                        (fld_ProductGuaranteeProductSerial, fld_ProductGuaranteeProductCode,
                        fld_ProductGuaranteeStatus,fld_ProductGuaranteeStartDate,fld_ProductGuaranteeEndDate,
                        fld_ProductGuaranteeActivationType,fld_ProductGuaranteeLastModifiedDateTime,
                        fld_ProductGuaranteeLastModifiedUserId)

                        SELECT tbl_Tags.ProductSerial, tbl_Tags.ProductCode,
                        N'0' AS  GuaranteeStatus, NULL AS StartDate,tbl_Products.fld_ProductGuaranteeEndDate AS EndDate,
                        COALESCE(tbl_Products.fld_ProductGuaranteeType ,0 ) AS ActivationType,
                        GETDATE() AS LastModifiedDateTime,N'{userId}' AS LastModifiedUserId

                        FROM tbl_Tags LEFT JOIN tbl_ProductGuarantee ON 
            			tbl_Tags.ProductSerial = tbl_ProductGuarantee.fld_ProductGuaranteeProductSerial LEFT OUTER JOIN
            			tbl_Products ON tbl_Products.ProductCode = tbl_Tags.ProductCode

                        WHERE  tbl_ProductGuarantee.fld_ProductGuaranteeProductSerial IS NULL
            """;

        return dataAccess.CmdSqlExecuteNonQuery(command);
    }

    public int SSaveGuaranteeForSerial(string productSerial, string productCode)
    {
        string userId = httpContext.User.GetUserId();

        string command =
            $"""
            INSERT INTO tbl_ProductGuarantee 
            (fld_ProductGuaranteeProductSerial, 
            fld_ProductGuaranteeProductCode,
            fld_ProductGuaranteeStatus,
            fld_ProductGuaranteeStartDate,
            fld_ProductGuaranteeEndDate,
            fld_ProductGuaranteeActivationType,
            fld_ProductGuaranteeLastModifiedDateTime,
            fld_ProductGuaranteeLastModifiedUserId) 
            SELECT N'{productSerial}', 
            N'{productCode}',
            N'0' AS  GuaranteeStatus, 
            NULL AS StartDate,
            NULL AS EndDate,
            tbl_Products.fld_ProductGuaranteeType AS ActivationType,
            GETDATE() AS LastModifiedDateTime,
            N'{userId}' AS LastModifiedUserId
            FROM tbl_Products WHERE tbl_Products.ProductCode = N'{productCode}'
            """;

        return dataAccess.CmdSqlExecuteNonQuery(command);
    }

    public DataTable SGetProductGuaranteeBySerial(string serial)
    {
        var command =
            $"""
        SELECT 
            CASE 
                WHEN fld_ProductGuaranteeStatus = 0 THEN N'شروع نشده' 
                WHEN fld_ProductGuaranteeStatus = 1 THEN N'فعال' 
                WHEN fld_ProductGuaranteeStatus = 2 THEN N'اتمام یافته' 
                ELSE N'' 
            END AS GuaranteeStatusTitle,
            fld_ProductGuaranteeStatus AS GuaranteeStatus,
            CASE fld_ProductGuaranteeActivationType 
                WHEN {(int)GuaranteeTypes.EnterToWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Enter}' 
                WHEN {(int)GuaranteeTypes.ExitFromWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Exit}' 
                WHEN {(int)GuaranteeTypes.AcceptInspect} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Inspect}' 
                WHEN {(int)GuaranteeTypes.ExitFromFactory} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Factory}'
                WHEN {(int)GuaranteeTypes.Sell} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Sell}'
                WHEN {(int)GuaranteeTypes.Install} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Install}'
                WHEN {(int)GuaranteeTypes.Customer} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Customer}'
                WHEN {(int)GuaranteeTypes.Date} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Date}'
                ELSE N'{TextResources.APP_StringKeys_NotChoosed}' 
            END AS GuaranteeActivationType,
            fld_ProductGuaranteeStartDate AS GuaranteeStartDate,
            fld_ProductGuaranteeEndDate AS GuaranteeEndDate,
            N'' AS GuaranteeRemainingDay,
          COALESCE((SELECT tbl_User.[Name] 
        FROM tbl_User 
        WHERE (TRY_CONVERT(uniqueidentifier, tbl_ProductGuarantee.fld_ProductGuaranteeLastModifiedUserId) IS NOT NULL AND tbl_User.Id =tbl_ProductGuarantee.fld_ProductGuaranteeLastModifiedUserId) OR
        (tbl_User.Username = tbl_ProductGuarantee.fld_ProductGuaranteeLastModifiedUserId)
        ), N'') AS Username
        FROM tbl_ProductGuarantee
        WHERE fld_ProductGuaranteeProductSerial = @serial
        """;

        return dataAccess.SqlDataAdapter(command, new KeyValuePair<string, object>("serial", serial));
    }

    public DataTable SGetProductExpireBySerial(string serial)
    {
        var command =
            $"""
        SELECT 
            CASE 
                WHEN fld_ProductExpireStatus = 0 THEN N'مشخص نشده' 
                WHEN fld_ProductExpireStatus = 1 THEN N'دارای انقضا' 
                WHEN fld_ProductExpireStatus = 2 THEN N'پایان یافته' 
                ELSE N'' 
            END AS ExpireStatusTitle,
            fld_ProductExpireStatus AS ExpireStatus,
            CASE fld_ProductExpireActivationType 
                WHEN {(int)GuaranteeTypes.EnterToWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Enter}' 
                WHEN {(int)GuaranteeTypes.ExitFromWarehouse} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Exit}' 
                WHEN {(int)GuaranteeTypes.AcceptInspect} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Inspect}' 
                WHEN {(int)GuaranteeTypes.ExitFromFactory} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Factory}'
                WHEN {(int)GuaranteeTypes.Sell} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Sell}'
                WHEN {(int)GuaranteeTypes.Install} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Install}'
                WHEN {(int)GuaranteeTypes.Customer} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Customer}'
                WHEN {(int)GuaranteeTypes.Date} THEN N'{TextResources.APP_StringKeys_ExpireAndGuarantee_Type_Date}'
                ELSE N'{TextResources.APP_StringKeys_NotChoosed}' 
            END AS ExpireActivationType,
            fld_ProductExpireStartDate AS ExpireStartDate,
            fld_ProductExpireEndDate AS ExpireEndDate,
            N'' AS ExpireRemainingDay,
            COALESCE((SELECT tbl_User.[Name] 
             FROM tbl_User 
             WHERE (TRY_CONVERT(uniqueidentifier, tbl_ProductExpire.fld_ProductExpireLastModifiedUserId) IS NOT NULL AND tbl_User.Id = tbl_ProductExpire.fld_ProductExpireLastModifiedUserId) OR
             (tbl_User.Username = tbl_ProductExpire.fld_ProductExpireLastModifiedUserId)), N'') AS Username
        FROM tbl_ProductExpire
        WHERE fld_ProductExpireProductSerial = @serial
        """;

        return dataAccess.SqlDataAdapter(command, new KeyValuePair<string, object>("serial", serial));
    }
    #endregion

    public List<string> SGetProductCreationRequiredFields()
    {
        List<string> result = new();

        string productCode = string.Empty;

        string productTitle = string.Empty;

        try
        {
            productCode = configuration["ProjectConfigs:WmsConfigs:CreateNewProductCode"].ToString();
        }
        catch
        {
            productCode = "";
        }
        try
        {
            productTitle = configuration["ProjectConfigs:WmsConfigs:CreateNewProductTitle"].ToString();
        }
        catch
        {
            productTitle = "";
        }

        if (productCode.HasNoValue())
        {
            result.Add("ProductCode");
        }

        if (productTitle.HasNoValue())
        {
            result.Add("ProductTitle");
        }

        return result;
    }

    public bool SIsTagProductCountEditable()
    => bool.Parse(configuration["ProjectConfigs:WmsConfigs:TagCountEditable"] ?? "false");
}
