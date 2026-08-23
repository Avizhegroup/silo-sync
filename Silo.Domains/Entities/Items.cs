namespace Silo.Domains.Entities;

[Table("tbl_Item")]
public class Items
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_Id")]
    public int Id { get; set; }

    [Required]
    [Column("fld_SaveDate")]
    public DateTime SaveDate { get; set; }

    [Required]
    [Column("fld_SaveUser")]
    [StringLength(128)]
    public string SaveUser { get; set; }
    
    [Column("fld_Data")]
    public string? Data { get; set; }
}
