namespace Silo.Domains.Android;

[Table("tbl_Gallery")]
public class AndroidGallery
{
    [Key]
    [Column("fld_GalleryId")]
    public int Id { get; set; }

    [Column("fld_GalleryUserId")]
    public string? UserId { get; set; }

    [Column("fld_GalleryMediaName")]
    public string? MediaName { get; set; }

    [Column("fld_GalleryMediaPath")]
    public string? MediaPath { get; set; }

    [Column("fld_GalleryUsageType")]
    public int? UsageType { get; set; }

    [Column("fld_GalleryUploadDateTime")]
    public DateTime? UploadDateTime { get; set; }

    [Column("fld_GalleryUsageId")]
    public string? UsageId { get; set; }

    [Column("fld_GalleryMediaExtensionType")]
    public int? Extension { get; set; }

    [Column("fld_GalleryAdditionalData")]
    public string? Data { get; set; }
}
