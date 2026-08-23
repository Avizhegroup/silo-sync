namespace Silo.Domains.Entities;

[Table("tbl_SalesShop")]
public class SalesShop
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_SalesShopId")]
    public int Id { get; set; }

    [Column("fld_SalesShopCode")]
    [StringLength(128)]
    public string Code { get; set; }

    [Column("fld_SalesShopTitle")]
    [StringLength(128)]
    public string Title { get; set; }

    [Column("fld_SalesShopManagerName")]
    [StringLength(128)]
    public string ManagerName { get; set; }

    [Column("fld_SalesShopCity")]
    public int CityId { get; set; }

    [Column("fld_SalesShopProvince")]
    public int ProvinceId { get; set; }

    [StringLength(20)]
    [Column("fld_SalesShopPhone")]
    public string? Phone { get; set; }

    [StringLength(11)]
    [Column("fld_SalesShopMobile")]
    public string? Mobile { get; set; }

    [Column("fld_SalesShopAddress")]
    public string? Address { get; set; }

    [StringLength(128)]
    [Column("fld_SalesShopUserId")]
    public string? UserId { get; set; }
    public User User { get; set; }
}
