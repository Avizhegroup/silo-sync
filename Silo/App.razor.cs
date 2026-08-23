using System.Reflection;
using Silo.Modules.Inspect.Pages;
using Silo.Modules.Product.Pages;
using Silo.Modules.TruckCross.Pages;
using Silo.Modules.Document.Pages;
using Silo.Modules.Guarantee.Pages;
using Silo.Modules.Ai.Pages.Agent;

namespace Silo;
public partial class App
{
    public List<Assembly> Assemblies = new();

    protected override async Task OnInitializedAsync()
    {
        Assemblies.Add(typeof(InspectStaticReport).Assembly);
        Assemblies.Add(typeof(TruckCross).Assembly);
        Assemblies.Add(typeof(AddBrand).Assembly);
        Assemblies.Add(typeof(DocumentAggregate).Assembly);
        Assemblies.Add(typeof(ExpireAndGuarantee).Assembly);
        Assemblies.Add(typeof(ChatBot).Assembly);
    }
}
