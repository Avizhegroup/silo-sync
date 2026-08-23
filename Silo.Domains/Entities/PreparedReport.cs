namespace Silo.Domains.Entities;

[Table("tbl_PreparedReports")]
public class PreparedReport
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_PRId")]
    public int Id { get; set; }

    [Required]
    [StringLength(128)]
    [Column("fld_PRTitle")]
    public string Title { get; set; }

    [Column("fld_PRVariables")]
    public string? Variables { get; set; }

    [Column("fld_PRDataSources")]
    public string? DataSources { get; set; }

    [Column("fld_PRImages")]
    public string? Images { get; set; }

    [Column("fld_PRUserId")]
    [StringLength(128)]
    public string UserId { get; set; }
    public User User { get; set; }

    [Required]
    [Column("fld_PRReportFileName")]
    public string ReportFileName { get; set; }
}
