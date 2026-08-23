namespace Silo.Domains.Entities;

[Table("tbl_ReportFormat")]
public class ReportFormat
{
    [Column("fld_ReportFormatId")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("fld_ReportFormatType")]
    public int? Type { get; set; }

    [Column("fld_ReportFormatUserId")]
    [StringLength(128)]
    public string? UserId { get; set; }
    public User User { get; set; }

    [Column("fld_ReportFormatPath")]
    [StringLength(256)]
    public string? Path { get; set; }

    [Column("fld_ReportFormatName")]
    [StringLength(256)]
    public string? Name { get; set; }
    
    [Column("fld_ReportFormatDetails")]
    public string? Details { get; set; }
}
