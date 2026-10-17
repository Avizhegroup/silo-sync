using System.Text.Json;
using Silo.Windows.Register.Config;
using Silo.Windows.Register.Devices;
using Silo.Windows.Register.Server;

namespace Silo.Windows.Register.Printing;

/// <summary>
/// Handles the label/printer commands of the socket protocol. Printing runs off the UI thread and the outcome
/// is always reported back to the browser (type "printResult", same id), so the page can await it.
/// </summary>
public sealed class PrintCommandHandler(
    RegisterSettings settings,
    LabelFormatStore formats,
    DeviceSocketServer server,
    Action<string> log)
{
    public static bool Handles(string type) => type.ToLowerInvariant() is
        "print" or "listprinters" or "listformats" or "printerstatus" or "printformat";

    public Task HandleAsync(SocketCommand command) => Task.Run(async () =>
    {
        try
        {
            switch (command.Type.ToLowerInvariant())
            {
                case "listprinters":
                    await server.SendReplyAsync("printers", command.Id, true,
                        JsonSerializer.Serialize(PrinterService.GetInstalledPrinters()));
                    break;
                case "listformats":
                    await server.SendReplyAsync("formats", command.Id, true, JsonSerializer.Serialize(formats.List()));
                    break;
                case "printerstatus":
                    await ReplyStatusAsync(command);
                    break;
                case "print":
                    PrinterService.SendRaw(ResolvePrinter(command.Printer), command.Data ?? string.Empty);
                    await ReplyResultAsync(command, new ZminResult(true, "Sent."));
                    break;
                case "printformat":
                    await ReplyResultAsync(command, PrintFormat(command));
                    break;
            }
        }
        catch (Exception exception)
        {
            log($"Command '{command.Type}' failed: {exception.Message}");
            await ReplyResultAsync(command, new ZminResult(false, exception.Message));
        }
    });

    private ZminResult PrintFormat(SocketCommand command)
    {
        if (!LabelFormatStore.TryParseKind(command.Kind, out var kind))
        {
            return new ZminResult(false, "Unknown label format kind (use 'zpl' or 'zmin').");
        }

        var path = formats.Resolve(kind, command.Format);
        if (kind == LabelFormatKind.Zpl)
        {
            ZplLabelPrinter.Print(ResolvePrinter(command.Printer), path, command.Fields, command.TechnicalData);
            log($"ZPL label printed: {command.Format}");
            return new ZminResult(true, "Sent.");
        }

        var result = ZminLabelPrinter.Print(path, command.Fields, command.TechnicalData, command.WriteRfid ?? false);
        log($"ZMIN label '{command.Format}': {result.Message}");
        return result;
    }

    private async Task ReplyStatusAsync(SocketCommand command)
    {
        if (LabelFormatStore.TryParseKind(command.Kind, out var kind) && kind == LabelFormatKind.Zmin)
        {
            var status = ZminLabelPrinter.GetStatus();
            await server.SendReplyAsync("printerStatus", command.Id, status.Ok, status.Message, status.StatusCode);
            return;
        }

        // ZPL RAW printers expose no status through the spooler API: report whether the printer exists.
        try
        {
            var name = ResolvePrinter(command.Printer);
            await server.SendReplyAsync("printerStatus", command.Id, true, $"Printer '{name}' is installed.");
        }
        catch (InvalidOperationException exception)
        {
            await server.SendReplyAsync("printerStatus", command.Id, false, exception.Message);
        }
    }

    private Task ReplyResultAsync(SocketCommand command, ZminResult result)
        => server.SendReplyAsync("printResult", command.Id, result.Ok, result.Message, result.StatusCode);

    /// <summary>Uses the requested printer when it is installed, otherwise the configured default.</summary>
    private string ResolvePrinter(string? requested)
    {
        var name = string.IsNullOrWhiteSpace(requested) ? settings.PrinterModel : requested.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("No printer is configured. Choose one in the settings of the device host.");
        }

        if (!PrinterService.GetInstalledPrinters().Contains(name, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Printer '{name}' is not installed on this computer.");
        }

        return name;
    }
}
