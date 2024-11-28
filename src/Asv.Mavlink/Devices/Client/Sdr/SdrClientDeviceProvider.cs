using Asv.Mavlink.Minimal;


namespace Asv.Mavlink;

public class SdrClientDeviceProvider(SdrClientDeviceConfig config) : IClientDeviceProvider
{
    public int Order => ClientDeviceFactory.DefaultOrder;
    public bool CanCreateDevice(HeartbeatPacket packet) => packet.Payload.Type == (MavType)Mavlink.AsvSdr.MavType.MavTypeAsvSdrPayload;
    public IClientDevice CreateDevice(HeartbeatPacket packet, MavlinkClientIdentity identity, ICoreServices core)
    {
        return new SdrClientDevice(identity,config,core);
    }
}