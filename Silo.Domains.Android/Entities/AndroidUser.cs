namespace Silo.Domains.Android;

[Table("tbl_User")]
public class AndroidUser
{
    [Column("fld_Id")]
    [Key]
    public string Id { get; set; }

    [Column("fld_Name")]
    public string? Name { get; set; }

    [Column("fld_Username")]
    public string? Username { get; set; }

    [Column("fld_PasswordHash")]
    public string? PasswordHash { get; set; }
}
