namespace Silo.Domains.Entities;

[Table("tbl_ChatSessions")]
public class ChatSessions
{
    [Key]
    [Column("SessionId")]
    public int SessionId { get; set; } 

    [Column("UserId")]
    public string? UserId { get; set; }

    [Column("SessionData")]
    public string? SessionData { get; set; }

    [Column("SessionMode")]
    public int? Mode { get; set; }

    [Column("CreatedDate")]
    public DateTime CreatedDate { get; set; }

    [Column("LastUpdated")]
    public DateTime LastUpdated { get; set; }

    [Column("TokenUsage")]
    public string? TokenUsage { get; set; }

    [Column("PriceUsage")]
    public decimal? PriceUsage { get; set; }
}
