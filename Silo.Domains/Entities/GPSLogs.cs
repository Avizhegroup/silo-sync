namespace Silo.Domains.Entities;


[Table("tbl_GPSLogs")]
public class GPSLogs
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_GpsLogId")]
    public int LogId { get; set; }

    [StringLength(128)]
    [Column("fld_GpsLogUserId")]
    public string? UserId { get; set; }
    public User? User { get; set; }

    [StringLength(128)]
    [Column("fld_GpsLogLat")]
    public string? Lat { get; set; }

    [StringLength(128)]
    [Column("fld_GpsLogLong")]
    public string? Long { get; set; }

    [Column("fld_GpsLogUsageType")]
    public int? UsageType { get; set; }

    [Column("fld_GpsLogDateTime")]
    public DateTime? LogDateTime { get; set; }

    [StringLength(128)]
    [Column("fld_GpsLogUsageId")]
    public string? UsageId { get; set; }
   
    [Column("fld_GpsLogAdditionalData")]
    public string? AdditionalData { get; set; }
}

