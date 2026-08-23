namespace Silo.Domains.Entities;

[Table("tbl_NotificationEventTypes")]
public class NotificationEventType
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("fld_NETitle")]
    [StringLength(50)]
    public string Title { get; set; }

    [Column("fld_NECommand")]
    public string Command { get; set; }
}
