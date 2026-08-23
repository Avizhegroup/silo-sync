namespace Silo.Application.Features;

public class SaveTruckCrossCommand
{
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque) )]
    public string Plaque { get; set; }

    public PlaqueParts PlaqueParts { get; set; } = new();


    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_DriverName) )]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DriverName { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Phone) )]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DriverPhone { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Company) )]
    public string Company { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_LicenseCode) )]
    public string LicenseCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_NationalCode) )]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string NationalCode { get; set; }
    public string Serial { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_TypeTruck) )]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int TypeId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_DescTruck) )]
    public string TypeDesc { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Description) )]
    public string? Desc { get; set; }

    #region Present
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Present_Cause) )]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int? PresentCause { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Turn) )]
    public int PresentTurn { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Present_DateTime) )]
    public DateTime? PresentDateTime { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Present_Desc) )]
    public string PresentDesc { get; set; }
    public string PresentUserId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Present_User) )]
    public string PresentUsername { get; set; }
    public bool PresentIsSaved { get; set; } = false;
    #endregion
    #region Enter
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Enter_DateTime) )]
    public DateTime? EnterDateTime { get; set; }
    public string EnterDesc { get; set; }
    public string EnterUserId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Enter_User) )]
    public string EnterUsername { get; set; }
    public string EnterEpc { get; set; }
    public string EnterOtherEpcs { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Enter_WeightTonage) )]
    public decimal EnterWeightTonage { get; set; }
    public bool EnterIsSaved { get; set; } = false;
    #endregion
    #region Exit
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_DateTime) )]
    public DateTime? ExitDateTime { get; set; }
    public string ExitDesc { get; set; }
    public string ExitUserId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_User) )]
    public string ExitUsername { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_WeightTonage) )]
    public decimal ExitWeightTonage { get; set; }
    public int ExitGateId { get; set; }
    public bool ExitIsSaved { get; set; } = false;
    public string ExitDestination { get; set; }
    #endregion

}
