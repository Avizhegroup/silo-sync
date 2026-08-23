namespace Silo.Application.Features;
public class GetWarehouseProductsListsVm
{
    public GetWarehouseProductsListsVm()
    {

    }

    public GetWarehouseProductsListsVm(List<GetWarehouseProductsVm> mains,
        List<GetWarehouseProductsVm> exits,
        List<GetWarehouseProductsVm> enters)
    {
        Mains = mains;

        Exits = exits;

        Enters = enters;
    }

    public List<GetWarehouseProductsVm> Mains { get; set; }
    public List<GetWarehouseProductsVm> Enters { get; set; }
    public List<GetWarehouseProductsVm> Exits { get; set; }
}
