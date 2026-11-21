namespace Silo.Application.Features;
public class UpdateTagCountDto
{
    /// <summary>
    /// سری ساخت
    /// </summary>
    [Required]
    public string ProductSerial { get; set; }

    /// <summary>
    /// مقدار اصلی
    /// تنها در صورتی که مقدار اصلی تغییر کرده باشد، ارسال شود. وگرنه مقدار null باشد.
    /// </summary>
    public decimal? OriginalCount { get; set; }

    /// <summary>
    /// مقدار فرعی
    /// تنها در صورتی که مقدار اصلی تغییر کرده باشد، ارسال شود. وگرنه مقدار null باشد. 
    /// </summary>
    public decimal? SubCount { get; set; }

    /// <summary>
    ///  واحد برای مقدار اصلی
    /// تنها در صورتی که مقدار اصلی تغییر کرده باشد، ارسال شود. وگرنه مقدار null باشد.
    /// </summary>
    public string? UnitForOriginalCount { get; set; }

    /// <summary>
    ///  واحد برای مقدار فرعی
    /// تنها در صورتی که مقدار اصلی تغییر کرده باشد، ارسال شود. وگرنه مقدار null باشد.
    /// </summary>
    public string? UnitForSubCount { get; set; }
}
