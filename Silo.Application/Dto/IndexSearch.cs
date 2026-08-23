namespace Silo.Application.Dto;
public class IndexSearch
{
    public string FromDate { get; set; } = "-1";
    public string ToDate { get; set; } = "-1";
    public string Regcode { get; set; } = "-1";
    public string Shift { get; set; } = "-1";
    public string Pl { get; set; } = "-1";
}

public class EnterRequest
{
    public string ProductSerial { get; set; } = "-1";
    public string ProductCode { get; set; } = "-1";
    public string ProductName { get; set; } = "-1";
    public string FromDate { get; set; } = "-1";
    public string ToDate { get; set; } = "-1";
    public string FromTime { get; set; } = "-1";
    public string ToTime { get; set; } = "-1";
    public string TechnicalCode { get; set; } = "-1";
    public bool TechnicalCodeLike { get; set; } = false;
    public string Shift { get; set; } = "-1";
    public string Qc { get; set; } = "-1";
    public string Size { get; set; } = "-1";
    public string DestinationCode { get; set; } = "-1";
    public string ProductGroup { get; set; } = "-1";
    public string ProductBrand { get; set; } = "-1";
    public string ProductType { get; set; } = "-1";
    public string GateCode { get; set; } = "-1";
    public int? ActionType { get; set; }
}

public class StoreRequest
{
    public string ProductCode { get; set; }

    public string FromDate { get; set; }

    public string ToDate { get; set; }

    public string TechnicalCode { get; set; }

    public string ProductStatus { get; set; }

    public string ProductSerial { get; set; }

    public string TagZone { get; set; }

    public string AgeRange { get; set; }

    public bool TechnicalCodeLike { get; set; }
    public bool TagZoneLike { get; set; }

    public string EnterStatus { get; set; }
    public string Qc { get; set; } = "-1";
    public string Size { get; set; } = "-1";
    public string WarehouseCode { get; set; }
}

public class StoreProductRequest
{
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductSerial { get; set; }
    public string TagZone { get; set; }
    public string AgeRange { get; set; }
    public bool TechnicalCodeLike { get; set; } = true;
    public bool TagZoneLike { get; set; } = true;
    public string Capacity { get; set; }
    public string MinCapacity { get; set; }
    public string MaxCapacity { get; set; }
    public string ZoneLayer { get; set; }
    public string WarehouseCode { get; set; }
}

public class InventoryRequest
{
    public string ProductCode { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string TechnicalCode { get; set; }
    public string Qc { get; set; }
    public string User { get; set; }
    public string ProductSerial { get; set; }
    public string Desc { get; set; }
    public string Place { get; set; }
    public bool ConflictsShown { get; set; } = false;
    public string InventoryHeaderId { get; set; }
    public string Warehouse { get; set; }
    public string Size { get; set; }
    public string Type { get; set; }
    public bool TechnicalCodeLike { get; set; } = false;
}
