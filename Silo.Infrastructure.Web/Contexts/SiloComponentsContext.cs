namespace Silo.Infrastructure.Web;
public class SiloComponentsContext
{
    private bool _isExpanded;
    private bool _isDarkMode;
    private Action _navbarFilterClicked;
    public Action<bool> NavbarTabChanged;
    public Action<bool> DarkModeChanged;
    public Func<Task> QuickAccessChanged;
    public event Action NavbarFilterStatusChanged;

    public Action NavbarFilterClicked
    {
        get => _navbarFilterClicked;
        set
        {
            _navbarFilterClicked = value;

            NavbarFilterStatusChanged?.Invoke();
        }
    }

    protected virtual void OnTabSelectionChanged()
    {
        NavbarTabChanged?.Invoke(_isExpanded);
    }

    protected virtual void OnDarkModeChanged()
    {
        DarkModeChanged?.Invoke(_isDarkMode);
    }

    public void SetTabStatus(bool isExpanded)
    {
        OnTabSelectionChanged();

        _isExpanded = isExpanded;
    }

    public void SetDarkModeStatus(bool isDarkMode)
    {
        _isDarkMode = isDarkMode;

        OnDarkModeChanged();
    }
}
