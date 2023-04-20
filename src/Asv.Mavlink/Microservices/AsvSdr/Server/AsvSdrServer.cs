﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.Mavlink.V2.AsvSdr;

namespace Asv.Mavlink
{
    public class AsvSdrServerConfig
    {
        public int StatusRateMs { get; set; } = 1000;
    }
    public class AsvSdrServer: MavlinkMicroserviceServer, IAsvSdrServer
    {
        private readonly AsvSdrServerConfig _config;
        private readonly MavlinkPacketTransponder<AsvSdrOutStatusPacket, AsvSdrOutStatusPayload> _transponder;

        public AsvSdrServer(IMavlinkV2Connection connection,
            MavlinkServerIdentity identity,AsvSdrServerConfig config, IPacketSequenceCalculator seq,
            IScheduler rxScheduler) 
            : base("SDR", connection, identity, seq, rxScheduler)
        {
            _config = config;
            _transponder =
                new MavlinkPacketTransponder<AsvSdrOutStatusPacket, AsvSdrOutStatusPayload>(connection, identity, seq)
                    .DisposeItWith(Disposable);
            OnRecordRequest = InternalFilter<AsvSdrRecordRequestPacket>(_=>_.Payload.TargetSystem,_=>_.Payload.TargetComponent)
                .Select(_ => _.Payload).Publish().RefCount();
            OnRecordDeleteRequest = InternalFilter<AsvSdrRecordDeleteRequestPacket>(_=>_.Payload.TargetSystem,_=>_.Payload.TargetComponent)
                .Select(_ => _.Payload).Publish().RefCount();
            OnRecordTagRequest = InternalFilter<AsvSdrRecordTagRequestPacket>(_=>_.Payload.TargetSystem,_=>_.Payload.TargetComponent)
                .Select(_ => _.Payload).Publish().RefCount();
            OnGetRecordTag = InternalFilter<AsvSdrRecordTagRequestPacket>(_=>_.Payload.TargetSystem,_=>_.Payload.TargetComponent)
                .Select(_ => _.Payload).Publish().RefCount();
            OnRecordTagDeleteRequest = InternalFilter<AsvSdrRecordTagDeleteRequestPacket>(_=>_.Payload.TargetSystem,_=>_.Payload.TargetComponent)
                .Select(_ => _.Payload).Publish().RefCount();
            OnRecordDataRequest = InternalFilter<AsvSdrRecordDataRequestPacket>(_=>_.Payload.TargetSystem,_=>_.Payload.TargetComponent)
                .Select(_ => _.Payload).Publish().RefCount();
        }

        public void Start()
        {
            _transponder.Start(TimeSpan.FromMilliseconds(_config.StatusRateMs));
        }

        public void Set(Action<AsvSdrOutStatusPayload> changeCallback)
        {
            _transponder.Set(changeCallback);
        }

        public IObservable<AsvSdrRecordRequestPayload> OnRecordRequest { get; }
        public Task SendRecordResponse(Action<AsvSdrRecordResponsePayload> setValueCallback, CancellationToken cancel = default)
        {
            return InternalSend<AsvSdrRecordResponsePacket>(_ =>{ setValueCallback(_.Payload); }, cancel);
        }

        public Task SendRecord(Action<AsvSdrRecordPayload> setValueCallback, CancellationToken cancel = default)
        {
            return InternalSend<AsvSdrRecordPacket>(_ =>{ setValueCallback(_.Payload); }, cancel);
        }

        public IObservable<AsvSdrRecordDeleteRequestPayload> OnRecordDeleteRequest { get; }
    
        public Task SendRecordDeleteResponse(Action<AsvSdrRecordDeleteResponsePayload> setValueCallback, CancellationToken cancel = default)
        {
            return InternalSend<AsvSdrRecordDeleteResponsePacket>(_ =>{ setValueCallback(_.Payload); }, cancel);
        }

        public IObservable<AsvSdrRecordTagRequestPayload> OnRecordTagRequest { get; }
        public Task SendRecordTagResponse(Action<AsvSdrRecordTagResponsePayload> setValueCallback, CancellationToken cancel = default)
        {
            return InternalSend<AsvSdrRecordTagResponsePacket>(_ =>{ setValueCallback(_.Payload); }, cancel);
        }

        public IObservable<AsvSdrRecordTagRequestPayload> OnGetRecordTag { get; }
        public Task SendRecordTag(Action<AsvSdrRecordTagPayload> setValueCallback, CancellationToken cancel = default)
        {
            return InternalSend<AsvSdrRecordTagPacket>(_ =>{ setValueCallback(_.Payload); }, cancel);
        }

        public IObservable<AsvSdrRecordTagDeleteRequestPayload> OnRecordTagDeleteRequest { get; }
        public Task SendRecordTagDeleteResponse(Action<AsvSdrRecordTagDeleteResponsePayload> setValueCallback, CancellationToken cancel = default)
        {
            return InternalSend<AsvSdrRecordTagDeleteResponsePacket>(_ =>{ setValueCallback(_.Payload); }, cancel);
        }

        public IObservable<AsvSdrRecordDataRequestPayload> OnRecordDataRequest { get; }
        public Task SendRecordDataResponse(Action<AsvSdrRecordDataResponsePayload> setValueCallback, CancellationToken cancel = default)
        {
            return InternalSend<AsvSdrRecordDataResponsePacket>(_ =>{ setValueCallback(_.Payload); }, cancel);
        }

        public Task SendRecordData<TPacket>(Action<TPacket> setValueCallback, CancellationToken cancel = default) 
            where TPacket : IPacketV2<IPayload>, new()
        {
            return InternalSend(setValueCallback, cancel);
        }
    }
}