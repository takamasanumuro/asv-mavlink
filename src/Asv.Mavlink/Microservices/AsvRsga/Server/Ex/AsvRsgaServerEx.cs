using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asv.Mavlink.AsvRsga;
using Asv.Mavlink.Common;
using Asv.Mavlink.V2.AsvRsga;
using Asv.Mavlink.V2.Common;
using Microsoft.Extensions.Logging;
using R3;
using ZLogger;
using MavCmd = Asv.Mavlink.V2.Common.MavCmd;

namespace Asv.Mavlink;

public class AsvRsgaServerEx : IAsvRsgaServerEx, IDisposable,IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly IDisposable _sub1;

    public AsvRsgaServerEx(
        IAsvRsgaServer server, 
        IStatusTextServer status, 
        ICommandServerEx<CommandLongPacket> commands )
    {
        _logger = server.Core.Log.CreateLogger<AsvRsgaServerEx>();
        Base = server;
        _sub1 = server.OnCompatibilityRequest.Subscribe(OnCompatibilityRequest);
        commands[(MavCmd)V2.AsvRsga.MavCmd.MavCmdAsvRsgaSetMode] = async (id,args, cancel) =>
        {
            if (SetMode == null) return CommandResult.FromResult(MavResult.MavResultUnsupported);
            RsgaHelper.GetArgsForSetMode(args.Payload, out var mode, out var p2, out var p3, out var p4, out var p5, out var p6, out var p7);
            var result = await SetMode(mode, cancel).ConfigureAwait(false);
            return CommandResult.FromResult(result);
        };
    }
    
    public IAsvRsgaServer Base { get; }

    private async void OnCompatibilityRequest(AsvRsgaCompatibilityRequestPayload rx)
    {
        var modes = new HashSet<AsvRsgaCustomMode> { AsvRsgaCustomMode.AsvRsgaCustomModeIdle };
        try
        {
            if (GetCompatibility != null)
            {
                foreach (var mode in GetCompatibility())
                {
                    modes.Add(mode);
                }
            }
            await Base.SendCompatibilityResponse(tx =>
            {
                tx.RequestId = rx.RequestId;
                tx.Result = AsvRsgaRequestAck.AsvRsgaRequestAckOk;
                RsgaHelper.SetSupportedModes(tx, modes);
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,$"Error to get compatibility:{e.Message}");
            await Base.SendCompatibilityResponse(tx =>
            {
                tx.RequestId = rx.RequestId;
                tx.Result = AsvRsgaRequestAck.AsvRsgaRequestAckFail;
            }).ConfigureAwait(false);
        }
    }

    public RsgaSetMode? SetMode { get; set; }
    public RsgaGetCompatibility? GetCompatibility { get; set; }

    #region Dispose

    public void Dispose()
    {
        SetMode = null;
        GetCompatibility = null;
        _sub1.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        SetMode = null;
        GetCompatibility = null;
        if (_sub1 is IAsyncDisposable sub1AsyncDisposable)
            await sub1AsyncDisposable.DisposeAsync().ConfigureAwait(false);
        else
            _sub1.Dispose();
    }

    #endregion
}