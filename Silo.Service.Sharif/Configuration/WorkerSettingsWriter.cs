using System.Text.Json;
using System.Text.Json.Nodes;

namespace Silo.Service.Sharif.Configuration;

public sealed class WorkerSettingsWriter
{
    private readonly ILogger<WorkerSettingsWriter> _logger;
    private readonly IConfigurationRoot _configuration;
    private readonly IHostEnvironment _environment;   

    public WorkerSettingsWriter(
        IConfiguration configuration,
        IHostEnvironment environment,               
        ILogger<WorkerSettingsWriter> logger)
    {
        _configuration = (IConfigurationRoot)configuration;
        _environment = environment;
        _logger = logger;
    }

    public string ConfigPath
        => Path.Combine(_environment.ContentRootPath, "appsettings.json");

    public async Task<bool> SaveAsync(
        RfidWorkerOptions options,
        CancellationToken cancellationToken = default)
    {
        var path = ConfigPath;
        var tempPath = path + ".tmp";

        try
        {
            JsonNode? node;

            await using (var stream = File.Open(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                node = await JsonNode.ParseAsync(
                    stream,
                    cancellationToken: cancellationToken);
            }

            if (node is null)
            {
                _logger.LogError("appsettings.json could not be parsed.");
                return false;
            }

            var workerNode = node["RfidWorker"] ??= new JsonObject();

            workerNode[nameof(options.StationCode)] = options.StationCode;
            workerNode[nameof(options.GateType)] = options.GateType;
            workerNode[nameof(options.ReaderPower)] = options.ReaderPower;
            workerNode[nameof(options.IdleDelayMilliseconds)] = options.IdleDelayMilliseconds;
            workerNode[nameof(options.CheckTimeSeconds)] = options.CheckTimeSeconds;

            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            await using (var tempStream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(
                    tempStream,
                    node,
                    serializerOptions,
                    cancellationToken);
            }

            File.Move(tempPath, path, true);

            _configuration.Reload();

            _logger.LogInformation(
                "Saved to {Path}. After reload ReaderPower = {ReaderPower}",
                path,
                _configuration["RfidWorker:ReaderPower"]);

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
                File.Delete(path);
        }
        catch { }
    }
}
