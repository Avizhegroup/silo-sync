using Silo.Application.Contracts;
using Silo.Application.Dto.Filter;
using Silo.Application.Features;

namespace Silo.Api.Business;
public class InspectBusiness(ILogger<InspectBusiness> logger
        , IDataAccess dataAccess
        , IHttpContextAccessor httpContextAccessor
        ) : ProjectBusiness(dataAccess, logger, httpContextAccessor)
{
    public DataTable SInspectReportDynamic(List<ReportFilterGeneric<InspectReportDynamicFilterType>> filters
                                  , List<ReportColumnGeneric<InspectReportDynamicColumnsType>> selectColumns
                                  , List<ReportCalculatingColumn<InspectReportDynamicColumnsType>> calculating
                                  , ReportColumnGeneric<InspectReportDynamicColumnsType> pivot)
    {
        string userId = httpContext.User.GetUserId();

        List<string> tempTagFields = new();

        List<string> tempTagSelects = new();

        List<string> tagWheres = new();

        List<string> subSelects = new();

        List<string> subWheres = new();

        List<string> subGroups = new();

        List<string> mainSelects = new();

        List<string> mainWheres = new();

        List<string> mainGroups = new();

        var pivotColumnClause = "";

        var pivotFor = "";

        var pivotColumn = "";

        #region Where Tags
        foreach (var item in filters)
        {
            if (item.Value.HasNoValue())
            {
                continue;
            }

            switch (item.FieldType)
            {
                case InspectReportDynamicFilterType.ProductCode:
                    tagWheres.Add($"Tags.ProductCode IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.TechnicalCode:
                    tagWheres.Add($"Tags.RegCode IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.ProductSerial:
                    tagWheres.Add($"Tags.ProductSerial IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.Line:
                    tagWheres.Add($"Tags.fld_ProductPropertyAId IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.Shift:
                    tagWheres.Add($"Tags.fld_ProductPropertyBId IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.Size:
                    tagWheres.Add($"Tags.fld_ProductPropertyCId IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.Qc:
                    tagWheres.Add($"tbl_ProductStatus.ProductStatusCode IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.ProductBrand:
                    tagWheres.Add($"Tags.fld_ProductBrand IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.ProductGroup:
                    tagWheres.Add($"Tags.fld_ProductGroup IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.RegisterDevice:
                    tagWheres.Add($"Tags.DeviceId IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.InspectStatus:
                    switch (item.Value)
                    {
                        case "0":
                            tagWheres.Add($"Tags.Lock = 1 AND Tags.fld_LastInspectResult <> N'[]' ");
                            break;

                        case "1":
                            tagWheres.Add($"Tags.Lock = 0 AND Tags.fld_LastInspectResult <> N'[]' ");
                            break;

                        case "2":
                            tagWheres.Add($"Tags.Lock = 0 AND Tags.fld_LastInspectResult = N'[]' ");
                            break;
                    }
                    break;

                case InspectReportDynamicFilterType.User:
                    subWheres.Add($"tbl_User.Username IN ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.FromDate:
                    subWheres.Add($"tbl_Inspect.fld_InspectShamsiDate  >= ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.ToDate:
                    subWheres.Add($"tbl_Inspect.fld_InspectShamsiDate < ('{string.Join("','", item.Value.Split(','))}') ");
                    break;
                case InspectReportDynamicFilterType.InspectElements:
                    subWheres.Add($""" tbl_Inspect.fld_InspectElementsResults LIKE N'%"InspectElementId":{item.FieldName},"InspectElementValue":"{item.Value}"%' """);
                    break;
            }
        }
        #endregion

        #region Select
        tempTagFields.Add($"[Serial] nvarchar(40)");
        tempTagSelects.Add($"COALESCE(Tags.ProductSerial,N'مشخص نشده') AS [Serial]");

        tempTagFields.Add($"[ProductCount] decimal(18,2)");
        tempTagSelects.Add($"COALESCE(Tags.ProductCount,0) AS [ProductCount]");

        foreach (var select in selectColumns)
        {
            string subSelectClause = string.Empty;

            string subGroupClause = string.Empty;

            switch (select.Type)
            {
                case InspectReportDynamicColumnsType.ProductCode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(Tags.ProductCode,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.ProductName:
                    tempTagFields.Add($"[{select.Title}] nvarchar(MAX)");
                    tempTagSelects.Add($"COALESCE(Tags.ProductName,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.ProductSerial:
                    subSelectClause = $"[Serial] as [{select.Title}]";
                    subGroupClause = $"[Serial]";
                    break;
                case InspectReportDynamicColumnsType.ProductCount:
                    subSelectClause = $"[ProductCount] as [{select.Title}]";
                    subGroupClause = $"[ProductCount]";
                    break;
                case InspectReportDynamicColumnsType.Regcode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(Tags.RegCode,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.LineCode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductPropertyA.fld_ProductPropertyAId,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.LineTitle:
                    tempTagFields.Add($"[{select.Title}] nvarchar(256)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductPropertyA.fld_ProductPropertyATitle,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.SizeCode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductPropertyC.fld_ProductPropertyCId,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.SizeTitle:
                    tempTagFields.Add($"[{select.Title}] nvarchar(256)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductPropertyC.fld_ProductPropertyCTitle,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.QcCode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(256)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductStatus.ProductStatusCode,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.QcTitle:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductStatus.ProductStatusTitle,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.TypeCode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(256)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductType.ProductTypeCode,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.TypeTitle:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductType.ProductTypeTitle,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.GroupCode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(128)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductGroup.fld_ProductGroupCode,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.GroupTitle:
                    tempTagFields.Add($"[{select.Title}] nvarchar(128)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductGroup.fld_ProductGroupTitle,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.BrandCode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(128)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductBrand.fld_ProductBrandCode,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.BrandTitle:
                    tempTagFields.Add($"[{select.Title}] nvarchar(128)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductBrand.fld_ProductBrandTitle,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.DocCode:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(Tags.ContractStatus,N'مشخص نشده') AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.RegisterDevice:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add($"CASE Tags.DeviceId WHEN N'0' THEN N'کیوسک رجیستر' WHEN N'' THEN N'هند هلد' ELSE N'مشخص نشده' END AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.InspectStatus:
                    tempTagFields.Add($"[{select.Title}] nvarchar(50)");
                    tempTagSelects.Add(@$"CASE WHEN Tags.Lock = 0 AND Tags.fld_LastInspectResult = N'[]' THEN N'بازرسی نشده' 
                                        WHEN Tags.Lock = 1 AND Tags.fld_LastInspectResult <> N'[]' THEN N'مردود شده'
                                        WHEN Tags.Lock = 0 AND Tags.fld_LastInspectResult <> N'[]' THEN N'تائید شده' ELSE N'' END AS [{select.Title}]");
                    break;
                case InspectReportDynamicColumnsType.FreezeStatus:
                    break;


                case InspectReportDynamicColumnsType.InspectUser:
                    subSelectClause = $"COALESCE(tbl_User.Name,N'مشخص نشده') AS [{select.Title}]";
                    subGroupClause = $"tbl_User.Name";
                    break;
                case InspectReportDynamicColumnsType.PersianDateFull:
                    subSelectClause = $"tbl_Inspect.fld_InspectShamsiDate AS [{select.Title}]";
                    subGroupClause = $"tbl_Inspect.fld_InspectShamsiDate";
                    break;
                case InspectReportDynamicColumnsType.PersianDateYear:
                    subSelectClause = $"SUBSTRING(tbl_Inspect.fld_InspectShamsiDate, 0, 5)  AS [{select.Title}]";
                    subGroupClause = $"SUBSTRING(tbl_Inspect.fld_InspectShamsiDate, 0, 5)";
                    break;
                case InspectReportDynamicColumnsType.PersianDateMonth:
                    subSelectClause = $"SUBSTRING(tbl_Inspect.fld_InspectShamsiDate, 6, 2)  AS [{select.Title}]";
                    subGroupClause = $"SUBSTRING(tbl_Inspect.fld_InspectShamsiDate, 6, 2)";
                    break;
                case InspectReportDynamicColumnsType.PersianDateWeek:
                    break;
                case InspectReportDynamicColumnsType.GregorianDateFull:
                    subSelectClause = $"CAST(tbl_Inspect.fld_InspectDateTime AS DATE) AS [{select.Title}]";
                    subGroupClause = $"CAST(tbl_Inspect.fld_InspectDateTime AS DATE)";
                    break;
                case InspectReportDynamicColumnsType.GregorianDateYear:
                    subSelectClause = $"DATEPART(year,tbl_Inspect.fld_InspectDateTime) AS [{select.Title}]";
                    subGroupClause = $"DATEPART(year,tbl_Inspect.fld_InspectDateTime)";
                    break;
                case InspectReportDynamicColumnsType.GregorianDateMonth:
                    subSelectClause = $"DATEPART(month,tbl_Inspect.fld_InspectDateTime) AS [{select.Title}]";
                    subGroupClause = $"DATEPART(month,tbl_Inspect.fld_InspectDateTime)";
                    break;
                case InspectReportDynamicColumnsType.GregorianDateWeek:
                    break;
                case InspectReportDynamicColumnsType.InspectElement:
                    subSelectClause = @$"COALESCE (dbo.FindJsonObjectValue(dbo.tbl_Inspect.fld_InspectElementsResults,
                                       N'InspectElementId', N'{select.Value}', N'InspectElementValue'), '') AS [{select.Title}] ";
                    subGroupClause = $@"COALESCE (dbo.FindJsonObjectValue(dbo.tbl_Inspect.fld_InspectElementsResults,
                                       N'InspectElementId', N'{select.Value}', N'InspectElementValue'), '')";
                    break;
            }

            if (subSelectClause.HasValue())
            {
                subSelects.Add(subSelectClause);
            }
            else
            {
                subSelects.Add($"[{select.Title}]");
            }

            if (subGroupClause.HasValue())
            {
                subGroups.Add(subGroupClause);
            }
            else
            {
                subGroups.Add($"[{select.Title}]");
            }

            mainSelects.Add($"[{select.Title}]");
            mainGroups.Add($"[{select.Title}]");
            mainWheres.Add($"[{select.Title}] IS NOT NULL");
        }
        #endregion

        #region SELECT Pivot
        if (pivot is not null)
        {
            switch (pivot.Type)
            {
                case InspectReportDynamicColumnsType.QcTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].ProductStatusTitle + '],' FROM (SELECT  Distinct tbl_ProductStatus.ProductStatusTitle FROM tbl_ProductStatus) as [NESTED] ;";
                    pivotColumn = "[ProductStatusTitle]";
                    tempTagFields.Add($"[ProductStatusTitle] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductStatus.ProductStatusTitle,N'مشخص نشده') AS [ProductStatusTitle]");
                    subSelects.Add($"[ProductStatusTitle]");
                    subGroups.Add($"[ProductStatusTitle]");
                    mainSelects.Add($"[ProductStatusTitle] AS [{pivot.Title}]");
                    mainGroups.Add($"[ProductStatusTitle]");
                    break;

                case InspectReportDynamicColumnsType.BrandTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductBrandTitle + '],' FROM (SELECT  Distinct tbl_ProductBrand.fld_ProductBrandTitle FROM tbl_ProductBrand) as [NESTED] ;";
                    pivotColumn = "[fld_ProductBrandTitle]";
                    tempTagFields.Add($"[fld_ProductBrandTitle] nvarchar(128)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductBrand.fld_ProductBrandTitle,N'مشخص نشده') AS [fld_ProductBrandTitle]");
                    subSelects.Add($"[fld_ProductBrandTitle]");
                    subGroups.Add($"[fld_ProductBrandTitle]");
                    mainSelects.Add($"[fld_ProductBrandTitle] AS [{pivot.Title}]");
                    mainGroups.Add($"[fld_ProductBrandTitle]");
                    break;

                case InspectReportDynamicColumnsType.TypeTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].ProductTypeTitle + '],' FROM (SELECT  Distinct tbl_ProductType.ProductTypeTitle FROM tbl_ProductType) as [NESTED] ;";
                    pivotColumn = "[ProductTypeTitle]";
                    tempTagFields.Add($"[ProductTypeTitle] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductType.ProductTypeTitle,N'مشخص نشده') AS [ProductTypeTitle]");
                    subSelects.Add($"[ProductTypeTitle]");
                    subGroups.Add($"[ProductTypeTitle]");
                    mainSelects.Add($"[ProductTypeTitle] AS [{pivot.Title}]");
                    mainGroups.Add($"[ProductTypeTitle]");
                    break;

                case InspectReportDynamicColumnsType.GroupTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductGroupTitle + '],' FROM (SELECT  Distinct tbl_ProductGroup.fld_ProductGroupTitle FROM tbl_ProductGroup) as [NESTED] ;";
                    pivotColumn = "[fld_ProductGroupTitle]";
                    tempTagFields.Add($"[fld_ProductGroupTitle] nvarchar(128)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductGroup.fld_ProductGroupCode,N'مشخص نشده') AS [fld_ProductGroupTitle]");
                    subSelects.Add($"[fld_ProductGroupTitle]");
                    subGroups.Add($"[fld_ProductGroupTitle]");
                    mainSelects.Add($"[fld_ProductGroupTitle] AS [{pivot.Title}]");
                    mainGroups.Add($"[fld_ProductGroupTitle]");
                    break;

                case InspectReportDynamicColumnsType.SizeTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductPropertyCTitle + '],' FROM (SELECT  Distinct tbl_ProductPropertyC.fld_ProductPropertyCTitle FROM tbl_ProductPropertyC) as [NESTED] ;";
                    pivotColumn = "[fld_ProductPropertyCTitle]";
                    tempTagFields.Add($"[fld_ProductPropertyCTitle] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductPropertyC.fld_ProductPropertyCId,N'مشخص نشده') AS [fld_ProductPropertyCTitle]");
                    subSelects.Add($"[fld_ProductPropertyCTitle]");
                    subGroups.Add($"[fld_ProductPropertyCTitle]");
                    mainSelects.Add($"[fld_ProductPropertyCTitle] AS [{pivot.Title}]");
                    mainGroups.Add($"[fld_ProductPropertyCTitle]");
                    break;

                case InspectReportDynamicColumnsType.LineTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductPropertyATitle + '],' FROM (SELECT  Distinct tbl_ProductPropertyA.fld_ProductPropertyATitle FROM tbl_ProductPropertyA) as [NESTED] ;";
                    pivotColumn = "[fld_ProductPropertyATitle]";
                    tempTagFields.Add($"[fld_ProductPropertyATitle] nvarchar(50)");
                    tempTagSelects.Add($"COALESCE(tbl_ProductPropertyA.fld_ProductPropertyAId,N'مشخص نشده') AS [fld_ProductPropertyATitle]");
                    subSelects.Add($"[fld_ProductPropertyATitle]");
                    subGroups.Add($"[fld_ProductPropertyATitle]");
                    mainSelects.Add($"[fld_ProductPropertyATitle] AS [{pivot.Title}]");
                    mainGroups.Add($"[fld_ProductPropertyATitle]");
                    break;

                case InspectReportDynamicColumnsType.InspectElement:
                    pivotColumnClause = @$"Select @columns +=  '['+ [NESTED].Expr1 + '],' FROM (SELECT DISTINCT tbl_InspectResult.fld_InspectResultValues AS Expr1
                                                                                                                    FROM  tbl_InspectResult LEFT OUTER JOIN
                                                                                                                          tbl_InspectElement ON tbl_InspectResult.fld_InspectResultInspectElementId = tbl_InspectElement.fld_InspectElementId
                                                                                                                    WHERE (tbl_InspectResult.fld_InspectResultInspectElementId = '{pivot.Value}')) as [NESTED] ;";
                    pivotColumn = "[InspectElement]";
                    subSelects.Add(@$"COALESCE (dbo.FindJsonObjectValue(dbo.tbl_Inspect.fld_InspectElementsResults,
                                       N'InspectElementId', N'{pivot.Value}', N'InspectElementValue'), '') AS [InspectElement] ");
                    subGroups.Add($@"COALESCE (dbo.FindJsonObjectValue(dbo.tbl_Inspect.fld_InspectElementsResults,
                                       N'InspectElementId', N'{pivot.Value}', N'InspectElementValue'), '')");
                    mainGroups.Add($"[InspectElement]");
                    break;
            }

            pivotFor = "[NestedPivotData].[Qc]";

            subSelects.Add($"SUM([ProductCount]) AS [ProductCount]");
        }
        #endregion

        #region Calculation
        foreach (var item in calculating)
        {
            switch (item.GroupColumnType)
            {
                case InspectReportDynamicColumnsType.ProductSerial:
                    subSelects.Add($"{item.Type.ToString()}(tbl_Inspect.fld_InspectId) AS [{item.Title}]");
                    mainSelects.Add($"SUM([{item.Title}]) AS [{item.Title}]");
                    break;

                case InspectReportDynamicColumnsType.ProductCount:
                    if (item.Type != ReportCalculatingColumnType.Percent)
                    {
                        subSelects.Add($"{item.Type.ToString()}([ProductCount]) AS [{item.Title}]");
                        mainSelects.Add($"SUM([{item.Title}]) AS [{item.Title}]");
                    }
                    else
                    {
                        string percentColumn = $"""
                            CAST(Sum([ProductCount])*100/(SELECT SUM(Taggs.ProductCount) 
                            FROM  tbl_Inspect as Inspect LEFT OUTER JOIN tbl_User ON Inspect.fld_InspectUser = tbl_User.Id LEFT OUTER JOIN 
                            @Temp as Taggs ON [Serial] = Inspect.fld_InspectSerial) AS decimal(16,2)) AS [{item.Title}]
                            {(subWheres.Any() ? "WHERE " + string.Join(" AND ", subWheres.Select(p=>p.Replace("tbl_Inspect", "Inspect"))) : "")}
                            """;

                        subSelects.Add(percentColumn);
                        mainSelects.Add($"SUM([{item.Title}]) AS {item.Title}");
                    }
                    break;

                case InspectReportDynamicColumnsType.InspectDate:
                    subSelects.Add($"{item.Type.ToString()}(tbl_Inspect.fld_InspectShamsiDate) AS [{item.Title}]");
                    subGroups.Add($"tbl_Inspect.fld_InspectShamsiDate");
                    mainSelects.Add($"[{item.Title}]");
                    mainGroups.Add($"[{item.Title}]");
                    break;
            }

        }
        subSelects.Add($"SUM([ProductCount]) AS [ProductCount2]");
        #endregion

        string tempTagCommand = string.Empty;

        string mainCommand = string.Empty;

        tempTagCommand = $"""
            DECLARE @Temp TABLE({(tempTagFields.Any() ? string.Join(" , ", tempTagFields) : "")})

            INSERT INTO @Temp
            SELECT {(tempTagSelects.Any() ? string.Join(" , ", tempTagSelects) : "")}
            FROM   tbl_Tags AS Tags LEFT OUTER JOIN
            tbl_ProductGroup ON Tags.fld_ProductGroup = tbl_ProductGroup.fld_ProductGroupCode LEFT OUTER JOIN
            tbl_ProductBrand ON Tags.fld_ProductBrand = tbl_ProductBrand.fld_ProductBrandCode LEFT OUTER JOIN
            tbl_ProductStatus ON Tags.ProductStatus = tbl_ProductStatus.ProductStatusCode LEFT OUTER JOIN
            tbl_ProductType ON Tags.ProductType = tbl_ProductType.ProductTypeCode LEFT OUTER JOIN
            tbl_ProductPropertyC ON Tags.fld_ProductPropertyCId = tbl_ProductPropertyC.fld_ProductPropertyCId LEFT OUTER JOIN
            tbl_ProductPropertyB ON Tags.fld_ProductPropertyBId = tbl_ProductPropertyB.fld_ProductPropertyBId LEFT OUTER JOIN
            tbl_ProductPropertyA ON Tags.fld_ProductPropertyAId = tbl_ProductPropertyA.fld_ProductPropertyAId
            {(tagWheres.Any() ? "WHERE " + string.Join(" AND ", tagWheres) : "")}
            """;

        if (pivot is null)
        {
            mainCommand = $"""
                {tempTagCommand}
                SELECT  {string.Join(" , ", mainSelects)}
                FROM(
                       SELECT  {string.Join(" , ", subSelects)}
            	       FROM  tbl_Inspect LEFT OUTER JOIN tbl_User ON tbl_Inspect.fld_InspectUser = tbl_User.Id LEFT OUTER JOIN 
                             @Temp ON [Serial] = fld_InspectSerial
                       {(subWheres.Any() ? "WHERE " + string.Join(" AND ", subWheres) : "")}
                       {(subGroups.Any() ? "GROUP BY " + string.Join(" , ", subGroups) : "")}
                ) AS [NESTED]
                {(mainWheres.Any() ? "WHERE " + string.Join(" AND ", mainWheres) : "")}
                {(mainGroups.Any() ? "GROUP BY " + string.Join(" , ", mainGroups) : "")}
            """;
        }
        else
        {
            mainCommand = $"""
                declare @columns nvarchar(max) = '', @sqlcmd    NVARCHAR(MAX) = '';
                {pivotColumnClause}
                IF @columns = ''
                BEGIN 
                RETURN
                END
                SET @columns = LEFT(@columns, LEN(@columns) - 1); 
                SET @sqlcmd = N'
                {tempTagCommand.Replace("'", "''")}
                SELECT * FROM
                (
                    SELECT SUM([ProductCount]) as [Count],{pivotColumn.Replace("'", "''")} AS [Qc],
                            {string.Join(" , ", mainSelects.Select(p => p.Replace("'", "''")))}
                    FROM(
                            SELECT  {string.Join(" , ", subSelects.Select(p => p.Replace("'", "''")))}
                	        FROM  tbl_Inspect LEFT OUTER JOIN tbl_User ON tbl_Inspect.fld_InspectUser = tbl_User.Id LEFT OUTER JOIN 
                                    @Temp ON [Serial] = fld_InspectSerial
                            {(subWheres.Any() ? "WHERE " + string.Join(" AND ", subWheres.Select(p => p.Replace("'", "''"))) : "")}
                            {(subGroups.Any() ? "GROUP BY " + string.Join(" , ", subGroups.Select(p => p.Replace("'", "''"))) : "")}
                    ) AS [NESTED]
                    {(mainWheres.Any() ? "WHERE " + string.Join(" AND ", mainWheres.Select(p => p.Replace("'", "''"))) : "")}
                    {(mainGroups.Any() ? "GROUP BY " + string.Join(" , ", mainGroups.Select(p => p.Replace("'", "''"))) : "")}
                ) AS [NestedPivotData]
                pivot (SUM([NestedPivotData].[Count])
                FOR {pivotFor} IN ('+@columns+') ) as PivotData ';
                EXECUTE sp_executesql @sqlcmd;
                """;
        }

        var dt = dataAccess.SqlDataAdapter(mainCommand);

        return dt;
    }
}
