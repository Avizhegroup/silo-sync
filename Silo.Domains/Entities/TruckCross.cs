using Silo.Domains.Entities.Api;

namespace Silo.Domains.Entities;

[Table("tbl_TruckCross")]
public class TruckCrossData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_TruckCrossId")]
    public long Id { get; set; }

    [StringLength(20)]
    [Column("fld_TruckCrossPlaque")]
    public string? Plaque { get; set; }

    [StringLength(20)]
    [Column("fld_TruckCrossInternationalPlaque")]
    public string? InternationalPlaque { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossDriverName")]
    public string? DriverName { get; set; }

    [StringLength(20)]
    [Column("fld_TruckCrossDriverPhone")]
    public string? DriverPhone { get; set; }

    [Required]
    [StringLength(20)]
    [Column("fld_TruckCrossNationalCode")]
    public string NationalCode { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossPassportCode")]
    public string? PassportCode { get; set; }

    [Column("fld_TruckCrossSerial")]
    public string? Serial { get; set; }

    [Column("fld_TruckCrossCompany")]
    public int? TruckCrossCompanyId { get; set; }
    public TruckCrossCompany? TruckCrossCompany { get; set; }

    [Column("fld_TruckCrossType")]
    public int? TypeId { get; set; }
    public TruckType? Type { get; set; }

    [StringLength(256)]
    [Column("fld_TruckCrossTypeDesc")]
    public string? TypeDesc { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossLicenseCode")]
    public string? LicenseCode { get; set; }

    [Column("fld_TruckCrossStatus")]
    public int? TruckCrossStatus { get; set; }

    [Column("fld_TruckCrossPresentCause")]
    public int? PresentCause { get; set; }
    public TruckCrossCause? Cause { get; set; }

    [Column("fld_TruckCrossPresentTurn")]
    public int? PresentTurn { get; set; }

    [Column("fld_TruckCrossPresentDateTime", TypeName = "datetime")]
    public DateTime? PresentDateTime { get; set; }

    [StringLength(250)]
    [Column("fld_TruckCrossPresentDesc")]
    public string? PresentDesc { get; set; }

    public User? PresentUser { get; set; }

    [StringLength(128)]
    [Column("fld_TruckCrossPresentUserId")]
    public string? PresentUserId { get; set; }

    [Column("fld_TruckCrossPresentOperationType")]
    public int? PresentOperationTypeId { get; set; }
    public TruckCrossOperationType? OperationType { get; set; }

    [Column("fld_TruckCrossPresentOperationDestination")]
    public int? PresentOperationDestinationId { get; set; }
    public TruckCrossOperationDestination? OperationDestination { get; set; }

    [Column("fld_TruckCrossPresentShipment")]
    public int? PresentShipmentId { get; set; }
    public TruckCrossShipment? Shipment { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossPresentShipmentNumber")]
    public string? PresentShipmentNumber { get; set; }

    [Column("fld_TruckCrossPresentCustomer")]
    public int? PresentCustomerId { get; set; }
    public TruckCrossCustomer? Customer { get; set; }

    [Column("fld_TruckCrossEnterDateTime", TypeName = "datetime")]
    public DateTime? EnterDateTime { get; set; }

    [Column("fld_TruckCrossEnterWeightTonage")]
    public decimal? EnterWeightTonage { get; set; }

    [StringLength(250)]
    [Column("fld_TruckCrossEnterDesc")]
    public string? EnterDesc { get; set; }

    public User? EnterUser { get; set; }

    [StringLength(128)]
    [Column("fld_TruckCrossEnterUserId")]
    public string? EnterUserId { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossEnterEpc")]
    public string? EnterEpc { get; set; }

    [StringLength(250)]
    [Column("fld_TruckCrossEnterOtherEpcs")]
    public string? EnterOtherEpcs { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossEnterAcceptor")]
    public string? EnterAcceptor { get; set; }

    [Column("fld_TruckCrossEnterAcceptPlace")]
    public int? EnterAcceptPlaceId { get; set; }
    public TruckCrossAcceptPlace? EnterAcceptPlace { get; set; }

    [Column("fld_TruckCrossPresentRevokeDateTime")]
    public DateTime? PresentRevokeDateTime { get; set; }

    public User? PresentRevokeUser { get; set; }

    [Column("fld_TruckCrossPresentRevokeUserId")]
    [StringLength(128)]
    public string? PresentRevokeUserId { get; set; }

    [Column("fld_TruckCrossExitDateTime", TypeName = "datetime")]
    public DateTime? ExitDateTime { get; set; }

    [StringLength(250)]
    [Column("fld_TruckCrossExitDesc")]
    public string? ExitDesc { get; set; }

    public User? ExitUser { get; set; }

    [StringLength(128)]
    [Column("fld_TruckCrossExitUserId")]
    public string? ExitUserId { get; set; }

    [Column("fld_TruckCrossExitWeightTonage")]
    public decimal? ExitWeightTonage { get; set; }

    [Column("fld_TruckCrossExitGateId")]
    public int? ExitGateId { get; set; }
    
    [StringLength(128)]
    [Column("fld_TruckCrossExitDestination")]
    public string? ExitDestination { get; set; }

    [Column("fld_TruckCrossExitPureWeightCargo")]
    public decimal? ExitPureWeightCargo { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossExitWeightbridgeReceiptNumber")]
    public string? ExitWeightbridgeReceiptNumber { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossExitCargoOwnerName")]
    public string? ExitCargoOwnerName { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossExitCargoOwnerPhone")]
    public string? ExitCargoOwnerPhone { get; set; }

    [StringLength(250)]
    [Column("fld_TruckCrossExitDeliveryAddress")]
    public string? ExitDeliveryAddress { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossExitShipmentCost")]
    public string? ExitShipmentCost { get; set; }

    [Column("fld_TruckCrossExitPaymentType")]
    public int? ExitPaymentType { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossExitUnitPrice")]
    public string? ExitUnitPrice { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossExitTotalCost")]
    public string? ExitTotalCost { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossExitDistance")]
    public string? ExitDistance { get; set; }

    [Column("fld_TruckCrossDynamicFields")]
    public string? DynamicData { get; set; }

    public MovementAction MovementAction { get; set; }

    public ICollection<TruckCrossItem> TruckCrossItems { get; set; }
    public ICollection<UHFReaderLogHeader> UHFReaderLogHeaders { get; set; }
}
