namespace Silo.Domains.Entities;

[Table("tbl_UserTokens")]
public class UserToken
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_Id")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TokenValue")]
    [StringLength(1000)]
    public string Value { get; set; }

    [Required]
    [Column("fld_TokenUserId")]
    [StringLength(128)]
    public string UserId { get; set; }
    public User User { get; set; }

    [Required]
    [Column("fld_TokenHasExpired")]
    public bool HasExpired { get; set; }
}
