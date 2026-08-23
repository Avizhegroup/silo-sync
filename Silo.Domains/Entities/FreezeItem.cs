namespace Silo.Domains.Entities;

[Table("tbl_FreezeItem")]
public class FreezeItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_FreezeItemId")]
    public int Id { get; set; }
    
    [StringLength(50)]
    [Column("fld_FreezeProductSerial")]
    public string? ProductSerial { get; set; }
    
    [Column("fld_FreezeHeaderId")]
    public int FreezeHeaderId { get; set; }

    public FreezeHeader FreezeHeader { get; set; }
}