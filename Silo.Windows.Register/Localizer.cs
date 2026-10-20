using System.Xml;
using System.Xml.Linq;

namespace Silo.Windows.Register;

/// <summary>
/// Texts shared with the web application. The web app loads them from the server into
/// <see cref="Silo.Shared.TextResources"/>; the device host has no server connection for this, so it reads the
/// <c>TextResources.resx</c> that ships next to the executable (copied from Silo.Shared). A missing file or key
/// leaves the English fallback in place, so the host never fails because of a text.
/// </summary>
internal static class Localizer
{
    public static void Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TextResources.resx");
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            var values = new Dictionary<string, string?>(StringComparer.Ordinal);
            foreach (var data in XDocument.Load(path).Root?.Elements("data") ?? [])
            {
                var name = (string?)data.Attribute("name");
                if (!string.IsNullOrEmpty(name))
                {
                    values[name] = (string?)data.Element("value");
                }
            }

            Silo.Shared.ResourceManager.Load(values);
        }
        catch (Exception exception) when (exception is XmlException or IOException or UnauthorizedAccessException)
        {
            // Keep the English fallbacks.
        }
    }

    /// <summary>The resource text, or <paramref name="fallback"/> when the resources were not loaded or the key is empty.</summary>
    public static string Text(string? resource, string fallback)
        => string.IsNullOrWhiteSpace(resource) ? fallback : resource;
}
