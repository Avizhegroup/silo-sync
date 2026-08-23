namespace Silo.Domains.Entities;

[Table("tbl_Gallery")]
public class Gallery
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_GalleryId")]
    public int Id { get; set; }

    [StringLength(128)]
    [Column("fld_GalleryUserId")]
    public string? UserId { get; set; }
    public User? User { get; set; }

    [StringLength(128)]
    [Column("fld_GalleryMediaName")]
    public string? MediaName { get; set; }

    [StringLength(512)]
    [Column("fld_GalleryMediaPath")]
    public string? MediaPath { get; set; }

    [Column("fld_GalleryUsageType")]
    public int? UsageType { get; set; }

    [Column("fld_GalleryUploadDateTime")]
    public DateTime? UpldoadDateTime { get; set; }

    [StringLength(128)]
    [Column("fld_GalleryUsageId")]
    public string? UsageId { get; set; }

    [Column("fld_GalleryMediaExtensionType")]
    public int? Extension { get; set; }

    [Column("fld_GalleryAdditionalData")]
    public string? Data { get; set; }
}
