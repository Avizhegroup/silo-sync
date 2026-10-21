namespace Silo.Modules.Register.Components;

/// <summary>
/// A TelerikGrid with the settings every Register page wants: paging only when there is more than one page, an empty
/// message instead of a bare header, a coloured row per state, and a fresh snapshot of the data on every render so rows
/// that a page adds to its list in place (a reader pushing tags, say) always show up.
/// </summary>
public partial class RegisterGrid<TItem>
{
    private List<TItem> _items = [];

    [Parameter] public IEnumerable<TItem> Data { get; set; } = [];
    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public bool Sortable { get; set; } = true;

    /// <summary>Shows a filter row above the columns (a search box per column).</summary>
    [Parameter] public bool Filterable { get; set; }

    /// <summary>Shown instead of the grid while there are no rows.</summary>
    [Parameter] public string? EmptyText { get; set; } = RegisterTexts.NoRows;

    /// <summary>Returns "success", "danger", "warning" or null for a row.</summary>
    [Parameter] public Func<TItem, string?>? RowKind { get; set; }

    /// <summary>The <c>GridColumn</c> elements.</summary>
    [Parameter] public RenderFragment? Columns { get; set; }

    protected override void OnParametersSet() => _items = Data.ToList();

    private void OnRowRender(GridRowRenderEventArgs args)
    {
        var kind = args.Item is TItem item ? RowKind?.Invoke(item) : null;
        if (!string.IsNullOrEmpty(kind))
        {
            args.Class = $"register-row-{kind}";
        }
    }
}
