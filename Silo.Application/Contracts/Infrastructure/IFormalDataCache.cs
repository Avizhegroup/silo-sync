using Silo.Application.Features;

namespace Silo.Application;
public interface IFormalDataCache
{
    Task<List<GetAllWarehousesVm>> GetWarehouses();
    Task UpdateWarehouses(List<GetAllWarehousesVm> warehouses);
    Task<List<GetAllProductQcsVm>> GetQcs();
    Task UpdateQcs(List<GetAllProductQcsVm> qcs);
    Task<List<GetAllProductSizeTitleAndCodeVm>> GetSizes();
    Task UpdateSizes(List<GetAllProductSizeTitleAndCodeVm> sizes);
    Task<List<GetAllProductBrandVm>> GetBrands();
    Task UpdateBrands(List<GetAllProductBrandVm> brands);
    Task<List<GetAllLinesVm>> GetLines();
    Task UpdateLines(List<GetAllLinesVm> lines);
    Task<List<GetAllProductGroupVm>> GetGroups();
    Task UpdateGroups(List<GetAllProductGroupVm> groups);
    Task<List<GetAllProductSubGroupVm>> GetSubGroups();
    Task UpdateSubGroups(List<GetAllProductSubGroupVm> subgroups);
    Task<List<GetAllProductClassVm>> GetProductClass();
    Task UpdateProductClass(List<GetAllProductClassVm> productClass);
    Task<List<GetAllProductTypeVm>> GetTypes();
    Task UpdateType(List<GetAllProductTypeVm> types);
    Task<List<GetAllShiftsVm>> GetShifts();
    Task<List<GetAllTextResourcesVm>> GetTextResources();
    Task UpdateTextResources(List<GetAllTextResourcesVm> textResources);
    Task HardRefreshCache();
}
