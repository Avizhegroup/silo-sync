namespace Silo.Domains.Android;

[Table("tbl_InventoryTags")]
public class InventoryTags
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_Id")]
    public int Id { get; set; }

    [Required]
    [Column("fld_Epc")]
    public string Epc { get; set; }

    [Required]
    [Column("fld_HeaderId")]
    public int HeaderId { get; set; }

    [Column("fld_StoreCode")]
    public string? StoreCode { get; set; }

    [Column("fld_InventoryUser")]
    public string? InventoryUser { get; set; }
}
