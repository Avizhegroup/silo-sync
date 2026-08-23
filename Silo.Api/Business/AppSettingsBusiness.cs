using Silo.Api.Dto.Setting;
using Silo.Application.Contracts;

namespace Silo.Api.Business;
public class AppSettingsBusiness : ProjectBusiness
{
    private readonly IConfiguration configuration;

    public AppSettingsBusiness(IDataAccess dataAccess
        , ILogger<AppSettingsBusiness> logger
        , IHttpContextAccessor httpContextAccessor
        , IConfiguration configuration) : base(dataAccess, logger, httpContextAccessor)
    {
        this.configuration = configuration;
    }

    public WmsAppSettingsDto SGetAllWmsBusiness()
    {
        var configurationSection = configuration.GetSection("ProjectConfigs").GetSection("WmsConfigs");

        List<DocumentSettingsDto> customerDatas = new();

        foreach (var item in configurationSection.GetSection("CustomerData").GetChildren())
        {
            customerDatas.Add(new()
            {
                Key = item.Key,
                Command = item[$"{item.Key}:Command"],
                FieldCheck = item[$"{item.Key}:Field_Check"],
                FieldKey = item[$"{item.Key}:Field_Key"],
                FieldOrder = item[$"{item.Key}:Field_Order"],
                Type = item[$"{item.Key}:Type"],
                ConnectionString = configuration.GetConnectionString(item.Key)
            });
        }

        if (httpContext.User.GetUserId().HasValue())
        {
            return new WmsAppSettingsDto()
            {
                ConnectionString = configuration.GetConnectionString("SqlDefaultConnectionString"),
                CreateNewProductCode = configurationSection["CreateNewProductCode"].Equals("true"),
                CreateNewProductTitle = configurationSection["CreateNewProductTitle"].Equals("true"),
                CreateNewSerial = configurationSection["CreateNewSerial"].Equals("true"),
                QcCheck = configurationSection["QcCheck"].Equals("true"),
                RegisterDefaultStoreCode = configurationSection["RegisterDefaultStoreCode"],
                GetMaxProductSerialBy = configurationSection["GetMaxProductSerialBy"],
                ProductUniquenessOn = configurationSection["ProductUniquenessOn"],
                DocumentGroupFields = configurationSection["DocumentGroupFields"],
                TruckCrossGate = new()
                {
                    DestStore = configurationSection["TruckCrossGate:DestStore"],
                    SourceStore = configurationSection["TruckCrossGate:SourceStore"],
                    GateNumber = configurationSection["TruckCrossGate:GateNumber"],
                    IsPhysical = configurationSection["TruckCrossGate:IsPhysical"].Equals("true")
                },
                Notification = new()
                {
                    Type = configurationSection["Notification:Sms:Type"],
                    Phone = configurationSection["Notification:Sms:Phone"],
                    FieldOrder = configurationSection["Notification:Sms:FieldOrder"],
                    Username = configurationSection["Notification:Sms:Username"],
                    Password = configurationSection["Notification:Sms:Password"],
                    Key = configurationSection["Notification:Sms:Key"],
                    Api = configurationSection["Notification:Sms:Api"]
                },
                CustomerData = customerDatas
            };
        }

        return null;
    }
}
