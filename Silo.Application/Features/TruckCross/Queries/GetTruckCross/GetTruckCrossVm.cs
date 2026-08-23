namespace Silo.Application.Features;
public class GetTruckCrossVm
{
    public long Id { get; set; }
    public string Plaque { get; set; }
    public string InternationalPlaque { get; set; }
    public string DriverName { get; set; }
    public string DriverPhone { get; set; }
    public string NationalCode { get; set; }
    public string PassportCode { get; set; }
    public string Serial { get; set; }
    public int TypeId { get; set; }
    public string TypeDesc { get; set; }
    public int TruckCrossCompanyId { get; set; }
    public string LicenseCode { get; set; }
    public TruckCrossStatuses TruckCrossStatus { get; set; } = TruckCrossStatuses.None;
    #region Present
    public int PresentCause { get; set; }
    public int PresentTurn { get; set; }
    public DateTime? PresentDateTime { get; set; }
    public string PresentDesc { get; set; }
    public string PresentUserId { get; set; }
    public string PresentUsername { get; set; }
    public string PresentRevokeUserId { get; set; }
    public string PresentRevokeUsername { get; set; }
    public bool PresentIsSaved { get; set; } = false;
    public int PresentOperationTypeId { get; set; }
    public int PresentOperationDestinationId { get; set; }
    public int PresentShipmentId { get; set; }
    public string PresentShipmentNumber { get; set; }
    public int PresentCustomerId { get; set; }
    #endregion

    #region Enter
    public DateTime? EnterDateTime { get; set; }
    public string EnterDesc { get; set; }
    public string EnterUserId { get; set; }
    public string EnterUsername { get; set; }
    public string EnterEpc { get; set; }
    public string EnterOtherEpcs { get; set; }
    public decimal EnterWeightTonage { get; set; }
    public bool EnterIsSaved { get; set; } = false;
    public int EnterProductType { get; set; }
    public decimal EnterProductCount { get; set; }
    public string EnterProductUnit { get; set; }
    public string EnterAcceptor { get; set; }
    public int EnterAcceptPlaceId { get; set; }
    #endregion

    #region Exit
    public DateTime? ExitDateTime { get; set; }
    public string ExitDesc { get; set; }
    public string ExitUserId { get; set; }
    public string ExitUsername { get; set; }
    public decimal ExitWeightTonage { get; set; }
    public int ExitGateId { get; set; }
    public bool ExitIsSaved { get; set; } = false;
    public string ExitDestination { get; set; }
    public decimal ExitPureweightcargo { get; set; }
    public string ExitWeightBridgeReceiptNumber { get; set; }
    public string ExitCargoOwnerName { get; set; }
    public string ExitCargoOwnerPhone { get; set; }
    public string ExitDeliveryAddress { get; set; }
    public string ExitShipmentCost { get; set; }
    public int ExitPaymentType { get; set; }
    public string ExitUnitPrice { get; set; }
    public string ExitTotalCost { get; set; }
    public string ExitDistance { get; set; }
    #endregion
}
