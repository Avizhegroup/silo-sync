using System.Runtime.InteropServices;
using System.Drawing.Printing;
using System.Text;

namespace Silo.Windows.Register.Devices;

public static class PrinterService
{
    public static IReadOnlyList<string> GetInstalledPrinters()
        => PrinterSettings.InstalledPrinters.Cast<string>().ToList();

    public static void SendRaw(string printerName, string data)
    {
        if (!OpenPrinter(printerName, out var handle, IntPtr.Zero))
        {
            throw new InvalidOperationException($"Cannot open printer '{printerName}'.");
        }

        try
        {
            var info = new DocInfo { DocName = "Silo label", DataType = "RAW" };
            if (!StartDocPrinter(handle, 1, info))
            {
                throw new InvalidOperationException("StartDocPrinter failed.");
            }

            try
            {
                StartPagePrinter(handle);
                var bytes = Encoding.UTF8.GetBytes(data);
                var buffer = Marshal.AllocCoTaskMem(bytes.Length);
                try
                {
                    Marshal.Copy(bytes, 0, buffer, bytes.Length);
                    if (!WritePrinter(handle, buffer, bytes.Length, out _))
                    {
                        throw new InvalidOperationException("WritePrinter failed.");
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(buffer);
                }
                EndPagePrinter(handle);
            }
            finally
            {
                EndDocPrinter(handle);
            }
        }
        finally
        {
            ClosePrinter(handle);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private sealed class DocInfo
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string? DocName;
        [MarshalAs(UnmanagedType.LPWStr)] public string? OutputFile;
        [MarshalAs(UnmanagedType.LPWStr)] public string? DataType;
    }

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool OpenPrinter(string printer, out IntPtr handle, IntPtr defaults);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr handle);

    [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool StartDocPrinter(IntPtr handle, int level, [In] DocInfo info);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr handle);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr handle);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr handle);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr handle, IntPtr bytes, int count, out int written);
}
