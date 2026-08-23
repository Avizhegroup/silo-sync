namespace Silo.Application.Features;
public class SaveExpireGuaranteeByProductCodesCommand
{
    public List<string> ProductCodes { get; set; } = new();
    public GuaranteeTypes GuaranteeType { get; set; } = GuaranteeTypes.None;
    [Range(0, 48)]
    public int GuaranteeMonths { get; set; } = 0;
    public string GuaranteeDate { get; set; }
    
    public GuaranteeTypes ExpireType { get; set; } = GuaranteeTypes.None;
    [Range(0,48)]
    public int ExpireMonths { get; set; } = 0;
    public string ExpireDate { get; set; }
}
