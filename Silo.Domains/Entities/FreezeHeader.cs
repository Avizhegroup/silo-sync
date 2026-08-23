namespace Silo.Domains.Entities;

[Table("tbl_FreezeHeader")]
public class FreezeHeader
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_FreezeHeaderId")]
    public int Id { get; set; }

    [StringLength(128)]
    [Column("fld_FreezeUserId")]
    public string? UserId { get; set; }
    public User User { get; set; }

    [Column("fld_FreezeSaveDateTime")]
    public DateTime? SaveDateTime { get; set; } = DateTime.Now;

    [StringLength(256)]
    [Column("fld_FreezeDesc")]
    public string? Description { get; set; }

    [Column("fld_FreezeResult")]
    public bool Status { get; set; }

    public ICollection<FreezeItem> FreezeItems { get; set; } 
}