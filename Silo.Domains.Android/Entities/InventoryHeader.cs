namespace Silo.Domains.Android;

[Table("tbl_InventoryHeader")]
public class InventoryHeader
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_InventoryHeaderId")]
    public int InventoryHeaderId { get; set; }

    [Required]
    [Column("fld_InventoryDate")]
    public string InventoryDate { get; set; }

    [Required]
    [Column("fld_InventoryTime")]
    public string InventoryTime { get; set; }

    [Column("fld_InventoryStoreCodes")]
    public string? InventoryStoreCodes { get; set; }


    [Column("fld_InventoryUserId")]
    public string? InventoryUserId { get; set; }


    [Column("fld_InventoryDescription")]
    public string? InventoryDescription { get; set; }


    [Column("fld_InventoryStatus")]
    public int? InventoryStatus { get; set; }
 }
