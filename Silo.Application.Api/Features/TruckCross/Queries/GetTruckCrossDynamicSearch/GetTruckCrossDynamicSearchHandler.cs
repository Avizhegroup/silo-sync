using Silo.Application.Contracts;
using Silo.Application.Dto;
using Silo.Application.Dto.Filter;
using Silo.Shared.Tools;

namespace Silo.Application.Api.Features;
public class GetTruckCrossDynamicSearchHandler(IDataAccess dataAccess
    , IWmsBusiness wmsBusiness) : IRequestHandler<GetTruckCrossDynamicSearchQuery, GetTruckCrossDynamicSearchVm>
{
    public async Task<GetTruckCrossDynamicSearchVm> Handle(GetTruckCrossDynamicSearchQuery request, CancellationToken cancellationToken)
    {
        List<string> selects = new();
        List<string> groups = new();
        List<string> wheres = new();
        List<string> subWheres = new();
        List<string> orders = new();

        List<DataElement> dataMiningElementDtos = new();

        if (request.DataMiningElements.Any())
        {
            dataMiningElementDtos = wmsBusiness.SGetDataMiningElementsByIds(request.DataMiningElements.Select(p => p.Value).ToList());
        }

        HandleWheres();
       
        HandleSelect();
       
        HandleCalculating();
        
        HandleDmes();

        string command = string.Empty;

        if (request.Pivot is null)
        {
            HandleQueryNonPivotMaking();
        }
        else
        {
            HandleQueryPivotMaking();
        }

        var dt = dataAccess.SqlDataAdapter(command, 180);
        
        return new()
        {
            List = DataTableTools.DataTableToObjects(dt)
        };

        string ReplaceDataMiningElementParameters(string cmd)
        {
            var parameters = new Dictionary<string, string>
            {
                { "@WhereProductSerialDME", " (tbl_TagsDME.ProductSerial = TruckCross.fld_TruckCrossId) " },
                { "@WhereProductCodeDME", " (tbl_ProductsDME.ProductCode = TruckCrossItems.fld_TruckCrossItemProductCode) " },
                { "@WhereTruckCrossIdDME", " (tbl_TruckCrossDME.fld_TruckCrossId = TruckCross.fld_TruckCrossId) " }
            };

            foreach (var parameter in parameters)
            {
                cmd = cmd.Replace(parameter.Key, parameter.Value);
            }

            return cmd;
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
            foreach (var item in request.Calculating)
            {
                switch (item.GroupColumnType)
                {
                    case TruckCrossReportColumnsType.Id:
                        if (item.Type == ReportCalculatingColumnType.Count)
                        {
                            selects.Add($"COALESCE(COUNT(TruckCross.fld_TruckCrossId), 0) AS [{item.Title}]");
                        }
                        else if (item.Type == ReportCalculatingColumnType.Percent)
                        {
                            selects.Add($"""
                            CAST(
                            COALESCE(COUNT(TruckCross.fld_TruckCrossId), 0) * 100.0 /
                            COALESCE(
                            (SELECT COUNT(TruckCross_3.fld_TruckCrossId) 
                            FROM tbl_TruckCross AS TruckCross_3
                            LEFT OUTER JOIN tbl_User AS PresentUser_3 ON TruckCross_3.fld_TruckCrossPresentUserId = PresentUser_3.Id
                            LEFT OUTER JOIN tbl_User AS EnterUser_3 ON TruckCross_3.fld_TruckCrossEnterUserId = EnterUser_3.Id
                            LEFT OUTER JOIN tbl_User AS ExitUser_3 ON TruckCross_3.fld_TruckCrossExitUserId = ExitUser_3.Id
                            {(subWheres.Any() ? "WHERE " + string.Join(" AND ", subWheres) : "")}), 1)
                            AS decimal(10,1)) AS [{item.Title}]
                            """);
                        }
                        break;

                    case TruckCrossReportColumnsType.EnterWeightTonage:
                        if (item.Type == ReportCalculatingColumnType.Sum)
                        {
                            selects.Add($"COALESCE(SUM(TruckCross.fld_TruckCrossEnterWeightTonage), 0) AS [{item.Title}]");
                        }
                        else if (item.Type == ReportCalculatingColumnType.Avg)
                        {
                            selects.Add($"COALESCE(AVG(TruckCross.fld_TruckCrossEnterWeightTonage), 0) AS [{item.Title}]");
                        }
                        else if (item.Type == ReportCalculatingColumnType.Max)
                        {
                            selects.Add($"COALESCE(MAX(TruckCross.fld_TruckCrossEnterWeightTonage), 0) AS [{item.Title}]");
                        }
                        else if (item.Type == ReportCalculatingColumnType.Min)
                        {
                            selects.Add($"COALESCE(MIN(TruckCross.fld_TruckCrossEnterWeightTonage), 0) AS [{item.Title}]");
                        }
                        break;

                    case TruckCrossReportColumnsType.ExitWeightTonage:
                        if (item.Type == ReportCalculatingColumnType.Sum)
                        {
                            selects.Add($"COALESCE(SUM(TruckCross.fld_TruckCrossExitWeightTonage), 0) AS [{item.Title}]");
                        }
                        else if (item.Type == ReportCalculatingColumnType.Avg)
                        {
                            selects.Add($"COALESCE(AVG(TruckCross.fld_TruckCrossExitWeightTonage), 0) AS [{item.Title}]");
                        }
                        break;

                    case TruckCrossReportColumnsType.ExitPureWeightCargo:
                        selects.Add($"COALESCE(SUM(TruckCross.fld_TruckCrossExitPureWeightCargo), 0) AS [{item.Title}]");
                        break;
                }
            }
        }
        void HandleSelect()
        {
            foreach (var select in request.SelectColumns)
            {
                string groupClause = string.Empty;
                string selectTitle = select.Title;

                switch (select.Type)
                {
                    case TruckCrossReportColumnsType.Id:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossId, 0) AS [{selectTitle}]");
                        subWheres.Add("(TruckCross_3.fld_TruckCrossId = TruckCross.fld_TruckCrossId)");
                        groupClause = "TruckCross.fld_TruckCrossId";
                        break;

                    case TruckCrossReportColumnsType.TruckCrossStatus:
                        selects.Add($"""
                        CASE TruckCross.fld_TruckCrossStatus
                            WHEN 0 THEN N'{TextResources.APP_StringKeys_TruckCross_Presented}'
                            WHEN 1 THEN N'{TextResources.APP_StringKeys_TruckCross_Entered}'
                            WHEN 2 THEN N'{TextResources.APP_StringKeys_TruckCross_Exited}'
                            WHEN 3 THEN N'{TextResources.APP_StringKeys_TruckCross_Revoked}'
                            ELSE N''
                        END AS [{selectTitle}]
                        """);
                        subWheres.Add("(TruckCross_3.fld_TruckCrossStatus = TruckCross.fld_TruckCrossStatus)");
                        groupClause = "TruckCross.fld_TruckCrossStatus";
                        break;

                    case TruckCrossReportColumnsType.PersianDateFull:
                        selects.Add($"dbo.GeorgianDateToJalaliDate(TruckCross.fld_TruckCrossPresentDateTime) AS [{selectTitle}]");
                        subWheres.Add("(TruckCross_3.fld_TruckCrossPresentDateTime = TruckCross.fld_TruckCrossPresentDateTime)");
                        groupClause = "TruckCross.fld_TruckCrossPresentDateTime";
                        break;

                    case TruckCrossReportColumnsType.PersianDateYear:
                        selects.Add($"SUBSTRING(dbo.GeorgianDateToJalaliDate(TruckCross.fld_TruckCrossPresentDateTime), 1, 4) AS [{selectTitle}]");
                        groupClause = "SUBSTRING(dbo.GeorgianDateToJalaliDate(TruckCross.fld_TruckCrossPresentDateTime), 1, 4)";
                        break;

                    case TruckCrossReportColumnsType.PersianDateMonth:
                        selects.Add($"SUBSTRING(dbo.GeorgianDateToJalaliDate(TruckCross.fld_TruckCrossPresentDateTime), 6, 2) AS [{selectTitle}]");
                        groupClause = "SUBSTRING(dbo.GeorgianDateToJalaliDate(TruckCross.fld_TruckCrossPresentDateTime), 6, 2)";
                        break;

                    case TruckCrossReportColumnsType.PersianDateDay:
                        selects.Add($"SUBSTRING(dbo.GeorgianDateToJalaliDate(TruckCross.fld_TruckCrossPresentDateTime), 9, 2) AS [{selectTitle}]");
                        groupClause = "SUBSTRING(dbo.GeorgianDateToJalaliDate(TruckCross.fld_TruckCrossPresentDateTime), 9, 2)";
                        break;

                    case TruckCrossReportColumnsType.DriverName:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossDriverName, N'') AS [{selectTitle}]");
                        subWheres.Add("(TruckCross_3.fld_TruckCrossDriverName = TruckCross.fld_TruckCrossDriverName)");
                        groupClause = "TruckCross.fld_TruckCrossDriverName";
                        break;

                    case TruckCrossReportColumnsType.NationalCode:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossNationalCode, N'') AS [{selectTitle}]");
                        subWheres.Add("(TruckCross_3.fld_TruckCrossNationalCode = TruckCross.fld_TruckCrossNationalCode)");
                        groupClause = "TruckCross.fld_TruckCrossNationalCode";
                        break;

                    case TruckCrossReportColumnsType.DriverPhone:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossDriverPhone, N'') AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossDriverPhone";
                        break;

                    case TruckCrossReportColumnsType.Plaque:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossPlaque, N'') AS [{selectTitle}]");
                        subWheres.Add("(TruckCross_3.fld_TruckCrossPlaque = TruckCross.fld_TruckCrossPlaque)");
                        groupClause = "TruckCross.fld_TruckCrossPlaque";
                        break;

                    case TruckCrossReportColumnsType.TruckTypeTitle:
                        selects.Add($"COALESCE(TruckType.fld_TruckTypeTitle, N'') AS [{selectTitle}]");
                        subWheres.Add("(TruckType_3.fld_TruckTypeTitle = TruckType.fld_TruckTypeTitle)");
                        groupClause = "TruckType.fld_TruckTypeTitle";
                        break;

                    case TruckCrossReportColumnsType.PresentDateTime:
                        selects.Add($"TruckCross.fld_TruckCrossPresentDateTime AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossPresentDateTime";
                        break;

                    case TruckCrossReportColumnsType.PresentUsername:
                        selects.Add($"COALESCE(PresentUser.[Name], N'') AS [{selectTitle}]");
                        subWheres.Add("(PresentUser_3.Id = PresentUser.Id)");
                        groupClause = "PresentUser.[Name]";
                        break;

                    case TruckCrossReportColumnsType.PresentCause:
                        selects.Add($"COALESCE(PresentCause.fld_TruckCrossCauseTitle, N'') AS [{selectTitle}]");
                        subWheres.Add("(PresentCause_3.fld_TruckCrossCauseId = PresentCause.fld_TruckCrossCauseId)");
                        groupClause = "PresentCause.fld_TruckCrossCauseTitle";
                        break;

                    case TruckCrossReportColumnsType.PresentDesc:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossPresentDesc, N'') AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossPresentDesc";
                        break;

                    case TruckCrossReportColumnsType.PresentTurn:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossPresentTurn, 0) AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossPresentTurn";
                        break;

                    case TruckCrossReportColumnsType.PresentOperationTypeTitle:
                        selects.Add($"COALESCE(OperationType.fld_TruckCrossOperationTypeTitle, N'') AS [{selectTitle}]");
                        subWheres.Add("(OperationType_3.fld_TruckCrossOperationTypeId = OperationType.fld_TruckCrossOperationTypeId)");
                        groupClause = "OperationType.fld_TruckCrossOperationTypeTitle";
                        break;

                    case TruckCrossReportColumnsType.PresentShipmentTitle:
                        selects.Add($"COALESCE(Shipment.fld_TruckCrossShipmentTitle, N'') AS [{selectTitle}]");
                        subWheres.Add("(Shipment_3.fld_TruckCrossShipmentId = Shipment.fld_TruckCrossShipmentId)");
                        groupClause = "Shipment.fld_TruckCrossShipmentTitle";
                        break;

                    case TruckCrossReportColumnsType.PresentOperationDestinationTitle:
                        selects.Add($"COALESCE(OperationDestination.fld_TruckCrossOperationDestinationTitle, N'') AS [{selectTitle}]");
                        subWheres.Add("(OperationDestination_3.fld_TruckCrossOperationDestinationId = OperationDestination.fld_TruckCrossOperationDestinationId)");
                        groupClause = "OperationDestination.fld_TruckCrossOperationDestinationTitle";
                        break;

                    case TruckCrossReportColumnsType.PresentCustomerTitle:
                        selects.Add($"COALESCE(Customer.fld_TruckCrossCustomerTitle, N'') AS [{selectTitle}]");
                        subWheres.Add("(Customer_3.fld_TruckCrossCustomerId = Customer.fld_TruckCrossCustomerId)");
                        groupClause = "Customer.fld_TruckCrossCustomerTitle";
                        break;

                    case TruckCrossReportColumnsType.PresentRevokeUsername:
                        selects.Add($"COALESCE(RevokeUser.[Name], N'') AS [{selectTitle}]");
                        groupClause = "RevokeUser.[Name]";
                        break;

                    case TruckCrossReportColumnsType.PresentRevokeDateTime:
                        selects.Add($"TruckCross.fld_TruckCrossPresentRevokeDateTime AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossPresentRevokeDateTime";
                        break;

                    case TruckCrossReportColumnsType.EnterDateTime:
                        selects.Add($"TruckCross.fld_TruckCrossEnterDateTime AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossEnterDateTime";
                        break;

                    case TruckCrossReportColumnsType.EnterUsername:
                        selects.Add($"COALESCE(EnterUser.[Name], N'') AS [{selectTitle}]");
                        subWheres.Add("(EnterUser_3.Id = EnterUser.Id)");
                        groupClause = "EnterUser.[Name]";
                        break;

                    case TruckCrossReportColumnsType.EnterWeightTonage:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossEnterWeightTonage, 0) AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossEnterWeightTonage";
                        break;

                    case TruckCrossReportColumnsType.ExitDateTime:
                        selects.Add($"TruckCross.fld_TruckCrossExitDateTime AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossExitDateTime";
                        break;

                    case TruckCrossReportColumnsType.ExitUsername:
                        selects.Add($"COALESCE(ExitUser.[Name], N'') AS [{selectTitle}]");
                        subWheres.Add("(ExitUser_3.Id = ExitUser.Id)");
                        groupClause = "ExitUser.[Name]";
                        break;

                    case TruckCrossReportColumnsType.ExitWeightTonage:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossExitWeightTonage, 0) AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossExitWeightTonage";
                        break;

                    case TruckCrossReportColumnsType.ExitPureWeightCargo:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossExitPureWeightCargo, 0) AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossExitPureWeightCargo";
                        break;

                    case TruckCrossReportColumnsType.GateOperationCode:
                        selects.Add($"COALESCE(TruckCross.fld_TruckCrossGateOperationCode, N'') AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossGateOperationCode";
                        break;

                    case TruckCrossReportColumnsType.MovementActionId:
                        selects.Add($"COALESCE(CAST(TruckCross.fld_TruckCrossMovementActionId AS nvarchar(50)), N'') AS [{selectTitle}]");
                        groupClause = "TruckCross.fld_TruckCrossMovementActionId";
                        break;

                    case TruckCrossReportColumnsType.DynamicFields:
                        selects.Add($"COALESCE(JSON_VALUE(TruckCross.fld_TruckCrossDynamicFields,N'$.\"{selectTitle}\"'), N'') AS [{selectTitle}]");
                        subWheres.Add($"""
                        COALESCE(JSON_VALUE(TruckCross_3.fld_TruckCrossDynamicFields,N'$."{selectTitle}"'), N'') = 
                        COALESCE(JSON_VALUE(TruckCross.fld_TruckCrossDynamicFields,N'$."{selectTitle}"'), N'')
                        """);
                        groupClause = $"COALESCE(JSON_VALUE(TruckCross.fld_TruckCrossDynamicFields,N'$.\"{selectTitle}\"'), N'')";
                        break;
                }

                if (groupClause.HasValue() && (request.Calculating.Any() || request.Pivot is not null))
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
            foreach (var filter in request.Filters)
            {
                string subQueryWhereCommand = string.Empty;

                if (filter.SqlWhereCommand.HasValue())
                {
                    continue;
                }

                if (filter.Type.Equals(FilterType.Dynamic))
                {
                    filter.SqlWhereCommand = $"JSON_VALUE(TruckCross.fld_TruckCrossDynamicFields,N'$.\"{filter.FieldName}\"')";
                    subQueryWhereCommand = $"JSON_VALUE(TruckCross_3.fld_TruckCrossDynamicFields,N'$.\"{filter.FieldName}\"')";

                    wheres.Add(GetDynamicWhere(filter));
                    subWheres.Add(GetDynamicWhere(filter).Replace(filter.SqlWhereCommand, subQueryWhereCommand));
                }
                else if (filter.Type.Equals(FilterType.Static))
                {
                    switch (filter.FieldType)
                    {
                        case TruckCrossReportFilterType.FromDate:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossPresentDateTime";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentDateTime";
                            filter.Values = filter.Values.Select(date => $"dbo.JalaliDateToGeorgianDate(N'{date}', N'06:00')").ToList();
                            break;

                        case TruckCrossReportFilterType.ToDate:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossPresentDateTime";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentDateTime";
                            filter.Values = filter.Values.Select(date => $"dbo.JalaliDateToGeorgianDate(N'{date}', N'06:00')").ToList();
                            break;

                        case TruckCrossReportFilterType.NationalCode:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossNationalCode";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossNationalCode";
                            break;

                        case TruckCrossReportFilterType.DriverName:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossDriverName";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossDriverName";
                            break;

                        case TruckCrossReportFilterType.PlaqueFirstPart:
                        case TruckCrossReportFilterType.PlaqueCharacter:
                        case TruckCrossReportFilterType.PlaqueSecondPart:
                        case TruckCrossReportFilterType.PlaqueCityPart:
                            // Plaque parts - combine into regex pattern
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossPlaque";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPlaque";
                            break;

                        case TruckCrossReportFilterType.PresentCause:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossPresentCause";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentCause";
                            break;

                        case TruckCrossReportFilterType.PresentOperationType:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossPresentOperationType";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentOperationType";
                            break;

                        case TruckCrossReportFilterType.PresentShipment:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossPresentShipment";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentShipment";
                            break;

                        case TruckCrossReportFilterType.PresentOperationDestination:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossPresentOperationDestination";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentOperationDestination";
                            break;

                        case TruckCrossReportFilterType.PresentCustomer:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossPresentCustomer";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossPresentCustomer";
                            break;

                        case TruckCrossReportFilterType.Status:
                            filter.SqlWhereCommand = "TruckCross.fld_TruckCrossStatus";
                            subQueryWhereCommand = "TruckCross_3.fld_TruckCrossStatus";
                            break;

                        case TruckCrossReportFilterType.ProductTitle:
                            wheres.Add($@"(TruckCross.fld_TruckCrossId IN 
                            (SELECT TruckCrossItems.fld_TruckCrossItemTruckCrossId 
                            FROM tbl_TruckCrossItems AS TruckCrossItems 
                            WHERE TruckCrossItems.fld_TruckCrossItemTitle LIKE N'%{filter.Values.First()}%'))");

                            subWheres.Add($@"(TruckCross_3.fld_TruckCrossId IN 
                            (SELECT TruckCrossItems_3.fld_TruckCrossItemTruckCrossId 
                            FROM tbl_TruckCrossItems AS TruckCrossItems_3 
                            WHERE TruckCrossItems_3.fld_TruckCrossItemTitle LIKE N'%{filter.Values.First()}%'))");
                            break;
                    }

                    if (filter.SqlWhereCommand.HasValue())
                    {
                        wheres.Add(GetStaticWhere(filter));

                        if (subQueryWhereCommand.HasValue())
                        {
                            subWheres.Add(GetStaticWhere(filter).Replace(filter.SqlWhereCommand, subQueryWhereCommand));
                        }
                    }
                }
            }
        }
        void HandleQueryNonPivotMaking()
        {
            command = $"""
            SELECT {string.Join(',', selects)}
            FROM tbl_TruckCross AS TruckCross
            LEFT OUTER JOIN tbl_TruckType AS TruckType ON TruckCross.fld_TruckCrossType = TruckType.fld_TruckTypeId
            LEFT OUTER JOIN tbl_User AS PresentUser ON TruckCross.fld_TruckCrossPresentUserId = PresentUser.Id
            LEFT OUTER JOIN tbl_User AS EnterUser ON TruckCross.fld_TruckCrossEnterUserId = EnterUser.Id
            LEFT OUTER JOIN tbl_User AS ExitUser ON TruckCross.fld_TruckCrossExitUserId = ExitUser.Id
            LEFT OUTER JOIN tbl_User AS RevokeUser ON TruckCross.fld_TruckCrossPresentRevokeUserId = RevokeUser.Id
            LEFT OUTER JOIN tbl_TruckCrossCause AS PresentCause ON TruckCross.fld_TruckCrossPresentCause = PresentCause.fld_TruckCrossCauseId
            LEFT OUTER JOIN tbl_TruckCrossOperationType AS OperationType ON TruckCross.fld_TruckCrossPresentOperationType = OperationType.fld_TruckCrossOperationTypeId
            LEFT OUTER JOIN tbl_TruckCrossShipment AS Shipment ON TruckCross.fld_TruckCrossPresentShipment = Shipment.fld_TruckCrossShipmentId
            LEFT OUTER JOIN tbl_TruckCrossOperationDestination AS OperationDestination ON TruckCross.fld_TruckCrossPresentOperationDestination = OperationDestination.fld_TruckCrossOperationDestinationId
            LEFT OUTER JOIN tbl_TruckCrossCustomer AS Customer ON TruckCross.fld_TruckCrossPresentCustomer = Customer.fld_TruckCrossCustomerId
            {(wheres.Any() ? "WHERE " + string.Join(" AND ", wheres) : "")}
            {(groups.Any() ? "GROUP BY " + string.Join(" , ", groups) : "")}
            {(orders.Any() ? "ORDER BY " + string.Join(" , ", orders) : "")}
            """
            ;
        }
        void HandleQueryPivotMaking()
        {
            var pivotColumnClause = "";
            var pivotFor = "";
            var pivotColumn = "";
            var pivotGroup = "";

            switch (request.Pivot.Type)
            {
                case TruckCrossReportColumnsType.TruckCrossStatus:
                    pivotColumnClause = $"""
                    SELECT @columns += '[' + StatusName + ']' 
                    FROM (SELECT DISTINCT 
                        CASE fld_TruckCrossStatus
                            WHEN 0 THEN N'{TextResources.APP_StringKeys_TruckCross_Presented}'
                            WHEN 1 THEN N'{TextResources.APP_StringKeys_TruckCross_Entered}'
                            WHEN 2 THEN N'{TextResources.APP_StringKeys_TruckCross_Exited}'
                            WHEN 3 THEN N'{TextResources.APP_StringKeys_TruckCross_Revoked}'
                        END AS StatusName
                    FROM tbl_TruckCross WHERE fld_TruckCrossStatus IS NOT NULL) as [NESTED];
                    """;
                    pivotFor = "[Nested].[Status]";
                    pivotColumn = $"""
                    CASE TruckCross.fld_TruckCrossStatus
                        WHEN 0 THEN N'{TextResources.APP_StringKeys_TruckCross_Presented}'
                        WHEN 1 THEN N'{TextResources.APP_StringKeys_TruckCross_Entered}'
                        WHEN 2 THEN N'{TextResources.APP_StringKeys_TruckCross_Exited}'
                        WHEN 3 THEN N'{TextResources.APP_StringKeys_TruckCross_Revoked}'
                    END
                    """;
                    subWheres.Add("(TruckCross_3.fld_TruckCrossStatus = TruckCross.fld_TruckCrossStatus)");
                    pivotGroup = "TruckCross.fld_TruckCrossStatus";
                    break;

                case TruckCrossReportColumnsType.TruckTypeTitle:
                    pivotColumnClause = """
                    SELECT @columns += '[' + [NESTED].fld_TruckTypeTitle + '],' 
                    FROM (SELECT DISTINCT fld_TruckTypeTitle 
                    FROM tbl_TruckType 
                    WHERE fld_TruckTypeTitle IS NOT NULL AND fld_TruckTypeTitle != '') as [NESTED];
                    """;
                    pivotFor = "[Nested].[Type]";
                    pivotColumn = "TruckType.fld_TruckTypeTitle";
                    subWheres.Add("(TruckType_3.fld_TruckTypeTitle = TruckType.fld_TruckTypeTitle)");
                    pivotGroup = pivotColumn;
                    break;

                case TruckCrossReportColumnsType.PresentCause:
                    pivotColumnClause = """
                    SELECT @columns += '[' + [NESTED].fld_TruckCrossCauseTitle + '],' 
                    FROM (SELECT DISTINCT fld_TruckCrossCauseTitle 
                    FROM tbl_TruckCrossCause 
                    WHERE fld_TruckCrossCauseTitle IS NOT NULL AND fld_TruckCrossCauseTitle != '') as [NESTED];
                    """;
                    pivotFor = "[Nested].[Cause]";
                    pivotColumn = "PresentCause.fld_TruckCrossCauseTitle";
                    subWheres.Add("(PresentCause_3.fld_TruckCrossCauseTitle = PresentCause.fld_TruckCrossCauseTitle)");
                    pivotGroup = pivotColumn;
                    break;

                case TruckCrossReportColumnsType.PresentOperationTypeTitle:
                    pivotColumnClause = """
                    SELECT @columns += '[' + [NESTED].fld_TruckCrossOperationTypeTitle + '],' 
                    FROM (SELECT DISTINCT fld_TruckCrossOperationTypeTitle 
                    FROM tbl_TruckCrossOperationType 
                    WHERE fld_TruckCrossOperationTypeTitle IS NOT NULL AND fld_TruckCrossOperationTypeTitle != '') as [NESTED];
                    """;
                    pivotFor = "[Nested].[OpType]";
                    pivotColumn = "OperationType.fld_TruckCrossOperationTypeTitle";
                    subWheres.Add("(OperationType_3.fld_TruckCrossOperationTypeTitle = OperationType.fld_TruckCrossOperationTypeTitle)");
                    pivotGroup = pivotColumn;
                    break;

                case TruckCrossReportColumnsType.PresentShipmentTitle:
                    pivotColumnClause = """
                    SELECT @columns += '[' + [NESTED].fld_TruckCrossShipmentTitle + '],' 
                    FROM (SELECT DISTINCT fld_TruckCrossShipmentTitle 
                    FROM tbl_TruckCrossShipment 
                    WHERE fld_TruckCrossShipmentTitle IS NOT NULL AND fld_TruckCrossShipmentTitle != '') as [NESTED];
                    """;
                    pivotFor = "[Nested].[Shipment]";
                    pivotColumn = "Shipment.fld_TruckCrossShipmentTitle";
                    subWheres.Add("(Shipment_3.fld_TruckCrossShipmentTitle = Shipment.fld_TruckCrossShipmentTitle)");
                    pivotGroup = pivotColumn;
                    break;

                case TruckCrossReportColumnsType.PresentCustomerTitle:
                    pivotColumnClause = """
                    SELECT @columns += '[' + [NESTED].fld_TruckCrossCustomerTitle + '],' 
                    FROM (SELECT DISTINCT fld_TruckCrossCustomerTitle 
                    FROM tbl_TruckCrossCustomer 
                    WHERE fld_TruckCrossCustomerTitle IS NOT NULL AND fld_TruckCrossCustomerTitle != '') as [NESTED];
                    """;
                    pivotFor = "[Nested].[Customer]";
                    pivotColumn = "Customer.fld_TruckCrossCustomerTitle";
                    subWheres.Add("(Customer_3.fld_TruckCrossCustomerTitle = Customer.fld_TruckCrossCustomerTitle)");
                    pivotGroup = pivotColumn;
                    break;
            }

            groups.Add(pivotGroup);

            command = $"""
            declare @columns nvarchar(max) = '', @sqlcmd NVARCHAR(MAX) = '';
            {pivotColumnClause}
            IF @columns = ''
            BEGIN 
            RETURN
            END
            SET @columns = LEFT(@columns, LEN(@columns) - 1); 
            SET @sqlcmd = N'
            SELECT * FROM
            (SELECT 
                COALESCE(COUNT(TruckCross.fld_TruckCrossId), 0) as [Count],
                {pivotColumn.Replace("'", "''")} AS [PivotValue],
                {string.Join(",", selects.Select(p => p.Replace("'", "''")))}
            FROM tbl_TruckCross AS TruckCross
            LEFT OUTER JOIN tbl_TruckType AS TruckType ON TruckCross.fld_TruckCrossType = TruckType.fld_TruckTypeId
            LEFT OUTER JOIN tbl_User AS PresentUser ON TruckCross.fld_TruckCrossPresentUserId = PresentUser.Id
            LEFT OUTER JOIN tbl_User AS EnterUser ON TruckCross.fld_TruckCrossEnterUserId = EnterUser.Id
            LEFT OUTER JOIN tbl_User AS ExitUser ON TruckCross.fld_TruckCrossExitUserId = ExitUser.Id
            LEFT OUTER JOIN tbl_TruckCrossCause AS PresentCause ON TruckCross.fld_TruckCrossPresentCause = PresentCause.fld_TruckCrossCauseId
            LEFT OUTER JOIN tbl_TruckCrossOperationType AS OperationType ON TruckCross.fld_TruckCrossPresentOperationType = OperationType.fld_TruckCrossOperationTypeId
            LEFT OUTER JOIN tbl_TruckCrossShipment AS Shipment ON TruckCross.fld_TruckCrossPresentShipment = Shipment.fld_TruckCrossShipmentId
            LEFT OUTER JOIN tbl_TruckCrossOperationDestination AS OperationDestination ON TruckCross.fld_TruckCrossPresentOperationDestination = OperationDestination.fld_TruckCrossOperationDestinationId
            LEFT OUTER JOIN tbl_TruckCrossCustomer AS Customer ON TruckCross.fld_TruckCrossPresentCustomer = Customer.fld_TruckCrossCustomerId
            {(wheres.Any() ? "WHERE " + string.Join(" AND ", wheres.Select(p => p.Replace("'", "''"))) : "")}
            {(groups.Any() ? "GROUP BY " + string.Join(" , ", groups.Select(p => p.Replace("'", "''"))) : "")}) AS [Nested]
            pivot (SUM([Nested].[Count])
            FOR {pivotFor} IN ('+@columns+') ) as PivotData ';
            EXECUTE sp_executesql @sqlcmd;
            """;
        }
    }

    private static string GetStaticWhere(ReportFilterGeneric<TruckCrossReportFilterType> filter)
    {
        string where = string.Empty;

        if (filter.EqualityType == FilterEqualityType.Equals)
        {
            where += $" {filter.SqlWhereCommand} IN('{string.Join("','", filter.Values)}')  ";
        }
        else if (filter.EqualityType == FilterEqualityType.Like)
        {
            foreach (var value in filter.Values)
            {
                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} LIKE N'%{value}%') ";
            }
        }
        else
        {
            foreach (var value in filter.Values)
            {
                string operatorString = filter.EqualityType switch
                {
                    FilterEqualityType.SmallerThan => "<=",
                    FilterEqualityType.BiggerThan => ">=",
                };

                if (where.HasValue())
                {
                    where += " OR ";
                }

                if (value.StartsWith("dbo.JalaliDateToGeorgianDate"))
                {
                    where += $" ({filter.SqlWhereCommand} {operatorString} {value}) ";
                }
                else
                {
                    where += $" ({filter.SqlWhereCommand} {operatorString} N'{value}') ";
                }
            }
        }

        return "(" + where + ")";
    }

    private static string GetDynamicWhere(ReportFilterGeneric<TruckCrossReportFilterType> filter)
    {
        string where = string.Empty;

        if (filter.EqualityType == FilterEqualityType.Equals)
        {
            where = $" {filter.SqlWhereCommand} IN(N'{string.Join("',N'", filter.Values)}')  ";
        }
        else if (filter.EqualityType == FilterEqualityType.Like)
        {
            foreach (var value in filter.Values)
            {
                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} LIKE N'%{value}%') ";
            }
        }
        else
        {
            foreach (var value in filter.Values)
            {
                string operatorString = filter.EqualityType switch
                {
                    FilterEqualityType.SmallerThan => "<=",
                    FilterEqualityType.BiggerThan => ">=",
                };

                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} {operatorString} N'{value}') ";
            }
        }

        return "(" + where + ")";
    }

    private static string GetTechnicalInfoWhere(ReportFilterGeneric<TruckCrossReportFilterType> filter)
    {
        string where = string.Empty;

        if (filter.EqualityType == FilterEqualityType.Equals)
        {
            where = $" {filter.SqlWhereCommand} IN('{string.Join("','", filter.Values)}')  ";
        }
        else if (filter.EqualityType == FilterEqualityType.Like)
        {
            foreach (var value in filter.Values)
            {
                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} LIKE N'%{value}%') ";
            }
        }
        else
        {
            foreach (var value in filter.Values)
            {
                string operatorString = filter.EqualityType switch
                {
                    FilterEqualityType.SmallerThan => "<=",
                    FilterEqualityType.BiggerThan => ">=",
                };

                if (where.HasValue())
                {
                    where += " OR ";
                }

                where += $" ({filter.SqlWhereCommand} {operatorString} N'{value}') ";
            }
        }

        return "(" + where + ")";
    }
}

