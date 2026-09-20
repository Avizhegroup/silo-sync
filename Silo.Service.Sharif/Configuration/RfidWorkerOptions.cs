using System.ComponentModel.DataAnnotations;

namespace Silo.Service.Sharif.Configuration;

public sealed class RfidWorkerOptions
{
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.SharifUi_StationCode_Required))]
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.SharifUi_StationCode))]
    public string StationCode { get; set; } = string.Empty;

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.SharifUi_GateType_Required))]
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.SharifUi_GateType))]
    public string GateType { get; set; } = string.Empty;

    [Range(1, 30, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.SharifUi_ReaderPower_Range))]
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.SharifUi_ReaderPower))]
    public byte ReaderPower { get; set; }

    [Range(100, int.MaxValue, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.SharifUi_IdleDelay_Range))]
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.SharifUi_IdleDelay))]
    public int IdleDelayMilliseconds { get; set; }

    public RfidWorkerOptions Clone()
        => new()
        {
            StationCode = StationCode,
            GateType = GateType,
            ReaderPower = ReaderPower,
            IdleDelayMilliseconds = IdleDelayMilliseconds
        };
}
