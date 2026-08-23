namespace Silo.Domains.Entities;

[Table("tbl_NotificationQueue")]
public class NotificationQueue
{
    [Key]
    [Column("fld_Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("fld_Text")]
    public string Text { get; set; }

    [Required]
    [Column("fld_SendType")]
    public int SendType { get; set; }

    [Required]
    [Column("fld_Contact")]
    [StringLength(256)]
    public string Contact { get; set; }

    [Column("fld_SendDateTime")]
    public DateTime? SendDateTime { get; set; }
    
    [Column("fld_SendDate")]
    [StringLength(10)]
    public string? SendDate { get; set; }

    [Column("fld_SendTime")]
    [StringLength(5)]
    public string? SendTime { get; set; }

    [Required]
    [Column("fld_SendStatus")]
    public int Status { get; set; }

    [Required]
    [Column("fld_NotificationOrderId")]
    public int OrderId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public NotificationOrder NotificationOrder { get; set; }

    [Column("fld_QueueActionCode")]
    [StringLength(256)]
    public string? ActionCode { get; set; }

    [Required]
    [Column("fld_SaveDateTime")]
    public DateTime SaveDateTime { get; set; }
}
