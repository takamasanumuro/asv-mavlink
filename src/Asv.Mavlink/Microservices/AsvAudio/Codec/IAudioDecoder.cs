using System;
using Asv.Mavlink.V2.AsvAudio;
using R3;

namespace Asv.Mavlink;

public interface IAudioDecoder: IDisposable, IAsyncDisposable
{   
    Observable<ReadOnlyMemory<byte>> Output { get; }
    AsvAudioCodec Codec { get; }
   
}