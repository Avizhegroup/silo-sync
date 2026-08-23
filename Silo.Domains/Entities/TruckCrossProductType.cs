namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossProductType")]
public class TruckCrossProductType
{
    [Key]
    [Column("fld_TruckCrossProductTypeId")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossProductTypeTitle")]
    [StringLength(256)]
    public string Title { get; set; }

    [Column("fld_TruckCrossCauseIdsArray")]
    public string? TruckCrossCauseIdsArray { get; set; }
    
    [NotMapped]
    public int[] TruckCrossCauseIds
    {
        get
        {
            if (string.IsNullOrEmpty(TruckCrossCauseIdsArray))
                return Array.Empty<int>();
            return TruckCrossCauseIdsArray.Split(',').Select(p => int.Parse(p)).ToArray();
        }
        set
        {
            if (value is not null)
            {
                TruckCrossCauseIdsArray = string.Join(',', value);
            }
            else
            {
                TruckCrossCauseIdsArray = string.Empty;
            }
        }
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<TruckCrossItem> TruckCrossItems { get; set; }
    public ICollection<TruckCrossShipmentFee> TruckCrossShipmentFees { get; set; }
}
