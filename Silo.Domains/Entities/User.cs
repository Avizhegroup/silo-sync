using Silo.Domains.Entities.Api;

namespace Silo.Domains.Entities;

[Table("tbl_User")]
public class User
{
    [Key]
    [StringLength(128)]
    public string Id { get; set; }

    [Column(TypeName = "datetime")]
    [Required]
    public DateTime LastModifiedDate { get; set; }

    [Column(TypeName = "datetime")]
    [Required]
    public DateTime CreateDate { get; set; }
    
    [Required]
    public bool IsActive { get; set; }

    [Required]
    [StringLength(128)]
    public string CreatorIdentityID { get; set; }
    
    [StringLength(128)]
    public string? LastModifierIdentityID { get; set; }
    
    [Required]
    [StringLength(512)]
    public string Name { get; set; }
    
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Row { get; set; }
    
    [StringLength(256)]
    public string? Email { get; set; }
    
    [Required]
    public bool EmailConfirmed { get; set; }
    
    public string? PasswordHash { get; set; }
    public string? SecurityStamp { get; set; }
    public string? PhoneNumber { get; set; }
    
    [Required]
    public bool PhoneNumberConfirmed { get; set; }

    [Required]
    public bool TwoFactorEnabled { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LockoutEndDateUtc { get; set; }

    [Required]
    public bool LockoutEnabled { get; set; }

    [Required]
    public int AccessFailedCount { get; set; }

    [Required]
    [StringLength(256)]
    public string Username { get; set; }

    public string? Details { get; set; }

    [StringLength(50)]
    public string? Image { get; set; }

    public ICollection<TruckCrossData> PresentCrosses { get; set; }
    public ICollection<TruckCrossData> PresentRevokeCrosses { get; set; }
    public ICollection<TruckCrossData> EnterCrosses { get; set; }
    public ICollection<TruckCrossData> ExitCrosses { get; set; }
    public ICollection<Gallery> GalleryMedias { get; set; }
    public ICollection<DynamicField> DynamicFields { get; set; }
    public ICollection<FreezeHeader> FreezeHeaders { get; set; }
    public ICollection<ReportFormat> ReportFormats { get; set; }
    public ICollection<DocumentLog> DocumentLogs { get; set; }
    public ICollection<DocumentHeader> DocumentHeaders { get; set; }
    public ICollection<DocumentHeader> DocumentHeadersStatus { get; set; }
    public ICollection<ExpireGuaranteeLog> ExpireGuaranteeLogs { get; set; }
    public ICollection<SalesShop> SalesShops { get; set; }
    public ICollection<SalesInstaller> SalesInstallers { get; set; }
    public ICollection<UHFReaderLogHeader> UHFReaderLogHeaders { get; set; }
    public ICollection<UserClaim> UserClaims { get; set; }
    public ICollection<PreparedReport> PreparedReports { get; set; }
    public ICollection<UserToken> Tokens { get; set; }
    public ICollection<TablesChangeLog> ChangeLogs { get; set; }
    public ICollection<Print> Prints { get; set; }
    public ICollection<GPSLogs> GPSLogs { get; set; }
}
