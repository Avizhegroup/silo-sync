namespace Silo.Application.Features;

public class GetTagsBySerialsDto
{
    /// <summary>
    /// سری ساخت
    /// </summary>
    public string ProductSerial { get; set; }

    /// <summary>
    /// سریال RFID
    /// </summary>
    public string Epc { get; set; }

    public GetTagsBySerialsStatusEnum Status { get; set; }

    /// <summary>
    /// کد انبار
    /// </summary>
    public string WarehouseCode { get; set; }

    /// <summary>
    /// کد لوکیشن
    /// </summary>
    public string LocationCode { get; set; }
}
