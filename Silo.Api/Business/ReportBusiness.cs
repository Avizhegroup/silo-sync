using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Silo.Api.Hubs;
using Silo.Api.Services;
using Silo.Api.Tools;
using Silo.Application;
using Silo.Application.Contracts;
using Silo.Application.Dto.Filter;
using Silo.Domains.Services;

namespace Silo.Api.Business;
public class ReportBusiness(ILogger<ReportBusiness> logger
        , IDataAccess dataAccess
        , IConfiguration configuration
        , WmsApiContext apiContext
        , IWmsBusiness wmsBusiness
        , IMapper mapper
        , IHttpContextAccessor httpContextAccessor
        , IHubContext<WmsHub> wmsHub
        , SmsHttpClient smsClient) : ProjectBusiness(dataAccess, logger, httpContextAccessor)
{
    public DataTable SRepRegisterTagSummary(List<ReportFilterGeneric<RegisterReportDynamicFilterType>> filters
                                          , List<ReportColumnGeneric<RegisterReportDynamicColumnsType>> selectColumns
                                          , List<ReportCalculatingColumn<RegisterReportDynamicColumnsType>> calculating
                                          , ReportColumnGeneric<RegisterReportDynamicColumnsType> pivot
                                          , List<ReportColumnGeneric<ExitActionDynamicReportColumnsType>> dataMiningElements)
    {
        List<string> selects = new();
        List<string> groups = new();
        List<string> wheres = new();
        List<string> orders = new();

        List<DataElement> dataMiningElementDtos = new();

        if (dataMiningElements.Any())
        {
            dataMiningElementDtos = wmsBusiness.SGetDataMiningElementsByIds(dataMiningElements.Select(p => p.Value).ToList());
        }

        #region Where
        foreach (var filter in filters)
        {
            if (filter.SqlWhereCommand.HasValue())
            {
                continue;
            }

            if (filter.Type.Equals(FilterType.Dynamic))
            {
                filter.SqlWhereCommand = $"JSON_VALUE(Tags.ProductProperties,N'$.\"{filter.FieldName}\"')";

                wheres.Add(DynamicFilteringTools.GetDynamicWhere(filter));
            }
            else if (filter.Type.Equals(FilterType.Static))
            {
                switch (filter.FieldType)
                {
                    case RegisterReportDynamicFilterType.ProductCode:
                        filter.SqlWhereCommand = "Tags.ProductCode";
                        break;

                    case RegisterReportDynamicFilterType.Qc:
                        filter.SqlWhereCommand = "Tags.ProductStatus";
                        break;

                    case RegisterReportDynamicFilterType.User:
                        filter.SqlWhereCommand = "Tags.TagRegisterUser";
                        break;

                    case RegisterReportDynamicFilterType.Shift:
                        filter.SqlWhereCommand = "Tags.fld_ProductPropertyBId";
                        break;

                    case RegisterReportDynamicFilterType.Size:
                        filter.SqlWhereCommand = $"Tags.fld_ProductPropertyCId";
                        break;

                    case RegisterReportDynamicFilterType.TechnicalCode:
                        filter.SqlWhereCommand = "Tags.RegCode";
                        break;

                    case RegisterReportDynamicFilterType.ProductSerial:
                        filter.SqlWhereCommand = "Tags.ProductSerial";
                        break;

                    case RegisterReportDynamicFilterType.FromDate:
                        filter.SqlWhereCommand = "Tags.TagRegisterShamsiUnixDate";
                        filter.Values = filter.Values.Select(date => $"{(date.Replace("/", ""))}000000").ToList();
                        break;

                    case RegisterReportDynamicFilterType.ToDate:
                        filter.SqlWhereCommand = "Tags.TagRegisterShamsiUnixDate";
                        filter.Values = filter.Values.Select(date => $"{(date.Replace("/", ""))}000000").ToList();
                        break;

                    case RegisterReportDynamicFilterType.ProductBrand:
                        filter.SqlWhereCommand = $"Tags.fld_ProductBrand";
                        break;

                    case RegisterReportDynamicFilterType.ProductGroup:
                        filter.SqlWhereCommand = $"Tags.fld_ProductGroup";
                        break;

                    case RegisterReportDynamicFilterType.ProductSubGroup:
                        filter.SqlWhereCommand = $"Tags.fld_ProductSubGroup";
                        break;

                    case RegisterReportDynamicFilterType.Line:
                        filter.SqlWhereCommand = "Tags.fld_ProductPropertyAId";
                        break;

                    case RegisterReportDynamicFilterType.RegisterDevice:
                        filter.SqlWhereCommand = $"Tags.DeviceId";
                        break;

                    case RegisterReportDynamicFilterType.Warehouse:
                        filter.SqlWhereCommand = $"Tags.TagInDestinationId";
                        break;

                    case RegisterReportDynamicFilterType.InspectStatus:
                        switch (filter.Values.First())
                        {
                            case "0":
                                wheres.Add("(Tags.Lock = 1 AND Tags.fld_LastInspectResult <> N'[]' )");
                                continue;

                            case "1":
                                wheres.Add("(Tags.Lock = 0 AND Tags.fld_LastInspectResult <> N'[]' )");
                                continue;

                            case "2":
                                wheres.Add("(Tags.Lock = 0 AND Tags.fld_LastInspectResult = N'[]')");
                                continue;
                        }
                        break;
                }

                if (filter.SqlWhereCommand.HasValue())
                {
                    wheres.Add(DynamicFilteringTools.GetStaticWhere(filter));
                }
            }
        }
        #endregion

        #region Select columns
        foreach (var select in selectColumns)
        {
            string groupClause = string.Empty;

            switch (select.Type)
            {
                case RegisterReportDynamicColumnsType.LineCode:
                    selects.Add($"COALESCE(tbl_ProductPropertyA.fld_ProductPropertyAId,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductPropertyA.fld_ProductPropertyAId";
                    break;
                case RegisterReportDynamicColumnsType.LineTitle:
                    selects.Add($"COALESCE(tbl_ProductPropertyA.fld_ProductPropertyATitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductPropertyA.fld_ProductPropertyATitle";
                    break;
                case RegisterReportDynamicColumnsType.ShiftCode:
                    selects.Add($"COALESCE(tbl_ProductPropertyB.fld_ProductPropertyBId,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductPropertyB.fld_ProductPropertyBId";
                    break;
                case RegisterReportDynamicColumnsType.ShiftTitle:
                    selects.Add($"COALESCE(tbl_ProductPropertyB.fld_ProductPropertyBTitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductPropertyB.fld_ProductPropertyBTitle";
                    break;
                case RegisterReportDynamicColumnsType.ProductCode:
                    selects.Add($"COALESCE(Tags.ProductCode,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "Tags.ProductCode";
                    break;
                case RegisterReportDynamicColumnsType.ProductName:
                    selects.Add($"COALESCE(Tags.ProductName,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "Tags.ProductName";
                    break;
                case RegisterReportDynamicColumnsType.ProductSerial:
                    selects.Add($"COALESCE(Tags.ProductSerial,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "Tags.ProductSerial";
                    break;
                case RegisterReportDynamicColumnsType.Regcode:
                    selects.Add($"COALESCE(Tags.RegCode,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "Tags.RegCode";
                    break;
                case RegisterReportDynamicColumnsType.SizeCode:
                    selects.Add($"COALESCE(tbl_ProductPropertyC.fld_ProductPropertyCId,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductPropertyC.fld_ProductPropertyCId";
                    break;
                case RegisterReportDynamicColumnsType.SizeTitle:
                    selects.Add($"COALESCE(tbl_ProductPropertyC.fld_ProductPropertyCTitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductPropertyC.fld_ProductPropertyCTitle";
                    break;
                case RegisterReportDynamicColumnsType.QcCode:
                    selects.Add($"COALESCE(tbl_ProductStatus.ProductStatusCode,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductStatus.ProductStatusCode";
                    break;
                case RegisterReportDynamicColumnsType.QcTitle:
                    selects.Add($"COALESCE(tbl_ProductStatus.ProductStatusTitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductStatus.ProductStatusTitle";
                    break;
                case RegisterReportDynamicColumnsType.DocCode:
                    selects.Add($"COALESCE(Tags.ContractStatus,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "Tags.ContractStatus";
                    break;
                case RegisterReportDynamicColumnsType.RegisterDevice:
                    selects.Add($"CASE Tags.DeviceId WHEN N'0' THEN N'کیوسک رجیستر' WHEN N'' THEN N'هند هلد' ELSE N'مشخص نشده' END AS [{select.Title}]");
                    groupClause = "Tags.DeviceId";
                    break;
                case RegisterReportDynamicColumnsType.GroupCode:
                    selects.Add($"COALESCE(tbl_ProductGroup.fld_ProductGroupCode,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductGroup.fld_ProductGroupCode";
                    break;
                case RegisterReportDynamicColumnsType.GroupTitle:
                    selects.Add($"COALESCE(tbl_ProductGroup.fld_ProductGroupTitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductGroup.fld_ProductGroupTitle";
                    break;
                case RegisterReportDynamicColumnsType.SubGroupCode:
                    selects.Add($"COALESCE(tbl_ProductSubGroup.fld_ProductSubGroupCode,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductSubGroup.fld_ProductSubGroupCode";
                    break;
                case RegisterReportDynamicColumnsType.SubGroupTitle:
                    selects.Add($"COALESCE(tbl_ProductSubGroup.fld_ProductSubGroupTitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductSubGroup.fld_ProductSubGroupTitle";
                    break;
                case RegisterReportDynamicColumnsType.BrandCode:
                    selects.Add($"COALESCE(tbl_ProductBrand.fld_ProductBrandCode,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductBrand.fld_ProductBrandCode";
                    break;
                case RegisterReportDynamicColumnsType.BrandTitle:
                    selects.Add($"COALESCE(tbl_ProductBrand.fld_ProductBrandTitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductBrand.fld_ProductBrandTitle";
                    break;
                case RegisterReportDynamicColumnsType.TypeCode:
                    selects.Add($"COALESCE(tbl_ProductType.ProductTypeCode,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductType.ProductTypeCode";
                    break;
                case RegisterReportDynamicColumnsType.TypeTitle:
                    selects.Add($"COALESCE(tbl_ProductType.ProductTypeTitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_ProductType.ProductTypeTitle";
                    break;
                case RegisterReportDynamicColumnsType.InspectStatus:
                    selects.Add(@$"CASE WHEN Tags.Lock = 0 AND Tags.fld_LastInspectResult = N'[]' THEN N'بازرسی نشده' 
                                        WHEN Tags.Lock = 1 AND Tags.fld_LastInspectResult <> N'[]' THEN N'مردود شده'
                                        WHEN Tags.Lock = 0 AND Tags.fld_LastInspectResult <> N'[]' THEN N'تائید شده' ELSE N'' END AS [{select.Title}]");
                    groupClause = "CASE WHEN Tags.Lock = 0 AND Tags.fld_LastInspectResult = N'[]' THEN N'بازرسی نشده' \r\n                                        WHEN Tags.Lock = 1 AND Tags.fld_LastInspectResult <> N'[]' THEN N'مردود شده'\r\n                                        WHEN Tags.Lock = 0 AND Tags.fld_LastInspectResult <> N'[]' THEN N'تائید شده' ELSE N'' END";
                    break;
                case RegisterReportDynamicColumnsType.RegisterUser:
                    selects.Add($"COALESCE((SELECT CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER, Tags.TagRegisterUser) IS NOT NULL THEN (SELECT InnerUser.[Name] From tbl_User as InnerUser WHERE InnerUser.Id = Tags.TagRegisterUser) ELSE Tags.TagRegisterUser END ),N'مشخص نشده')AS [{select.Title}]");
                    groupClause = "Tags.TagRegisterUser";
                    break;
                case RegisterReportDynamicColumnsType.PersianDateFull:
                    selects.Add($"SUBSTRING(Tags.TagRegisterShamsiUnixDate, 0, 5) + '/' + SUBSTRING(Tags.TagRegisterShamsiUnixDate, 5, 2) + '/' + SUBSTRING(Tags.TagRegisterShamsiUnixDate, 7, 2) AS [{select.Title}]");
                    groupClause = "SUBSTRING(Tags.TagRegisterShamsiUnixDate, 0, 5) + '/' + SUBSTRING(Tags.TagRegisterShamsiUnixDate, 5, 2) + '/' + SUBSTRING(Tags.TagRegisterShamsiUnixDate, 7, 2)";
                    break;
                case RegisterReportDynamicColumnsType.PersianDateYear:
                    selects.Add($"SUBSTRING(Tags.TagRegisterShamsiUnixDate, 0, 5)  AS [{select.Title}]");
                    groupClause = "SUBSTRING(Tags.TagRegisterShamsiUnixDate, 0, 5)";
                    break;
                case RegisterReportDynamicColumnsType.PersianDateMonth:
                    selects.Add($"SUBSTRING(Tags.TagRegisterShamsiUnixDate, 5, 2)  AS [{select.Title}]");
                    groupClause = "SUBSTRING(Tags.TagRegisterShamsiUnixDate, 5, 2)";
                    break;
                case RegisterReportDynamicColumnsType.PersianDateWeek:
                    break;
                case RegisterReportDynamicColumnsType.GregorianDateFull:
                    selects.Add($"CAST(Tags.TagRegisterDateTime AS DATE) AS [{select.Title}]");
                    groupClause = "CAST(Tags.TagRegisterDateTime AS DATE)";
                    break;
                case RegisterReportDynamicColumnsType.GregorianDateYear:
                    selects.Add($"DATEPART(year,Tags.TagRegisterDateTime) AS [{select.Title}]");
                    groupClause = "DATEPART(year,Tags.TagRegisterDateTime)";
                    break;
                case RegisterReportDynamicColumnsType.GregorianDateMonth:
                    selects.Add($"DATEPART(month,Tags.TagRegisterDateTime) AS [{select.Title}]");
                    groupClause = "DATEPART(month,Tags.TagRegisterDateTime)";
                    break;
                case RegisterReportDynamicColumnsType.GregorianDateWeek:
                    break;
                case RegisterReportDynamicColumnsType.DynamicFields:
                    selects.Add($"JSON_VALUE(Tags.ProductProperties,N'$.\"{select.Title}\"') AS [{select.Title}]");
                    groupClause = $"JSON_VALUE(Tags.ProductProperties,N'$.\"{select.Title}\"')";
                    break;
                case RegisterReportDynamicColumnsType.WarehouseCode:
                    selects.Add($"COALESCE(tbl_Destination.DestinationCode,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_Destination.DestinationCode";
                    break;
                case RegisterReportDynamicColumnsType.WarehouseTitle:
                    selects.Add($"COALESCE(tbl_Destination.DestinationTitle,N'مشخص نشده') AS [{select.Title}]");
                    groupClause = "tbl_Destination.DestinationTitle";
                    break;

            }

            if (calculating.Any() || pivot is not null)
            {
                groups.Add(groupClause);
            }

            if (select.SortType != ReportColumnSortType.None)
            {
                orders.Add($" [{select.Title}] {(select.SortType == ReportColumnSortType.Asc ? "ASC" : "DESC")}");
            }
        }
        #endregion

        #region Calculation
        foreach (var item in calculating)
        {
            switch (item.GroupColumnType)
            {
                case RegisterReportDynamicColumnsType.ProductSerial:
                    selects.Add($"{item.Type.ToString()}(Tags.ProductSerial) AS [{item.Title}]");
                    break;

                case RegisterReportDynamicColumnsType.ProductCount:
                    if (item.Type != ReportCalculatingColumnType.Percent)
                    {
                        selects.Add($"{item.Type.ToString()}(Tags.ProductCount) AS [{item.Title}]");
                    }
                    else
                    {
                        string percentColumn = $"""
                            CAST(((SUM(Tags.ProductCount) * 100/(SELECT SUM(InnerTags.ProductCount) 
                            FROM tbl_Tags as InnerTags
                            {(wheres.Any() ? " WHERE " + string.Join(" AND ", wheres.Select(p => p.Replace("Tags", "InnerTags"))) : "")}
                            ))) AS decimal(16,2)) AS [{item.Title}]
                            """;

                        selects.Add(percentColumn);
                    }
                    break;

                case RegisterReportDynamicColumnsType.PersianDateFull:
                    selects.Add($"{item.Type.ToString()}(SUBSTRING(Tags.TagRegisterShamsiUnixDate, 0, 5) + '/' + SUBSTRING(Tags.TagRegisterShamsiUnixDate, 5, 2) + '/' + SUBSTRING(Tags.TagRegisterShamsiUnixDate, 7, 2)) AS [{item.Title}]");
                    break;

                case RegisterReportDynamicColumnsType.DynamicFields:
                    selects.Add($"{item.Type.ToString()}(TRY_CAST(JSON_VALUE(Tags.ProductProperties,N'$.\"{item.FieldName}\"') AS decimal(18,2))) AS [{item.Title}]");
                    break;
            }
        }
        #endregion

        #region Data Mining Elements
        foreach (var element in dataMiningElementDtos)
        {
            string selectTitle = element.DataMiningElementsTitle;

            string elementCommand = ReplaceDataMiningElementParameters(element.DataMiningElementsCommand);

            selects.Add($"{elementCommand} AS [{selectTitle}]");
        }
        #endregion

        string command = string.Empty;

        if (pivot is null)
        {
            command = $"""
            SELECT {string.Join(",", selects)}
            FROM tbl_Tags AS Tags LEFT OUTER JOIN
            tbl_ProductPropertyA ON Tags.fld_ProductPropertyAId = tbl_ProductPropertyA.fld_ProductPropertyAId LEFT OUTER JOIN
            tbl_ProductPropertyB ON Tags.fld_ProductPropertyBId = tbl_ProductPropertyB.fld_ProductPropertyBId LEFT OUTER JOIN
            tbl_ProductPropertyC ON Tags.fld_ProductPropertyCId = tbl_ProductPropertyC.fld_ProductPropertyCId LEFT OUTER JOIN
            tbl_Products ON Tags.ProductCode = tbl_Products.ProductCode LEFT OUTER JOIN
            tbl_ProductStatus ON Tags.ProductStatus = tbl_ProductStatus.ProductStatusCode LEFT OUTER JOIN
            tbl_ProductType ON Tags.ProductType = tbl_ProductType.ProductTypeCode LEFT OUTER JOIN
            tbl_ProductGroup ON Tags.fld_ProductGroup = tbl_ProductGroup.fld_ProductGroupCode LEFT OUTER JOIN
            tbl_ProductSubGroup ON Tags.fld_ProductSubGroup = tbl_ProductSubGroup.fld_ProductSubGroupCode LEFT OUTER JOIN
            tbl_ProductBrand ON Tags.fld_ProductBrand = tbl_ProductBrand.fld_ProductBrandCode LEFT OUTER JOIN
            tbl_Destination ON Tags.TagInDestinationId = tbl_Destination.DestinationCode
            {(wheres.Any() ? "WHERE " + string.Join(" AND ", wheres) : "")}
            {(groups.Any() ? "GROUP BY " + string.Join(" , ", groups) : "")}
            {(orders.Any() ? "ORDER BY " + string.Join(" , ", orders) : "")}
            """;
        }
        else
        {
            var pivotColumnClause = "";
            var pivotFor = "";
            var pivotColumn = "";

            switch (pivot.Type)
            {
                case RegisterReportDynamicColumnsType.QcTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].ProductStatusTitle + '],' FROM (SELECT  Distinct tbl_ProductStatus.ProductStatusTitle FROM tbl_ProductStatus) as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_ProductStatus.ProductStatusTitle";
                    break;

                case RegisterReportDynamicColumnsType.BrandTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductBrandTitle + '],' FROM (SELECT  Distinct tbl_ProductBrand.fld_ProductBrandTitle FROM tbl_ProductBrand) as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_ProductBrand.fld_ProductBrandTitle";
                    break;

                case RegisterReportDynamicColumnsType.TypeTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].ProductTypeTitle + '],' FROM (SELECT  Distinct tbl_ProductType.ProductTypeTitle FROM tbl_ProductType) as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_ProductType.ProductTypeTitle";
                    break;

                case RegisterReportDynamicColumnsType.GroupTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductGroupTitle + '],' FROM (SELECT  Distinct tbl_ProductGroup.fld_ProductGroupTitle FROM tbl_ProductGroup) as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_ProductGroup.fld_ProductGroupTitle";
                    break;

                case RegisterReportDynamicColumnsType.SizeTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductPropertyCTitle + '],' FROM (SELECT  Distinct tbl_ProductPropertyC.fld_ProductPropertyCTitle FROM tbl_ProductPropertyC) as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_ProductPropertyC.fld_ProductPropertyCTitle";
                    break;

                case RegisterReportDynamicColumnsType.LineTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductPropertyATitle + '],' FROM (SELECT  Distinct tbl_ProductPropertyA.fld_ProductPropertyATitle FROM tbl_ProductPropertyA) as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_ProductPropertyA.fld_ProductPropertyATitle";
                    break;

                case RegisterReportDynamicColumnsType.ShiftTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductPropertyBTitle + '],' FROM (SELECT  Distinct tbl_ProductPropertyB.fld_ProductPropertyBTitle FROM tbl_ProductPropertyB) as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_ProductPropertyB.fld_ProductPropertyBTitle";
                    break;
            }

            groups.Add(pivotColumn);

            command = $"""
                declare @columns nvarchar(max) = '', @sqlcmd    NVARCHAR(MAX) = '';
                {pivotColumnClause}
                IF @columns = ''
                BEGIN 
                RETURN
                END
                SET @columns = LEFT(@columns, LEN(@columns) - 1); 
                SET @sqlcmd = N'
                SELECT * FROM
                (SELECT SUM(Tags.ProductCount) as [Count],{pivotColumn} AS [Qc],{string.Join(",", selects.Select(p => p.Replace("'", "''")))}
                FROM tbl_Tags AS Tags LEFT OUTER JOIN
                tbl_ProductPropertyA ON Tags.fld_ProductPropertyAId = tbl_ProductPropertyA.fld_ProductPropertyAId LEFT OUTER JOIN
                tbl_ProductPropertyB ON Tags.fld_ProductPropertyBId = tbl_ProductPropertyB.fld_ProductPropertyBId LEFT OUTER JOIN
                tbl_ProductPropertyC ON Tags.fld_ProductPropertyCId = tbl_ProductPropertyC.fld_ProductPropertyCId LEFT OUTER JOIN
                tbl_Products ON Tags.ProductCode = tbl_Products.ProductCode LEFT OUTER JOIN
                tbl_ProductStatus ON Tags.ProductStatus = tbl_ProductStatus.ProductStatusCode LEFT OUTER JOIN
                tbl_ProductType ON Tags.ProductType = tbl_ProductType.ProductTypeCode LEFT OUTER JOIN
                tbl_ProductGroup ON Tags.fld_ProductGroup = tbl_ProductGroup.fld_ProductGroupCode LEFT OUTER JOIN
                tbl_ProductSubGroup ON Tags.fld_ProductSubGroup = tbl_ProductSubGroup.fld_ProductSubGroupCode LEFT OUTER JOIN
                tbl_ProductBrand ON Tags.fld_ProductBrand = tbl_ProductBrand.fld_ProductBrandCode LEFT OUTER JOIN
                tbl_Destination ON Tags.TagInDestinationId = tbl_Destination.DestinationCode
                {(wheres.Any() ? "WHERE " + string.Join(" AND ", wheres.Select(p => p.Replace("'", "''"))) : "")}
                {(groups.Any() ? "GROUP BY " + string.Join(" , ", groups.Select(p => p.Replace("'", "''"))) : "")}) AS [Nested]
                pivot (SUM([Nested].[Count])
                FOR {pivotFor} IN ('+@columns+') ) as PivotData ';
                EXECUTE sp_executesql @sqlcmd;
                """;
        }

        var dt = dataAccess.SqlDataAdapter(command);

        return dt;

        string ReplaceDataMiningElementParameters(string command)
        {
            var parameters = new Dictionary<string, string>
            {
                { "@WhereProductSerialDME", " (tbl_TagsDME.ProductSerial = Tags.ProductSerial) " } ,
                { "@WhereProductCodeDME"," (tbl_ProductsDME.ProductCode = tbl_Products.ProductCode) " } ,
            };

            foreach (var parameter in parameters)
            {
                command = command.Replace(parameter.Key, parameter.Value);
            }

            return command;
        }
    }

    public DataTable SReportExitAction(List<ReportFilterGeneric<ExitActionDynamicReportFilterType>> filters
                                              , List<ReportColumnGeneric<ExitActionDynamicReportColumnsType>> selectColumns
                                          , List<ReportCalculatingColumn<ExitActionDynamicReportColumnsType>> calculating
                                          , ReportColumnGeneric<ExitActionDynamicReportColumnsType> pivot
                                          , List<ReportColumnGeneric<ExitActionDynamicReportColumnsType>> dataMiningElements)
    {
        string[] StartShiftTime = new string[3];
        DataTable shiftList = wmsBusiness.SPGetShiftStartENDList();
        foreach (DataRow dr in shiftList.Rows)
        {
            string Temp = dr["fld_ProductPropertyBDesc"].ToString().Split('-')[0].Split(':')[0] + dr["fld_ProductPropertyBDesc"].ToString().Split('-')[0].Split(':')[1] + "01";
            StartShiftTime[Convert.ToInt32(dr["fld_ProductPropertyBId"].ToString()) - 1] = Temp;
        }

        string recordCount = string.Empty;

        List<string> selects = new();
        List<string> groups = new();
        List<string> wheres = new();
        List<string> subWheres = new();
        List<string> subWhereTotalSum = new();
        List<string> orders = new();

        bool isTagsRequired = false;

        List<DataElement> dataMiningElementDtos = new();

        if (dataMiningElements.Any())
        {
            dataMiningElementDtos = wmsBusiness.SGetDataMiningElementsByIds(dataMiningElements.Select(p => p.Value).ToList());
        }

        HandleWheres();

        HandleSelect();

        HandleCalculating();

        HandleDmes();

        string command = string.Empty;

        if (pivot is null)
        {
            HandleQueryNonPivotMaking();
        }
        else
        {
            var pivotColumnClause = "";
            var pivotFor = "";
            var pivotColumn = "";
            var pivotGroup = "";

            switch (pivot.Type)
            {
                case ExitActionDynamicReportColumnsType.SizeTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductPropertyCTitle + '],' FROM (SELECT  Distinct tbl_ProductPropertyC.fld_ProductPropertyCTitle FROM tbl_ProductPropertyC WHERE tbl_ProductPropertyC.fld_ProductPropertyCTitle is not null AND tbl_ProductPropertyC.fld_ProductPropertyCTitle != '' ) as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_Products.ProductSize";
                    subWheres.Add("(tbl_Products_3.ProductSize = tbl_Products.ProductSize)");
                    pivotGroup = pivotColumn;
                    break;

                case ExitActionDynamicReportColumnsType.BrandTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductBrandTitle + '],' FROM (SELECT  Distinct tbl_ProductBrand.fld_ProductBrandTitle FROM tbl_ProductBrand WHERE tbl_ProductBrand.fld_ProductBrandTitle is not null AND tbl_ProductBrand.fld_ProductBrandTitle != '') as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_Products.fld_ProductBrand";
                    subWheres.Add("(tbl_Products_3.fld_ProductBrand = tbl_Products.fld_ProductBrand)");
                    pivotGroup = pivotColumn;
                    break;

                case ExitActionDynamicReportColumnsType.GroupTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductGroupTitle + '],' FROM (SELECT  Distinct tbl_ProductGroup.fld_ProductGroupTitle FROM tbl_ProductGroup WHERE tbl_ProductGroup.fld_ProductGroupTitle is not null AND tbl_ProductGroup.fld_ProductGroupTitle != '') as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_Products.fld_ProductGroup";
                    subWheres.Add("(tbl_Products_3.fld_ProductGroup = tbl_Products.fld_ProductGroup)");
                    pivotGroup = pivotColumn;
                    break;

                case ExitActionDynamicReportColumnsType.TypeTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].ProductTypeTitle + '],' FROM (SELECT  Distinct tbl_ProductType.ProductTypeTitle FROM tbl_ProductType WHERE tbl_ProductType.ProductTypeTitle is not null AND tbl_ProductType.ProductTypeTitle != '') as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_Products.ProductType";
                    subWheres.Add("(tbl_Products_3.ProductType = tbl_Products.ProductType)");
                    pivotGroup = pivotColumn;
                    break;

                case ExitActionDynamicReportColumnsType.QcTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].ProductStatusTitle + '],' FROM (SELECT  Distinct tbl_ProductStatus.ProductStatusTitle FROM tbl_ProductStatus WHERE tbl_ProductStatus.ProductStatusTitle is not null AND tbl_ProductStatus.ProductStatusTitle != '') as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = $"tbl_Products.ProductStatus";
                    subWheres.Add("(tbl_Products_3.ProductStatus = tbl_Products.ProductStatus)");
                    pivotGroup = pivotColumn;
                    break;

                case ExitActionDynamicReportColumnsType.SubGroupTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductSubGroupTitle + '],' FROM (SELECT  Distinct tbl_ProductSubGroup.fld_ProductSubGroupTitle FROM tbl_ProductSubGroup WHERE tbl_ProductSubGroup.fld_ProductSubGroupTitle is not null AND tbl_ProductSubGroup.fld_ProductSubGroupTitle != '') as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_Products.fld_ProductSubGroup";
                    subWheres.Add("(tbl_Products_3.fld_ProductSubGroup = tbl_Products.fld_ProductSubGroup)");
                    pivotGroup = pivotColumn;
                    break;

                case ExitActionDynamicReportColumnsType.ClassTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductClassTitle + '],' FROM (SELECT  Distinct tbl_ProductClass.fld_ProductClassTitle FROM tbl_ProductClass WHERE tbl_ProductClass.fld_ProductClassTitle is not null AND tbl_ProductClass.fld_ProductClassTitle != '') as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn = "tbl_Products.fld_ProductClass";
                    subWheres.Add("(tbl_Products_3.fld_ProductClass = tbl_Products.fld_ProductClass)");
                    pivotGroup = pivotColumn;
                    break;

                case ExitActionDynamicReportColumnsType.LineTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductPropertyATitle + '],' FROM (SELECT  Distinct tbl_ProductPropertyA.fld_ProductPropertyATitle FROM tbl_ProductPropertyA WHERE tbl_ProductPropertyA.fld_ProductPropertyATitle is not null AND tbl_ProductPropertyA.fld_ProductPropertyATitle != '') as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn =
                        $"""
                        (SELECT  COALESCE(tbl_ProductPropertyA.fld_ProductPropertyATitle,N'')
                        FROM tbl_Tags AS Tags INNER JOIN tbl_ProductPropertyA ON 
                        Tags.fld_ProductPropertyAId = tbl_ProductPropertyA.fld_ProductPropertyAId
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial)
                        """;
                    subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                    pivotGroup = "tbl_TagsMovement_1.ProductSerial";
                    break;

                case ExitActionDynamicReportColumnsType.ShiftTitle:
                    pivotColumnClause = "Select @columns +=  '['+ [NESTED].fld_ProductPropertyBTitle + '],' FROM (SELECT  Distinct tbl_ProductPropertyB.fld_ProductPropertyBTitle FROM tbl_ProductPropertyB WHERE tbl_ProductPropertyB.fld_ProductPropertyBTitle is not null AND tbl_ProductPropertyB.fld_ProductPropertyBTitle != '') as [NESTED] ;";
                    pivotFor = "[Nested].[Qc]";
                    pivotColumn =
                        $"""
                        (SELECT  COALESCE(tbl_ProductPropertyB.fld_ProductPropertyBTitle,N'')
                        FROM tbl_Tags AS Tags INNER JOIN tbl_ProductPropertyB ON 
                        Tags.fld_ProductPropertyBId = tbl_ProductPropertyB.fld_ProductPropertyBId
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial)
                        """;
                    subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                    pivotGroup = "tbl_TagsMovement_1.ProductSerial";
                    break;
            }

            groups.Add(pivotGroup);

            command = $"""
            declare @columns nvarchar(max) = '', @sqlcmd    NVARCHAR(MAX) = '';
            {pivotColumnClause}
            IF @columns = ''
            BEGIN 
            RETURN
            END
            SET @columns = LEFT(@columns, LEN(@columns) - 1); 
            SET @sqlcmd = N'
            SELECT * FROM
            (SELECT 
                COALESCE(
                (SELECT     SUM(ProductCount) AS Expr1
                    FROM     tbl_Tags
                    WHERE    (ProductSerial IN
                                 (SELECT DISTINCT tbl_TagsMovement_3.ProductSerial
                                 FROM            tbl_ActionTypes AS tbl_ActionTypes_3 RIGHT OUTER JOIN
                                                tbl_MovementActions AS tbl_MovementActions_3 ON tbl_ActionTypes_3.fld_ActionTypeId = tbl_MovementActions_3.MovementActionTp LEFT OUTER JOIN
                                                tbl_TagsMovement AS tbl_TagsMovement_3 LEFT OUTER JOIN
                                                tbl_Products AS tbl_Products_3 ON tbl_TagsMovement_3.ProductCode = tbl_Products_3.ProductCode ON 
                                                tbl_MovementActions_3.MovementActionId = tbl_TagsMovement_3.RMovementActionId  LEFT OUTER JOIN
                                                tbl_TruckCross AS TruckCross_3 ON tbl_MovementActions_3.MovementActionTruckCrossId = TruckCross_3.fld_TruckCrossId          
                                {(subWheres.Any() ? "WHERE " + string.Join(" AND ", subWheres.Select(p => p.Replace("'", "''"))) : "")} )
                                     )
                ), 0) as [Count],
            {pivotColumn.Replace("'", "''")} AS [Qc],{string.Join(",", selects.Select(p => p.Replace("'", "''")))}
            FROM tbl_Destination AS tbl_Destination_1 INNER JOIN
                    tbl_MovementActions ON tbl_Destination_1.DestinationCode = tbl_MovementActions.MovementActionDestinationId LEFT OUTER JOIN
                    tbl_Destination ON tbl_MovementActions.MovementActionStore = tbl_Destination.DestinationCode LEFT OUTER JOIN
                    tbl_ActionTypes ON tbl_MovementActions.MovementActionTp = tbl_ActionTypes.fld_ActionTypeId LEFT OUTER JOIN
                    tbl_User ON tbl_MovementActions.MovementActionUserId = tbl_User.Id LEFT OUTER JOIN
                    tbl_TagsMovement AS tbl_TagsMovement_1 LEFT OUTER JOIN
                    tbl_Products ON tbl_TagsMovement_1.ProductCode = tbl_Products.ProductCode ON tbl_MovementActions.MovementActionId = tbl_TagsMovement_1.RMovementActionId LEFT OUTER JOIN
                    tbl_TruckCross AS TruckCross_1 ON tbl_MovementActions.MovementActionTruckCrossId = TruckCross_1.fld_TruckCrossId   
                {(isTagsRequired? " LEFT JOIN tbl_Tags AS [MainTags] ON MainTags.ProductSerial = tbl_TagsMovement_1.ProductSerial " : "")}       
            {(wheres.Any() ? "WHERE " + string.Join(" AND ", wheres.Select(p => p.Replace("'", "''"))) : "")}
            {(groups.Any() ? "GROUP BY " + string.Join(" , ", groups.Select(p => p.Replace("'", "''"))) : "")}) AS [Nested]
            pivot (SUM([Nested].[Count])
            FOR {pivotFor} IN ('+@columns+') ) as PivotData ';
            EXECUTE sp_executesql @sqlcmd;
            """;
        }

        var dt = dataAccess.SqlDataAdapter(command, 180);

        return dt;

        string ReplaceDataMiningElementParameters(string command)
        {
            var parameters = new Dictionary<string, string>
            {
                { "@WhereProductSerialDME", " (tbl_TagsDME.ProductSerial = tbl_TagsMovement_1.ProductSerial) " } ,
                { "@WhereProductCodeDME"," (tbl_ProductsDME.ProductCode = tbl_TagsMovement_1.ProductCode) " } ,
                { "@WhereMovementActionIdDME", " (tbl_MovementActionsDME.MovementActionId = tbl_MovementActions.MovementActionId) " }
            };

            foreach (var parameter in parameters)
            {
                command = command.Replace(parameter.Key, parameter.Value);
            }

            return command;
        }

        void HandleDmes()
        {
            foreach (var element in dataMiningElementDtos)
            {
                string selectTitle = element.DataMiningElementsTitle;

                string elementCommand = ReplaceDataMiningElementParameters(element.DataMiningElementsCommand);

                selects.Add($"{elementCommand} AS [{selectTitle}]");
            }
        }

        void HandleCalculating()
        {
            foreach (var item in calculating)
            {
                switch (item.GroupColumnType)
                {
                    case ExitActionDynamicReportColumnsType.ProductCount:
                        if (item.Type == ReportCalculatingColumnType.Max)
                        {
                            selects.Add($"""COALESCE(Max(tbl_TagsMovement_1.ProductCount), 0) AS [{item.Title}]""");
                        }
                        else if (item.Type == ReportCalculatingColumnType.Min)
                        {
                            selects.Add($"""COALESCE(Min(tbl_TagsMovement_1.ProductCount), 0) AS [{item.Title}]""");
                        }
                        else if (item.Type == ReportCalculatingColumnType.Avg)
                        {
                            selects.Add($"""
                            COALESCE(
                            SUM(tbl_TagsMovement_1.ProductCount)/Count(tbl_TagsMovement_1.ProductSerial)
                            , 0) AS [{item.Title}]
                            """);
                        }
                        break;

                    case ExitActionDynamicReportColumnsType.ProductSerial:
                        selects.Add($"""COALESCE(Count(tbl_TagsMovement_1.ProductSerial), 0) AS [{item.Title}]""");
                        break;

                    case ExitActionDynamicReportColumnsType.SumCount:
                        if (item.Type == ReportCalculatingColumnType.Sum)
                        {
                            selects.Add($"""
                        COALESCE(
                        (SELECT     SUM(ProductCount) AS Expr1
                            FROM     tbl_Tags AS [InnerTag]
                            WHERE    (ProductSerial IN
                                         (SELECT DISTINCT tbl_TagsMovement_3.ProductSerial
                                         FROM            tbl_ActionTypes AS tbl_ActionTypes_3 RIGHT OUTER JOIN
                                                        tbl_MovementActions AS tbl_MovementActions_3 ON tbl_ActionTypes_3.fld_ActionTypeId = tbl_MovementActions_3.MovementActionTp LEFT OUTER JOIN
                                                        tbl_TagsMovement AS tbl_TagsMovement_3 LEFT OUTER JOIN
                                                        tbl_Products AS tbl_Products_3 ON tbl_TagsMovement_3.ProductCode = tbl_Products_3.ProductCode ON 
                                                        tbl_MovementActions_3.MovementActionId = tbl_TagsMovement_3.RMovementActionId  LEFT OUTER JOIN
                                                        tbl_TruckCross AS TruckCross_3 ON tbl_MovementActions_3.MovementActionTruckCrossId = TruckCross_3.fld_TruckCrossId          
                                        {(subWheres.Any() ? " WHERE " + string.Join(" AND ", subWheres) : "")} )
                                     )
                        ), 0) AS [{item.Title}]
                        """);
                       }
                        else if (item.Type == ReportCalculatingColumnType.Percent)
                        {
                            selects.Add($"""
                        CAST(
                        COALESCE(
                        (SELECT     SUM(ProductCount) AS Expr1
                            FROM     tbl_Tags AS [InnerTag]
                            WHERE    (ProductSerial IN
                                         (SELECT DISTINCT tbl_TagsMovement_3.ProductSerial
                                         FROM            tbl_ActionTypes AS tbl_ActionTypes_3 RIGHT OUTER JOIN
                                                        tbl_MovementActions AS tbl_MovementActions_3 ON tbl_ActionTypes_3.fld_ActionTypeId = tbl_MovementActions_3.MovementActionTp LEFT OUTER JOIN
                                                        tbl_TagsMovement AS tbl_TagsMovement_3 LEFT OUTER JOIN
                                                        tbl_Products AS tbl_Products_3 ON tbl_TagsMovement_3.ProductCode = tbl_Products_3.ProductCode ON 
                                                        tbl_MovementActions_3.MovementActionId = tbl_TagsMovement_3.RMovementActionId  LEFT OUTER JOIN
                                                        tbl_TruckCross AS TruckCross_3 ON tbl_MovementActions_3.MovementActionTruckCrossId = TruckCross_3.fld_TruckCrossId          
                                        {(subWheres.Any() ? " WHERE " + string.Join(" AND ", subWheres) : "")} )
                                     )
                        ), 0)/
                        COALESCE(
                        (SELECT     SUM(ProductCount) AS Expr1
                            FROM     tbl_Tags
                            WHERE    (ProductSerial IN
                                         (SELECT DISTINCT tbl_TagsMovement_3.ProductSerial
                                         FROM            tbl_ActionTypes AS tbl_ActionTypes_3 RIGHT OUTER JOIN
                                                        tbl_MovementActions AS tbl_MovementActions_3 ON tbl_ActionTypes_3.fld_ActionTypeId = tbl_MovementActions_3.MovementActionTp LEFT OUTER JOIN
                                                        tbl_TagsMovement AS tbl_TagsMovement_3 LEFT OUTER JOIN
                                                        tbl_Products AS tbl_Products_3 ON tbl_TagsMovement_3.ProductCode = tbl_Products_3.ProductCode ON 
                                                        tbl_MovementActions_3.MovementActionId = tbl_TagsMovement_3.RMovementActionId  LEFT OUTER JOIN
                                                        tbl_TruckCross AS TruckCross_3 ON tbl_MovementActions_3.MovementActionTruckCrossId = TruckCross_3.fld_TruckCrossId          
                                        {(subWhereTotalSum.Any() ? " WHERE " + string.Join(" AND ", subWhereTotalSum) : "")} )
                                     )
                        ), 1) * 100
                        AS decimal(10,1))
                        AS [{item.Title}]
                        """);
                        }
                        break;

                    case ExitActionDynamicReportColumnsType.ProductCountInPack:
                        selects.Add($"COALESCE(SUM(tbl_Products.ProductCountInPack), 0) AS [{item.Title}]");
                        break;

                    case ExitActionDynamicReportColumnsType.OperationCode:
                        selects.Add($"COALESCE(Count(DISTINCT tbl_MovementActions.MovementActionId), 0) AS [{item.Title}]");
                        break;
                }
            }
        }

        void HandleSelect()
        {
            foreach (var select in selectColumns)
            {
                string groupClause = string.Empty;
                string selectTitle = select.Title;

                switch (select.Type)
                {
                    case ExitActionDynamicReportColumnsType.DocumentCode:
                        selects.Add($"COALESCE(tbl_MovementActions.MovementActionDocumentId, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_MovementActions_3.MovementActionDocumentId = tbl_MovementActions.MovementActionDocumentId)");
                        groupClause = "tbl_MovementActions.MovementActionDocumentId";
                        break;
                    case ExitActionDynamicReportColumnsType.OperationCode:
                        selects.Add($"CAST(tbl_MovementActions.MovementActionId AS nvarchar(250)) AS [{selectTitle}]");
                        subWheres.Add("(tbl_MovementActions_3.MovementActionId = tbl_MovementActions.MovementActionId)");
                        groupClause = "tbl_MovementActions.MovementActionId";
                        break;
                    case ExitActionDynamicReportColumnsType.OperationTime:
                        selects.Add($"tbl_MovementActions.MovementActionTime AS [{selectTitle}]");
                        subWheres.Add("(tbl_MovementActions_3.MovementActionTime = tbl_MovementActions.MovementActionTime)");
                        groupClause = "tbl_MovementActions.MovementActionTime";
                        break;
                    case ExitActionDynamicReportColumnsType.PersianDateFull:
                        selects.Add($"tbl_MovementActions.MovementActionDate AS [{selectTitle}]");
                        subWheres.Add("(tbl_MovementActions_3.MovementActionDate = tbl_MovementActions.MovementActionDate)");
                        groupClause = "tbl_MovementActions.MovementActionDate";
                        break;
                    case ExitActionDynamicReportColumnsType.PersianDateYear:
                        selects.Add($"SUBSTRING(tbl_MovementActions.MovementActionDate, 0, 5) AS [{selectTitle}]");
                        subWheres.Add("(tbl_MovementActions_3.MovementActionDate = tbl_MovementActions.MovementActionDate)");
                        groupClause = "SUBSTRING(tbl_MovementActions.MovementActionDate, 0, 5)";
                        break;
                    case ExitActionDynamicReportColumnsType.PersianDateMonth:
                        selects.Add($"SUBSTRING(tbl_MovementActions.MovementActionDate, 6, 2) AS [{selectTitle}]");
                        subWheres.Add("""
                        (SUBSTRING(tbl_MovementActions_3.MovementActionDate, 6, 2) =
                        SUBSTRING(tbl_MovementActions.MovementActionDate, 6, 2))
                        """);
                        groupClause = "SUBSTRING(tbl_MovementActions.MovementActionDate, 6, 2)";
                        break;
                    case ExitActionDynamicReportColumnsType.PersianDateDay:
                        selects.Add($"SUBSTRING(tbl_MovementActions.MovementActionDate, 9, 2) AS [{selectTitle}]");
                        subWheres.Add("""
                        (SUBSTRING(tbl_MovementActions_3.MovementActionDate, 9, 2) =
                        SUBSTRING(tbl_MovementActions.MovementActionDate, 9, 2))
                        """);
                        groupClause = "SUBSTRING(tbl_MovementActions.MovementActionDate, 9, 2)";
                        break;
                    case ExitActionDynamicReportColumnsType.GregorianDateFull:
                        selects.Add($"CAST(tbl_MovementActions.MovementActionDateTime AS DATE) AS [{selectTitle}]");
                        subWheres.Add("""
                        (CAST(tbl_MovementActions_3.MovementActionDateTime AS DATE) =
                        CAST(tbl_MovementActions.MovementActionDateTime AS DATE))
                        """);
                        groupClause = "CAST(tbl_MovementActions.MovementActionDateTime AS DATE)";
                        break;
                    case ExitActionDynamicReportColumnsType.GregorianDateYear:
                        selects.Add($"DATEPART(year, tbl_MovementActions.MovementActionDateTime) AS [{selectTitle}]");
                        subWheres.Add("""
                        (DATEPART(year, tbl_MovementActions_3.MovementActionDateTime) = 
                        DATEPART(year, tbl_MovementActions.MovementActionDateTime))
                        """);
                        groupClause = "DATEPART(year, tbl_MovementActions.MovementActionDateTime)";
                        break;
                    case ExitActionDynamicReportColumnsType.GregorianDateMonth:
                        selects.Add($"DATEPART(month, tbl_MovementActions.MovementActionDateTime) AS [{selectTitle}]");
                        subWheres.Add("""
                        (DATEPART(month, tbl_MovementActions_3.MovementActionDateTime) = 
                        DATEPART(month, tbl_MovementActions.MovementActionDateTime))
                        """);
                        groupClause = "DATEPART(month, tbl_MovementActions.MovementActionDateTime)";
                        break;
                    case ExitActionDynamicReportColumnsType.GregorianDateDay:
                        selects.Add($"DATEPART(day, tbl_MovementActions.MovementActionDateTime) AS [{selectTitle}]");
                        subWheres.Add("""
                        (DATEPART(day, tbl_MovementActions_3.MovementActionDateTime) = 
                        DATEPART(day, tbl_MovementActions.MovementActionDateTime))
                        """);
                        groupClause = "DATEPART(day, tbl_MovementActions.MovementActionDateTime)";
                        break;
                    case ExitActionDynamicReportColumnsType.StationName:
                        selects.Add($@"COALESCE((SELECT tbl_Station.fld_StationName FROM tbl_Station 
                                   WHERE tbl_Station.fld_StationCode = tbl_MovementActions.MovementActionUHFLogGate),N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_MovementActions_3.MovementActionUHFLogGate = tbl_MovementActions.MovementActionUHFLogGate)");
                        groupClause = "tbl_MovementActions.MovementActionUHFLogGate";
                        break;
                    case ExitActionDynamicReportColumnsType.ProductSerial:
                        selects.Add($"COALESCE(tbl_TagsMovement_1.ProductSerial, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                        groupClause = "tbl_TagsMovement_1.ProductSerial";
                        break;
                    case ExitActionDynamicReportColumnsType.ProductCode:
                        selects.Add($"COALESCE(tbl_TagsMovement_1.ProductCode, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_TagsMovement_3.ProductCode = tbl_TagsMovement_1.ProductCode)");
                        groupClause = "tbl_TagsMovement_1.ProductCode";
                        break;
                    case ExitActionDynamicReportColumnsType.Regcode:
                        selects.Add($"COALESCE(tbl_Products.ProductTechnicalCode, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.ProductTechnicalCode = tbl_Products.ProductTechnicalCode)");
                        groupClause = "tbl_Products.ProductTechnicalCode";
                        break;
                    case ExitActionDynamicReportColumnsType.ProductName:
                        selects.Add($"COALESCE(tbl_Products.ProductTitle, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.ProductTitle = tbl_Products.ProductTitle)");
                        groupClause = "tbl_Products.ProductTitle";
                        break;
                    case ExitActionDynamicReportColumnsType.LineCode:
                        selects.Add($"""
                        (SELECT Tags.fld_ProductPropertyAId 
                        FROM tbl_Tags AS Tags 
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial) AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                        groupClause = "tbl_TagsMovement_1.ProductSerial";
                        break;
                    case ExitActionDynamicReportColumnsType.LineTitle:
                        selects.Add($"""
                        (SELECT  COALESCE(tbl_ProductPropertyA.fld_ProductPropertyATitle,N'')
                        FROM tbl_Tags AS Tags INNER JOIN tbl_ProductPropertyA ON 
                        Tags.fld_ProductPropertyAId = tbl_ProductPropertyA.fld_ProductPropertyAId
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial) AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                        groupClause = "tbl_TagsMovement_1.ProductSerial";
                        break;
                    case ExitActionDynamicReportColumnsType.ShiftCode:
                        selects.Add($"""
                        (SELECT Tags.fld_ProductPropertyBId 
                        FROM tbl_Tags AS Tags 
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial) AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                        groupClause = "tbl_TagsMovement_1.ProductSerial";
                        break;
                    case ExitActionDynamicReportColumnsType.ShiftTitle:
                        selects.Add($"""
                        (SELECT  COALESCE(tbl_ProductPropertyB.fld_ProductPropertyBTitle,N'')
                        FROM tbl_Tags AS Tags INNER JOIN tbl_ProductPropertyB ON 
                        Tags.fld_ProductPropertyBId = tbl_ProductPropertyB.fld_ProductPropertyBId
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial) AS [{select.Title}]
                        """);
                        subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                        groupClause = "tbl_TagsMovement_1.ProductSerial";
                        break;
                    case ExitActionDynamicReportColumnsType.QcCode:
                        selects.Add($"COALESCE(tbl_Products.ProductStatus, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.ProductStatus = tbl_Products.ProductStatus)");
                        groupClause = "tbl_Products.ProductStatus";
                        break;
                    case ExitActionDynamicReportColumnsType.QcTitle:
                        selects.Add($"""
                        COALESCE((
                        SELECT tbl_ProductStatus.ProductStatusTitle 
                        FROM tbl_ProductStatus 
                        WHERE tbl_ProductStatus.ProductStatusCode = tbl_Products.ProductStatus), N'') AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_Products_3.ProductStatus = tbl_Products.ProductStatus)");
                        groupClause = "tbl_Products.ProductStatus";
                        break;
                    case ExitActionDynamicReportColumnsType.TypeCode:
                        selects.Add($"COALESCE(tbl_Products.ProductType, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.ProductType = tbl_Products.ProductType)");
                        groupClause = "tbl_Products.ProductType";
                        break;
                    case ExitActionDynamicReportColumnsType.TypeTitle:
                        selects.Add($"""
                        COALESCE((SELECT tbl_ProductType.ProductTypeTitle 
                        FROM tbl_ProductType 
                        WHERE tbl_ProductType.ProductTypeCode = tbl_Products.ProductType), N'') AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_Products_3.ProductType = tbl_Products.ProductType)");
                        groupClause = "tbl_Products.ProductType";
                        break;
                    case ExitActionDynamicReportColumnsType.SizeCode:
                        selects.Add($"COALESCE(tbl_Products.ProductSize, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.ProductSize = tbl_Products.ProductSize)");
                        groupClause = "tbl_Products.ProductSize";
                        break;
                    case ExitActionDynamicReportColumnsType.SizeTitle:
                        selects.Add($"""
                        COALESCE((SELECT tbl_ProductPropertyC.fld_ProductPropertyCTitle 
                        FROM tbl_ProductPropertyC 
                        WHERE tbl_ProductPropertyC.fld_ProductPropertyCId = tbl_Products.ProductSize), N'') AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_Products_3.ProductSize = tbl_Products.ProductSize)");
                        groupClause = "tbl_Products.ProductSize";
                        break;
                    case ExitActionDynamicReportColumnsType.BrandCode:
                        selects.Add($"COALESCE(tbl_Products.fld_ProductBrand, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.fld_ProductBrand = tbl_Products.fld_ProductBrand)");
                        groupClause = "tbl_Products.fld_ProductBrand";
                        break;
                    case ExitActionDynamicReportColumnsType.BrandTitle:
                        selects.Add($"""
                        COALESCE((SELECT tbl_ProductBrand.fld_ProductBrandTitle 
                        FROM tbl_ProductBrand 
                        WHERE tbl_ProductBrand.fld_ProductBrandCode = tbl_Products.fld_ProductBrand), N'') AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_Products_3.fld_ProductBrand = tbl_Products.fld_ProductBrand)");
                        groupClause = "tbl_Products.fld_ProductBrand";
                        break;
                    case ExitActionDynamicReportColumnsType.GroupCode:
                        selects.Add($"COALESCE(tbl_Products.fld_ProductGroup, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.fld_ProductGroup = tbl_Products.fld_ProductGroup)");
                        groupClause = "tbl_Products.fld_ProductGroup";
                        break;
                    case ExitActionDynamicReportColumnsType.GroupTitle:
                        selects.Add($"""
                        COALESCE((SELECT tbl_ProductGroup.fld_ProductGroupTitle 
                        FROM tbl_ProductGroup 
                        WHERE tbl_ProductGroup.fld_ProductGroupCode = tbl_Products.fld_ProductGroup), N'') AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_Products_3.fld_ProductGroup = tbl_Products.fld_ProductGroup)");
                        groupClause = "tbl_Products.fld_ProductGroup";
                        break;
                    case ExitActionDynamicReportColumnsType.SubGroupCode:
                        selects.Add($"COALESCE(tbl_Products.fld_ProductSubGroup, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.fld_ProductSubGroup = tbl_Products.fld_ProductSubGroup)");
                        groupClause = "tbl_Products.fld_ProductSubGroup";
                        break;
                    case ExitActionDynamicReportColumnsType.SubGroupTitle:
                        selects.Add($"""
                        COALESCE((SELECT tbl_ProductSubGroup.fld_ProductSubGroupTitle 
                        FROM tbl_ProductSubGroup 
                        WHERE tbl_ProductSubGroup.fld_ProductSubGroupCode = tbl_Products.fld_ProductSubGroup), N'') AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_Products_3.fld_ProductSubGroup = tbl_Products.fld_ProductSubGroup)");
                        groupClause = "tbl_Products.fld_ProductSubGroup";
                        break;
                    case ExitActionDynamicReportColumnsType.ClassCode:
                        selects.Add($"COALESCE(tbl_Products.fld_ProductClass, N'') AS [{selectTitle}]");
                        subWheres.Add("(tbl_Products_3.ProductCode = tbl_Products.ProductCode)");
                        groupClause = "tbl_Products.fld_ProductClass";
                        break;
                    case ExitActionDynamicReportColumnsType.ClassTitle:
                        selects.Add($"""
                        COALESCE((SELECT tbl_ProductClass.fld_ProductClassTitle 
                        FROM tbl_ProductClass 
                        WHERE tbl_ProductClass.fld_ProductClassCode = tbl_Products.fld_ProductClass), N'') AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_Products_3.ProductCode = tbl_Products.ProductCode)");
                        groupClause = "tbl_Products.fld_ProductClass";
                        break;

                    case ExitActionDynamicReportColumnsType.RegisterUser:
                        selects.Add($"""
                        (SELECT  COALESCE(tbl_User.Name,N'')
                        FROM tbl_Tags AS Tags INNER JOIN tbl_User ON 
                        Tags.TagRegisterUser = tbl_User.Id
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial) AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                        groupClause = "tbl_TagsMovement_1.ProductSerial";
                        break;
                    case ExitActionDynamicReportColumnsType.RegisterDate:
                        selects.Add($"""
                        (SELECT SUBSTRING(TagRegisterShamsiUnixDate, 1, 4) + '/' + 
                        SUBSTRING(TagRegisterShamsiUnixDate, 5, 2) + '/' + 
                        SUBSTRING(TagRegisterShamsiUnixDate, 7, 2)  
                        FROM tbl_Tags AS Tags
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial) AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                        groupClause = "tbl_TagsMovement_1.ProductSerial";
                        break;
                    case ExitActionDynamicReportColumnsType.RegisterTime:

                        selects.Add($"""
                        (SELECT SUBSTRING(TagRegisterShamsiUnixDate, 9, 2) + ':' + 
                        SUBSTRING(TagRegisterShamsiUnixDate, 11, 2) + ':' + 
                        SUBSTRING(TagRegisterShamsiUnixDate, 13, 2)
                        FROM tbl_Tags AS Tags 
                        WHERE Tags.ProductSerial = tbl_TagsMovement_1.ProductSerial) AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_TagsMovement_3.ProductSerial = tbl_TagsMovement_1.ProductSerial)");
                        groupClause = "tbl_TagsMovement_1.ProductSerial";
                        break;

                    case ExitActionDynamicReportColumnsType.ExtiActionUser:
                        selects.Add($"""
                        (SELECT  COALESCE(tbl_User.Name,N'')
                        FROM  tbl_User
                        WHERE tbl_User.Id = tbl_MovementActions.MovementActionUserId) AS [{selectTitle}]
                        """);
                        subWheres.Add("(tbl_MovementActions_3.MovementActionUserId = tbl_MovementActions.MovementActionUserId)");
                        groupClause = "tbl_MovementActions.MovementActionUserId";
                        break;

                    case ExitActionDynamicReportColumnsType.DynamicFields:
                        if (select.AdditionalData["DynamicColumnActionType"] == "0")
                        {
                            selects.Add($$"""
                        JSON_VALUE(MainTags.ProductProperties,N'$."{{selectTitle}}"') AS [{{selectTitle}}]
                        """);

                            subWheres.Add($$"""
                        (InnerTag.ProductSerial = tbl_TagsMovement_3.ProductSerial) AND (JSON_VALUE(InnerTag.ProductProperties,N'$."{{selectTitle}}"') = JSON_VALUE(MainTags.ProductProperties,N'$."{{selectTitle}}"'))
                        """);

                            groupClause = $"JSON_VALUE(MainTags.ProductProperties,N'$.\"{selectTitle}\"')";

                            isTagsRequired = true;  
                        }
                        else
                        {
                            selects.Add($"""
                        COALESCE(JSON_VALUE(tbl_MovementActions.MovementActionData,N'$."{selectTitle}"'), N'') AS [{selectTitle}]
                        """);
                            subWheres.Add($"""
                        COALESCE(JSON_VALUE(tbl_MovementActions_3.MovementActionData,N'$."{selectTitle}"'), N'') = 
                        COALESCE(JSON_VALUE(tbl_MovementActions.MovementActionData,N'$."{selectTitle}"'), N'')
                        """);
                            groupClause = $"COALESCE(JSON_VALUE(tbl_MovementActions.MovementActionData,N'$.\"{selectTitle}\"'), N'')";
                        }
                        break;
                }

                if (groupClause.HasValue() && (calculating.Any() || pivot is not null))
                {
                    groups.Add(groupClause);
                }

                if (select.SortType != ReportColumnSortType.None)
                {
                    orders.Add($" [{selectTitle}] {(select.SortType == ReportColumnSortType.Asc ? "ASC" : "DESC")}");
                }
            }
        }

        void HandleWheres()
        {
            foreach (ReportFilterGeneric<ExitActionDynamicReportFilterType> filter in filters)
            {
                string subQueryWhereCommand = string.Empty;

                if (filter.SqlWhereCommand.HasValue())
                {
                    continue;
                }

                if (filter.Type.Equals(FilterType.Dynamic))
                {
                    if (filter.AdditionalData["DynamicFilterActionType"] == "0")
                    {
                        wheres.Add(
                            $"""
                        (tbl_TagsMovement_1.ProductSerial 
                        IN(SELECT ProductSerial
                        FROM tbl_Tags AS Tags
                        WHERE tbl_TagsMovement_1.ProductSerial = Tags.ProductSerial
                        AND JSON_VALUE(Tags.ProductProperties,N'$."{filter.FieldName}"') IN(N'{string.Join("',N'", filter.Values)}')))
                        """);

                        subWheres.Add(
                            $"""
                        (tbl_TagsMovement_3.ProductSerial 
                        IN(SELECT ProductSerial
                        FROM tbl_Tags AS Tags_3
                        WHERE tbl_TagsMovement_3.ProductSerial = Tags_3.ProductSerial
                        AND JSON_VALUE(Tags_3.ProductProperties,N'$."{filter.FieldName}"') IN(N'{string.Join("',N'", filter.Values)}')))
                        """);

                        subWhereTotalSum.Add(
                            $"""
                        (tbl_TagsMovement_3.ProductSerial 
                        IN(SELECT ProductSerial
                        FROM tbl_Tags AS Tags_3
                        WHERE tbl_TagsMovement_3.ProductSerial = Tags_3.ProductSerial
                        AND JSON_VALUE(Tags_3.ProductProperties,N'$."{filter.FieldName}"') IN(N'{string.Join("',N'", filter.Values)}')))
                        """);
                    }
                    else
                    {
                        filter.SqlWhereCommand = $"JSON_VALUE(tbl_MovementActions.MovementActionData,N'$.\"{filter.FieldName}\"')";

                        subQueryWhereCommand = $"JSON_VALUE(tbl_MovementActions_3.MovementActionData,N'$.\"{filter.FieldName}\"')";

                        wheres.Add(DynamicFilteringTools.GetDynamicWhere(filter));

                        subWheres.Add(DynamicFilteringTools.GetDynamicWhere(filter).Replace(filter.SqlWhereCommand, subQueryWhereCommand));

                        subWhereTotalSum.Add(DynamicFilteringTools.GetDynamicWhere(filter).Replace(filter.SqlWhereCommand, subQueryWhereCommand));
                    }
                }
                else if (filter.Type.Equals(FilterType.TechnicalInfo))
                {
                    filter.SqlWhereCommand = $"JSON_VALUE(tbl_Products.ProductTechnicalData,N'$.\"{filter.FieldName}\"')";

                    subQueryWhereCommand = $"JSON_VALUE(tbl_Products_3.ProductTechnicalData,N'$.\"{filter.FieldName}\"')";

                    wheres.Add(DynamicFilteringTools.GetTechnicalInfoWhere(filter));

                    subWheres.Add(DynamicFilteringTools.GetTechnicalInfoWhere(filter).Replace(filter.SqlWhereCommand, subQueryWhereCommand));

                    subWhereTotalSum.Add(DynamicFilteringTools.GetTechnicalInfoWhere(filter).Replace(filter.SqlWhereCommand, subQueryWhereCommand));
                }
                else if (filter.Type.Equals(FilterType.Static))
                {
                    switch (filter.FieldType)
                    {
                        case ExitActionDynamicReportFilterType.OperationCode:
                            filter.SqlWhereCommand = "MovementActionId";
                            break;

                        case ExitActionDynamicReportFilterType.ProductCode:
                            filter.SqlWhereCommand = "tbl_TagsMovement_1.ProductCode";
                            subQueryWhereCommand = "tbl_TagsMovement_3.ProductCode";
                            break;

                        case ExitActionDynamicReportFilterType.TechnicalCode:
                            filter.SqlWhereCommand = "tbl_Products.ProductTechnicalCode";
                            subQueryWhereCommand = "tbl_Products_3.ProductTechnicalCode";
                            break;

                        case ExitActionDynamicReportFilterType.FromDate:
                            filter.SqlWhereCommand = "tbl_MovementActions.MovementActionDateTime";
                            subQueryWhereCommand = "tbl_MovementActions_3.MovementActionDateTime";
                            filter.Values = filter.Values.Select(date => $"dbo.JalaliDateToGeorgianDate(N'{date}', N'06:00')").ToList();
                            break;

                        case ExitActionDynamicReportFilterType.ToDate:
                            filter.SqlWhereCommand = "tbl_MovementActions.MovementActionDateTime";
                            subQueryWhereCommand = "tbl_MovementActions_3.MovementActionDateTime";
                            filter.Values = filter.Values.Select(date => $"dbo.JalaliDateToGeorgianDate(N'{date}', N'06:00')").ToList();
                            break;

                        case ExitActionDynamicReportFilterType.GateOpCode:
                            filter.SqlWhereCommand = "tbl_MovementActions.MovementActionUHFLogId";
                            subQueryWhereCommand = "tbl_MovementActions_3.MovementActionUHFLogId";
                            break;

                        case ExitActionDynamicReportFilterType.StoreCode:
                            filter.SqlWhereCommand = "tbl_MovementActions.MovementActionStore";
                            subQueryWhereCommand = "tbl_MovementActions_3.MovementActionStore";
                            break;

                        case ExitActionDynamicReportFilterType.Size:
                            filter.SqlWhereCommand = "tbl_Products.ProductSize";
                            subQueryWhereCommand = "tbl_Products_3.ProductSize";
                            break;

                        case ExitActionDynamicReportFilterType.ProductGroup:
                            filter.SqlWhereCommand = "tbl_Products.fld_ProductGroup";
                            subQueryWhereCommand = "tbl_Products_3.fld_ProductGroup";
                            break;

                        case ExitActionDynamicReportFilterType.ProductBrand:
                            filter.SqlWhereCommand = "tbl_Products.fld_ProductBrand";
                            subQueryWhereCommand = "tbl_Products_3.fld_ProductBrand";
                            break;

                        case ExitActionDynamicReportFilterType.ProductType:
                            filter.SqlWhereCommand = "tbl_Products.ProductType";
                            subQueryWhereCommand = "tbl_Products_3.ProductType";
                            break;

                        case ExitActionDynamicReportFilterType.GateCode:
                            filter.SqlWhereCommand = "tbl_MovementActions.MovementActionUHFLogGate";
                            subQueryWhereCommand = "tbl_MovementActions_3.MovementActionUHFLogGate";
                            break;

                        case ExitActionDynamicReportFilterType.ActionType:
                            filter.SqlWhereCommand = "tbl_MovementActions.MovementActionTp";
                            subQueryWhereCommand = "tbl_MovementActions_3.MovementActionTp";
                            break;

                        case ExitActionDynamicReportFilterType.DocumentKey:
                            filter.SqlWhereCommand = "tbl_MovementActions.MovementActionDocumentId";
                            subQueryWhereCommand = "tbl_MovementActions_3.MovementActionDocumentId";
                            break;

                        case ExitActionDynamicReportFilterType.ExitActionUser:
                            filter.SqlWhereCommand = "tbl_MovementActions.MovementActionUserId";
                            subQueryWhereCommand = "tbl_MovementActions_3.MovementActionUserId";
                            break;

                        case ExitActionDynamicReportFilterType.RecordsCount:
                            recordCount = $@"TOP({filter.Value})";
                            break;

                        case ExitActionDynamicReportFilterType.TruckType:
                            filter.SqlWhereCommand = "TruckCross_1.fld_TruckCrossType";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossType";
                            break;

                        case ExitActionDynamicReportFilterType.CarPlaque:
                            filter.SqlWhereCommand = "TruckCross_1.fld_TruckCrossPlaque";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPlaque";
                            break;

                        case ExitActionDynamicReportFilterType.NationalCode:
                            filter.SqlWhereCommand = "TruckCross_1.fld_TruckCrossNationalCode";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossNationalCode";
                            break;

                        case ExitActionDynamicReportFilterType.DriverName:
                            filter.SqlWhereCommand = "TruckCross_1.fld_TruckCrossDriverName";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossDriverName";
                            break;

                        case ExitActionDynamicReportFilterType.TruckCrossCause:
                            filter.SqlWhereCommand = "TruckCross_1.fld_TruckCrossPresentCause";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentCause";
                            break;

                        case ExitActionDynamicReportFilterType.TruckCrossOperationType:
                            filter.SqlWhereCommand = "TruckCross_1.fld_TruckCrossPresentOperationType";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentOperationType";
                            break;

                        case ExitActionDynamicReportFilterType.TruckCrossShipment:
                            filter.SqlWhereCommand = "TruckCross_1.fld_TruckCrossPresentShipment";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentShipment";
                            break;

                        case ExitActionDynamicReportFilterType.TruckCrossOperationDestination:
                            filter.SqlWhereCommand = "TruckCross_1.fld_TruckCrossPresentOperationDestination";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentOperationDestination";
                            break;

                        case ExitActionDynamicReportFilterType.Line:
                            wheres.Add($@"(tbl_TagsMovement_1.ProductSerial IN 
                        (SELECT Tags.ProductSerial FROM tbl_Tags AS Tags 
                        WHERE Tags.fld_ProductPropertyAId IN('{string.Join("','", filter.Values)}') ))");

                            subWheres.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        WHERE Tags_3.fld_ProductPropertyAId IN('{string.Join("','", filter.Values)}') ))");

                            subWhereTotalSum.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        WHERE Tags_3.fld_ProductPropertyAId IN('{string.Join("','", filter.Values)}') ))");
                            break;

                        case ExitActionDynamicReportFilterType.Shift:
                            wheres.Add($@"(tbl_TagsMovement_1.ProductSerial 
                        IN (SELECT Tags.ProductSerial FROM tbl_Tags AS Tags 
                        WHERE Tags.fld_ProductPropertyBId IN('{string.Join("','", filter.Values)}') ))");

                            subWheres.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        WHERE Tags_3.fld_ProductPropertyBId IN('{string.Join("','", filter.Values)}') ))");

                            subWhereTotalSum.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        WHERE Tags_3.fld_ProductPropertyBId IN('{string.Join("','", filter.Values)}') ))");
                            break;

                        case ExitActionDynamicReportFilterType.RegisterUser:
                            wheres.Add($@"(tbl_TagsMovement_1.ProductSerial 
                        IN (SELECT Tags.ProductSerial FROM tbl_Tags AS Tags 
                        WHERE Tags.TagRegisterUser IN('{string.Join("','", filter.Values)}') ))");

                            subWheres.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        WHERE Tags_3.TagRegisterUser IN('{string.Join("','", filter.Values)}') ))");

                            subWhereTotalSum.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        WHERE Tags_3.TagRegisterUser IN('{string.Join("','", filter.Values)}') ))");
                            break;

                        case ExitActionDynamicReportFilterType.FromRegisterDate:

                            var fromRegisterDateTime = string.Join(" OR ", filter.Values.Select(value =>
                            $"(Tags.TagRegisterShamsiUnixDate >= {value.Replace("/", "") + StartShiftTime[0]})"));

                            wheres.Add($@"(tbl_TagsMovement_1.ProductSerial 
                        IN (SELECT Tags.ProductSerial FROM tbl_Tags AS Tags 
                        {(fromRegisterDateTime.HasValue() ? ("Where" + fromRegisterDateTime) : string.Empty)} ))");

                            subWheres.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        {(fromRegisterDateTime.HasValue() ?
                            ("Where" + fromRegisterDateTime.Replace("Tags.TagRegisterShamsiUnixDate", "Tags_3.TagRegisterShamsiUnixDate")) : string.Empty)} ))");

                            subWhereTotalSum.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        {(fromRegisterDateTime.HasValue() ?
                            ("Where" + fromRegisterDateTime.Replace("Tags.TagRegisterShamsiUnixDate", "Tags_3.TagRegisterShamsiUnixDate")) : string.Empty)} ))");

                            break;

                        case ExitActionDynamicReportFilterType.ToRegisterDate:

                            var toRegisterDateTime = string.Join(" OR ", filter.Values.Select(value =>
                                $"(Tags.TagRegisterShamsiUnixDate <= {value.Replace("/", "") + StartShiftTime[0]})"));

                            wheres.Add($@"(tbl_TagsMovement_1.ProductSerial 
                        IN (SELECT Tags.ProductSerial FROM tbl_Tags AS Tags 
                        {(toRegisterDateTime.HasValue() ? ("Where" + toRegisterDateTime) : string.Empty)} ))");

                            subWheres.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        {(toRegisterDateTime.HasValue() ?
                            ("Where" + toRegisterDateTime.Replace("Tags.TagRegisterShamsiUnixDate", "Tags_3.TagRegisterShamsiUnixDate")) : string.Empty)} ))");

                            subWhereTotalSum.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        {(toRegisterDateTime.HasValue() ?
                            ("Where" + toRegisterDateTime.Replace("Tags.TagRegisterShamsiUnixDate", "Tags_3.TagRegisterShamsiUnixDate")) : string.Empty)} ))");

                            break;

                        case ExitActionDynamicReportFilterType.FromRegisterTime:

                            var fromRegisterTime = string.Join(" OR ", filter.Values.Select(value =>
                                    $"(CAST(Tags.TagRegisterDateTime AS TIME) >= '{value}')"));

                            wheres.Add($@"(tbl_TagsMovement_1.ProductSerial 
                        IN (SELECT Tags.ProductSerial FROM tbl_Tags AS Tags 
                        {(fromRegisterTime.HasValue() ? "WHERE" + fromRegisterTime : string.Empty)}))");

                            subWheres.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        {(fromRegisterTime.HasValue() ?
                            "WHERE" + fromRegisterTime.Replace("Tags.TagRegisterDateTime", "Tags_3.TagRegisterDateTime") : string.Empty)}))");

                            subWhereTotalSum.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        {(fromRegisterTime.HasValue() ?
                            "WHERE" + fromRegisterTime.Replace("Tags.TagRegisterDateTime", "Tags_3.TagRegisterDateTime") : string.Empty)}))");

                            break;

                        case ExitActionDynamicReportFilterType.ToRegisterTime:

                            var toRegisterTime = string.Join(" OR ", filter.Values.Select(value =>
                                    $"(CAST(Tags.TagRegisterDateTime AS TIME) <= '{value}')"));

                            wheres.Add($@"(tbl_TagsMovement_1.ProductSerial 
                        IN (SELECT Tags.ProductSerial FROM tbl_Tags AS Tags 
                        {(toRegisterTime.HasValue() ? "WHERE" + toRegisterTime : string.Empty)}))");

                            subWheres.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        {(toRegisterTime.HasValue() ?
                            "WHERE" + toRegisterTime.Replace("Tags.TagRegisterDateTime", "Tags_3.TagRegisterDateTime") : string.Empty)}))");

                            subWhereTotalSum.Add($@"(tbl_TagsMovement_3.ProductSerial 
                        IN (SELECT Tags_3.ProductSerial FROM tbl_Tags AS Tags_3 
                        {(toRegisterTime.HasValue() ?
                            "WHERE" + toRegisterTime.Replace("Tags.TagRegisterDateTime", "Tags_3.TagRegisterDateTime") : string.Empty)}))");

                            break;

                    }

                    if (filter.SqlWhereCommand.HasValue())
                    {
                        wheres.Add(DynamicFilteringTools.GetStaticWhere(filter));

                        if (subQueryWhereCommand.HasValue())
                        {
                            subWheres.Add(DynamicFilteringTools.GetStaticWhere(filter).Replace(filter.SqlWhereCommand, subQueryWhereCommand));

                            subWhereTotalSum.Add(DynamicFilteringTools.GetStaticWhere(filter).Replace(filter.SqlWhereCommand, subQueryWhereCommand));
                        }
                    }
                }
            }
        }

        void HandleQueryNonPivotMaking()
        {
   command = $"""
SELECT  {string.Join(',', selects)}
FROM tbl_Destination AS tbl_Destination_1 INNER JOIN
tbl_MovementActions ON tbl_Destination_1.DestinationCode = tbl_MovementActions.MovementActionDestinationId LEFT OUTER JOIN
tbl_Destination ON tbl_MovementActions.MovementActionStore = tbl_Destination.DestinationCode LEFT OUTER JOIN
tbl_ActionTypes ON tbl_MovementActions.MovementActionTp = tbl_ActionTypes.fld_ActionTypeId LEFT OUTER JOIN
tbl_User ON tbl_MovementActions.MovementActionUserId = tbl_User.Id LEFT OUTER JOIN
tbl_TagsMovement AS tbl_TagsMovement_1 LEFT OUTER JOIN
tbl_Products ON tbl_TagsMovement_1.ProductCode = tbl_Products.ProductCode ON tbl_MovementActions.MovementActionId = tbl_TagsMovement_1.RMovementActionId LEFT OUTER JOIN
tbl_TruckCross AS TruckCross_1 ON tbl_MovementActions.MovementActionTruckCrossId = TruckCross_1.fld_TruckCrossId   
{(isTagsRequired? " LEFT JOIN tbl_Tags AS [MainTags] ON MainTags.ProductSerial = tbl_TagsMovement_1.ProductSerial " : "")}       
{(wheres.Any() ? "WHERE " + string.Join(" AND ", wheres) : "")}
{(groups.Any() ? "GROUP BY " + string.Join(" , ", groups) : "")}
{(orders.Any() ? "ORDER BY " + string.Join(" , ", orders) : "")}
""";
     }

    }
}
