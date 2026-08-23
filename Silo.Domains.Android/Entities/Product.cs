namespace Silo.Domains.Android;

[Table("tbl_Products")]
public class Product
{
    [Key]
    [Column("id", Order = 0)]
    public int Id { get; set; }

    [Column("productCode", Order = 1)]
    public string ProductCode { get; set; }

    [Column("productName", Order = 2)]
    public string ProductName { get; set; }  

    [Column("quality", Order = 3)]
    public string Quality { get; set; }

    [Column("technicalCode", Order = 4)]
    public string TechnicalCode { get; set; }

    [Column("productStatus", Order = 5)]
    public string ProductStatus { get; set; }

    [Column("productTypeCode", Order = 6)]
    public string ProductTypeCode { get; set; }

    [Column("productBrandCode", Order = 7)]
    public string ProductBrandCode { get; set; }

    [Column("productGroupCode", Order = 8)]
    public string ProductGroupCode { get; set; }    

    [Column("productSizeCode", Order = 9)]
    public string ProductSizeCode { get; set; }

    [Column("ProductPackValue", Order = 10)]
    public string ProductPackValue { get; set; }

    [Column("ProductUnit", Order = 11)]
    public string ProductUnit { get; set; }

     [Column("ProductSubGroup", Order = 12)]
    public string ProductSubGroup { get; set; }

    [Column("ProductClass", Order = 13)]
    public string ProductClass { get; set; }


    [Column("ProductValue", Order = 14)]
    public string ProductValue { get; set; }

    [Column("CountInNextlevelUnit", Order = 15)]
    public string CountInNextlevelUnit { get; set; }

    [Column("NextlevelUnitTitle", Order = 16)]
    public string NextlevelUnitTitle { get; set; }

    [Column("HasDoubleTag", Order = 17)]
    public bool? HasDoubleTag { get; set; }
}
