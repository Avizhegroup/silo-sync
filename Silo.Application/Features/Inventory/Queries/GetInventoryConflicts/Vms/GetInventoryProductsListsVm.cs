namespace Silo.Application.Features;
public class GetInventoryProductsListsVm
{
    public GetInventoryProductsListsVm()
    {

    }

    public GetInventoryProductsListsVm(List<GetInventoryResultTagVm> mains,
        List<GetInventoryResultTagVm> exits,
        List<GetInventoryResultTagVm> enters)
    {
        Mains = mains;

        Exits = exits;

        Enters = enters;
    }

    public List<GetInventoryResultTagVm> Mains { get; set; }
    public List<GetInventoryResultTagVm> Enters { get; set; }
    public List<GetInventoryResultTagVm> Exits { get; set; }
}
