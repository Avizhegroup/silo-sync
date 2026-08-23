namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossShippingFee")]
public class TruckCrossShipmentFee
{
    [Key]
    [Column("fld_TruckCrossShippingFeeId")]
    public int Id { get; set; }

    [Column("fld_TruckCrossShippingFeeCompanyId")]
    public int? CompanyId { get; set; }
    public TruckCrossCompany? TruckCrossCompany { get; set; }

    [Column("fld_TruckCrossShippingFeeCustomerId")]
    public int? CustomerId { get; set; }
    public TruckCrossCustomer? TruckCrossCustomer { get; set; }

    [Column("fld_TruckCrossShippingFeeProductTypeId")]
    public int? ProductTypeId { get; set; }
    public TruckCrossProductType? TruckCrossProductType { get; set; }

    [Column("fld_TruckCrossShippingFeeShipmentId")]
    public int? ShipmentId { get; set; }
    public TruckCrossShipment? TruckCrossShipment { get; set; }

    [Column("fld_TruckCrossShippingFeeFromDate")]
    [StringLength(50)]
    public string? FromDate { get; set; }

    [Column("fld_TruckCrossShippingFeeToDate")]
    [StringLength(50)]
    public string? ToDate { get; set; }

    [Column("fld_TruckCrossShippingFeeStatus")]
    public bool FeeStatus { get; set; }

    [Column("fld_TruckCrossShippingFeeAmount")]
    public decimal FeeAmount { get; set; }

    [Column("fld_TruckCrossShippingFeeWeight")]
    public decimal FeeWeight { get; set; }

    [Column("fld_TruckCrossShippingFeeDistance")]
    public decimal FeeDistance { get; set; }
}
