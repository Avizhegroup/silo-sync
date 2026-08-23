namespace Silo.Domains.Entities;

[Table("tbl_UserClaim")]
public class UserClaim
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(128)]
    public string UserId { get; set; }
    public User User { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}
