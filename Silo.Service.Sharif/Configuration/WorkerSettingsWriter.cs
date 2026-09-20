using System.Text.Json;
using System.Text.Json.Nodes;

namespace Silo.Service.Sharif.Configuration;

public sealed class WorkerSettingsWriter
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WorkerSettingsWriter> _logger;

    public WorkerSettingsWriter(IConfiguration configuration, ILogger<WorkerSettingsWriter> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string ConfigPath
        => Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    public async Task<bool> SaveAsync(RfidWorkerOptions options, CancellationToken cancellationToken = default)
    {
        var path = ConfigPath;
        var tempPath = path + ".tmp";

        try
        {
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var node = await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);

            if (node is null)
            {
                _logger.LogError("appsettings.json could not be parsed.");
                return false;
            }

            var workerNode = node["RfidWorker"] ??= new JsonObject();
            workerNode[nameof(options.StationCode)] = options.StationCode;
            workerNode[nameof(options.GateType)] = options.GateType;
            workerNode[nameof(options.ReaderPower)] = options.ReaderPower.ToString();
            workerNode[nameof(options.IdleDelayMilliseconds)] = options.IdleDelayMilliseconds.ToString();

            var options1 = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            await using (var tempStream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(tempStream, node, options1, cancellationToken);
            }

            File.Replace(tempPath, path, null, true);
            _logger.LogInformation("RfidWorker settings persisted to {ConfigPath}.", path);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist RfidWorker settings.");
            TryDelete(tempPath);
            return false;
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }
}
