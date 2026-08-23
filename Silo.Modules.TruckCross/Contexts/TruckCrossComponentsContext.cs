namespace Silo.Modules.TruckCross;
public class TruckCrossComponentsContext
{
    private TruckCrossDataDto _cross;

    public Func<TruckCrossDataDto,Task> TruckCrossDataHasChanged;
    public Func<Task> SaveHasFired;

    protected virtual async Task OnTruckCrossSelectionChanged(TruckCrossDataDto cross)
    {
        if (cross is null)
        {
            return;
        }

        await TruckCrossDataHasChanged?.Invoke(cross);
    }

    protected virtual async Task OnTruckCrossSaveButtonClick()
    {
        await SaveHasFired?.Invoke();
    }

    public async Task SetTabCross(TruckCrossDataDto cross)
    {
        await OnTruckCrossSelectionChanged(cross);

        _cross = cross;
    }

    public async Task SetSaveHasFired()
    {
        await OnTruckCrossSaveButtonClick();
    }
}
