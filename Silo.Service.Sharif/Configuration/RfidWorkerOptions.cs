using System.ComponentModel.DataAnnotations;

namespace Silo.Service.Sharif.Configuration;

public sealed class RfidWorkerOptions
{
    [Required(ErrorMessage = "کد ایستگاه الزامی است.")]
    [Display(Name = "کد ایستگاه")]
    public string StationCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع گیت الزامی است.")]
    [Display(Name = "نوع گیت")]
    public string GateType { get; set; } = string.Empty;

    [Range(1, 30, ErrorMessage = "توان آنتن باید بین ۱ تا ۳۰ باشد.")]
    [Display(Name = "توان آنتن")]
    public byte ReaderPower { get; set; }

    [Range(100, int.MaxValue, ErrorMessage = "تأخیر میان خوانش حداقل ۱۰۰ میلی‌ثانیه باید باشد.")]
    [Display(Name = "تأخیر میان خوانش (میلی‌ثانیه)")]
    public int IdleDelayMilliseconds { get; set; }

    public RfidWorkerOptions Clone()
        => new()
        {
            StationCode = StationCode,
            GateType = GateType,
            ReaderPower = ReaderPower,
            IdleDelayMilliseconds = IdleDelayMilliseconds
        };
}
