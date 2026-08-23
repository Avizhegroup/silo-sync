namespace Silo.Domains.Entities.Api;

[Table("tbl_UserQuickAccess")]
public class UserQuickAccess
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_UserQuickAccessId")]
    public int Id { get; set; }

    [Required]
    [StringLength(128)]
    [Column("fld_UserQuickAccessUserId")]
    public string UserId { get; set; }

    [Column("fld_UserQuickAccessMenuLinkId")]
    public int MenuLinkId { get; set; }
    public MenuLink MenuLink { get; set; }
}
