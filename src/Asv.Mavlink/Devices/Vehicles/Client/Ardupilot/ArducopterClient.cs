#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.Mavlink.V2.Ardupilotmega;
using Asv.Mavlink.V2.Common;

namespace Asv.Mavlink;

public class ArduCopterClient:ArduVehicle
{
    public ArduCopterClient(IMavlinkV2Connection connection, MavlinkClientIdentity identity, VehicleClientConfig config, IPacketSequenceCalculator seq, IScheduler? scheduler = null) 
        : base(connection, identity, config, seq, scheduler)
    {
        

    }

    protected override Task<string> GetCustomName(CancellationToken cancel)
    {
        return Task.FromResult("ArduCopter");
    }

    public override DeviceClass Class => DeviceClass.Copter;
    protected override Task<IReadOnlyCollection<ParamDescription>> GetParamDescription()
    {
        // TODO: Read from device by FTP or load from XML file
        return Task.FromResult((IReadOnlyCollection<ParamDescription>)new List<ParamDescription>());
    }

    public override Task SetAutoMode(CancellationToken cancel = default)
    {
        return SetVehicleMode(ArdupilotCopterMode.Auto, cancel);
    }

    public override IEnumerable<IVehicleMode> AvailableModes => ArdupilotCopterMode.AllModes;
    protected override IVehicleMode? InternalInterpretMode(HeartbeatPayload heartbeatPayload)
    {
        return AvailableModes.Cast<ArdupilotCopterMode>()
            .FirstOrDefault(_ => _.CustomMode == (CopterMode)heartbeatPayload.CustomMode);
    }

    public override Task SetVehicleMode(IVehicleMode mode, CancellationToken cancel = default)
    {
        if (mode is not ArdupilotCopterMode) throw new Exception($"Invalid mode. Only {nameof(ArdupilotCopterMode)} supported");
        return Commands.DoSetMode(1, (uint)((ArdupilotCopterMode)mode).CustomMode, 0,cancel);
    }


    public override async Task GoTo(GeoPoint point, CancellationToken cancel = default)
    {
        await EnsureInGuidedMode(cancel).ConfigureAwait(false);
        await Position.SetTarget(point, cancel).ConfigureAwait(false);
    }

    public override async Task DoLand(CancellationToken cancel = default)
    {
        await EnsureInGuidedMode(cancel).ConfigureAwait(false);
        await SetVehicleMode(ArdupilotCopterMode.Land, cancel).ConfigureAwait(false);
    }

    public override async Task DoRtl(CancellationToken cancel = default)
    {
        await EnsureInGuidedMode(cancel).ConfigureAwait(false);
        await SetVehicleMode(ArdupilotCopterMode.Rtl, cancel).ConfigureAwait(false);
    }
    public override Task<bool> CheckGuidedMode(CancellationToken cancel)
    {
        return Task.FromResult(
            Heartbeat.RawHeartbeat.Value.BaseMode.HasFlag(MavModeFlag.MavModeFlagCustomModeEnabled) &&
            Heartbeat.RawHeartbeat.Value.CustomMode == (int)CopterMode.CopterModeGuided);
    }
    public override async Task EnsureInGuidedMode(CancellationToken cancel)
    {
        if (!await CheckGuidedMode(cancel).ConfigureAwait(false))
        {
            await SetVehicleMode(ArdupilotCopterMode.Guided, cancel).ConfigureAwait(false);
        }
    }
}

