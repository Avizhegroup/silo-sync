using Silo.Domains.Entities.Api;

namespace Silo.Domains.Entities;

[Table("tbl_DocumentHeader")]
public class DocumentHeader
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_Id")]
    public int Id { get; set; }

    [Key]
    [Column("fld_DocumentKey")]
    public string Key { get; set; }

    [StringLength(50)]
    [Column("fld_DocumentSaveUserId")]
    public string? UserId { get; set; }
    public User User { get; set; }  

    [Column("fld_DocumentImportType")]
    public ImportType ImportType { get; set; }

    [Column("fld_DocumentImportFileName")]
    public string? FileName { get; set; }

    [Column("fld_DocumentType")]
    public string? DocumentType { get; set; }

    [Column("fld_DocumentType1")]
    public string? DocumentType1 { get; set; }

    [Column("fld_DocumentType2")]
    public string? DocumentType2 { get; set; }

    [Column("fld_DocumentImportDatetime")]
    public DateTime? ImportDateTime { get; set; }

    [StringLength(200)]
    [Column("fld_DocumentDesc")]
    public string? Description { get; set; }

    [Column("fld_DocumentStatus")]
    public int DocumentStatusId { get; set; }
    public DocumentStatus DocumentStatus { get; set; }

    [Column("fld_DocumentHeaderData")]
    public string? HeaderData { get; set; }

    [Column("fld_DocumentParent")]
    public string? Parent { get; set; }

    [Column("fld_DocumentAggStatus")]
    public int? AggStatus { get; set; }

    [Column("fld_DocumentDivideParent")]
    public string? DivideParent { get; set; }

    [StringLength(50)]
    [Column("fld_DocumentChangeStatusLastUserId")]
    public string? ChangeStatusLastUserId { get; set; }

    [Column("fld_DocumentCheckType")]
    public int? DocumentCheckType { get; set; }

    public User UserStatus { get; set; }

    public ICollection<DocumentItem> DocumentItems { get; set; }
    public ICollection<UHFReaderLogHeader> UHFReaderLogHeaders { get; set; }
}

public enum ImportType
{
    Excel,
    Api,
    Manual,
    Other,
    Aggregate,
    Divide
}
