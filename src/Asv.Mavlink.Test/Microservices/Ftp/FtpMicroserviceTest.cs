using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Asv.Mavlink.V2.Common;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Mavlink.Test;

public class FtpMicroserviceTest
{
    private readonly ITestOutputHelper _output;

    public FtpMicroserviceTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public async Task Ftp_Directory_Existence_After_Creation()
    {
        var link = new VirtualLink();
        
        var server = new FtpServerEx(new FtpServer(
            link.Server,
            new MavlinkServerIdentity{ComponentId = 13, SystemId = 13},
            new FtpConfig(),
            new PacketSequenceCalculator(),
            TaskPoolScheduler.Default));
        
        var client = new FtpClientEx(new FtpClient(
            link.Client,
            new MavlinkClientIdentity{SystemId = 1, ComponentId = 1, TargetComponentId = 13, TargetSystemId = 13},
            new FtpConfig(),
            new PacketSequenceCalculator(),
            TaskPoolScheduler.Default));
        
        await client.CreateDirectory("pathdir", new CancellationToken());

        Assert.True(Directory.Exists("pathdir"));
        
        Directory.Delete("pathdir");
    }
    
    [Fact]
    public async Task Ftp_Directory_Existence_After_Removal()
    {
        var link = new VirtualLink();
        
        var server = new FtpServerEx(new FtpServer(
            link.Server,
            new MavlinkServerIdentity{ComponentId = 13, SystemId = 13},
            new FtpConfig(),
            new PacketSequenceCalculator(),
            TaskPoolScheduler.Default));
        
        var client = new FtpClientEx(new FtpClient(
            link.Client,
            new MavlinkClientIdentity{SystemId = 1, ComponentId = 1, TargetComponentId = 13, TargetSystemId = 13},
            new FtpConfig(),
            new PacketSequenceCalculator(),
            TaskPoolScheduler.Default));
        
        Directory.CreateDirectory("pathdir");

        await client.RemoveDirectory("pathdir", new CancellationToken());

        Assert.False(Directory.Exists("pathdir"));
    }
    
    [Fact]
    public async Task Ftp_List_Current_Directory()
    {
        var link = new VirtualLink();
        
        var server = new FtpServerEx(new FtpServer(
            link.Server,
            new MavlinkServerIdentity{ComponentId = 13, SystemId = 13},
            new FtpConfig(),
            new PacketSequenceCalculator(),
            TaskPoolScheduler.Default));
        
        var client = new FtpClientEx(new FtpClient(
            link.Client,
            new MavlinkClientIdentity{SystemId = 1, ComponentId = 1, TargetComponentId = 13, TargetSystemId = 13},
            new FtpConfig(),
            new PacketSequenceCalculator(),
            TaskPoolScheduler.Default));
        
        var result = await client.ListDirectory(".", new CancellationToken());
        
        Assert.True(result.Any(_ => _.FileName == "Asv.Common.dll"));
        Assert.True(result.Any(_ => _.FileName == "Asv.IO.dll"));
        Assert.True(result.Any(_ => _.FileName == "Asv.Mavlink.dll"));
    }
}