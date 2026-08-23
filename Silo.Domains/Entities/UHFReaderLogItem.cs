namespace Silo.Domains.Entities.Api;

[Table("tbl_UHF_ReaderLog")]
public class UHFReaderLogItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("fld_TagSerial")]
    [StringLength(50)]
    public string? TagSerial { get; set; }

    [Column("fld_Reader_Gate")]
    [StringLength(256)]
    public string? StationCode { get; set; }

    [Column("fld_ReaderIp")]
    [StringLength(15)]
    public string? ReaderIp { get; set; }

    [Column("fld_TagRead_DateTime")]
    [StringLength(50)]
    public string? TagReadDateTime { get; set; }

    [Column("fld_TagSelectedFlag")]
    public byte? TagSelectedFlag { get; set; }

    [Column("fld_InventoryId")]
    public int? UHFReaderLogHeaderId { get; set; }
    public UHFReaderLogHeader? UHFReaderLogHeader { get; set; }

    [Column("fld_Reader_GateType")]
    public int? ReaderGateType { get; set; }

    [Column("fld_Desc")]
    [StringLength(256)]
    public string? Description { get; set; }

    [Column("fld_ReaderDeviceType")]
    public int? ReaderDeviceType { get; set; }

    [Column("ActionStatus")]
    public int? ActionStatus { get; set; }

    [Column("ActionDesc")]
    [StringLength(250)]
    public string? ActionDescription { get; set; }

    [Column("fld_DocumentId")]
    [StringLength(128)]
    public string? DocumentCode { get; set; }

    [Column("fld_WMUserId")]
    [StringLength(50)]
    public string? WMUserId { get; set; }

    [Column("fld_InventoryPackage")]
    public int? InventoryPackage { get; set; }

    [Column("MovementActionId")]
    public int? MovementActionId { get; set; }

    [Column("fld_TagRead_DateTimeMiladi")]
    public DateTime? TagReadDateTimeMiladi { get; set; }

    [Column("fld_SaveUserId")]
    [StringLength(128)]
    public string? SaveUserId { get; set; }

    [Column("fld_ProductSerial")]
    [StringLength(50)]
    public string? ProductSerial { get; set; }
}
