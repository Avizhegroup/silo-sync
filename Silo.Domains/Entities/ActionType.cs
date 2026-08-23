namespace Silo.Domains.Entities;

[Table("tbl_ActionTypes")]
public class ActionType
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    [Key]
    public int Id { get; set; }

    [Column("fld_ActionTypeId")]
    public int? Code { get; set; }

    [Column("fld_ActionTypeFromDestinationType")]
    public string? From { get; set; }

    [Column("fld_ActionTypeToTypeDestinationType")]
    public string? To { get; set; }
    
    [Column("fld_ActionTypeTitle")]
    public string? Title { get; set; }

    [Column("fld_ActionTypePermitedDocStatus")]
    public string? DocStatusPermitted { get; set; }

    [Column("fld_ActionTypeChangeDocStatus")]
    public string? DocStatusChange { get; set; }

    [Column("fld_ActionTypeActiveControls")]
    public string? ActiveControls { get; set; }

    [Column("fld_ActionTypeRFIDPower")]
    public int? RFIDPower { get; set; }

    [Column("fld_ActionTypeChangeTagLocation")]
    public int? ChangeTagLocation { get; set; }

    [Column("fld_ActionTypeProductType")]
    public string? ProductType { get; set; }


    public ICollection<TruckCrossCause> EnterTruckCrossCauses { get; set; }
    public ICollection<TruckCrossCause> ExitTruckCrossCauses { get; set; }
}
