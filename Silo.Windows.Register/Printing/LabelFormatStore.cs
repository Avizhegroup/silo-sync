namespace Silo.Windows.Register.Printing;

public enum LabelFormatKind
{
    Zpl,
    Zmin
}

public sealed record LabelFormatInfo(string Kind, string Name);

/// <summary>
/// Label templates kept on the workstation. Two folders are searched, the per-user one first:
/// %AppData%\Silo.Windows.Register\Formats and &lt;application folder&gt;\Formats, each with a "zpl" (*.txt)
/// and a "zmin" (*.lsf) sub-folder. The browser only ever refers to a template by kind and name.
/// </summary>
public sealed class LabelFormatStore
{
    private static readonly string[] Roots =
    [
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Silo.Windows.Register", "Formats"),
        Path.Combine(AppContext.BaseDirectory, "Formats")
    ];

    public static bool TryParseKind(string? value, out LabelFormatKind kind)
    {
        switch (value?.Trim().ToLowerInvariant())
        {
            case "zpl":
                kind = LabelFormatKind.Zpl;
                return true;
            case "zmin":
                kind = LabelFormatKind.Zmin;
                return true;
            default:
                kind = default;
                return false;
        }
    }

    public IReadOnlyList<LabelFormatInfo> List()
    {
        var result = new List<LabelFormatInfo>();
        foreach (var kind in Enum.GetValues<LabelFormatKind>())
        {
            var names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var root in Roots)
            {
                var folder = Path.Combine(root, FolderOf(kind));
                if (!Directory.Exists(folder))
                {
                    continue;
                }

                foreach (var file in Directory.EnumerateFiles(folder, "*" + ExtensionOf(kind)))
                {
                    names.Add(Path.GetFileNameWithoutExtension(file));
                }
            }

            result.AddRange(names.Select(name => new LabelFormatInfo(FolderOf(kind), name)));
        }

        return result;
    }

    /// <summary>Resolves a template to a file path; the name must be a plain file name (no folders).</summary>
    public string Resolve(LabelFormatKind kind, string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || name != Path.GetFileName(name) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException("Invalid label format name.");
        }

        foreach (var root in Roots)
        {
            var path = Path.Combine(root, FolderOf(kind), name + ExtensionOf(kind));
            if (File.Exists(path))
            {
                return path;
            }
        }

        throw new FileNotFoundException($"Label format '{name}' ({FolderOf(kind)}) was not found.");
    }

    private static string FolderOf(LabelFormatKind kind) => kind == LabelFormatKind.Zpl ? "zpl" : "zmin";

    private static string ExtensionOf(LabelFormatKind kind) => kind == LabelFormatKind.Zpl ? ".txt" : ".lsf";
}
