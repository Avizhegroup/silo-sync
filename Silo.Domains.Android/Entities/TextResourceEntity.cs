namespace Silo.Domains.Android;

[Table("tbl_TextResources")]
public class TextResourceEntity
{
    [Key]
    [Column("fld_TextResourceId", Order =0)]
    public int Id { get; set; }

    [Column("fld_TextResourceKey", Order = 1)]
    public string? Key { get; set; }

    [Column("fld_TextResourceValue", Order = 2)]
    public string? Value { get; set; }
}
