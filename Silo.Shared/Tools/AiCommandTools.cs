namespace Silo.Shared.Tools;

/// <summary>
/// Utility for parsing structured command blocks embedded in AI agent responses.
///
/// The AI can embed commands like:
/// <code>
/// &lt;&lt;SQL
/// SELECT * FROM tbl_Products
/// &gt;&gt;
///
/// &lt;&lt;CONFIG
/// ...
/// &gt;&gt;
/// </code>
///
/// This class extracts command data from the AI response and removes the
/// command blocks from the text that is visible to the user.
///
/// New AI command types should be added to this class as dedicated methods.
/// </summary>
public class AiCommandTools
{
    private const string SqlStart = "<<SQL";
    private const string ConfigStart = "<<CONFIG";

    private const string BarChartStart = "<<BarChart";
    private const string LineChartStart = "<<LineChart";
    private const string PieChartStart = "<<PieChart";

    private const string BlockEnd = ">>";

    /// <summary>
    /// Strips all <c>&lt;&lt;SQL ... &gt;&gt;</c> blocks from
    /// <paramref name="text"/>.
    ///
    /// Extracted SQL commands are returned through
    /// <paramref name="collectedCommands"/>.
    /// </summary>
    /// <param name="text">
    /// Raw AI response text that may contain SQL command blocks.
    /// </param>
    /// <param name="collectedCommands">
    /// Optional list that receives the extracted SQL commands.
    /// </param>
    /// <returns>
    /// The cleaned response text with all SQL command blocks removed.
    /// </returns>
    public static string StripSqlBlocks(
        string text,
        List<string>? collectedCommands = null)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var startIndex = 0;

        while (startIndex < text.Length)
        {
            var blockStart = text.IndexOf(
                SqlStart,
                startIndex,
                StringComparison.OrdinalIgnoreCase);

            if (blockStart == -1)
                break;

            var contentStart = blockStart + SqlStart.Length;

            var blockEnd = text.IndexOf(
                BlockEnd,
                contentStart,
                StringComparison.OrdinalIgnoreCase);

            if (blockEnd == -1)
                break;

            var sql = text
                .Substring(
                    contentStart,
                    blockEnd - contentStart)
                .Trim();

            if (collectedCommands is not null && sql.Length > 0)
                collectedCommands.Add(sql);

            var blockLength =
                (blockEnd + BlockEnd.Length) - blockStart;

            text = text.Remove(
                blockStart,
                blockLength);

            startIndex = blockStart;
        }

        return text.Trim();
    }

    /// <summary>
    /// Strips all <c>&lt;&lt;CONFIG ... &gt;&gt;</c> blocks from
    /// <paramref name="text"/>.
    ///
    /// Extracted CONFIG commands are returned through
    /// <paramref name="collectedCommands"/>.
    /// </summary>
    /// <param name="text">
    /// Raw AI response text that may contain CONFIG command blocks.
    /// </param>
    /// <param name="collectedCommands">
    /// Optional list that receives the extracted CONFIG commands.
    /// </param>
    /// <returns>
    /// The cleaned response text with all CONFIG command blocks removed.
    /// </returns>
    public static string StripConfigBlocks(
        string text,
        List<string>? collectedCommands = null)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var startIndex = 0;

        while (startIndex < text.Length)
        {
            var blockStart = text.IndexOf(
                ConfigStart,
                startIndex,
                StringComparison.OrdinalIgnoreCase);

            if (blockStart == -1)
                break;

            var contentStart = blockStart + ConfigStart.Length;

            var blockEnd = text.IndexOf(
                BlockEnd,
                contentStart,
                StringComparison.OrdinalIgnoreCase);

            if (blockEnd == -1)
                break;

            var config = text
                .Substring(
                    contentStart,
                    blockEnd - contentStart)
                .Trim();

            if (collectedCommands is not null && config.Length > 0)
                collectedCommands.Add(config);

            var blockLength =
                (blockEnd + BlockEnd.Length) - blockStart;

            text = text.Remove(
                blockStart,
                blockLength);

            startIndex = blockStart;
        }

        return text.Trim();
    }

    public static bool TryExtractChartBlock(
    string text,
    out string cleanText,
    out AiReportCommandType chartType,
    out string chartQuery)
    {
        cleanText = text;
        chartQuery = string.Empty;
        chartType = default;

        if (string.IsNullOrEmpty(text))
            return false;

        var chartMarkers = new[]
        {
        (Marker: BarChartStart, Type: AiReportCommandType.BarChart),
        (Marker: LineChartStart, Type: AiReportCommandType.LineChart),
        (Marker: PieChartStart, Type: AiReportCommandType.PieChart)
    };

        var blockStart = -1;
        var contentStart = -1;
        var selectedType = default(AiReportCommandType);

        foreach (var item in chartMarkers)
        {
            var index = text.IndexOf(
                item.Marker,
                StringComparison.OrdinalIgnoreCase);

            if (index >= 0 && (blockStart == -1 || index < blockStart))
            {
                blockStart = index;
                contentStart = index + item.Marker.Length;
                selectedType = item.Type;
            }
        }

        if (blockStart == -1)
            return false;

        var blockEnd = text.IndexOf(
            BlockEnd,
            contentStart,
            StringComparison.OrdinalIgnoreCase);

        if (blockEnd == -1)
            return false;

        chartQuery = text
            .Substring(contentStart, blockEnd - contentStart)
            .Trim();

        if (string.IsNullOrWhiteSpace(chartQuery))
            return false;

        cleanText = text.Remove(
            blockStart,
            blockEnd + BlockEnd.Length - blockStart
        ).Trim();

        chartType = selectedType;

        return true;
    }
}



