namespace Silo.Application.Features;

public class GetNewApkHandler() : IRequestHandler<GetNewApkQuery, GetNewApkVm>
{
    public async Task<GetNewApkVm> Handle(GetNewApkQuery request, CancellationToken cancellationToken)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Apks");
       
        if (!Directory.Exists(directory))
        {
            return new()
            {
                Result = null
            };
        }

        var files = Directory.GetFiles(directory, "*.apk");

        var currentVersion = ExtractVersion(request.CurrentVersion);

        var newerApkFile = files
            .Select(filePath => new
            {
                FilePath = filePath,
                Version = ExtractVersion(Path.GetFileNameWithoutExtension(filePath))
            })
            .Where(x => x.Version is not null && x.Version.CompareTo(currentVersion) > 0)
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();

        if (newerApkFile is not null)
        {
            return new()
            {
                Result = await File.ReadAllBytesAsync(newerApkFile.FilePath, cancellationToken),
                NewVersion = newerApkFile.Version.ToString()
            };
        }

        return new()
        {
            Result = null
        };
    }

    private Version? ExtractVersion(string? value)
    {
        if (value.HasNoValue())
        {
            return null;
        }

        var versionMatch = System.Text.RegularExpressions.Regex.Match(value, @"\d+(\.\d+){1,3}");

        if (!versionMatch.Success)
        {
            return null;
        }

        return Version.TryParse(versionMatch.Value, out var version) ? version : null;
    }
}
