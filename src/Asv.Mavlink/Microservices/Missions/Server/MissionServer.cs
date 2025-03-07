using System.Threading;
using System.Threading.Tasks;
using Asv.Mavlink.Common;

using R3;

namespace Asv.Mavlink;

public sealed class MissionServer : MavlinkMicroserviceServer, IMissionServer
{
    private ushort _currentMissionIndex;

    public MissionServer(MavlinkIdentity identity, IMavlinkContext core) : base(MissionHelper.MicroserviceName, identity, core)
    {
        OnMissionRequestList = InternalFilter<MissionRequestListPacket>(p => p.Payload.TargetSystem, p => p.Payload.TargetComponent);
        OnMissionRequestInt = InternalFilter<MissionRequestIntPacket>(p => p.Payload.TargetSystem, p => p.Payload.TargetComponent);
        OnMissionClearAll = InternalFilter<MissionClearAllPacket>(p => p.Payload.TargetSystem, p => p.Payload.TargetComponent);
        OnMissionSetCurrent = InternalFilter<MissionSetCurrentPacket>(p => p.Payload.TargetSystem, p => p.Payload.TargetComponent);
        OnMissionCount = InternalFilter<MissionCountPacket>(p => p.Payload.TargetSystem, p => p.Payload.TargetComponent);
        OnMissionAck = InternalFilter<MissionAckPacket>(p => p.Payload.TargetSystem, p => p.Payload.TargetComponent)
            .Select(p => p.Payload);
    }

    public Observable<MissionAckPayload> OnMissionAck { get; }
    public Observable<MissionCountPacket> OnMissionCount { get; }
    public Observable<MissionRequestListPacket> OnMissionRequestList { get; }
    public Observable<MissionRequestIntPacket> OnMissionRequestInt { get; }
    public Observable<MissionClearAllPacket> OnMissionClearAll { get; }
    public Observable<MissionSetCurrentPacket> OnMissionSetCurrent { get; }
    public ValueTask SendMissionAck(MavMissionResult result, byte targetSystemId = 0, byte targetComponentId = 0,
        MavMissionType? type = null)
    {
        return InternalSend<MissionAckPacket>(x =>
        {
            x.Payload.TargetSystem = targetSystemId;
            x.Payload.TargetComponent = targetComponentId;
            x.Payload.Type = result;
            if (type.HasValue)
            {
                x.Payload.MissionType = type.Value;
            }
        }, DisposeCancel);
    }

    public ValueTask SendMissionCount(ushort count, byte targetSystemId = 0, byte targetComponentId = 0)
    {
        return InternalSend<MissionCountPacket>(x =>
        {
            x.Payload.TargetSystem = targetSystemId;
            x.Payload.TargetComponent = targetComponentId;
            x.Payload.Count = count;
        }, DisposeCancel);
    }

    public ValueTask SendReached(ushort seq, CancellationToken cancel)
    {
        return InternalSend<MissionItemReachedPacket>(x => x.Payload.Seq = seq,DisposeCancel);
    }

    public ValueTask SendMissionCurrent(ushort current, CancellationToken cancel = default)
    {
        _currentMissionIndex = current;
        return InternalSend<MissionCurrentPacket>(x =>
        {
            x.Payload.Seq = current;
        }, cancel);
    }

    public ValueTask SendMissionItemInt(ServerMissionItem item,byte targetSystemId = 0, byte targetComponentId = 0,CancellationToken cancel = default)
    {
        return InternalSend<MissionItemIntPacket>(x =>
        {
            x.Payload.Frame = item.Frame;
            x.Payload.TargetSystem = targetSystemId;
            x.Payload.TargetComponent = targetComponentId;
            x.Payload.MissionType = item.MissionType;
            x.Payload.Seq = item.Seq;
            x.Payload.Current = (byte)(_currentMissionIndex == item.Seq ? 1:0);
            x.Payload.Autocontinue = item.Autocontinue;
            x.Payload.Param1 = item.Param1;
            x.Payload.Param2 = item.Param2;
            x.Payload.Param3 = item.Param3;
            x.Payload.Param4 = item.Param4;
            x.Payload.X = item.X;
            x.Payload.Y = item.Y;
            x.Payload.Z = item.Z;
            x.Payload.Command = item.Command;
            x.Payload.Frame = item.Frame;
        }, cancel);
    }

    public Task<ServerMissionItem> RequestMissionItem(
        ushort index, 
        MavMissionType type,
        byte targetSystemId = 0, 
        byte targetComponentId = 0, 
        CancellationToken cancel = default
    )
    {
        return InternalCall<ServerMissionItem, MissionRequestPacket, MissionItemIntPacket>(
            x =>
            {
                x.Payload.MissionType = type;
                x.Payload.TargetComponent = targetComponentId;
                x.Payload.TargetSystem = targetSystemId;
                x.Payload.Seq = index;
            }, 
            p=>p.Payload.TargetSystem, 
            p=>p.Payload.TargetComponent, 
            p=> p.Payload.Seq == index, 
            AsvSdrHelper.Convert,
            cancel: cancel
        );
    }
}