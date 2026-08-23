namespace Silo.Application.Features;
public class GetUhfLogReportSerialsVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public string ProductTitle { get; set; }
    public decimal ProductCount { get; set; }
    public int ActionStatus { get; set; }
    public UhflogActionStatusTitleEnum ActionStatusEnum
    {
        get
        {
            if (Enum.IsDefined(typeof(UhflogActionStatusTitleEnum), ActionStatus))
            {
                return (UhflogActionStatusTitleEnum)ActionStatus;
            }

            return UhflogActionStatusTitleEnum.Unknown;
        }
    }

    public string ActionStatusTitle => ActionStatusEnum switch
    {
        UhflogActionStatusTitleEnum.Ok => "بدون مشکل",
        UhflogActionStatusTitleEnum.NotInSource => "ناموجود در مبدا",
        UhflogActionStatusTitleEnum.NotRegistered => "رجیستر نشده",
        UhflogActionStatusTitleEnum.Frozen => "فریز شده",
        UhflogActionStatusTitleEnum.QcFailed => "کنترل کیفیت مردود شده",
        UhflogActionStatusTitleEnum.NotInspected => "بازرسی نشده",
        UhflogActionStatusTitleEnum.Ignored => "نادیده گرفتن",
        _ => ""
    };
}
