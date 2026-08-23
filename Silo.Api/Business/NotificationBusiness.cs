using DocumentFormat.OpenXml;
using EFCore.BulkExtensions;
using Silo.Api.Services;
using Silo.Application.Contracts;
using Silo.Domains.Entities;
using Silo.Domains.Services;

namespace Silo.Api.Business;
public class NotificationBusiness(ILogger<NotificationBusiness> logger
        , IDataAccess dataAccess
        , IHttpContextAccessor httpContextAccessor
        , SmsHttpClient smsClient
        , IConfiguration configuration
        , WmsApiContext apiContext) : ProjectBusiness(dataAccess, logger, httpContextAccessor)
{
    public bool SGetScheduleNotifications()
    {
        List<NotificationQueue> queues = new();

        string orderCommand = """
             SELECT [Id]
                ,[Id]
                ,[fld_NOStatus]
                ,[fld_NODateTime]
                ,[fld_NOUserId]
                ,[fld_NOType]
                ,[fld_NOTitle]
                ,[fld_NOEventType]
                ,[fld_NOTimePeriod]
                ,[fld_NOSendDay]
                ,[fld_NOSendClock]
                ,[fld_NOSendType]
                ,[fld_NOSendContacts]
                ,[fld_NOContent]
            FROM [dbo].[tbl_NotificationOrders]
            WHERE [fld_NOType] = 1
            """;

        var scheduleOrders = dataAccess.SqlDataAdapter(orderCommand).Select();

        int maxOrder = 0;

        foreach (DataRow row in scheduleOrders)
        {
            int orderId = (int)row.ItemArray[1];

            int period = (int)row.ItemArray[8];

            int sendDay = int.Parse(row.ItemArray[9].ToString());

            string sendClock = row.ItemArray[10].ToString();

            string contact = row.ItemArray[12].ToString();

            if (orderId > maxOrder)
            {
                maxOrder = orderId;
            }

            // 0=> هر روز
            if (sendDay == 1)//شنبه 
            {
                if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
                {
                    continue;
                }
            }
            else if (sendDay == 2)//پنج شنبه
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                {
                    continue;
                }
            }
            else if (sendDay == 3)//جمعه
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                {
                    continue;
                }
            }
            else if (sendDay == 4)//روز آخر ماه
            {
                if (PersianCalendarTools.GetLastDayOfMonth(DateTime.Now)
                        == PersianCalendarTools.GregorianToPersian(DateTime.Now))
                {
                    continue;
                }
            }
            else if (sendDay == 5)//روز اول ماه
            {
                if (PersianCalendarTools.GetFirstDayOfMonth(DateTime.Now)
                        == PersianCalendarTools.GregorianToPersian(DateTime.Now))
                {
                    continue;
                }
            }

            if (sendClock != DateTime.Now.ToString("HH:mm"))
            {
                continue;
            }

            string content = row.ItemArray[13].ToString();

            var finalContent = SReplaceValuesInRules(content);

            List<string> contacts = new();

            if (contact.Contains(","))
            {
                contacts = contact.Split(',').ToList();
            }
            else
            {
                contacts.Add(contact);
            }

            foreach (string itemContact in contacts)
            {
                queues.Add(new()
                {
                    Contact = itemContact,
                    OrderId = orderId,
                    SaveDateTime = DateTime.Now,
                    SendType = int.Parse(row.ItemArray[11].ToString()),
                    Text = finalContent,
                    Status = 0
                });
            }
        }

        apiContext.BulkInsert(queues);

        string commandJobLog = @$" INSERT INTO tbl_JobLog (fld_JobLogType,fld_JobLogDateTime,fld_JobLogDate,fld_JobLogTime,fld_JobLogValue) 
                                   VALUES (2,GETDATE(),dbo.GeorgianDateToJalaliDate(GETDATE()),FORMAT(GETDATE(), 'HH:mm'),NULL)";

        bool resultSaveJobLog = dataAccess.CmdSqlExecuteNonQuery(commandJobLog) > 0;

        logger.LogInformation("SGetScheduleNotifications" + Environment.NewLine
                            + "resultSaveJobLog:" + resultSaveJobLog);

        return true;
    }

    public bool SGetEventNotification()
    {
        List<NotificationQueue> queues = new();

        string orderCommand = """
             SELECT [Id]
                ,[Id]
                ,[fld_NOStatus]
                ,[fld_NODateTime]
                ,[fld_NOUserId]
                ,[fld_NOType]
                ,[fld_NOTitle]
                ,[fld_NOEventType]
                ,[fld_NOTimePeriod]
                ,[fld_NOSendDay]
                ,[fld_NOSendClock]
                ,[fld_NOSendType]
                ,[fld_NOSendContacts]
                ,[fld_NOContent]
            FROM [dbo].[tbl_NotificationOrders]
            WHERE [fld_NOType] = 0 AND [fld_NOStatus] = 1
            """;

        string commandGetJobLogDateTime = """
                SELECT TOP(1) fld_JobLogDateTime FROM tbl_JobLog WHERE fld_JobLogType = 3 ORDER BY fld_JobLogDateTime DESC
                """;

        var lastjobLogDt = dataAccess.SqlDataAdapter(commandGetJobLogDateTime)
                                     .Select();

        string commandJobLog = @$" INSERT INTO tbl_JobLog (fld_JobLogType,fld_JobLogDateTime,fld_JobLogDate,fld_JobLogTime,fld_JobLogValue) 
                                   VALUES (3,GETDATE(),dbo.GeorgianDateToJalaliDate(GETDATE()),FORMAT(GETDATE(), 'HH:mm'),NULL)";

        bool resultSaveJobLog = dataAccess.CmdSqlExecuteNonQuery(commandJobLog) > 0;

        logger.LogInformation("SGetEventNotification" + Environment.NewLine
                            + "resultSaveJobLog:" + resultSaveJobLog);

        var eventOrders = dataAccess.SqlDataAdapter(orderCommand).Select();

        foreach (DataRow order in eventOrders)
        {
            int orderId = (int)order.ItemArray[1];

            int eventTypeId = (int)order.ItemArray[7];

            if (lastjobLogDt.Any())
            {
                string eventCommand = """
                 SELECT  [fld_NECommand]
                 FROM [dbo].[tbl_NotificationEventTypes]
                 WHERE [Id] = @EventId 
                 """;

                string commandEventType = dataAccess.SqlDataAdapter(eventCommand
                    , new KeyValuePair<string, object>("EventId", eventTypeId))
                          .Select().First().ItemArray[0].ToString();

                List<string> ids = dataAccess.SqlDataAdapter(commandEventType
                    , new KeyValuePair<string, object>("JobTime", (DateTime)lastjobLogDt.First().ItemArray[0]))
                                             .Select()
                                             .Select(p => p.ItemArray[0].ToString())
                                             .ToList();

                if (!ids.Any())
                {
                    continue;
                }

                for (int i = 0; i < ids.Count; i++)
                {
                    string id = ids[i];

                    Dictionary<string, object> dict = new();

                    dict.Add($"Temp", id);

                    string content = order.ItemArray[13].ToString();

                    var finalContent = SReplaceValuesInRules(content, dict);

                    List<string> contacts = new();

                    string contact = order.ItemArray[12].ToString();

                    if (contact.Contains(","))
                    {
                        var spliteds = contact.Split(',').ToList();

                        foreach (string splited in spliteds)
                        {
                            if (contact.Contains("["))
                            {
                                contacts.Add(SReplaceValuesInRules(splited, dict));
                            }
                            else
                            {
                                contacts.Add(splited);
                            }
                        }
                    }
                    else
                    {
                        if (contact.Contains("["))
                        {
                            contacts.Add(SReplaceValuesInRules(contact, dict));
                        }
                        else
                        {
                            contacts.Add(contact);
                        }
                    }

                    foreach (string itemContact in contacts)
                    {
                        queues.Add(new()
                        {
                            Contact = itemContact,
                            OrderId = orderId,
                            SaveDateTime = DateTime.Now,
                            SendType = int.Parse(order.ItemArray[11].ToString()),
                            Text = finalContent,
                            Status = 0
                        });
                    }
                }
            }
        }

        apiContext.BulkInsert(queues);

        return true;
    }

    public bool SSendQueueNotifications()
    {
        var queues = apiContext.NotificationQueues.Where(p => p.Status == 0);

        foreach (var queue in queues)
        {
            if (queue.SendType == 1)
            {
                var config = configuration.GetSection("ProjectConfigs").GetSection("WmsConfigs").GetSection("Notification").GetSection("Sms");

                string result = smsClient.Post(
                     new("from", config["Phone"])
                   , new("to", queue.Contact)
                   , new("text", queue.Text)
                   , new("panel_type", config["Type"])
                   , new("password", config["Password"])
                   , new("api_key", config["Key"])
                   , new("username", config["Username"]));

                logger.LogInformation("SSendQueueNotifications" + Environment.NewLine
                    + $"sms result : {result}");
            }

            queue.Status = 1;

            queue.SendDateTime = DateTime.Now;

            queue.SendDate = PersianCalendarTools.GregorianToPersian(DateTime.Now);

            queue.SendTime = DateTime.Now.ToString("HH:mm");
        }

        apiContext.SaveChanges();

        return true;
    }

    public DataTable SReportNotificationQueue(GetNotificationQueueQuery search)
    {
        string whereString = "";

        if (search.OrderId.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_NotificationOrderId = N'{search.OrderId}' ";
        }

        if (search.SendType.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_SendType = N'{search.SendType}' ";
        }

        if (search.SendContacts.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_Contact = N'{search.SendContacts}' ";
        }

        if (search.Content.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_Text like N'%{search.Content}%' ";
        }

        if (search.SendStatus.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_SendStatus = N'{search.SendStatus}' ";
        }

        if (search.FromDate.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_SendDate >= N'{search.FromDate}' ";
        }

        if (search.ToDate.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_SendDate < N'{search.ToDate}' ";
        }

        if (search.FromTime.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_SendTime >= N'{search.FromTime}' ";
        }

        if (search.ToTime.HasValue())
        {
            if (whereString.HasValue())
            {
                whereString += " AND ";
            }

            whereString += $" fld_SendTime < N'{search.ToTime}' ";
        }

        string command =
            $"""
            SELECT  fld_Text AS [Text], 
                    CASE WHEN fld_SendType = N'0' THEN N'EMAIL' 
            			 WHEN fld_SendType = N'1' THEN N'SMS'
            			 WHEN fld_SendType = N'2' THEN N'PUSH'
            			 WHEN fld_SendType = N'3' THEN N'WHATSAPP'
            			 WHEN fld_SendType = N'4' THEN N'TELEGRAM'
            			 ELSE N'' END AS [SendType],
            		fld_Contact AS [Contact], 
                    fld_SendDateTime AS [SendDateTime],
            		fld_SendDate AS [SendDate],
                    fld_SendTime AS [SendTime],
            		CASE WHEN fld_SendStatus = N'0' THEN N'در انتظار ارسال' 
            			 WHEN fld_SendStatus = N'1' THEN N'ارسال شده'
            			 ELSE N'' END AS [Status],
                    fld_NotificationOrderId AS [OrderId],
            		Orders.fld_NOTitle AS [OrderTitle]
            FROM    tbl_NotificationQueue AS Queues  
            LEFT OUTER JOIN tbl_NotificationOrders AS Orders ON Queues.fld_NotificationOrderId = Orders.fld_NOId
            {(whereString.HasValue() ? "WHERE " + whereString : "")}
            """;

        return sqlDataAccess.SqlDataAdapter(command);
    }

    private string SReplaceValuesInRules(string command, Dictionary<string, object> parameters = null)
    {
        if (parameters is null)
        {
            parameters = new();
        }

        var elements = FindDataParameter(command, "[", "]");

        var queryDict = SGetDataElementQueries(elements);

        var elementValue = new Dictionary<string, object>();

        foreach (KeyValuePair<string, string> queryItem in queryDict)
        {
            var queryParameters = FindDataParameter(queryItem.Value, "@", " ");

            var parameterCommand = "";

            for (int i = 0; i < queryParameters.Count; i++)
            {
                if (!parameters.ContainsKey(queryParameters[i]))
                {
                    continue;
                }

                object value = parameters[queryParameters[i]];

                if (value is string strValue)
                {
                    parameterCommand += $"declare @{queryParameters[i]} nvarchar({strValue.Length}) = N'{strValue}';";
                }

                if (value is int intValue)
                {
                    parameterCommand += $"declare @{queryParameters[i]} int = {intValue};";
                }

                if (value is List<string> listValue)
                {
                    parameterCommand += $"declare @{queryParameters[i]} TABLE (DataValue nvarchar(max));";

                    foreach (string itemValue in listValue)
                    {
                        parameterCommand += $" \n INSERT INTO @{queryParameters[i]} (DataValue) VALUES (N'{itemValue}'); ";
                    }
                }
            }

            var commandFinal = parameterCommand + queryItem.Value;

            var dt = dataAccess.SqlDataAdapter(commandFinal).Select();

            if (dt.Any() && dt.First().ItemArray.Length > 0)
            {
                elementValue.Add(queryItem.Key, dt.First().ItemArray[0]);
            }
        }

        foreach (KeyValuePair<string, object> item in elementValue)
        {
            if (item.Value is string stringValue)
            {
                command = command.Replace($"[{item.Key.Trim()}]", $"{stringValue}");
            }

            if (item.Value is int intValue)
            {
                command = command.Replace($"[{item.Key.Trim()}]", $"{intValue}");
            }

            if (item.Value is decimal decimalValue)
            {
                command = command.Replace($"[{item.Key.Trim()}]", $"{decimalValue}");
            }
        }

        command = ReplaceCSharpScripts(command);

        //var result = (bool)RunStringScriptAsCode(command);
        return command;

        string ReplaceCSharpScripts(string passedCommand)
        {
            passedCommand = passedCommand.Replace($"[C#:NewLine]", Environment.NewLine);

            return passedCommand;
        }
    }

    private Dictionary<string, string> SGetDataElementQueries(List<string> elements)
    {
        string strElements = string.Empty;

        if (elements.Any())
        {
            strElements = elements.Aggregate(string.Empty, (first, next) =>
            {
                return first + (string.IsNullOrEmpty(next) ? string.Empty : (",N'" + next + "'"));
            }).Remove(0, 1);
        }

        var command = $@"SELECT fld_DataMiningElementsTitle,fld_DataMiningElementsCommand FROM tbl_DataMiningElements WHERE 
                                          fld_DataMiningElementsTitle IN ({strElements})";

        var dt = dataAccess.SqlDataAdapter(command).Select();

        var dict = new Dictionary<string, string>();

        foreach (DataRow row in dt)
        {
            dict.Add(row.ItemArray[0].ToString(), row.ItemArray[1].ToString());
        }

        return dict;
    }

    /// <summary>
    /// Method to find data mining elements (DME) or sql parameters of a DME command. 
    /// DME are strings that are enclosed in passed chars.
    /// </summary>
    /// <returns>List of found DMEs or parameters</returns>
    private List<string> FindDataParameter(string text, string start, string end)
    {
        var inCommandElements = new List<string>();

        bool isInElement = false;

        string thisElement = string.Empty;

        for (int i = 0; i < text.Length; i++)
        {
            string character = text[i].ToString();

            if (!isInElement)
            {
                if (character.Equals(start))
                {
                    isInElement = true;

                    continue;
                }
            }
            else
            {
                if (character.NotEquals(end))
                {
                    thisElement += character;
                }
            }

            if (character.Equals(end))
            {
                if (thisElement.HasValue())
                {
                    isInElement = false;

                    if (inCommandElements.Neither(p => p.Equals(thisElement)))
                    {
                        inCommandElements.Add(thisElement);
                    }

                    thisElement = string.Empty;
                }
            }
        }

        if (thisElement.HasValue())
        {
            if (inCommandElements.Neither(p => p.Equals(thisElement)))
            {
                inCommandElements.Add(thisElement);
            }
        }

        return inCommandElements;
    }
}
