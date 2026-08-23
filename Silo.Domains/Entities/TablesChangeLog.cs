namespace Silo.Domains.Entities;

[Table("tbl_TablesChangeLog")]

public class TablesChangeLog
{
    [Key]
    [Column("Id")]
    public long Id { get; set; }

    [Column("TableName")]
    public string TableName { get; set; }

    [Column("RecordKey")]
    public string? RecordKey { get; set; }

    [Column("ChangeDescription")]
    public string Description { get; set; }

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("UserId")]
    public string? UserId { get; set; }
    public User? User { get; set; }
}
