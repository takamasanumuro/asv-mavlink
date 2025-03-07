﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Cfg;
using Asv.Mavlink.Common;

using Microsoft.Extensions.Logging;
using R3;
using ZLogger;

namespace Asv.Mavlink;

public struct ParamChangedEvent
{
    public ParamChangedEvent((ushort, IMavParamTypeMetadata) param, MavParamValue oldValue, MavParamValue newValue, bool isRemoteChange)
    {
        ParamIndex = param.Item1;
        Metadata = param.Item2;
        OldValue = oldValue;
        NewValue = newValue;
        IsRemoteChange = isRemoteChange;
    }
    public bool IsRemoteChange { get; }
    public IMavParamTypeMetadata Metadata { get; }
    public ushort ParamIndex { get; }
    public MavParamValue OldValue { get; }
    public MavParamValue NewValue { get; }
}

public class ParamsServerExConfig
{
    public int SendingParamItemDelayMs { get; set; } = 10;
    public string? CfgPrefix { get; set; } = "MAV_CFG_";
}

public class ParamsServerEx : MavlinkMicroserviceServer, IParamsServerEx
{
    private readonly IParamsServer _server;
    private readonly IStatusTextServer _statusTextServer;
    private readonly IMavParamEncoding _encoding;
    private readonly IConfiguration _cfg;
    private readonly ParamsServerExConfig _serverCfg;

    private readonly ILogger _logger;
    private readonly Subject<Exception> _onErrorSubject = new();
    private int _sendingInProgressFlag;
    private readonly ImmutableDictionary<string,(ushort,IMavParamTypeMetadata)> _paramDict;
    private readonly ImmutableList<IMavParamTypeMetadata> _paramList;
    private readonly Subject<ParamChangedEvent> _onParamChangedSubject;
    private readonly IDisposable _sub2;
    private readonly IDisposable _sub3;
    private readonly IDisposable _sub4;

    public ParamsServerEx(
        IParamsServer server, 
        IStatusTextServer statusTextServer, 
        IEnumerable<IMavParamTypeMetadata> paramDescriptions, 
        IMavParamEncoding encoding, 
        IConfiguration cfg, 
        ParamsServerExConfig serverCfg) : base(ParamsHelper.MicroserviceExName,server.Identity,server.Core)
    {
        _logger = server.Core.LoggerFactory.CreateLogger<ParamsServerEx>();
        _server = server;
        _statusTextServer = statusTextServer ?? throw new ArgumentNullException(nameof(statusTextServer));
        _encoding = encoding;
        _cfg = cfg;
        _serverCfg = serverCfg;
        
        _paramList = paramDescriptions.OrderBy(m=>m.Name).ToImmutableList();
        var dict = ImmutableDictionary.CreateBuilder<string, (ushort,IMavParamTypeMetadata)>();
        for (var i = 0; i < _paramList.Count; i++)
        {
            dict.Add(_paramList[i].Name,((ushort)i,_paramList[i]));
        }
        _paramDict = dict.ToImmutable();
        _onParamChangedSubject = new Subject<ParamChangedEvent>();
        
        _sub2 = server.OnParamSet.Subscribe(OnParamSet);
        _sub3 = server.OnParamRequestList.Subscribe(OnParamRequestList);
        _sub4 = server.OnParamRequestRead.Subscribe(OnParamRequestRead);
        
    }

    private async void OnParamRequestRead(ParamRequestReadPacket _)
    {
        (ushort,IMavParamTypeMetadata) param;
        if (_.Payload.ParamIndex < 0)
        {
            var name =  MavlinkTypesHelper.GetString(_.Payload.ParamId);
            if (_paramDict.TryGetValue(name, out param) == false)
            {
                _logger.ZLogError($"Error to get mavlink param: param '{name}' not found");
                _statusTextServer.Error($"Param '{name}' not found");
                _onErrorSubject.OnNext(new ArgumentException($"Error to get mavlink param: param '{name}' not found",name));
                return;
            }
        }
        else
        {
            if (_.Payload.ParamIndex >= _paramDict.Count)
            {
                _logger.ZLogError($"Error to get mavlink param: param '{_.Payload.ParamIndex}' not found");
                _statusTextServer.Error($"Param '{_.Payload.ParamIndex}' not found");
                _onErrorSubject.OnNext(new ArgumentException($"Error to get mavlink param: param with index '{_.Payload.ParamIndex}' not found"));
                return;
            }
            param = ((ushort)_.Payload.ParamIndex, _paramList[_.Payload.ParamIndex]);
        }
            
        var currentValue = param.Item2.ReadFromConfig(_cfg, _serverCfg.CfgPrefix);
        await SendParam(param, currentValue, DisposeCancel).ConfigureAwait(false);
    }

    private async void OnParamRequestList(ParamRequestListPacket paramRequestListPacket)
    {
        if (Interlocked.CompareExchange(ref _sendingInProgressFlag, 1, 0) == 1)
        {
            _logger.LogWarning("Skip duplicate request for params list");
            _statusTextServer.Error($"Skip duplicate request");
            return;
        }
        try
        {
            for (var index = 0; index < _paramList.Count; index++)
            {
                var param = _paramList[index];
                var currentValue =  param.ReadFromConfig(_cfg, _serverCfg.CfgPrefix);
                await SendParam(((ushort)index,param), currentValue, DisposeCancel).ConfigureAwait(false);
                if (_serverCfg.SendingParamItemDelayMs > 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(_serverCfg.SendingParamItemDelayMs), Core.TimeProvider, DisposeCancel).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            Interlocked.Exchange(ref _sendingInProgressFlag, 0);
        }
    }

    private async void OnParamSet(ParamSetPacket _)
    {
        var name = MavlinkTypesHelper.GetString(_.Payload.ParamId);
        if (_paramDict.TryGetValue(name, out var param) == false)
        {
            _logger.ZLogError($"Error to set mavlink param: param '{name}' not found");
            _statusTextServer.Error($"Param '{name}' not found");
            _onErrorSubject.OnNext(new ArgumentException($"Error to set mavlink param: param '{name}' not found", name));
            return;
        }

        var currentValue = param.Item2.ReadFromConfig(_cfg, _serverCfg.CfgPrefix);
        if (param.Item2.Type != _.Payload.ParamType)
        {
            _logger.ZLogError($"Error to set mavlink param: param '{name}' type didn't equal. Want {param.Item2.Type} but found {_.Payload.ParamType}");
            _statusTextServer.Error($"param type '{name}' not equal");
            _onErrorSubject.OnNext(new ArgumentException($"Error to set mavlink param: param '{name}' type didn't equal. Want {param.Item2.Type} but found {_.Payload.ParamType}", name));
            await SendParam(param, currentValue, DisposeCancel).ConfigureAwait(false);
            return;
        }
        
        var newValue = _encoding.ConvertFromMavlinkUnion(_.Payload.ParamValue, param.Item2.Type);
        if (param.Item2.IsValid(newValue) == false)
        {
            var errorMsg = param.Item2.GetValidationError(newValue);
            _logger.ZLogError($"Error to set mavlink param '{name}' [{param.Item2.Type}]: {errorMsg}");
            _statusTextServer.Error($"param '{name}':{errorMsg}");
            _onErrorSubject.OnNext(new ArgumentException($"Error to set mavlink param '{name}' [{param.Item2.Type}]: {errorMsg}", name));
            await SendParam(param, currentValue, DisposeCancel).ConfigureAwait(false);
            return;
        }
        _logger.ZLogInformation($"Set param {param.Item2.Name} from {currentValue} => {newValue}");
        param.Item2.WriteToConfig(_cfg, newValue,_serverCfg.CfgPrefix);
        _onParamChangedSubject.OnNext(new ParamChangedEvent(param, currentValue, newValue, true));
        await SendParam(param, newValue, DisposeCancel).ConfigureAwait(false);
    }



    private async Task SendParam((ushort, IMavParamTypeMetadata) param, MavParamValue value, CancellationToken cancel)
    {
        await _server.SendParamValue(p =>
        {
            p.ParamIndex = param.Item1;
            MavlinkTypesHelper.SetString(p.ParamId, param.Item2.Name);
            p.ParamType = param.Item2.Type;
            p.ParamCount = (ushort)_paramList.Count;
            p.ParamValue = _encoding.ConvertToMavlinkUnion(value);
        }, cancel).ConfigureAwait(false);
    }

    

    public Observable<Exception> OnError => _onErrorSubject;
    public Observable<ParamChangedEvent> OnUpdated => _onParamChangedSubject;

    public MavParamValue this[string name]
    {
        get
        {
            if (_paramDict.TryGetValue(name, out var param) == false)
            {
                throw new ArgumentException($"Param '{name}' not found", nameof(name));
            }
            return param.Item2.ReadFromConfig(_cfg, _serverCfg.CfgPrefix);
        }
        set
        {
            if (_paramDict.TryGetValue(name, out var param) == false)
            {
                throw new ArgumentException($"Param '{name}' not found", nameof(name));
            }
            
            if (param.Item2.IsValid(value) == false)
            {
                var errorMsg = param.Item2.GetValidationError(value);
                throw new ArgumentException($"Error to set mavlink param '{name}' [{param.Item2.Type}]: {errorMsg}", nameof(value));
            }
            var oldValue = param.Item2.ReadFromConfig(_cfg, _serverCfg.CfgPrefix);
            if (param.Item2.Volatile == false)
            {
                param.Item2.WriteToConfig(_cfg,value, _serverCfg.CfgPrefix);
            }
                
            _onParamChangedSubject.OnNext(new ParamChangedEvent(param, oldValue, value, false));
            // TODO: put to queue and send in background using delay (throttle)
            SendParam(param, value, DisposeCancel).Wait();
        }
    }

    /// <summary>
    /// Gets or sets the MavParamValue associated with the specified IMavParamTypeMetadata.
    /// </summary>
    /// <param name="param">The IMavParamTypeMetadata to retrieve or set the MavParamValue for.</param>
    /// <returns>The MavParamValue associated with the specified IMavParamTypeMetadata.</returns>
    public MavParamValue this[IMavParamTypeMetadata param]
    {
        get => this[param.Name];
        set => this[param.Name] = value;
    }

    public IReadOnlyList<IMavParamTypeMetadata> AllParamsList => _paramList;
    public IReadOnlyDictionary<string,(ushort,IMavParamTypeMetadata)> AllParamsDict => _paramDict;

    #region Dispose

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _onErrorSubject.Dispose();
            _onParamChangedSubject.Dispose();
            _sub2.Dispose();
            _sub3.Dispose();
            _sub4.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        await CastAndDispose(_onErrorSubject).ConfigureAwait(false);
        await CastAndDispose(_onParamChangedSubject).ConfigureAwait(false);
        await CastAndDispose(_sub2).ConfigureAwait(false);
        await CastAndDispose(_sub3).ConfigureAwait(false);
        await CastAndDispose(_sub4).ConfigureAwait(false);

        await base.DisposeAsyncCore().ConfigureAwait(false);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync().ConfigureAwait(false);
            else
                resource.Dispose();
        }
    }

    #endregion
}