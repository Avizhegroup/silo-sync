
namespace Silo.Domains.Entities;

[Table("tbl_DashboardItems")]
public class DashboardItems
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("ItemCode")]
    public string ItemCode { get; set; }

    [Column("ItemTitle")]
    public string ItemTitle { get; set; }

    [Column("ItemType")]
    public string ItemType { get; set; }
}
