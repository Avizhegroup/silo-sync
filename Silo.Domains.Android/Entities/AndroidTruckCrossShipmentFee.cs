namespace Silo.Domains.Android;

[Table("tbl_TruckCrossShippingFee")]
public class AndroidTruckCrossShipmentFee
{
    [Key]
    [Column("fld_TruckCrossShippingFeeId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Column("fld_TruckCrossShippingFeeCompanyId", Order = 1)]
    public int? CompanyId { get; set; }

    [Column("fld_TruckCrossShippingFeeCustomerId", Order = 2)]
    public int? CustomerId { get; set; }

    [Column("fld_TruckCrossShippingFeeProductTypeId", Order = 3)]
    public int? ProductTypeId { get; set; }

    [Column("fld_TruckCrossShippingFeeShipmentId", Order = 4)]
    public int? ShipmentId { get; set; }

    [Column("fld_TruckCrossShippingFeeFromDate", Order = 5)]
    [StringLength(50)]
    public string? FromDate { get; set; }

    [Column("fld_TruckCrossShippingFeeToDate", Order = 6)]
    [StringLength(50)]
    public string? ToDate { get; set; }

    [Column("fld_TruckCrossShippingFeeStatus", Order = 7)]
    public bool FeeStatus { get; set; }

    [Column("fld_TruckCrossShippingFeeAmount", Order = 8)]
    public decimal FeeAmount { get; set; }

    [Column("fld_TruckCrossShippingFeeWeight", Order = 9)]
    public decimal FeeWeight { get; set; }

    [Column("fld_TruckCrossShippingFeeDistance", Order = 10)]
    public decimal FeeDistance { get; set; }
}
