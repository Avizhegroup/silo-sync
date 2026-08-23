namespace Silo.Domains.Android;

[Table("tbl_Permission")]
public class AndroidPermission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_Id")]
    public int Id { get; set; }

    [Column("fld_UserId")]
    public string? UserId { get; set; }

    [Column("fld_PermissionText")]
    public string? Permission { get; set; }
}
