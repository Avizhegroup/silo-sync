namespace Silo.Application.Dto;

public class FreezeSaveDto
{
    public bool Status { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

    public List<string> Serials { get; set; }
}

public class GetFreezeHeaderReportDto
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string UserId { get; set; }
    public string TechnicalCode { get; set; }
    public bool TechnicalCodeLike { get; set; }
    public string ProductCode { get; set; }
    public string ProductSerial { get; set; }
}

public class GetFreezeItemReportDto
{
    public string HeaderId { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string UserId { get; set; }
    public string TechnicalCode { get; set; }
    public bool TechnicalCodeLike { get; set; }
    public string ProductCode { get; set; }
    public string ProductSerial { get; set; }
}

public class GetFreezeHeaderBySerialDto
{
    public int Id { get; set; }

    [StringLength(128)]
    public string? UserId { get; set; }

    public string UserName { get; set; }

    public DateTime? SaveDateTime { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

    public bool Status { get; set; }
}
