﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Mavlink.Common;
using R3;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Mavlink.Test;

public class AdsbVehicleComplexTest : ComplexTestBase<AdsbVehicleClient, AdsbVehicleServer>, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly AdsbVehicleClientConfig _clientConfig = new()
    {
        TargetTimeoutMs = 10000,
        CheckOldDevicesMs = 1000,
    };

    public AdsbVehicleComplexTest(ITestOutputHelper output) : base(output)
    {
        TaskCompletionSource<AdsbVehiclePayload> taskCompletionSource = new();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override AdsbVehicleServer CreateServer(MavlinkIdentity identity, IMavlinkContext core) =>
        new(identity, core);

    protected override AdsbVehicleClient CreateClient(MavlinkClientIdentity identity, IMavlinkContext core) =>
        new(identity, _clientConfig,core);

    [Fact]
    public async Task UpdateTarget_AddSingleTarget_Success()
    {
        // Arrange
        int count = 0;
        var icao = (uint)987654321;
        Client.OnTarget.Subscribe(_ => { count++; });

        // Act
        await Server.Send(p => p.IcaoAddress = 987654321, _cancellationTokenSource.Token);

        // Assert
        Assert.NotNull(Client.Targets.Values.FirstOrDefault());
        Assert.True( Client.Targets.ContainsKey(icao));
        Assert.Equal(count, (int)Link.Client.Statistic.RxMessages);
        Assert.Equal(count, Client.Targets.Count);
    }

    [Fact]
    public async Task DeleteTargets_DeleteWhenTimeout_Success()
    {
        // Arrange
        var count = 0;
        var icao = (uint)987654321;
        using var sub = Client.OnTarget.Subscribe(_ => { count++; });

        // Act
        await Server.Send(p => p.IcaoAddress = 987654321, _cancellationTokenSource.Token);
        ClientTime.Advance(TimeSpan.FromMilliseconds(10001));

        // Assert
        Assert.NotEqual(count, Client.Targets.Count);
        Assert.False(Client.Targets.ContainsKey(icao));
    }

    [Fact]
    public async Task UpdateTargetAndDeleteTarget_DeleteOldTargetWhenTimeout_NewTargetAdded()
    {
        // Arrange
        var packetsCount = 10;
        var icao = (uint)123;
        using var sub = Client.OnTarget.Subscribe();

        // Act
        for (var i = 0; i < packetsCount; i++)
        {
            await Server.Send(p => p.IcaoAddress = icao, _cancellationTokenSource.Token);
            await Server.Send(p => p.IcaoAddress = 321, _cancellationTokenSource.Token);
        }

        ClientTime.Advance(TimeSpan.FromMilliseconds(5000));
        Assert.Equal(2, Client.Targets.Count);

        for (var i = 0; i < packetsCount; i++)
        {
            await Server.Send(p => p.IcaoAddress = icao, _cancellationTokenSource.Token);
        }
        ClientTime.Advance(TimeSpan.FromMilliseconds(5001));

        // Assert
        Assert.Equal(1, Client.Targets.Count);
        Assert.True(Client.Targets.ContainsKey(icao));
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}