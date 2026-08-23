namespace Silo.Application.Features;
public class SalesShopExcelDto
{
    public int Id { get; set; }

    public string Code { get; set; }

    public string Title { get; set; }

    public string ManagerName { get; set; }

    public string CityId { get; set; }

    public string ProvinceId { get; set; }

    public string? Phone { get; set; }

    public string? Mobile { get; set; }

    public string? Address { get; set; }

    public string? Password { get; set; }

    public string ErrorMessage { get; set; }
}
