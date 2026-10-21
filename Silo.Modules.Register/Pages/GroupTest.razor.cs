using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Silo.Modules.Register.Models;
using Silo.Shared.Components;

namespace Silo.Modules.Register.Pages;

/// <summary>
/// Reads many tags at once and shows which are registered (legacy F13). The reader runs in continuous mode; EPCs are
/// collected and looked up in one batch every second (SGetProductsByEpcs), so the server is called only when new tags
/// were read.
/// </summary>
public partial class GroupTest
{
    /// <summary>The steps of the "how to use this page" panel.</summary>
    public string[] HowToSteps { get; } =
    [
        RegisterTexts.GroupHowTo1,
        RegisterTexts.GroupHowTo2,
        RegisterTexts.GroupHowTo3,
        RegisterTexts.GroupHowTo4
    ];

    private const int BatchSize = 200;
    private static readonly TimeSpan LookupInterval = TimeSpan.FromSeconds(1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HashSet<string> _seen = new(StringComparer.OrdinalIgnoreCase);
    private readonly Queue<string> _pending = new();
    private CancellationTokenSource? _loopCts;
    private bool _lookingUp;

    public record DeviceMessage(string Type, string? Value);

    public List<GroupTagRow> Rows { get; set; } = [];
    public int NotRegisteredCount => Rows.Count(p => !p.IsRegistered);
    public bool IsReading { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public LocalWebSocketComponent? Socket { get; set; }
    public int DevicePort { get; set; } = 5005;
    public bool IsDeviceConnected { get; set; }

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    #region Reading

    public async Task StartAsync()
    {
        ErrorMessage = string.Empty;
        IsReading = true;
        StartLookupLoop();
        await SendAsync(new { type = "startReader", mode = 2 });
    }

    public async Task StopAsync()
    {
        IsReading = false;
        await SendAsync(new { type = "stopReader" });
    }

    public async Task ResetAsync()
    {
        _seen.Clear();
        _pending.Clear();
        Rows = [];
        ErrorMessage = string.Empty;

        // Restarting the reader also clears the host's list of EPCs it already delivered.
        await SendAsync(new { type = "stopReader" });
        if (IsReading)
        {
            await SendAsync(new { type = "startReader", mode = 2 });
        }
    }

    private async Task SendAsync(object command)
    {
        if (Socket is null || !IsDeviceConnected)
        {
            return;
        }

        try
        {
            await Socket.SendCommandAsync(command);
        }
        catch (InvalidOperationException)
        {
            ErrorMessage = RegisterTexts.GroupDeviceNotConnected;
        }
    }

    #endregion

    #region Lookup

    private void StartLookupLoop()
    {
        if (_loopCts is not null)
        {
            return;
        }

        _loopCts = new CancellationTokenSource();
        _ = LookupLoopAsync(_loopCts.Token);
    }

    private async Task LookupLoopAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(LookupInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                if (_pending.Count > 0)
                {
                    await InvokeAsync(LookupAsync);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task LookupAsync()
    {
        if (_lookingUp || _pending.Count == 0)
        {
            return;
        }

        _lookingUp = true;
        var batch = new List<string>();
        while (_pending.Count > 0 && batch.Count < BatchSize)
        {
            batch.Add(_pending.Dequeue());
        }

        try
        {
            var found = (await Api.PostAsync<List<GroupTagInfo>>("SGetProductsByEpcs"
                , new KeyValuePair<string, object>("epcList", batch)
                , new KeyValuePair<string, object>("userToken", string.Empty))).Value ?? [];

            var byEpc = found
                .GroupBy(p => p.TagEpc, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var epc in batch)
            {
                byEpc.TryGetValue(epc, out var info);
                Rows.Insert(0, new GroupTagRow(Rows.Count + 1, epc, info is not null
                    , info?.ProductSerial ?? string.Empty, info?.ProductCode ?? string.Empty, info?.ProductName ?? string.Empty
                    , info?.RegCode ?? string.Empty, info?.ContractStatus ?? string.Empty));
            }

            ErrorMessage = string.Empty;
        }
        catch
        {
            // Keep the tags: they are looked up again on the next tick.
            batch.ForEach(_pending.Enqueue);
            ErrorMessage = TextResources.APP_StringKeys_Alert_Fail;
        }
        finally
        {
            _lookingUp = false;
            StateHasChanged();
        }
    }

    #endregion

    #region Device host

    public async Task OnDeviceConnectionChanged(bool connected)
    {
        IsDeviceConnected = connected;
        if (connected && IsReading)
        {
            await SendAsync(new { type = "startReader", mode = 2 });
        }

        if (!connected && IsReading)
        {
            ErrorMessage = RegisterTexts.GroupDeviceDisconnected;
            IsReading = false;
        }

        StateHasChanged();
    }

    public Task OnDeviceMessageReceived(string json)
    {
        DeviceMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<DeviceMessage>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return Task.CompletedTask;
        }

        switch (message?.Type)
        {
            case "epc" when IsReading && BarcodeParser.IsValidEpc(message.Value?.Trim()):
                var epc = BarcodeParser.NormalizeEpc(message.Value);
                if (_seen.Add(epc))
                {
                    _pending.Enqueue(epc);
                    StateHasChanged();
                }
                break;
            case "error" when message.Value is not null:
                ErrorMessage = message.Value;
                StateHasChanged();
                break;
        }

        return Task.CompletedTask;
    }

    public void OnDeviceError(string message) => Notification.Show(message, "error");

    #endregion

    public void Dispose()
    {
        _loopCts?.Cancel();
        _loopCts?.Dispose();
    }
}
