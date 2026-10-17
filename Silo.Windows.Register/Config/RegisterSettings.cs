using System.Text.Json;

namespace Silo.Windows.Register.Config;

public enum DeskReaderType
{
    Model6061 = 0,
    Tiny = 1,
    T64 = 2,
    Serial = 3,
    Simulated = 4
}

public sealed class RegisterSettings
{
    public string ServerIP { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    public DeskReaderType DeskReaderType { get; set; } = DeskReaderType.Simulated;
    public string DeskUHFReaderPort { get; set; } = "COM9";
    public int BaudRate { get; set; } = 57600;
    public int ReaderPower { get; set; } = 7;
    public string IgnoreEpcList { get; set; } = string.Empty;

    public int SerialLength { get; set; } = 5;
    public string BarcodeSpliter { get; set; } = ",";
    public int BarcodeIdleMilliseconds { get; set; } = 80;

    public string PrinterModel { get; set; } = string.Empty;

    public int SocketPort { get; set; } = 5005;
    public string SocketToken { get; set; } = Guid.NewGuid().ToString("N");
    public List<string> AllowedOrigins { get; set; } = [];

    public IReadOnlySet<string> GetIgnoredEpcs() => IgnoreEpcList
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
}

public sealed class SettingsStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public string FilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Silo.Windows.Register",
        "settings.json");

    public RegisterSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                return JsonSerializer.Deserialize<RegisterSettings>(File.ReadAllText(FilePath)) ?? new RegisterSettings();
            }
        }
        catch (Exception exception) when (exception is IOException or JsonException)
        {
        }

        var settings = new RegisterSettings();
        Save(settings);
        return settings;
    }

    public void Save(RegisterSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, Options));
    }
}
