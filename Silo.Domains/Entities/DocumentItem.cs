namespace Silo.Domains.Entities;

[Table("tbl_DocumentItem")]
public class DocumentItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_Id")]
    public int Id { get; set; }

    [Column("fld_DocumentKey")]
    public string? Key { get; set; }
    
    [Column("fld_DocumentType")]
    public string DocumentType { get; set; }

    [Column("fld_DocumentType1")]
    public string DocumentType1 { get; set; }

    [Column("fld_DocumentType2")]
    public string DocumentType2 { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public DocumentHeader DocumentHeader { get; set; }

    [StringLength(50)]
    [Column("fld_DocumentItemProductCode")]
    public string? ProductCode { get; set; }

    [StringLength(50)]
    [Column("fld_DocumentItemProductTitle")]
    public string? ProductTitle { get; set; }

    [Column("fld_DocumentItemCount")]
    public decimal Count { get; set; }

    [StringLength(50)]
    [Column("fld_DocumentItemProducUnit")]
    public string? ProductUnit { get; set; }

    [Column("fld_DocumentItemsData")]
    public string? ItemData { get; set; }
}
