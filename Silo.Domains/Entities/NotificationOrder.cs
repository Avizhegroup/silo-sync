namespace Silo.Domains.Entities;

[Table("tbl_NotificationOrders")]
public class NotificationOrder
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("fld_NOId")]
    public int NOId { get; set; }

    [Column("fld_NOStatus")]
    public int Status { get; set; }

    [Column("fld_NODateTime")]
    public DateTime? DateTime { get; set; }

    [Column("fld_NOUserId")]
    [StringLength(50)]
    public string? UserId { get; set; }

    [Column("fld_NOType")]
    public int Type { get; set; }

    [Column("fld_NOTitle")]
    [StringLength(50)]
    public string? Title { get; set; }

    [Column("fld_NOEventType")]
    public int EventType { get; set; }

    [Column("fld_NOTimePeriod")]
    public int TimePeriod { get; set; }

    [Column("fld_NOSendDay")]
    [StringLength(50)]
    public string? SendDay { get; set; }

    [Column("fld_NOSendClock")]
    [StringLength(5)]
    public string? SendClock { get; set; }

    [Column("fld_NOSendType")]
    [StringLength(50)]
    public string? SendType { get; set; }

    [Column("fld_NOSendContacts")]
    public string? SendContacts { get; set; }

    [Column("fld_NOContent")]
    public string? Content { get; set; }

    public ICollection<NotificationQueue> NotificationQueues { get; set; }
}
