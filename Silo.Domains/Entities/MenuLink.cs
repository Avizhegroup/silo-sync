namespace Silo.Domains.Entities.Api;

[Table("tbl_MenuLink")]
public class MenuLink
{
    [Key]
    [Column("fld_MenuLinkId")]
    public int Id { get; set; }

    [Column("fld_MenuLinkTitle")]
    [StringLength(256)]
    public string? Title { get; set; }

    [Column("fld_MenuLinkParentId")]
    public int? ParentId { get; set; }
    public MenuLink? Parent { get; set; }

    public ICollection<MenuLink> ChildrenLinks { get; set; }

    [Column("fld_MenuLinkLevel")]
    public int? Level { get; set; }

    [Column("fld_MenuLinkUrl")]
    [StringLength(256)]
    public string? Url { get; set; }

    [Column("fld_MenuLinkShown")]
    public bool IsShown { get; set; }

    [Column("fld_MenuLinkIconName")]
    [StringLength(256)]
    public string? IconName { get; set; }

    [Column("fld_MenuLinkIsUserDedicated")]
    public bool? IsDedicated { get; set; }
}
