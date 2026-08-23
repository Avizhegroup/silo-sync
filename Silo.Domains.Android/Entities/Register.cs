using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Silo.Domains.Android;

[Table("tbl_RegisteringTags")]
public class Register
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(Order = 0)]
    public int Id { get; set; }

    [Column(Order = 1)]
    public string TagEpc { get; set; }

    [Column(Order = 2)]
    public string ProductSerial { get; set; }

    [Column(Order = 3)]
    public string ProductCode { get; set; }

    [Column(Order = 4)]
    public string RegisterUser { get; set; }

    [Column(Order = 5)]
    public string RegisterDateTime { get; set; }

    [Column(Order = 6)]
    public string ProductProduceLine { get; set; }

    [Column(Order = 7)]
    public string ProductProduceShift { get; set; }

    [Column(Order = 8)]
    public string ProductDocCode { get; set; }

    [Column(Order = 9)]
    public string ProductZone { get; set; }

    [Column(Order = 10)]
    public string ProductProperties { get; set; }
}
