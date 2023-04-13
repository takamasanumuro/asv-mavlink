using System;
using System.Collections.Concurrent;
using System.Threading;
using Asv.Common;
using Asv.Mavlink.V2.Common;
using NLog;

namespace Asv.Mavlink;

public abstract class CommandServerEx<TArgPacket> : DisposableOnceWithCancel, ICommandServerEx<TArgPacket>
    where TArgPacket : IPacketV2<IPayload>
{
    private readonly ICommandServer _server;
    private readonly Func<TArgPacket,ushort> _cmdGetter;
    private readonly Func<TArgPacket,byte> _confirmationGetter;
    private readonly ConcurrentDictionary<ushort, CommandDelegate<TArgPacket>> _registry = new();
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private int _isBusy;
    private int _lastCommand = -1;

    protected CommandServerEx(ICommandServer server, IObservable<TArgPacket> commandsPipe, Func<TArgPacket,ushort> cmdGetter, Func<TArgPacket,byte> confirmationGetter)
    {
        _server = server;
        _cmdGetter = cmdGetter;
        _confirmationGetter = confirmationGetter;
        commandsPipe.Subscribe(OnRequest).DisposeItWith(Disposable);
        Disposable.AddAction(() => _registry.Clear());
    }

    public CommandDelegate<TArgPacket> this[MavCmd cmd]
    {
        set { _registry.AddOrUpdate((ushort)cmd, value, (mavCmd, del) => value); }
    }


    private async void OnRequest(TArgPacket pkt)
    {
        var requester = new DeviceIdentity() { ComponentId = pkt.ComponenId, SystemId = pkt.SystemId };
        var cmd = _cmdGetter(pkt);
        
        var confirmation = _confirmationGetter(pkt);
        try
        {
            // wait until prev been executed
            if (Interlocked.CompareExchange(ref _isBusy, 1, 0) !=0)
            {
                if (confirmation != 0 && _lastCommand == cmd)
                {
                    // do nothing, we already doing this task
                    return;
                }
                Logger.Warn($"Reject command {pkt}): too busy now");
                await _server.SendCommandAck((MavCmd)cmd, requester,
                    new CommandResult(MavResult.MavResultTemporarilyRejected), DisposeCancel).ConfigureAwait(false);
                return;
            }
            _lastCommand = cmd;
            if (_registry.TryGetValue(cmd, out var callback) == false)
            {
                Logger.Warn($"Reject unknown command {pkt})");
                _lastCommand = -1;
                await _server.SendCommandAck((MavCmd)cmd, requester,
                    new CommandResult(MavResult.MavResultUnsupported), DisposeCancel).ConfigureAwait(false);
                return;
            }

            var result = await callback(requester,pkt, DisposeCancel).ConfigureAwait(false);
            await _server.SendCommandAck((MavCmd)cmd, requester, result, DisposeCancel).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.Error($"Error to execute command {pkt}:{e.Message}");
            _lastCommand = -1;
            await _server.SendCommandAck((MavCmd)cmd, requester,
                new CommandResult(MavResult.MavResultTemporarilyRejected), DisposeCancel).ConfigureAwait(false);

        }
        finally
        {
            Interlocked.Exchange(ref _isBusy, 0);
        }
    }

    
}