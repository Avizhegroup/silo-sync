using System.Text;

namespace Silo.Service.Sharif.Services;

public enum LogKind
{
    Info,
    Exception
}

public sealed class LogTailService
{
    private readonly ILogger<LogTailService> _logger;

    public LogTailService(ILogger<LogTailService> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<string> GetLogFiles(LogKind kind)
    {
        var directory = GetDirectory(kind);
        if (!Directory.Exists(directory))
        {
            return Array.Empty<string>();
        }

        try
        {
            return Directory.GetFiles(directory, "*.log")
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list log files from {Directory}.", directory);
            return Array.Empty<string>();
        }
    }

    public string GetNewestLogFile(LogKind kind)
        => GetLogFiles(kind).FirstOrDefault() ?? string.Empty;

    public async Task<string> GetTailAsync(LogKind kind, int lines = 200, string? filePath = null)
    {
        var path = filePath ?? GetNewestLogFile(kind);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return string.Empty;
        }

        try
        {
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,
                bufferSize: 8192,
                useAsync: true);

            return await ReadTailAsync(stream, lines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to tail log file {Path}.", path);
            return $"Error reading log: {ex.Message}";
        }
    }

    private static string GetDirectory(LogKind kind)
    {
        var logsRoot = Path.Combine(AppContext.BaseDirectory, "Logs");
        return kind == LogKind.Exception
            ? Path.Combine(logsRoot, "Exceptions")
            : Path.Combine(logsRoot, "InfoLogs");
    }

    private static async Task<string> ReadTailAsync(Stream stream, int lineCount)
    {
        const int bufferSize = 4096;
        var encoding = Encoding.UTF8;
        var decoder = encoding.GetDecoder();

        long position = stream.Length;
        var lines = new List<string>();
        var currentLine = new StringBuilder();

        bool endOfFileReached = false;
        var buffer = new byte[bufferSize];

        while (!endOfFileReached && lines.Count < lineCount)
        {
            long toRead = Math.Min(bufferSize, position);
            if (toRead <= 0)
            {
                endOfFileReached = true;
                break;
            }

            position -= toRead;
            stream.Position = position;
            int read = await stream.ReadAsync(buffer.AsMemory(0, (int)toRead));

            int charsUsed;
            int bytesUsed;
            bool completed;
            var chars = new char[encoding.GetMaxByteCount(read)];
            decoder.Convert(buffer, 0, read, chars, 0, chars.Length, false, out bytesUsed, out charsUsed, out completed);

            for (int i = charsUsed - 1; i >= 0; i--)
            {
                char c = chars[i];
                if (c == '\n')
                {
                    if (currentLine.Length > 0)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                    }

                    if (lines.Count >= lineCount)
                    {
                        break;
                    }
                }
                else if (c != '\r')
                {
                    currentLine.Insert(0, c);
                }
            }

            endOfFileReached = position == 0;
        }

        if (currentLine.Length > 0 && lines.Count < lineCount)
        {
            lines.Add(currentLine.ToString());
        }

        lines.Reverse();
        return string.Join(Environment.NewLine, lines);
    }
}
