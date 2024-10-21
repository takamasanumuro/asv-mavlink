using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.Mavlink.V2.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using R3;
using ZLogger;

namespace Asv.Mavlink;

public class ParameterClientConfig
{
    public int ReadAttemptCount { get; set; } = 3;
    public int ReadTimeouMs { get; set; } = 1000;
}

public class ParamsClient : MavlinkMicroserviceClient, IParamsClient
{
    private readonly ParameterClientConfig _config;
    private readonly ILogger _logger;
    private readonly System.Reactive.Subjects.Subject<ParamValuePayload> _onParamValue;
    private readonly IDisposable _disposeIt;

    public ParamsClient(MavlinkClientIdentity identity, ParameterClientConfig config, ICoreServices core) 
        : base("PARAMS",  identity, core)
    {
        _logger = core.Log.CreateLogger<ParamsClient>();
        _config = config;
        _onParamValue = new System.Reactive.Subjects.Subject<ParamValuePayload>();
        var d1 = InternalFilter<ParamValuePacket>().Select(p => p.Payload).Subscribe(_onParamValue);
        _disposeIt = Disposable.Combine(_onParamValue, d1);

    }

    public IObservable<ParamValuePayload> OnParamValue => _onParamValue;
    
    public Task SendRequestList(CancellationToken cancel = default)
    {
        _logger.ZLogInformation($"{LogSend} Request all params from vehicle");
        return InternalSend<ParamRequestListPacket>(p =>
        {
            p.Payload.TargetComponent = Identity.Target.ComponentId;
            p.Payload.TargetSystem = Identity.Target.SystemId;
        }, cancel);
    }

    public Task<ParamValuePayload> Read(string name, CancellationToken cancel = default)
    {
        return InternalCall<ParamValuePayload, ParamRequestReadPacket, ParamValuePacket>(p =>
            {
                p.Payload.TargetComponent = Identity.Target.ComponentId;
                p.Payload.TargetSystem = Identity.Target.SystemId;
                p.Payload.ParamIndex = -1;
                MavlinkTypesHelper.SetString(p.Payload.ParamId, name);
            }, p => name.Equals(MavlinkTypesHelper.GetString(p.Payload.ParamId)),
            p => p.Payload,
            _config.ReadAttemptCount, timeoutMs: _config.ReadTimeouMs, cancel: cancel);
    }
    
    public Task<ParamValuePayload> Read(ushort index, CancellationToken cancel = default)
    {
        return InternalCall<ParamValuePayload, ParamRequestReadPacket, ParamValuePacket>(p =>
            {
                p.Payload.TargetComponent = Identity.Target.ComponentId;
                p.Payload.TargetSystem = Identity.Target.SystemId;
                MavlinkTypesHelper.SetString(p.Payload.ParamId, string.Empty);
                p.Payload.ParamIndex = (short)index;
            }, p => index == p.Payload.ParamIndex,
            p => p.Payload,
            _config.ReadAttemptCount, timeoutMs: _config.ReadTimeouMs, cancel: cancel);
    }

    public Task<ParamValuePayload> Write(string name, MavParamType type, float value, CancellationToken cancel = default)
    {
        return InternalCall<ParamValuePayload, ParamSetPacket, ParamValuePacket>(p =>
            {
                p.Payload.TargetComponent = Identity.Target.ComponentId;
                p.Payload.TargetSystem = Identity.Target.SystemId;
                p.Payload.ParamType = type;
                p.Payload.ParamValue = value;
                MavlinkTypesHelper.SetString(p.Payload.ParamId, name);
            }, p => name.Equals(MavlinkTypesHelper.GetString(p.Payload.ParamId)),
            p => p.Payload,
            _config.ReadAttemptCount, timeoutMs: _config.ReadTimeouMs, cancel: cancel);
    }

    public override void Dispose()
    {
        _disposeIt.Dispose();
        base.Dispose();
    }
}