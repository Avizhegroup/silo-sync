namespace Silo.Components;

public partial class NavBread
{
    [Parameter]
    public List<NavbarAllTitle> Items { get; set; } = new List<NavbarAllTitle>();

    [Parameter] public EventCallback<int> OnItemClick { get; set; }

    public async Task OnBreadItemClick(int id)
    {
        foreach (var mainLike in Items)
        {
            if (mainLike.Id == id)
            {
                await OnItemClick.InvokeAsync(mainLike.Id);

                return;
            }

            foreach (var category in mainLike.Children)
            {
                if (category.Id == id)
                {
                    await OnItemClick.InvokeAsync(category.Id);

                    return;
                }
            }
        }
    }
}
