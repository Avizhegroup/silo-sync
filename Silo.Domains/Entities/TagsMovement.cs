namespace Silo.Domains.Entities;

[Table("tbl_TagsMovement")]
public class TagsMovement
{
    [Key]
    [Column("TagsMovementId")]
    public int TagsMovementId { get; set; }

    [StringLength(50)]
    [Column("TagEpc")]
    public string? TagEpc { get; set; }

    [StringLength(50)]
    [Column("ProductCode")]
    public string? ProductCode { get; set; }
    public Product? Product { get; set; }

    [StringLength(50)]
    [Column("ProductSerial")]
    public string? ProductSerial { get; set; }
    public Tag? Tag { get; set; }

    [Column("ProductCount")]
    public decimal? ProductCount { get; set; }

    [Column("HMovementActionId")]
    public int? HMovementActionId { get; set; }

    [StringLength(10)]
    [Column("HTagsMovementDate")]
    public string? HTagsMovementDate { get; set; }

    [StringLength(5)]
    [Column("HTagsMovementTime")]
    public string? HTagsMovementTime { get; set; }

    [Column("HTagsMovementDateTime")]
    public DateTime? HTagsMovementDateTime { get; set; }

    [Column("HTagsMovementSt")]
    public int? HTagsMovementSt { get; set; }

    [Column("RMovementActionId")]
    public int? RMovementActionId { get; set; }
    public MovementAction? MovementAction { get; set; }

    [StringLength(10)]
    [Column("RTagsMovementDate")]
    public string? RTagsMovementDate { get; set; }

    [StringLength(5)]
    [Column("RTagsMovementTime")]
    public string? RTagsMovementTime { get; set; }

    [Column("RTagsMovementDateTime")]
    public DateTime? RTagsMovementDateTime { get; set; }

    [Column("RTagsMovementSt")]
    public int? RTagsMovementSt { get; set; }

    [Column("MovementData")]
    public string? MovementData { get; set; }

    [Column("ApiSendStatus")]
    public int? ApiSendStatus { get; set; }

    [StringLength(128)]
    [Column("ApiSendUser")]
    public string? ApiSendUser { get; set; }

    [Column("ApiSendDateTime")]
    public DateTime? ApiSendDateTime { get; set; }

    [Column("ApiSendData")]
    public string? ApiSendData { get; set; }

    [Column("RMovementActionType")]
    public int? RMovementActionType { get; set; }

    [StringLength(128)]
    [Column("RMovementActionDocumentId")]
    public string? RMovementActionDocumentId { get; set; }

    [StringLength(256)]
    [Column("RMovementActionUHFLogId")]
    public string? RMovementActionUHFLogId { get; set; }

    [StringLength(50)]
    [Column("RMovementActionDestinationId")]
    public string? RMovementActionDestinationId { get; set; }
}
