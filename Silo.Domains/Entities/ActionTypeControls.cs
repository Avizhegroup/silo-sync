namespace Silo.Domains.Entities;

[Table("tbl_ActionControls")]
public class ActionTypeControls
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_Id")]
    [Key]
    public int Id { get; set; }

    [Column("fld_ActionControlsCode")]
    public string? Code { get; set; }

    [Column("fld_ActionControlsName")]
    [StringLength(50)]
    public string? Name { get; set; }
}
