namespace Silo.Domains.Entities.Api;

[Table("tbl_Station")]
public class Station
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_StationId")]
    public int Id { get; set; }

    [Column("fld_StationCode")]
    [StringLength(128)]
    public string? Code { get; set; }

    [Column("fld_StationName")]
    [StringLength(512)]
    public string? Name { get; set; }

    [Column("fld_StationType")]
    public int? Type { get; set; }

    [Column("fld_StationStatus")]
    public int? StationStatus { get; set; }
    
    [Column("fld_StationReaders")]
    public string? Readers { get; set; } 
    
    [StringLength(1024)]
    [Column("fld_StationDescription")]
    public string? Desc { get; set; }

    [Column("fld_StationSettings")]
    public string? Settings { get; set; }

    [Column("fld_StationFromDestination")]
    public string? From { get; set; }

    [Column("fld_StationToDestination")]
    public string? To { get; set; }
    
    [Column("fld_StationActionType")]
    public int? ActionType { get; set; }

    [Column("fld_StationMacAddress")]
    [StringLength(50)]
    public string? MacAddress { get; set; }

    public ICollection<UHFReaderLogHeader> UHFReaderLogHeaders { get; set; }
}

