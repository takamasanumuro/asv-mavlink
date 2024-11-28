using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asv.IO;
using Asv.Mavlink.Common;
using DeepEqual.Syntax;
using JetBrains.Annotations;
using R3;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Mavlink.Test;

[TestSubject(typeof(ParamsServer))]
public class ParamsServerTest : ServerTestBase<ParamsServer>, IDisposable
{
    private readonly TaskCompletionSource<IProtocolMessage> _taskCompletionSource;
    private readonly CancellationTokenSource _cancellationTokenSource;
    protected override ParamsServer CreateClient(MavlinkIdentity identity, CoreServices core) => new(identity, core);

    public ParamsServerTest(ITestOutputHelper log) : base(log)
    {
        _taskCompletionSource = new TaskCompletionSource<IProtocolMessage>();
        _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        _cancellationTokenSource.Token.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    [Fact]
    public async Task Send_SinglePacket_Success()
    {
        // Arrange
        var payload = new ParamValuePayload
        {
            ParamValue = 123f,
        };
        using var sub = Link.Client.OnRxMessage.Subscribe(
            p => _taskCompletionSource.TrySetResult(p)
        );
        
        // Act
        await Server.SendParamValue(p => p.ParamValue = 123f, _cancellationTokenSource.Token);
        
        // Assert
        var result = await _taskCompletionSource.Task as ParamValuePacket;
        Assert.NotNull(result);
        Assert.Equal(Link.Server.Statistic.TxMessages, Link.Client.Statistic.RxMessages);
        Assert.Equal(payload.ParamValue, result.Payload.ParamValue);
    }
    
    [Theory]
    [InlineData(10)]
    [InlineData(200)]
    [InlineData(20000)]
    public async Task SendCompatibilityResponse_SendManyPacket_Success(int packetCount)
    {
        // Arrange
        var called = 0;
        var results = new List<ParamValuePayload>();
        var serverResults = new List<ParamValuePayload>();
        using var sub = Link.Client.OnRxMessage.Subscribe(p =>
        {
            called++;
            if (p is ParamValuePacket packet)
            {
                results.Add(packet.Payload);
            }
            if (called >= packetCount)
            {
                _taskCompletionSource.TrySetResult(p);
            }
        });

        // Act
        for (var i = 0; i < packetCount; i++)
        {
            await Server.SendParamValue(p => serverResults.Add(p), _cancellationTokenSource.Token);
        }

        // Assert
        await _taskCompletionSource.Task;
        Assert.Equal(packetCount, (int)Link.Server.Statistic.TxMessages);
        Assert.Equal(packetCount, (int)Link.Client.Statistic.RxMessages);
        Assert.Equal(packetCount, results.Count);
        Assert.Equal(serverResults.Count, results.Count);
        for (var i = 0; i < results.Count; i++)
        {
            Assert.True(results[i].IsDeepEqual(serverResults[i]));
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}