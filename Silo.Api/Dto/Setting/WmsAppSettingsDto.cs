namespace Silo.Api.Dto.Setting;

public class WmsAppSettingsDto
{
    public string ConnectionString { get; set; }
    public bool CreateNewProductCode { get; set; }
    public bool CreateNewProductTitle { get; set; }
    public bool CreateNewSerial { get; set; }
    public bool QcCheck { get; set; }
    public string RegisterDefaultStoreCode { get; set; }
    public string GetMaxProductSerialBy { get; set; }
    public string ProductUniquenessOn { get; set; }
    public string DocumentGroupFields { get; set; }
    public TruckCrossSettingsDto TruckCrossGate { get; set; }   = new();
    public List<DocumentSettingsDto> CustomerData { get; set; } = new();
    public NotificationSettingsDto Notification { get; set; } = new();
}

public class TruckCrossSettingsDto
{
    public string SourceStore { get; set; }
    public string DestStore { get; set; }
    public string GateNumber { get; set; }
    public bool IsPhysical { get; set; }
}

public class DocumentSettingsDto
{
    public string Key { get; set; }
    public string Command { get; set; }
    public string FieldCheck { get; set; }
    public string FieldKey { get; set; }
    public string FieldOrder { get; set; }
    public string Type { get; set; }
    public string ConnectionString { get; set; }
}

public class NotificationSettingsDto
{
    public string Sms { get; set; }
    public string Type { get; set; }
    public string Phone { get; set; }
    public string FieldOrder { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Key { get; set; }
    public string Api { get; set; }
}