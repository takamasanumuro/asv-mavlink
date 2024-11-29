using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Asv.Mavlink.Minimal;


namespace Asv.Mavlink;

/// <summary>
/// Used to create a client device by providing a heartbeat packet.
/// </summary>
public interface IClientDeviceProvider
{
    int Order { get; }
    bool CanCreateDevice(HeartbeatPacket packet);
    IClientDevice CreateDevice(HeartbeatPacket packet, MavlinkClientIdentity identity, ICoreServices core);
}

/// <summary>
/// Used to create a client device.
/// </summary>
public interface IClientDeviceFactory
{
    IClientDevice? Create(HeartbeatPacket packet);
}

public class ClientDeviceFactory : IClientDeviceFactory
{
    public const int DefaultOrder = 100;
    public const int MinimumOrder = int.MinValue;
    
    private readonly MavlinkIdentity _selfIdentity;
    private readonly ICoreServices _core;
    private readonly ImmutableArray<IClientDeviceProvider> _providers;

    public ClientDeviceFactory(MavlinkIdentity selfIdentity, IEnumerable<IClientDeviceProvider> providers, ICoreServices core)
    {
        ArgumentNullException.ThrowIfNull(core);
        _selfIdentity = selfIdentity;
        _core = core;
        _providers = [..providers.OrderByDescending(x => x.Order)];
    }
    public IClientDevice? Create(HeartbeatPacket packet)
    {
        foreach (var provider in _providers)
        {
            if (provider.CanCreateDevice(packet))
            {
                return provider.CreateDevice(packet,new MavlinkClientIdentity(_selfIdentity.SystemId,_selfIdentity.ComponentId,packet.SystemId,packet.ComponentId), _core);
            }
        }
        return null;
    }
}