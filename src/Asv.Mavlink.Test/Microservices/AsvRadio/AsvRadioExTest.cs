using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Asv.Mavlink.V2.AsvAudio;
using Asv.Mavlink.V2.AsvRadio;
using Asv.Mavlink.V2.Common;
using Xunit;

namespace Asv.Mavlink.Test;

public class AsvRadioExTest
{
    private void Create(AsvRadioCapabilities cap, out IAsvRadioClientEx clientEx, out IAsvRadioServerEx serverEx)
    {
        var link = new VirtualMavlinkConnection();
        var clientId = new MavlinkClientIdentity
        {
            SystemId = 1,
            ComponentId = 2,
            TargetSystemId = 3,
            TargetComponentId = 4
        };
        var clientSeq = new PacketSequenceCalculator();
        var client = new AsvRadioClient(link.Client, clientId,clientSeq);
        var heartBeatClient = new HeartbeatClient(link.Client, clientId, clientSeq, new HeartbeatClientConfig());
        var commandClient = new CommandClient(link.Client, clientId, clientSeq, new CommandProtocolConfig());
        clientEx = new AsvRadioClientEx(client, heartBeatClient, commandClient);
        
        
        var serverId = new MavlinkIdentity(clientId.TargetSystemId, clientId.TargetComponentId);
        var serverSeq = new PacketSequenceCalculator();
        var heartBeatServer = new HeartbeatServer(link.Server, serverSeq, serverId, new MavlinkHeartbeatServerConfig(), Scheduler.Default);
        var commandServer = new CommandServer(link.Server, serverSeq, serverId, Scheduler.Default);
        var commandLongServerEx = new CommandLongServerEx(commandServer);
        var status = new StatusTextServer(link.Server, serverSeq, serverId, new StatusTextLoggerConfig(), Scheduler.Default);
        
        var server = new AsvRadioServer(link.Server, serverId, new AsvRadioServerConfig(), new PacketSequenceCalculator(),Scheduler.Default);
        serverEx = new AsvRadioServerEx(cap, server, heartBeatServer,commandLongServerEx,status);
        
        serverEx.Start();
    }
    
    [Fact]
    public async void Client_call_EnableRadio_command_and_server_accept_it()
    {
        
        var cap = new AsvRadioCapabilitiesBuilder()
            .SetFrequencyHz(0,uint.MaxValue)
            .SetSupportedModulations(AsvRadioModulation.AsvRadioModulationAm,AsvRadioModulation.AsvRadioModulationFm)
            .SetReferencePowerDbm(-100,10)
            .SetTxPowerDbm(-100,10)
            .Build();
        
        
        Create(cap, out var client,out var server);
        
        server.EnableRadio = (freq,modulation,refRxPower,txPower,codec,codecConfig, cancel) =>
        {
            Assert.Equal(uint.MaxValue, freq);
            Assert.Equal(AsvRadioModulation.AsvRadioModulationAm, modulation);
            Assert.Equal(0.1f, refRxPower);
            Assert.Equal(-10.0f, txPower);
            Assert.Equal(AsvAudioCodec.AsvAudioCodecUnknown, codec);
            Assert.Equal(1, codecConfig);
            return Task.FromResult(MavResult.MavResultAccepted);
        };
        
        var result = await client.EnableRadio(uint.MaxValue, AsvRadioModulation.AsvRadioModulationAm, 0.1f, -10.0f, AsvAudioCodec.AsvAudioCodecUnknown, 1);
        Assert.Equal(MavResult.MavResultAccepted,result);
    }
    
    [Fact]
    public async void Client_call_EnableRadio_command_and_server_reject_it()
    {
        var cap = new AsvRadioCapabilitiesBuilder()
            .SetFrequencyHz(0,uint.MaxValue)
            .SetSupportedModulations(AsvRadioModulation.AsvRadioModulationAm,AsvRadioModulation.AsvRadioModulationFm)
            .SetReferencePowerDbm(-100,10)
            .SetTxPowerDbm(-100,10)
            .Build();
        Create(cap, out var client,out var server);
        
        server.EnableRadio = (_,_,_,_,_,_, _) => Task.FromResult(MavResult.MavResultFailed);
        
        var result = await client.EnableRadio(uint.MaxValue, AsvRadioModulation.AsvRadioModulationAm, 0.1f, -10.0f, AsvAudioCodec.AsvAudioCodecUnknown, 1);
        Assert.Equal(MavResult.MavResultFailed,result);
    }
    
    [Fact]
    public async void Client_call_DisableRadio_command_and_server_accept_it()
    {
        
        Create(AsvRadioCapabilities.Empty, out var client,out var server);
        
        server.DisableRadio = (_) => Task.FromResult(MavResult.MavResultAccepted);
        
        var result = await client.DisableRadio();
        Assert.Equal(MavResult.MavResultAccepted,result);
    }
    
    [Fact]
    public async void Client_get_Capabilities_command_and_server_return_it()
    {
        var cap = new AsvRadioCapabilitiesBuilder()
            .SetFrequencyHz(0,uint.MaxValue)
            .SetSupportedModulations(AsvRadioModulation.AsvRadioModulationAm,AsvRadioModulation.AsvRadioModulationFm)
            .SetReferencePowerDbm(-100,10)
            .SetTxPowerDbm(-100,10)
            .Build();
        
        Create(cap, out var client,out var server);
        
        var result = await client.GetCapabilities();
        Assert.Equal(cap.MinFrequencyHz, result.MinFrequencyHz);
        Assert.Equal(cap.MaxFrequencyHz, result.MaxFrequencyHz);
        Assert.Equal(cap.MinReferencePowerDbm, result.MinReferencePowerDbm);
        Assert.Equal(cap.MaxReferencePowerDbm, result.MaxReferencePowerDbm);
        Assert.Equal(cap.SupportedModulations, result.SupportedModulations.ToArray());
        Assert.Equal(cap.SupportedCodecs.Keys, result.SupportedCodecs.Keys.ToArray());
        foreach (var codec in cap.SupportedCodecs)
        {
            Assert.Equal(codec.Value, result.SupportedCodecs[codec.Key].ToArray());
        }
    }
    
    [Fact]
    public async void Client_check_server_status()
    {
        Create(AsvRadioCapabilities.Empty, out var client,out var server);

        server.CustomMode.OnNext(AsvRadioCustomMode.AsvRadioCustomModeIdle);
        
        await client.CustomMode.FirstAsync(x => x == AsvRadioCustomMode.AsvRadioCustomModeIdle);
        
        server.CustomMode.OnNext(AsvRadioCustomMode.AsvRadioCustomModeOnair);
        
        await client.CustomMode.FirstAsync(x => x == AsvRadioCustomMode.AsvRadioCustomModeOnair);
        
    }
    
}