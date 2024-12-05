using Asv.Common;
using Asv.IO;
using TimeProviderExtensions;
using Xunit.Abstractions;

namespace Asv.Mavlink.Test;

public abstract class ServerTestBase<TServer>
{
    private TServer? _server;

    protected ServerTestBase(ITestOutputHelper log)
    {
        Log = log;
        
        ServerTime = new ManualTimeProvider();
        Seq = new PacketSequenceCalculator();
        Identity = new MavlinkIdentity(3, 4);
        var loggerFactory = new TestLoggerFactory(log, ServerTime, "SERVER");
        
        var protocol = Protocol.Create(builder =>
        {
            builder.SetLog(loggerFactory);
            builder.SetTimeProvider(ServerTime);
            builder.RegisterMavlinkV2Protocol();
            builder.RegisterBroadcastFeature<MavlinkMessage>();
            builder.RegisterSimpleFormatter();
        });
        Link = protocol.CreateVirtualConnection();
        Core = new CoreServices(Link.Server, Seq, loggerFactory, ServerTime, new DefaultMeterFactory());
    }

    

    protected abstract TServer CreateClient(MavlinkIdentity identity, CoreServices core);
    protected TServer Server => _server ??= CreateClient(Identity, Core);
    protected MavlinkIdentity Identity { get; }
    protected ITestOutputHelper Log { get; }
    protected CoreServices Core { get; }
    protected PacketSequenceCalculator Seq { get; }
    protected ManualTimeProvider ServerTime { get; }
    protected IVirtualConnection Link { get; }
}