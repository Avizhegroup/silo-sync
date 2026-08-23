using System.Text.Json.Serialization;

namespace Silo.Domains.Entities;

[Table("tbl_InputFileLog")]
public class InputFileData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_InputId")]
    public int Id { get; set; }

    [StringLength(256)]
    [Column("fld_InputFileName")]
    public string? FileName { get; set; }
    
    [StringLength(20)]
    [Column("fld_InputDateTime")]
    public DateTime? DateTime { get; set; }
   
    [StringLength(128)]
    [Column("fld_InputType")]
    public string? Type { get; set; }

    [StringLength(128)]
    [Column("fld_InputType1")]
    public string? Type1 { get; set; }
 
    [StringLength(128)]
    [Column("fld_InputType2")]
    public string? Type2 { get; set; }
 
    [StringLength(128)]
    [Column("fld_InputUser")]
    [JsonIgnore]
    public string? User { get; set; }

    [Column("fld_InputData")]
    [JsonIgnore]
    public string? Data { get; set; }
}
