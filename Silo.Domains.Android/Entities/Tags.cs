namespace Silo.Domains.Android;

[Table("tbl_tags")]
public class Tag
{
    [Key]
    [Column(Order = 9)]
    public int Id { get; set; }

    [Column("tagEpc", Order = 0)]
    public string TagEpc { get; set; }

    [Column("serial", Order = 1)]
    public string Serial { get; set; }

    [Column("code", Order = 2)]
    public string ProductCode { get; set; }

    [Column("tagZone", Order = 3)]
    public string TagZone { get; set; }   

    [Column("productCount", Order = 4)]
    public string ProductCount { get; set; }

    [Column("tagStatus", Order = 5)]
    public string TagStatus { get; set; }

    [Column("technicalCode", Order = 6)]
    public string technicalCode { get; set; }

    [Column("tagInDestination", Order = 7)]
    public string TagInDestination { get; set; }

    [Column("productproperties", Order = 8)]
    public string ProductProperties { get; set; }

    [Column("FreezStatus", Order = 10)]
    public string FreezStatus { get; set; } 

    [Column("InspectStatus", Order = 11)]
    public string InspectStatus { get; set; }

    [Column("LockStatus", Order = 12)]
    public string LockStatus { get; set; }

    [Column("ShamsiRegisterDateUnix", Order = 13)]
    public string ShamsiRegisterDateUnix { get; set; }

    [Column("MiladiRegisterDate", Order = 14)]
    public string MiladiRegisterDate { get; set; }
 
    [Column("TagTreeParentsEpc", Order = 15)]
    public string TagTreeParentsEpc { get; set; }

    [Column("TagEpc2", Order = 16)]
    public string? TagEpcSecond { get; set; }
}
