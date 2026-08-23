namespace Silo.Application.Features;

public class SavePrintCommand
{
    [Required]
    public string SelectedLine { get; set; } = string.Empty;

    [Required]
    public string SelectedShift { get; set; } = string.Empty;

    [Required]
    public string SelectedWarehouse { get; set; } = string.Empty;

    public string SelectedStatus { get; set; } = string.Empty;

    public string DocumentId { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Count { get; set; } = 1;

    [Required]
    public string ProductCode { get; set; } = string.Empty;

    [Required]
    public string ProductStatus { get; set; } = string.Empty;

    public string ProductRegCode { get; set; } = string.Empty;
    public string ProductTitle { get; set; } = string.Empty;
    public string ProductSize { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string ProductUnit { get; set; } = string.Empty;
    public string ProductValue { get; set; } = string.Empty;
    public string ProductCountInPack { get; set; } = string.Empty;
    public string ProductPackValue { get; set; } = string.Empty;
    public string ProductPackWeight { get; set; } = string.Empty;
    public string ProductPackVolume { get; set; } = string.Empty;
    public string ProductStatusTitle { get; set; } = string.Empty;
}
