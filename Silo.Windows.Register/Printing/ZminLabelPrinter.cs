using ZminLib = global::LabelPrinter;

namespace Silo.Windows.Register.Printing;

public sealed record ZminResult(bool Ok, string Message, int StatusCode = 0);

/// <summary>
/// ZMIN RFID label printers through the vendor LabelPrinter.dll (USB, no Windows driver needed).
/// Templates are ".lsf" files designed with LabelSoft; text objects may contain [token] placeholders.
/// Mirrors the legacy PrintZminFormatAsync flow (status check, OpenLabel, variable substitution, PrintLabel).
/// </summary>
public static class ZminLabelPrinter
{
    private static readonly object Gate = new();

    public static ZminResult GetStatus()
    {
        lock (Gate)
        {
            return ReadStatus(NewPrinter());
        }
    }

    /// <param name="writeRfid">
    /// false (legacy behaviour): objects whose name starts with "rfid" are removed so the label is printed
    /// without writing the chip. true: they are kept and their [token] placeholders (for example [شناسه_تگ]) are filled.
    /// </param>
    public static ZminResult Print(
        string lsfPath,
        IReadOnlyDictionary<string, string>? fields,
        IReadOnlyDictionary<string, string>? technicalData,
        bool writeRfid)
    {
        lock (Gate)
        {
            var printer = NewPrinter();
            var status = ReadStatus(printer);
            if (!status.Ok)
            {
                return status;
            }

            var label = new ZminLib.ZMLabel();
            var objects = new List<ZminLib.LabelObject>();
            var openStatus = new ZminLib.LSFUtility().OpenLabel(lsfPath, ref printer, ref label, ref objects);
            if (objects.Count == 0)
            {
                return new ZminResult(false, $"The label file could not be opened ({openStatus}).");
            }

            foreach (var labelObject in objects)
            {
                if (labelObject.Variables.Any() && !string.IsNullOrEmpty(labelObject.Variables[0].data))
                {
                    var variable = labelObject.Variables[0];
                    variable.data = LabelTokens.Apply(variable.data, fields, technicalData, sanitizeForZpl: false);
                    labelObject.Variables.Clear();
                    labelObject.Variables.Add(variable);
                }

                if (technicalData is { Count: > 0 } && !string.IsNullOrEmpty(labelObject.objectdata))
                {
                    labelObject.objectdata = LabelTokens.Apply(labelObject.objectdata, null, technicalData, sanitizeForZpl: false);
                }
            }

            if (!writeRfid)
            {
                objects.RemoveAll(o => o.ObjectName != null && o.ObjectName.StartsWith("rfid", StringComparison.OrdinalIgnoreCase));
            }

            var message = new ZminLib.PrintUtility().PrintLabel(printer, label, objects, true, true);
            return message.StartsWith("Error:", StringComparison.OrdinalIgnoreCase)
                ? new ZminResult(false, message)
                : new ZminResult(true, string.IsNullOrWhiteSpace(message) ? "Printed." : message);
        }
    }

    private static ZminLib.ZMPrinter NewPrinter()
    {
        var printer = new ZminLib.ZMPrinter();
        printer.printerinterface = ZminLib.PrinterStyle.RFID_USB;
        return printer;
    }

    private static ZminResult ReadStatus(ZminLib.ZMPrinter printer)
    {
        var code = new ZminLib.PrintUtility().getPrinterStatusCode(printer);
        return new ZminResult(code == 0, code == 0 ? "Ready." : Describe(code), code);
    }

    public static string Describe(int code) => code switch
    {
        0 => "Ready.",
        1 => "The printer received an invalid instruction. Check the command format and restart the printer.",
        81 => "Printer mainboard hardware fault. Contact after-sales service.",
        82 => "Ribbon not detected. Check that the ribbon is installed correctly.",
        83 => "Label not detected. Check the label roll and calibrate the media.",
        86 => "Cutter detection error. Check that the cutter is enabled.",
        88 => "The printer is paused. Press the pause button on the printer to resume.",
        89 => "Out of labels. Load a new roll.",
        90 => "Error reading or writing the RFID tag.",
        91 or 92 => "RFID tag signal calibration failed.",
        96 => "The label peeler is waiting for the label to be removed.",
        99 or 120 => "The printer finished a firmware update. Follow the upgrade tool instructions.",
        _ => $"Printer error, code {code}."
    };
}
