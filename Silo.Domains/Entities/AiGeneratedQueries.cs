namespace Silo.Domains.Entities;

[Table("tbl_AiGeneratedQueries")]
public class AiGeneratedQueries
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("UserId")]
    public string? UserId { get; set; }

    [Column("SessionId")]
    public int? SessionId { get; set; }

    [Column("QueryText")]
    public string QueryText { get; set; } = string.Empty;

    [Column("CreatedDate")]
    public DateTime CreatedDate { get; set; }
}
