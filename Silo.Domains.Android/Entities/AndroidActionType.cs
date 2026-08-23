namespace Silo.Domains.Android;

[Table("tbl_ActionTypes")]
public class AndroidActionType
{
    [Key]
    [Column("Id", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("fld_ActionTypeId", Order = 1)]
    public int? Code { get; set; }

    [Column("fld_ActionTypeFromDestinationType", Order = 2)]
    public string? From { get; set; }

    [Column("fld_ActionTypeToTypeDestinationType", Order = 3)]
    public string? To { get; set; }

    [Column("fld_ActionTypeTitle", Order = 4)]
    public string? Title { get; set; }

    [Column("fld_ActionTypePermitedDocStatus", Order = 5)]
    public string? DocStatusPermitted { get; set; }

    [Column("fld_ActionTypeChangeDocStatus", Order = 6)]
    public string? DocStatusChange { get; set; }
    

    [Column("fld_ActionTypeActiveControls", Order = 7)]
    public string? ActiveControls { get; set; }

    [Column("fld_ActionTypeRFIDPower", Order = 8)]
    public int? RFIDPower { get; set; }
}
