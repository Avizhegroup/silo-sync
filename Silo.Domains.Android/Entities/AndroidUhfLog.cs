namespace Silo.Domains.Android;

[Table("tbl_UHFLog")]
public class AndroidUhfLog
{
    [Key]
    [Column("Id", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("Epc", Order = 1, TypeName = "TEXT")]
    public string? Epc { get; set; }

    [Column("OperationCode", Order = 2, TypeName = "TEXT")]
    public string? OperationCode { get; set; }

    [Column("ActionType", Order = 3)]
    public int? ActionType { get; set; }

    [Column("DateTime", Order = 4)]
    public string? DateTime { get; set; }
}
