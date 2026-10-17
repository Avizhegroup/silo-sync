using System.Collections.Concurrent;
using System.Text.Json;
using Silo.Shared.Components;

namespace Silo.Modules.Register.Services;

/// <summary>A reply from the device host: {"type","id","ok","value","code"}.</summary>
public sealed record DeviceReply(string Type, string? Id, bool Ok, string? Value, int Code);

/// <summary>
/// Request/reply on top of <see cref="LocalWebSocketComponent"/>: each command carries an id and the page awaits
/// the host's answer with the same id. One instance per page; feed every received message to <see cref="TryComplete"/>.
/// </summary>
public sealed class DeviceRpc
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ConcurrentDictionary<string, TaskCompletionSource<DeviceReply>> _pending = new();
    private int _sequence;

    /// <param name="buildCommand">Builds the command object for the given request id (it must include "id").</param>
    public async Task<DeviceReply> RequestAsync(LocalWebSocketComponent socket, Func<string, object> buildCommand, TimeSpan timeout)
    {
        var id = Interlocked.Increment(ref _sequence).ToString();
        var completion = new TaskCompletionSource<DeviceReply>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pending[id] = completion;

        try
        {
            await socket.SendCommandAsync(buildCommand(id));
            return await completion.Task.WaitAsync(timeout);
        }
        catch (TimeoutException)
        {
            throw new TimeoutException("The device host did not answer in time.");
        }
        finally
        {
            _pending.TryRemove(id, out _);
        }
    }

    /// <summary>Completes the matching request. Returns false when the message is not a reply to a pending request.</summary>
    public bool TryComplete(string json)
    {
        DeviceReply? reply;
        try
        {
            reply = JsonSerializer.Deserialize<DeviceReply>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return false;
        }

        return reply?.Id is not null
            && _pending.TryGetValue(reply.Id, out var completion)
            && completion.TrySetResult(reply);
    }

    /// <summary>Fails every waiting request, for example when the socket disconnects.</summary>
    public void FailAll(string reason)
    {
        foreach (var completion in _pending.Values)
        {
            completion.TrySetException(new InvalidOperationException(reason));
        }
    }
}
