using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Asv.Mavlink.V2.AsvRsga;
using Asv.Mavlink.V2.Common;

namespace Asv.Mavlink;

public delegate Task<MavResult> RsgaSetMode(AsvRsgaCustomMode mode, ulong freq, CancellationToken cancel = default);
public delegate IEnumerable<AsvRsgaCustomMode> RsgaGetCompatibility();

public interface IAsvRsgaServerEx
{
    IAsvRsgaServer Base { get; }
    RsgaSetMode? SetMode { get; set; }
    RsgaGetCompatibility? GetCompatibility { get; set; }
}