using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Silo.Application.Contracts;
using Silo.Domains.Entities;
using Silo.Domains.Services;

namespace Silo.Api.Business;
public class DocumentBusiness(ILogger<DocumentBusiness> logger
        , IDataAccess dataAccess
        , IHttpContextAccessor httpContextAccessor
        , DocumentLogBusiness documentLog
        , IConfiguration configuration
        , IMapper mapper
        , WmsApiContext apiContext) : ProjectBusiness(dataAccess, logger, httpContextAccessor)
{
    #region Document Manage
    public DataTable SGetAllDocumentType()
    {
        string command =
            $"""
            SELECT DISTINCT fld_DocumentType  as Code ,tbl_ActionTypes.fld_ActionTypeTitle AS Title
            FROM tbl_DocumentHeader INNER JOIN tbl_ActionTypes ON tbl_DocumentHeader.fld_DocumentType = tbl_ActionTypes.fld_ActionTypeId
            """;

        var result = dataAccess.SqlDataAdapter(command);

        return result;
    }

    public List<DocumentStatus> SGetAllDocumentStatus()
    => apiContext.DocumentStatuses.ToList();

    public DocumentHeader SGetDocumentHeaderAndItems(string documentKey, string documentType)
    => apiContext.DocumentHeaders
            .Include(header => header.DocumentItems.Where(p => p.Key == documentKey && p.DocumentType == documentType))
            .Where(header => header.DocumentType == documentType.ToString() && header.Key == documentKey)
            .FirstOrDefault();

    public List<DocumentItemDto> SGetAllDocItems(string docKey, string docType)
    => apiContext.DocumentItems.Where(p => p.Key == docKey && p.DocumentType == docType)
                                .Select(p => new DocumentItemDto()
                                {
                                    ProductCode = p.ProductCode,
                                    ProductTitle = p.ProductTitle,
                                    Count = p.Count,
                                    ProductUnit = p.ProductUnit
                                }).ToList();
    #endregion

    #region Document Aggregate And Revoke
    public DocumentHeader SAggregateDocuments(string docAggCode, string documentType, int documentStatus,string description)
    {
        string userId = httpContext.User.GetUserId();

        List<string> docKeys = docAggCode.Split('-').ToList();

        DocumentHeader document = apiContext.DocumentHeaders
                                                .FirstOrDefault(p => docKeys.Any(q => q == p.Key)
                                                                         && p.DocumentType == documentType
                                                                         && p.DocumentStatusId == documentStatus);

        if (document is null)
        {
            logger.LogInformation("SAggregateDocuments => No document found from db");

            return null;
        }

        if (docKeys.Any())
        {
            List<string> commands = new();

            commands.Add($"""
                DELETE FROM tbl_DocumentHeader 
                WHERE tbl_DocumentHeader.fld_DocumentKey IN (
                SELECT DISTINCT fld_DocumentParent
                FROM tbl_DocumentHeader
                WHERE tbl_DocumentHeader.fld_DocumentParent <> '0' AND
                	  tbl_DocumentHeader.fld_DocumentType = '{documentType}' AND
                	  tbl_DocumentHeader.fld_DocumentStatus = {documentStatus} AND 
                	  tbl_DocumentHeader.fld_DocumentKey IN('{string.Join("','", docKeys)}')
                	  ) 
                """);

            commands.Add($"""
                DELETE FROM tbl_DocumentItem 
                WHERE tbl_DocumentItem.fld_DocumentKey IN (
                SELECT DISTINCT fld_DocumentParent
                FROM tbl_DocumentHeader
                WHERE tbl_DocumentHeader.fld_DocumentParent <> '0' AND
                	  tbl_DocumentHeader.fld_DocumentType = '{documentType}' AND
                	  tbl_DocumentHeader.fld_DocumentStatus = {documentStatus} AND 
                	  tbl_DocumentHeader.fld_DocumentKey IN('{string.Join("','", docKeys)}')
                	  ) 
                """);

            commands.Add($"""
                UPDATE  tbl_DocumentHeader
                SET fld_DocumentParent = '{docAggCode}'
                WHERE tbl_DocumentHeader.fld_DocumentType = '{documentType}' AND
                	  tbl_DocumentHeader.fld_DocumentStatus = {documentStatus} AND 
                	  tbl_DocumentHeader.fld_DocumentKey IN('{string.Join("','", docKeys)}')
                """);

            DocumentHeader newDocument = new()
            {
                Key = docAggCode,
                UserId = userId,
                ImportType = ImportType.Aggregate,
                ImportDateTime = DateTime.Now,
                DocumentStatusId = document.DocumentStatusId,
                DocumentType = document.DocumentType,
                DocumentType1 = document.DocumentType1,
                DocumentType2 = document.DocumentType2,
                Description = document.Description,
                HeaderData = document.HeaderData,
                AggStatus = 2,
                Parent = "0",
                FileName = "Agg_" + docAggCode + Guid.NewGuid().ToString().Substring(0, 6)
            };

            commands.Add(
                $"""
                INSERT INTO tbl_DocumentHeader 
                (fld_DocumentKey, fld_DocumentSaveUserId, fld_DocumentImportType,
                fld_DocumentImportDatetime,fld_DocumentStatus,
                fld_DocumentType,fld_DocumentType1,fld_DocumentType2,
                fld_DocumentDesc,fld_DocumentHeaderData,fld_DocumentAggStatus,
                fld_DocumentParent,fld_DocumentImportFileName)
                VALUES 
                (N'{newDocument.Key}', N'{newDocument.UserId}', N'{(int)newDocument.ImportType}',
                N'{newDocument.ImportDateTime}',N'{newDocument.DocumentStatusId}',
                N'{newDocument.DocumentType}',N'{newDocument.DocumentType1}',N'{newDocument.DocumentType2}',
                N'{newDocument.Description}',N'{newDocument.HeaderData}',N'{newDocument.AggStatus}',
                N'{newDocument.Parent}',N'{newDocument.FileName}');
                """);

            newDocument.DocumentItems = new List<DocumentItem>();

            var aggItems = apiContext.DocumentItems
                            .Where(p => docKeys.Any(q => q == p.Key))
                            .GroupBy(p => p.ProductCode).ToList();

            foreach (var aggItem in aggItems)
            {
                DocumentItem newDocItem = new()
                {
                    Key = docAggCode,
                    DocumentType = aggItem.First().DocumentType,
                    DocumentType1 = aggItem.First().DocumentType1,
                    DocumentType2 = aggItem.First().DocumentType2,
                    ProductCode = aggItem.First().ProductCode,
                    ProductTitle = aggItem.First().ProductTitle,
                    ProductUnit = aggItem.First().ProductUnit,
                    ItemData = "[]"
                };

                foreach (var item in aggItem)
                {
                    newDocItem.Count += item.Count;
                }

                commands.Add(
                    $"""
                    INSERT INTO tbl_DocumentItem 
                    (fld_DocumentKey, fld_DocumentItemProductCode, 
                    fld_DocumentItemCount, fld_DocumentItemProductTitle, 
                    fld_DocumentType, fld_DocumentType1, fld_DocumentType2,
                    fld_DocumentItemProducUnit, fld_DocumentItemsData)
                    VALUES 
                    (N'{newDocItem.Key}', N'{newDocItem.ProductCode}',
                    N'{newDocItem.Count}', N'{newDocItem.ProductTitle}',
                    N'{newDocItem.DocumentType}',N'{newDocItem.DocumentType1}',N'{newDocItem.DocumentType2}',
                    N'{newDocItem.ProductUnit}',N'{newDocItem.ItemData}');
                    """);
            }

            if (dataAccess.CmdSqlExecuteNonQueryWithTransaction(commands) > 0)
            {
                List<DocumentKeyTypeDto> changedDocuments = new()
                {
                    new()
                    {
                        Key = docAggCode,
                        Type = documentType
                    }
                };

                documentLog.SLogDocumentEvents(new()
                {
                    DocKeyTypes = changedDocuments,
                    EventType = DocumentEventType.Aggregate,
                    Status = documentStatus,
                    Description = description,
                    UserId= userId
                });

                return newDocument;
            }
        }
        return null;

    }

    public DataTable SGetDocAggSuggestsByDocumentTypeAndStatus(string documentType, int documentStatus)
    {
        string userId = httpContext.User.GetUserId();

        string documentGroupFields = configuration.GetSection("ProjectConfigs").GetSection("WmsConfigs")["DocumentGroupFields"];

        var docGroupFields = apiContext.DynamicFields
            .Where(p => p.ActionType == int.Parse(documentType) && p.IsDocAggregateField)
            .ToList();

        if (docGroupFields.Any())
        {
            string subSelectString = "";

            string selectString = "";

            List<string> groupByString = new();

            string whereString = $"""
            WHERE  (tbl_DocumentHeader.fld_DocumentStatus = {documentStatus}) AND tbl_DocumentHeader.fld_DocumentParent = N'0'
            AND tbl_DocumentHeader.fld_DocumentAggStatus IN(1,2)
            """;

            int valueSelectIndex = 1;

            foreach (string groupField in docGroupFields.Select(p => p.Title))
            {
                if (whereString.HasValue())
                {
                    whereString += " AND ";
                }

                whereString += $"JSON_VALUE(fld_DocumentHeaderData, N'$.\"{groupField}\"') IS NOT NULL";

                groupByString.Add($"JSON_VALUE(fld_DocumentHeaderData, N'$.\"{groupField}\"')");

                selectString += $",JSON_VALUE(fld_DocumentHeaderData, N'$.\"{groupField}\"') as GroupDataValue{valueSelectIndex}";

                subSelectString += $",[NESTED].GroupDataValue{valueSelectIndex}";

                valueSelectIndex++;
            }

            groupByString.Add("tbl_DocumentHeader.fld_DocumentType");

            var cmd = $@"SELECT	(SELECT STRING_AGG([NESTED2].value,'+') 
					        FROM (SELECT DISTINCT *
					        FROM STRING_SPLIT([NESTED].NewCode,'+')
					        )AS [NESTED2] ) AS DocAggCode,
							[NESTED].[DocumentType],
				            [NESTED].[DocumentCount],
				            CAST([NESTED].[ItemSum] AS decimal(18, 2)) AS ItemSum
                {subSelectString}
                FROM(
	                SELECT        STRING_AGG ( CAST(tbl_DocumentHeader.fld_DocumentKey AS NVARCHAR(MAX)), '+') WITHIN GROUP(ORDER BY tbl_DocumentHeader.fld_DocumentKey) AS NewCode,
				                  SUM(tbl_DocumentItem.fld_DocumentItemCount) AS ItemSum, 
				                  COUNT(DISTINCT tbl_DocumentHeader.fld_DocumentKey) AS DocumentCount,
                                  tbl_DocumentHeader.fld_DocumentType AS DocumentType
                                  {selectString}
	                FROM          tbl_DocumentHeader LEFT OUTER JOIN
							      tbl_DocumentItem ON tbl_DocumentHeader.fld_DocumentKey = tbl_DocumentItem.fld_DocumentKey
                                                  AND tbl_DocumentHeader.fld_DocumentType = tbl_DocumentItem.fld_DocumentType
	                {whereString}
                    {(groupByString.Any() ? "GROUP BY " + string.Join(',', groupByString) : "")}
                ) as [NESTED]
                WHERE [NESTED].[DocumentCount] > 1 AND [NESTED].[DocumentType] = {documentType}
                    ";

            return dataAccess.SqlDataAdapter(cmd);
        }

        return new();
    }

    public DataTable SGetAllDocAggSuggestDetailByAggCode(string aggCode, string documentType, int documentStatus)
    {
        string userId = httpContext.User.GetUserId();

        List<string> documentKeys = new();

        if (aggCode.HasValue())
        {
            foreach (var code in aggCode.Split('+').ToList())
            {
                documentKeys.Add($" tbl_DocumentHeader.fld_DocumentKey = '{code}' ");
            }
        }

        var command = $"""
            SELECT DISTINCT tbl_DocumentHeader.fld_DocumentKey AS DocumentKey,
            tbl_DocumentHeader.fld_DocumentType AS DocumentType,
            tbl_DocumentHeader.fld_DocumentImportDatetime AS ImportDateTime,
            tbl_DocumentHeader.fld_DocumentHeaderData AS DocumentData,
            COUNT(tbl_DocumentItem.fld_Id) AS ItemCount,
            SUM(tbl_DocumentItem.fld_DocumentItemCount) AS ItemSum

            FROM tbl_DocumentHeader INNER JOIN tbl_DocumentItem ON tbl_DocumentHeader.fld_DocumentKey = tbl_DocumentItem.fld_DocumentKey 
            												   AND tbl_DocumentHeader.fld_DocumentType = tbl_DocumentItem.fld_DocumentType
            WHERE
            tbl_DocumentHeader.fld_DocumentType = '{documentType}' AND tbl_DocumentHeader.fld_DocumentStatus = '{documentStatus}' AND fld_DocumentParent = '0'
            {(documentKeys.Any() ? " AND (" + string.Join(" OR ", documentKeys) + ")" : "")}
            GROUP BY tbl_DocumentHeader.fld_DocumentKey, tbl_DocumentHeader.fld_DocumentType, 
                     tbl_DocumentHeader.fld_DocumentImportDatetime, tbl_DocumentHeader.fld_DocumentHeaderData
            """;

        return dataAccess.SqlDataAdapter(command);
    }

    public List<GetAllAggDocDto> SGetAggDocsByDocTypeAndStatus(string documentType, int documentStatus)
    {
        string userId = httpContext.User.GetUserId();

        return apiContext.DocumentHeaders
                                .Include(p => p.DocumentItems.Where(q => q.DocumentType == documentType))
                                .Where(p => p.DocumentStatusId == documentStatus && p.DocumentType == documentType && p.Parent == "0"
                                         && (p.AggStatus == 1 || p.AggStatus == 2))
                                .Select(p => new GetAllAggDocDto
                                {
                                    DocumentKey = p.Key,
                                    DocumentType = p.DocumentType,
                                    ImportDateTime = p.ImportDateTime,
                                    ItemCount = p.DocumentItems.Count,
                                    ItemSum = p.DocumentItems.Sum(q => q.Count),
                                    DocumentData = p.HeaderData,
                                    DocumentItems = p.DocumentItems
                                })
                                .ToList();
    }

    public List<DocumentHeader> SGetAllAggregatedDocs(string documentType, int documentStatus)
    => apiContext.DocumentHeaders.Where(p => p.DocumentStatusId == documentStatus
                                                      && p.DocumentType == documentType
                                                      && p.Parent == "0"
                                                      && p.AggStatus == 2)
                                 .ToList();

    public DataTable SGetAggregatedDocDetailsByAggCode(string aggCode, string documentType, int documentStatus)
    {
        List<string> documentKeys = new();

        var command = $"""
            SELECT DISTINCT tbl_DocumentHeader.fld_DocumentKey AS DocumentKey,
            tbl_DocumentHeader.fld_DocumentType AS DocumentType,
            tbl_DocumentHeader.fld_DocumentImportDatetime AS ImportDateTime,
            tbl_DocumentHeader.fld_DocumentHeaderData AS DocumentData,
            COUNT(tbl_DocumentItem.fld_Id) AS ItemCount,
            SUM(tbl_DocumentItem.fld_DocumentItemCount) AS ItemSum

            FROM tbl_DocumentHeader INNER JOIN tbl_DocumentItem ON tbl_DocumentHeader.fld_DocumentKey = tbl_DocumentItem.fld_DocumentKey 
            												   AND tbl_DocumentHeader.fld_DocumentType = tbl_DocumentItem.fld_DocumentType
            WHERE
            tbl_DocumentHeader.fld_DocumentType = '{documentType}' AND 
            tbl_DocumentHeader.fld_DocumentStatus = '{documentStatus}' AND fld_DocumentParent = '{aggCode}'
            GROUP BY tbl_DocumentHeader.fld_DocumentKey, tbl_DocumentHeader.fld_DocumentType, 
                     tbl_DocumentHeader.fld_DocumentImportDatetime, tbl_DocumentHeader.fld_DocumentHeaderData
            """;

        return dataAccess.SqlDataAdapter(command);
    }

    public List<string> SGetDocumentGroupFields(string documentType)
    => apiContext.DynamicFields
                 .Where(p => p.ActionType == int.Parse(documentType) && p.IsDocAggregateField)
                 .Select(p => p.Title)
                 .ToList();
    #endregion

    #region Dynamic Field
    public List<DynamicFieldDto> GetDynamicFieldsBySectionId(int sectionId)
    {
        var result = apiContext.DynamicFields
                               .Where(p => p.SectionId == sectionId)
                               .Include(field => field.User)
                               .OrderByDescending(p => p.Id);

        return mapper.Map<List<DynamicFieldDto>>(result).ToList();
    }

    public List<DynamicFieldDto> SGetAllDynamicFields()
    {
        var result = apiContext.DynamicFields
                               .Include(field => field.User)
                               .OrderByDescending(p => p.Id);

        return mapper.Map<List<DynamicFieldDto>>(result).ToList();
    }

    public List<DynamicFieldDto> SGetDynamicFieldsByActionTypeId(int actionTypeId)
    {
        var result = apiContext.DynamicFields
                               .Where(p => p.ActionType == actionTypeId)
                               .OrderByDescending(p => p.Id);

        return mapper.Map<List<DynamicFieldDto>>(result).ToList();
    }

    public int SSaveDynamicField(DynamicField dynamicField)
    {
        var userId = httpContext.User.GetUserId();

        if (dynamicField.Id == 0)
        {
            dynamicField.UserId = userId;

            apiContext.Add(dynamicField);

            if (apiContext.SaveChanges() == 1)
            {
                return int.Parse(GetLatestIdOfIdentityTable("tbl_DynamicFields"));
            }
        }
        else
        {
            apiContext.Update(dynamicField);

            return apiContext.SaveChanges();
        }

        return 0;
    }

    public bool SDeleteDynamicField(int dynamicFieldId)
    {
        apiContext.DynamicFields.Remove(new()
        {
            Id = dynamicFieldId
        });

        return apiContext.SaveChanges() >= 1;
    }
    #endregion

    #region Dynamic Field Section
    public List<GetAllDynamicFieldSectionsVm> GetAllDynamicFieldSections()
    {
        return mapper.Map<List<GetAllDynamicFieldSectionsVm>>(apiContext.DynamicFieldSections);
    }
    #endregion
}
