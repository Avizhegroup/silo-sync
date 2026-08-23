using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Silo.Application;
public static class ApplicationApiServices
{
    public static void AddApplicationApiServices(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            options.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzk5MDIwODAwIiwiaWF0IjoiMTc2NzUxNjAwOSIsImFjY291bnRfaWQiOiIwMTliODgyOTUyYzE3Y2ZiYjU0Y2Q5N2EyMGEzNDk3YyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2U0Mmt4ZGh6MmQxNHdtaDNkcDh2ZHpjIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.OWmLlgIz0MAEVBUnniwR6sH5NRuN_FQzxCr3A8I0IuPVKLakD1piCAw9FY1FmgJjdnFx_Whh5sFk_Jc24E13pLoKQa8C7qxVq8-Vhg-yiCL-rw3PWa5ngh4edxpVBYmu2ZmvMieSa3UMERsf7VFaaoCc21Rj18QsDZ6nRIEDnXCJbbNodsLIA1wlRRQgmnHobEU30qTsuhMlhjkThy8dqcMYz7l-CdJ-NtlX_Yo3yEzwggXUkyIZDc-l_y0g4Wx-VfLrWyooTb4MXpuTXccGdr11bpoOFiCLFR6hi2FCZsOX5qPD-4_AdjS2q4eM5VLvqFv2qUZJRlZCcu_txmk2hA";
        });
    }
}
