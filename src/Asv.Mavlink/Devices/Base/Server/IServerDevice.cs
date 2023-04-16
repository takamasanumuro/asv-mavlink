using System.Reactive.Concurrency;

namespace Asv.Mavlink;

public interface IServerDevice
{
    void Start();
    IMavlinkV2Connection Connection { get; }
    IPacketSequenceCalculator Seq { get; }
    IScheduler Scheduler { get; }
    MavlinkServerIdentity Identity { get; }
    IStatusTextServer StatusText { get; }
    IHeartbeatServer Heartbeat { get; }
}