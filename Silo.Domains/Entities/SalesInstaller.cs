namespace Silo.Domains.Entities;

[Table("tbl_SalesInstaller")]
public class SalesInstaller
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_SalesInstallerId")]
    public int Id { get; set; }

    [Column("fld_SalesInstallerCode")]
    [StringLength(128)]
    public string Code { get; set; }

    [Column("fld_SalesInstallerName")]
    [StringLength(128)]
    public string Name { get; set; }

    [StringLength(128)]
    [Column("fld_SalesInstallerUserId")]
    public string? UserId { get; set; }
    public User User { get; set; }
}
