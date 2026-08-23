namespace Silo.Application.Dto;

public class ZoneDto
{
    public int Id { get; set; }
    public string ZoneCode { get; set; }
    public string Title { get; set; }
    public string Dimention { get; set; }
    public string ParentCode { get; set; }
    public string ParentLayer { get; set; }
    public string StoreCode { get; set; }
    public string ZoneCountPixle { get; set; }
    public string MinCapacity { get; set; }
    public string MaxCapacity { get; set; }
    public string RowIndex { get; set; }
    public string UserCode { get; set; }
}