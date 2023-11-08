using System;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.Mavlink.V2.AsvSdr;
using Asv.Mavlink.V2.Common;
using DynamicData;

namespace Asv.Mavlink;

public interface IAsvSdrClientEx
{
    IAsvSdrClient Base { get; }
    IRxValue<AsvSdrCustomModeFlag> SupportedModes { get; }
    IRxValue<AsvSdrCustomMode> CustomMode { get; }
    IRxValue<ushort> RecordsCount { get; }
    IObservable<IChangeSet<IAsvSdrClientRecord,Guid>> Records { get; }
    IRxValue<Guid> CurrentRecord { get; }
    IRxValue<bool> IsRecordStarted { get; }
    Task DeleteRecord(Guid recordName, CancellationToken cancel = default);
    Task<bool> DownloadRecordList(IProgress<double> progress = null, CancellationToken cancel = default);
    Task<MavResult> SetMode(AsvSdrCustomMode mode, ulong frequencyHz, float recordRate, uint sendingThinningRatio, CancellationToken cancel = default);
    Task<MavResult> StartRecord(string recordName, CancellationToken cancel = default);
    Task<MavResult> StopRecord(CancellationToken cancel = default);
    Task<MavResult> CurrentRecordSetTag(string tagName, AsvSdrRecordTagType type, byte[] rawValue , CancellationToken cancel = default);
    Task<MavResult> SystemControlAction(AsvSdrSystemControlAction action, CancellationToken cancel = default);
    Task<MavResult> StartMission(ushort missionIndex = 0, CancellationToken cancel = default);
    Task<MavResult> StopMission(CancellationToken cancel = default);
}

public static class AsvSdrClientExHelper
{
    public static async Task StartMissionAndCheckResult(this IAsvSdrClientEx src, ushort missionIndex, CancellationToken cancel = default)
    {
        var result = await src.StartMission(missionIndex, cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Start mission '{missionIndex:g}' failed. Result: {result}");
    }
    public static async Task StopMissionAndCheckResult(this IAsvSdrClientEx src, CancellationToken cancel = default)
    {
        var result = await src.StopMission(cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Stop mission failed. Result: {result}");
    }
    public static async Task SystemControlActionCheckResult(this IAsvSdrClientEx src, AsvSdrSystemControlAction action, CancellationToken cancel = default)
    {
        var result = await src.SystemControlAction(action, cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"System control action {action:G} failed. Result: {result}");
    }
    public static async Task SetModeAndCheckResult(this IAsvSdrClientEx src, AsvSdrCustomMode mode, ulong frequencyHz, float recordRate, uint sendingThinningRatio, CancellationToken cancel)
    {
        var result = await src.SetMode(mode, frequencyHz, recordRate, sendingThinningRatio, cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Set mode failed. Result: {result}");
    }
    
    public static async Task StartRecordAndCheckResult(this IAsvSdrClientEx src, string recordName, CancellationToken cancel)
    {
        var result = await src.StartRecord(recordName, cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Start record failed. Result: {result}");
    }
    
    public static async Task StopRecordAndCheckResult(this IAsvSdrClientEx src, CancellationToken cancel)
    {
        var result = await src.StopRecord(cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Stop record failed. Result: {result}");
    }

    public static async Task CurrentRecordSetTagAndCheckResult(this IAsvSdrClientEx src, string tagName, string value, CancellationToken cancel)
    {
        var result = await src.CurrentRecordSetTag(tagName, value, cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Set tag failed. Result: {result}");
    }
    public static async Task CurrentRecordSetTagAndCheckResult(this IAsvSdrClientEx src, string tagName, ulong value, CancellationToken cancel)
    {
        var result = await src.CurrentRecordSetTag(tagName, value, cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Set tag failed. Result: {result}");
    }
    public static async Task CurrentRecordSetTagAndCheckResult(this IAsvSdrClientEx src, string tagName, long value, CancellationToken cancel)
    {
        var result = await src.CurrentRecordSetTag(tagName, value, cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Set tag failed. Result: {result}");
    }
    public static async Task CurrentRecordSetTagAndCheckResult(this IAsvSdrClientEx src, string tagName, double value, CancellationToken cancel)
    {
        var result = await src.CurrentRecordSetTag(tagName, value, cancel).ConfigureAwait(false);
        if (result != MavResult.MavResultAccepted) throw new Exception($"Set tag failed. Result: {result}");
    }
    
    public static Task<MavResult> CurrentRecordSetTag(this IAsvSdrClientEx src, string tagName, string value, CancellationToken cancel)
    {
        if (value.Length > AsvSdrHelper.RecordTagValueLength) 
            throw new Exception($"Tag string value is too long. Max length is {AsvSdrHelper.RecordTagValueLength}");
        var nameArray = new byte[AsvSdrHelper.RecordTagValueLength];
        MavlinkTypesHelper.SetString(nameArray,value);
        return src.CurrentRecordSetTag(tagName, AsvSdrRecordTagType.AsvSdrRecordTagTypeString8, nameArray, cancel);
    }
    public static Task<MavResult> CurrentRecordSetTag(this IAsvSdrClientEx src, string tagName, ulong value, CancellationToken cancel)
    {
        return src.CurrentRecordSetTag(tagName, AsvSdrRecordTagType.AsvSdrRecordTagTypeUint64, BitConverter.GetBytes(value), cancel);
    }
    public static Task<MavResult> CurrentRecordSetTag(this IAsvSdrClientEx src, string tagName, long value, CancellationToken cancel)
    {
        return src.CurrentRecordSetTag(tagName, AsvSdrRecordTagType.AsvSdrRecordTagTypeInt64, BitConverter.GetBytes(value), cancel);
    }
    
    public static Task<MavResult> CurrentRecordSetTag(this IAsvSdrClientEx src, string tagName, double value, CancellationToken cancel)
    {
        return src.CurrentRecordSetTag(tagName, AsvSdrRecordTagType.AsvSdrRecordTagTypeReal64, BitConverter.GetBytes(value), cancel);
    }
    
}