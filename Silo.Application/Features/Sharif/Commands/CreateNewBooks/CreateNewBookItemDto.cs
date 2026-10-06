namespace Silo.Application.Features;

public class CreateNewBookItemDto
{
    /// <summary>
    /// Barcode = ProductSerial
    /// </summary>
    public string? Barcode { get; set; }
    public string? Title { get; set; }
    public string? description { get; set; }

    /// <summary>
    /// PID = ProducCode
    /// </summary>
    public string? PID { get; set; }

}
