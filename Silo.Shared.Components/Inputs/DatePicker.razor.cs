using System.Globalization;

namespace Silo.Shared.Components;
public partial class DatePicker
{
    private int year;
    private int month;
    private int day;
    private DateTime SelectedDate = DateTime.Today;
    public PersianCalendar pc = new();
    public Guid Id = Guid.NewGuid();
    public bool IsDatePickerShown = false;
    public List<string> PersianWeekDays = new();
    public string[] PersianMonthNames = new string[]
    {
        "فروردین", "اردیبهشت", "خرداد",
        "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر",
        "دی", "بهمن", "اسفند"
    };
    public List<int> Years = new();
    public DatePickerMode Mode = DatePickerMode.Daily;
    public List<List<DateTime>> DisplayedWeeks
    {
        get
        {
            var weeks = new List<List<DateTime>>();
            var startDate = pc.ToDateTime(year, month, 1, 0, 0, 0, 0);

            while (startDate.DayOfWeek != DayOfWeek.Saturday)
            {
                startDate = startDate.AddDays(-1);
            }

            for (var i = 0; i < 6; i++)
            {
                var week = new List<DateTime>();

                for (var j = 0; j < 7; j++)
                {
                    var date = startDate.AddDays(i * 7 + j);
                    week.Add(date);
                }

                weeks.Add(week);
            }

            return weeks;
        }
    }
    public string? CurrentValue
    {
        get => Value;
        set
        {
            Value = value;

            SelectedDate = PersianCalendarTools.PersianToGregorian(Value);

            _ = ValueChanged.InvokeAsync(CurrentValue);
        }
    }

    [Inject] public IJSRuntime JSRuntime { get; set; }

    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string? InputClass { get; set; }
    [Parameter] public string? Width { get; set; }
    [Parameter] public bool IsEnabled { get; set; } = true;
    [Parameter] public EventCallback<string?> OnChange { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public string? Value { get; set; }

    protected override void OnInitialized()
    {
        PersianWeekDays = GetPersianWeekDays();

        year = pc.GetYear(DateTime.Now);

        month = pc.GetMonth(DateTime.Now);

        day = pc.GetDayOfMonth(DateTime.Now);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initDatepicker"
                                           , DotNetObjectReference.Create(this)
                                           , $"bdate-{Id}");
        }
    }

    public List<string> GetPersianWeekDays()
    {
        var persianCulture = new CultureInfo("fa-IR");
        var persianWeekDays = new List<string>();

        for (int i = 0; i < 7; i++)
        {
            var dayOfWeek = (DayOfWeek)((i + 6) % 7); 

            persianWeekDays.Add(persianCulture.DateTimeFormat.GetDayName(dayOfWeek));
        }

        return persianWeekDays;
    }

    public string DayClass(DateTime date)
    {
        if (pc.GetMonth(date) != month)
        {
            return "other-month";
        }
        else if (date == DateTime.Now.Date)
        {
            return "today-day";
        }
        else if (date == SelectedDate.Date)
        {
            return "selected-day";
        }
        else
        {
            return "";
        }
    }

    public async Task OnInputClick()
    {
        if (!IsEnabled)
        {
            return;
        }

        Mode = DatePickerMode.Daily;

        IsDatePickerShown = true;
    }

    public async Task SelectDate(DateTime selectedDate)
    {
        SelectedDate = selectedDate;

        CurrentValue = PersianCalendarTools.GregorianToPersian(SelectedDate);

        if (ValueChanged.HasDelegate is false) return;

        await OnChange.InvokeAsync(Value);

        year = pc.GetYear(selectedDate);
        month = pc.GetMonth(selectedDate);
        day = pc.GetYear(selectedDate);

        IsDatePickerShown = false;
    }

    public async Task OnPreviousMonthClick(MouseEventArgs e)
    {
        if (Mode == DatePickerMode.Daily)
        {
            if (month - 1 == 0)
            {
                year--;
                month = 12;
                day = pc.IsLeapYear(year) ? 30 : 29;
            }
            else
            {
                month--;
                day = 1;
            }
        }
        else if (Mode == DatePickerMode.Month)
        {
            year--;
        }
        else if (Mode == DatePickerMode.Yearly)
        {
            year -= 4;

            await OnYearTitleClick(e);
        }
    }

    public async Task OnNextMonthClick(MouseEventArgs e)
    {
        if (Mode == DatePickerMode.Daily)
        {
            if (month + 1 > 12)
            {
                year++;
                month = 1;
                day = 1;
            }
            else
            {
                month++;
                day = 1;
            }
        }
        else if (Mode == DatePickerMode.Month)
        {
            year++;
        }
        else if (Mode == DatePickerMode.Yearly)
        {
            year += 4;

            await OnYearTitleClick(e);
        }
    }

    public async Task OnMonthClick(string monthName)
    {
        for (int i = 0; i < PersianMonthNames.Length; i++)
        {
            if (PersianMonthNames[i].Equals(monthName))
            {
                month = i + 1;

                break;
            }
        }

        Mode = DatePickerMode.Daily;
    }

    public async Task OnMonthTitleClick(MouseEventArgs e)
    {
        Mode = DatePickerMode.Month;
    }

    public async Task OnYearClick(int year)
    {
        this.year = year;

        Mode = DatePickerMode.Month;
    }

    public async Task OnYearTitleClick(MouseEventArgs e)
    {
        Years.Clear();

        for (int i = year - 5; i < year + 4; i++)
        {
            Years.Add(i);
        }

        Mode = DatePickerMode.Yearly;
    }

    [JSInvokable]
    public void InformDatePickerClick()
    {
        IsDatePickerShown = false;

        StateHasChanged();
    }
}
