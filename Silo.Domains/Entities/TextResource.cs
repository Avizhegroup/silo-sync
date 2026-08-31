namespace Silo.Domains.Entities;

[Table("tbl_TextResources")]
public class TextResource
{
    [Key]
    [Column("fld_TextResourceId", Order = 0)]
    public int Id { get; set; }

    [Required]
    [StringLength(512)]
    [Column("fld_TextResourceKey", Order = 1)]
    public string Key { get; set; } = null!;

    [Column("fld_TextResourceValue", Order = 2)]
    public string? Value { get; set; }
}
