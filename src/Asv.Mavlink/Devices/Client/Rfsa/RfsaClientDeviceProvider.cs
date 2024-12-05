using Asv.Mavlink.Minimal;


namespace Asv.Mavlink;

public class RfsaClientDeviceProvider(RfsaClientDeviceConfig config) : IClientDeviceProvider
{
    public int Order => ClientDeviceFactory.DefaultOrder;
    public bool CanCreateDevice(HeartbeatPacket packet) => packet.Payload.Type == (MavType)Mavlink.AsvRfsa.MavType.MavTypeAsvRfsa;
    public IClientDevice CreateDevice(HeartbeatPacket packet, MavlinkClientIdentity identity, ICoreServices core)
    {
        return new RfsaClientDevice(identity,config,core);
    }
}