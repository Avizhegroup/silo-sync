using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Silo.Application.Contracts;
using Silo.Application.Features;
using Silo.Domains.Entities;
using Silo.Domains.Services;

namespace Silo.Api.Business;
public class DocumentLogBusiness : ProjectBusiness
{
    private readonly IConfiguration configuration;
    private readonly WmsApiContext apiContext;
    private readonly ILogger<DocumentLogBusiness> logger;
    private readonly IDataAccess dataAccess;
    private readonly IMapper mapper;

    public DocumentLogBusiness(ILogger<DocumentLogBusiness> logger
        , IDataAccess dataAccess
        , IHttpContextAccessor httpContextAccessor
        , IConfiguration configuration
        , IMapper mapper
        , WmsApiContext apiContext) : base(dataAccess, logger, httpContextAccessor)
    {
        this.apiContext = apiContext;
        this.logger = logger;
        this.dataAccess = dataAccess;
        this.mapper = mapper;
        this.configuration = configuration;
    }

    public bool SLogDocumentEvents(SaveDocumentLogCommand eventLog)
    {
        if (eventLog.UserId.HasNoValue())
        {
            eventLog.UserId = httpContext.User.GetUserId();
        }

        var documentStatuses = apiContext.DocumentStatuses.ToList();

        switch (eventLog.EventType)
        {
            case DocumentEventType.InsertDocument:
                foreach (var mainDocumentKeyType in eventLog.DocKeyTypes)
                {
                    var currentDoc = apiContext.DocumentHeaders.Where(p => p.Key == mainDocumentKeyType.Key &&
                                                                     (p.DocumentType == mainDocumentKeyType.Type ||
                                                                      p.DocumentType1 == mainDocumentKeyType.Type ||
                                                                      p.DocumentType2 == mainDocumentKeyType.Type))
                                                               .ToList();

                    apiContext.DocumentLogs.Add(new()
                    {
                        Key = mainDocumentKeyType.Key,
                        DocumentType = currentDoc.Any() ? currentDoc.First().DocumentType : mainDocumentKeyType.Type,
                        Status = eventLog.Status,
                        UserId = eventLog.UserId,
                        DateTime = DateTime.Now,
                        ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                        EventType = (int)DocumentEventType.InsertDocument,
                        Description = eventLog.Description
                    });
                }
                break;
            case DocumentEventType.ChangeStatusForward:
                foreach (var currentChangedDocumentKeyType in eventLog.DocKeyTypes)
                {
                    var currentDoc = apiContext.DocumentHeaders.FirstOrDefault(p => p.Key == currentChangedDocumentKeyType.Key &&
                                                                         (p.DocumentType == currentChangedDocumentKeyType.Type ||
                                                                          p.DocumentType1 == currentChangedDocumentKeyType.Type ||
                                                                          p.DocumentType2 == currentChangedDocumentKeyType.Type));

                    if (currentDoc is not null)
                    {
                        if (currentDoc.AggStatus == 1)
                        {
                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = currentChangedDocumentKeyType.Key,
                                DocumentType = currentChangedDocumentKeyType.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.ChangeStatusMasterForward,
                                Description = eventLog.Description
                            });
                        }
                        else if (currentDoc.AggStatus == 2)
                        {
                            //Log aggregated document
                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = currentChangedDocumentKeyType.Key,
                                DocumentType = currentChangedDocumentKeyType.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.ChangeStatusMasterForward,
                                Description = eventLog.Description
                            });

                            //Log main documents
                            foreach (var key in currentChangedDocumentKeyType.Key.Split('-'))
                            {
                                apiContext.DocumentLogs.Add(new()
                                {
                                    Key = key,
                                    DocumentType = currentChangedDocumentKeyType.Type,
                                    Status = eventLog.Status,
                                    UserId = eventLog.UserId,
                                    DateTime = DateTime.Now,
                                    ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                    EventType = (int)DocumentEventType.ChangeStatusForward,
                                    Description = eventLog.Description
                                });
                            }
                        }
                        else if (currentDoc.AggStatus == 3 || currentDoc.AggStatus == 4)
                        {
                            //Log divided document change status
                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = currentChangedDocumentKeyType.Key,
                                DocumentType = currentChangedDocumentKeyType.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.ChangeStatusMasterForward,
                                Description = eventLog.Description
                            });

                            var mainDocument = apiContext.DocumentHeaders.FirstOrDefault(p => p.Key == currentDoc.DivideParent &&
                                                      (p.DocumentType == currentChangedDocumentKeyType.Type ||
                                                       p.DocumentType1 == currentChangedDocumentKeyType.Type ||
                                                       p.DocumentType2 == currentChangedDocumentKeyType.Type));

                            if (mainDocument.DocumentStatusId == eventLog.Status)
                            {
                                //Log main document based on min change status
                                apiContext.DocumentLogs.Add(new()
                                {
                                    Key = mainDocument.Key,
                                    DocumentType = currentChangedDocumentKeyType.Type,
                                    Status = eventLog.Status,
                                    UserId = eventLog.UserId,
                                    DateTime = DateTime.Now,
                                    ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                    EventType = (int)DocumentEventType.ChangeStatusForward,
                                    Description = eventLog.Description
                                });
                            }
                        }
                    }
                }
                break;
            case DocumentEventType.ChangeStatusBackward:
                foreach (var currentChangedDocumentKeyType in eventLog.DocKeyTypes)
                {
                    var currentDoc = apiContext.DocumentHeaders.FirstOrDefault(p => p.Key == currentChangedDocumentKeyType.Key &&
                                                                          (p.DocumentType == currentChangedDocumentKeyType.Type ||
                                                                           p.DocumentType1 == currentChangedDocumentKeyType.Type ||
                                                                           p.DocumentType2 == currentChangedDocumentKeyType.Type));
                    if (currentDoc is not null)
                    {
                        if (currentDoc.AggStatus == 1)
                        {
                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = currentChangedDocumentKeyType.Key,
                                DocumentType = currentChangedDocumentKeyType.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.ChangeStatusMasterBackward,
                                Description = eventLog.Description
                            });
                        }
                        else if (currentDoc.AggStatus == 2)
                        {
                            //Log aggregated document
                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = currentChangedDocumentKeyType.Key,
                                DocumentType = currentChangedDocumentKeyType.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.ChangeStatusMasterBackward,
                                Description = eventLog.Description
                            });

                            //Log main documents
                            foreach (var key in currentChangedDocumentKeyType.Key.Split('-'))
                            {
                                apiContext.DocumentLogs.Add(new()
                                {
                                    Key = key,
                                    DocumentType = currentChangedDocumentKeyType.Type,
                                    Status = eventLog.Status,
                                    UserId = eventLog.UserId,
                                    DateTime = DateTime.Now,
                                    ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                    EventType = (int)DocumentEventType.ChangeStatusBackward,
                                    Description = eventLog.Description
                                });
                            }
                        }
                        else if (currentDoc.AggStatus == 3 || currentDoc.AggStatus == 4)
                        {
                            //Log divided document change status
                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = currentChangedDocumentKeyType.Key,
                                DocumentType = currentChangedDocumentKeyType.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.ChangeStatusMasterBackward,
                                Description = eventLog.Description
                            });

                            var mainDocument = apiContext.DocumentHeaders.FirstOrDefault(p => p.Key == currentDoc.DivideParent &&
                                      (p.DocumentType == currentChangedDocumentKeyType.Type ||
                                       p.DocumentType1 == currentChangedDocumentKeyType.Type ||
                                       p.DocumentType2 == currentChangedDocumentKeyType.Type));

                            List<int> statuses = apiContext.DocumentHeaders.Where(p => p.DivideParent == mainDocument.Key &&
                                                    (p.DocumentType == currentChangedDocumentKeyType.Type ||
                                                    p.DocumentType1 == currentChangedDocumentKeyType.Type ||
                                                    p.DocumentType2 == currentChangedDocumentKeyType.Type) &&
                                                    (p.AggStatus == 3 || p.AggStatus == 4))
                                               .Select(p => p.DocumentStatusId)
                                               .ToList();

                            if (mainDocument.DocumentStatusId == eventLog.Status && statuses.Count(p => p == eventLog.Status) == 1)
                            {
                                //Log main document based on min change status
                                apiContext.DocumentLogs.Add(new()
                                {
                                    Key = mainDocument.Key,
                                    DocumentType = currentChangedDocumentKeyType.Type,
                                    Status = eventLog.Status,
                                    UserId = eventLog.UserId,
                                    DateTime = DateTime.Now,
                                    ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                    EventType = (int)DocumentEventType.ChangeStatusForward,
                                    Description = eventLog.Description
                                });
                            }
                        }
                    }
                }
                break;
            case DocumentEventType.Aggregate:
                foreach (var aggregatedDocument in eventLog.DocKeyTypes)
                {
                    //Log aggregated document
                    apiContext.DocumentLogs.Add(new()
                    {
                        Key = aggregatedDocument.Key,
                        DocumentType = aggregatedDocument.Type,
                        Status = eventLog.Status,
                        UserId = eventLog.UserId,
                        DateTime = DateTime.Now,
                        ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                        EventType = (int)DocumentEventType.InsertAggregate,
                        Description = eventLog.Description
                    });

                    //Log main documents
                    foreach (var key in aggregatedDocument.Key.Split('-'))
                    {
                        apiContext.DocumentLogs.Add(new()
                        {
                            Key = key,
                            DocumentType = aggregatedDocument.Type,
                            Status = eventLog.Status,
                            UserId = eventLog.UserId,
                            DateTime = DateTime.Now,
                            ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                            EventType = (int)DocumentEventType.Aggregate,
                            Description = eventLog.Description
                        });
                    }
                }
                break;
            case DocumentEventType.RevokeAggregate:
                foreach (var aggregatedDocument in eventLog.DocKeyTypes)
                {
                    //Log removed aggregated document
                    apiContext.DocumentLogs.Add(new()
                    {
                        Key = aggregatedDocument.Key,
                        DocumentType = aggregatedDocument.Type,
                        Status = eventLog.Status,
                        UserId = eventLog.UserId,
                        DateTime = DateTime.Now,
                        ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                        EventType = (int)DocumentEventType.RemoveAggregate,
                        Description = eventLog.Description
                    });

                    //Log revoke aggregate main documents
                    foreach (var key in aggregatedDocument.Key.Split('-'))
                    {
                        apiContext.DocumentLogs.Add(new()
                        {
                            Key = key,
                            DocumentType = aggregatedDocument.Type,
                            Status = eventLog.Status,
                            UserId = eventLog.UserId,
                            DateTime = DateTime.Now,
                            ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                            EventType = (int)DocumentEventType.RevokeAggregate,
                            Description = eventLog.Description
                        });
                    }
                }
                break;
            case DocumentEventType.Divide:
                foreach (var dividedDocument in eventLog.DocKeyTypes)
                {
                    if (dividedDocument.Key.Contains("_"))
                    {
                        string mainDocument = dividedDocument.Key.Split("_").First();
                        string remainDocument = dividedDocument.Key.Split("_").First() + "_0";
                        string dividedBranch = dividedDocument.Key.Split("_").Last();

                        apiContext.DocumentLogs.Add(new()
                        {
                            Key = mainDocument,
                            DocumentType = dividedDocument.Type,
                            Status = eventLog.Status,
                            UserId = eventLog.UserId,
                            DateTime = DateTime.Now,
                            ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                            EventType = (int)DocumentEventType.Divide,
                            Description = eventLog.Description
                        });

                        //Allways last remain document eventType is InsertDivide and all other remain document logs become divide
                        apiContext.DocumentLogs.Where(p => p.Key == remainDocument && p.DocumentType == dividedDocument.Type)
                            .ExecuteUpdate(p => p.SetProperty(q => q.EventType, (int)DocumentEventType.Divide));

                        apiContext.DocumentLogs.Add(new()
                        {
                            Key = remainDocument,
                            DocumentType = dividedDocument.Type,
                            Status = eventLog.Status,
                            UserId = eventLog.UserId,
                            DateTime = DateTime.Now,
                            ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                            EventType = (int)DocumentEventType.InsertDivide,
                            Description = eventLog.Description
                        });

                        apiContext.DocumentLogs.Add(new()
                        {
                            Key = dividedDocument.Key,
                            DocumentType = dividedDocument.Type,
                            Status = eventLog.Status,
                            UserId = eventLog.UserId,
                            DateTime = DateTime.Now,
                            ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                            EventType = (int)DocumentEventType.InsertDivide,
                            Description = eventLog.Description
                        });

                    }
                }
                break;
            case DocumentEventType.RevokeDivide:
                foreach (var dividedDocument in eventLog.DocKeyTypes)
                {
                    if (dividedDocument.Key.Contains("_"))
                    {
                        string mainDocument = dividedDocument.Key.Split("_").First();
                        string remainDocument = dividedDocument.Key.Split("_").First() + "_0";
                        string dividedBranch = dividedDocument.Key.Split("_").Last();

                        DocumentHeader currentMainDocument = apiContext.DocumentHeaders
                                                                .FirstOrDefault(p => p.Key == mainDocument
                                                                    && p.DocumentType == dividedDocument.Type
                                                                    && p.Parent == "Divided");

                        // Main document is still divided
                        if (currentMainDocument is not null)
                        {
                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = mainDocument,
                                DocumentType = dividedDocument.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.RevokeDivide,
                                Description = eventLog.Description
                            });

                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = remainDocument,
                                DocumentType = dividedDocument.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.RevokeDivide,
                                Description = eventLog.Description
                            });

                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = dividedDocument.Key,
                                DocumentType = dividedDocument.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.RemoveDivide,
                                Description = eventLog.Description
                            });
                        }
                        else // Main document is no longer divided
                        {
                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = mainDocument,
                                DocumentType = dividedDocument.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.RevokeDivide,
                                Description = eventLog.Description
                            });

                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = remainDocument,
                                DocumentType = dividedDocument.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.RemoveDivide,
                                Description = eventLog.Description
                            });

                            apiContext.DocumentLogs.Add(new()
                            {
                                Key = dividedDocument.Key,
                                DocumentType = dividedDocument.Type,
                                Status = eventLog.Status,
                                UserId = eventLog.UserId,
                                DateTime = DateTime.Now,
                                ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                                EventType = (int)DocumentEventType.RemoveDivide,
                                Description = eventLog.Description
                            });
                        }
                    }
                }
                break;
            case DocumentEventType.RemoveDocument:
                foreach (var keyType in eventLog.DocKeyTypes)
                {
                    var currentDoc = apiContext.DocumentHeaders.Where(p => p.Key == keyType.Key &&
                                                                          (p.DocumentType == keyType.Type ||
                                                                           p.DocumentType1 == keyType.Type ||
                                                                           p.DocumentType2 == keyType.Type))
                                                               .ToList();

                    apiContext.DocumentLogs.Add(new()
                    {
                        Key = keyType.Key,
                        DocumentType = currentDoc.Any() ? currentDoc.First().DocumentType : keyType.Type,
                        Status = eventLog.Status,
                        UserId = eventLog.UserId,
                        DateTime = DateTime.Now,
                        ShamsiDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
                        EventType = (int)eventLog.EventType,
                        Description = eventLog.Description
                    });
                }
                break;
            default:
                return false;
        }

        return apiContext.SaveChanges() > 0;
    }

    public DataTable SGetAllDocumentLogs(GetAllDocumentLogQuery request)
    {
        string whereString = "";

        string whereSubString = "";


        if (request.DocumentKey.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }
            whereString += $" tbl_DocumentLog.fld_LogDocumentKey like N'%{request.DocumentKey}%' ";
        }

        if (request.DocumentType.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }
            whereString += $" tbl_DocumentLog.fld_LogDocumentType = N'{request.DocumentType}' ";
        }

        if (request.FromDate.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }
            whereString += $" tbl_DocumentLog.fld_LogDateTime >= N'{PersianCalendarTools.PersianToGregorian(request.FromDate)}' ";
        }

        if (request.ToDate.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }
            whereString += $" tbl_DocumentLog.fld_LogDateTime < N'{PersianCalendarTools.PersianToGregorian(request.ToDate)}' ";
        }

        if (request.Description.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }
            whereString += $" tbl_DocumentLog.fld_LogDescription Like N'%{request.Description}%' ";
        }

        if (request.User.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }
            whereString += $" tbl_DocumentLog.fld_LogUserId = N'{request.User}' ";
        }

        if (request.DocumentEventType.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            if (request.DocumentEventType.Contains('*'))
            {
                whereString += $" tbl_DocumentLog.fld_LogDocumentStatus = N'{request.DocumentEventType.Remove(0,1)}' ";

                whereString += " AND ";

                whereString +=
                    $"""
                (
                    tbl_DocumentLog.fld_LogEventType IN({(int)DocumentEventType.ChangeStatusMasterForward},
                                                        {(int)DocumentEventType.ChangeStatusMasterBackward}) 
                )
                """;
            }
            else
            {
                whereString += $" tbl_DocumentLog.fld_LogEventType = N'{request.DocumentEventType}' ";
            }
        }
        else
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }
            whereString +=
                $"""
                (
                    tbl_DocumentLog.fld_LogEventType IN({(int)DocumentEventType.InsertDocument},
                                                        {(int)DocumentEventType.ChangeStatusMasterForward},
                                                        {(int)DocumentEventType.ChangeStatusMasterBackward},
                                                        {(int)DocumentEventType.InsertAggregate},
                                                        {(int)DocumentEventType.RemoveAggregate},
                                                        {(int)DocumentEventType.InsertDivide},
                                                        {(int)DocumentEventType.RemoveDivide},
                                                        {(int)DocumentEventType.RemoveDocument}) 
                )
                """;
        }

        if (request.HeaderData.HasValue())
        {
            if (whereSubString.HasValue())
            {
                whereSubString += " AND ";
            }
            whereSubString += $" [NESTED].HeaderData Like N'%{request.HeaderData}%' ";
        }

        string command =
            $"""
            SELECT *,
            COALESCE( DATEDIFF(MINUTE, [NESTED].NextDateTime, DateTime) ,0)AS MinutesUntilNext
            FROM(
               SELECT tbl_DocumentLog.fld_LogDocumentKey AS DocumentKey,
                tbl_DocumentLog.fld_LogDocumentType AS DocumentType,
            	COALESCE(CONVERT(nvarchar(50),(SELECT tbl_DocumentHeader.fld_DocumentImportDatetime FROM tbl_DocumentHeader 
                WHERE tbl_DocumentHeader.fld_DocumentKey = tbl_DocumentLog.fld_LogDocumentKey 
                AND ( tbl_DocumentHeader.fld_DocumentType = tbl_DocumentLog.fld_LogDocumentType OR
            	      tbl_DocumentHeader.fld_DocumentType1 = tbl_DocumentLog.fld_LogDocumentType OR
            	      tbl_DocumentHeader.fld_DocumentType2 = tbl_DocumentLog.fld_LogDocumentType)),21),N'') AS ImportDateTime,
            	COALESCE((SELECT tbl_DocumentHeader.fld_DocumentHeaderData FROM tbl_DocumentHeader 
            	WHERE tbl_DocumentHeader.fld_DocumentKey = tbl_DocumentLog.fld_LogDocumentKey 
            	AND ( tbl_DocumentHeader.fld_DocumentType = tbl_DocumentLog.fld_LogDocumentType OR
            		  tbl_DocumentHeader.fld_DocumentType1 = tbl_DocumentLog.fld_LogDocumentType OR
            		  tbl_DocumentHeader.fld_DocumentType2 = tbl_DocumentLog.fld_LogDocumentType)),N'') AS HeaderData,
                tbl_DocumentLog.fld_LogEventType AS DocumentEventType,
                tbl_DocumentLog.fld_LogDateTime AS DateTime,
                tbl_DocumentLog.fld_LogShamsiDate AS ShamsiDate,
                COALESCE(tbl_DocumentLog.fld_LogDescription,N'' ) AS Description,
                tbl_DocumentLog.fld_LogDocumentStatus AS DocumentStatus,
                COALESCE(CASE WHEN TRY_CONVERT(UNIQUEIDENTIFIER, tbl_DocumentLog.fld_LogUserId) IS NOT NULL 
            		     THEN  (SELECT [InnerUser].[Name] FROM tbl_User as [InnerUser] WHERE [InnerUser].Id = tbl_DocumentLog.fld_LogUserId)
                         ELSE COALESCE(tbl_DocumentLog.fld_LogUserId,N'') END , N'') AS [User],
                LEAD(fld_LogDateTime) OVER (ORDER BY fld_LogDateTime DESC) AS NextDateTime
                FROM tbl_DocumentLog
                {(whereString.HasValue() ? "WHERE " + whereString : "")}
            	) AS [NESTED]
            {(whereSubString.HasValue() ? "WHERE " + whereSubString : "")}
            ORDER BY [NESTED].[DateTime] DESC
            """;

        return dataAccess.SqlDataAdapter(command);
    }

    public DataTable SGetAllDocumentLogUser()
    {
        string command =
            $"""
            SELECT DISTINCT tbl_DocumentLog.fld_LogUserId AS Code ,tbl_User.Name AS Title 
            FROM tbl_DocumentLog INNER JOIN tbl_User ON tbl_User.Id = tbl_DocumentLog.fld_LogUserId
            """;

        var result = dataAccess.SqlDataAdapter(command);

        return result;
    }
}
