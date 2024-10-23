using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.Mavlink.V2.Minimal;
using DynamicData;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using R3;
using ZLogger;

namespace Asv.Mavlink;

public interface IClientDeviceBrowser
{
    IObservable<IChangeSet<IClientDevice, ushort>> Devices { get; }
    BindableReactiveProperty<TimeSpan> DeviceTimeout { get; }
}

public class DeviceBrowserConfig
{
    public int DeviceTimeoutMs { get; set; } = 10_000;
    public int DeviceCheckIntervalMs { get; set; } = 1000;
}

public sealed class ClientDeviceBrowser : IClientDeviceBrowser, IDisposable,IAsyncDisposable
{
    private readonly ICoreServices _core;
    private readonly SourceCache<IClientDevice,ushort> _deviceCache;
    private readonly ConcurrentDictionary<ushort,long> _lastUpdateTime = new ();
    
    private readonly IDisposable _subscribe1;
    private readonly ILogger _logger;
    private readonly BindableReactiveProperty<TimeSpan> _deviceTimeout;
    private readonly ITimer _timer;

    public ClientDeviceBrowser(DeviceBrowserConfig config,ICoreServices core)
    {
        _core = core;
        _logger = core.Log.CreateLogger<ClientDeviceBrowser>();
        _deviceCache = new SourceCache<IClientDevice, ushort>(x => x.FullId);
        _deviceTimeout = new BindableReactiveProperty<TimeSpan>(TimeSpan.FromMilliseconds(config.DeviceTimeoutMs));
        _subscribe1 = core.Connection
            .Filter<HeartbeatPacket>()
            .Subscribe(UpdateDevice);
        Devices = _deviceCache
            .Connect()
            .DisposeMany()
            // filter only devices that was init
            .FilterOnObservable(x=>x.OnInit.AsSystemObservable().Select(s=>s == InitState.Complete))
            .ObserveOn(core.Scheduler)
            .RefCount();
        _timer = core.TimeProvider.CreateTimer(RemoveOldDevices, null, TimeSpan.FromMilliseconds(config.DeviceCheckIntervalMs), TimeSpan.FromMilliseconds(config.DeviceCheckIntervalMs));
    }

    private void RemoveOldDevices(object? state)
    {
        var itemsToDelete = _lastUpdateTime
            .Where(x => _core.TimeProvider.GetElapsedTime(x.Value) > _deviceTimeout.Value).ToImmutableArray();
        if (itemsToDelete.Length == 0) return;
        _deviceCache.Edit(update =>
        {
            foreach (var item in itemsToDelete)
            {
                update.Remove(item.Key);
                _logger.ZLogInformation($"Remove device {item.Key}");
            }
        });
    }

    public BindableReactiveProperty<TimeSpan> DeviceTimeout => _deviceTimeout;
    
    private void UpdateDevice(HeartbeatPacket packet)
    {
        _lastUpdateTime.AddOrUpdate(packet.FullId, _core.TimeProvider.GetTimestamp(),
            (_, _) => _core.TimeProvider.GetTimestamp());

        _deviceCache.Edit(update =>
        {
            var item = update.Lookup(packet.FullId);
            if (item.HasValue) return;
            update.AddOrUpdate(ClientDeviceFactory.Create(packet, _core));
            _logger.ZLogInformation($"Found new device {packet.FullId}");
        });
    }
    
    public IObservable<IChangeSet<IClientDevice,ushort>> Devices { get; }

    public void Dispose()
    {
        _deviceCache.Dispose();
        _subscribe1.Dispose();
        _deviceTimeout.Dispose();
        _timer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await CastAndDispose(_deviceCache).ConfigureAwait(false);
        await CastAndDispose(_subscribe1).ConfigureAwait(false);
        await CastAndDispose(_deviceTimeout).ConfigureAwait(false);
        await _timer.DisposeAsync().ConfigureAwait(false);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync().ConfigureAwait(false);
            else
                resource.Dispose();
        }
    }
}