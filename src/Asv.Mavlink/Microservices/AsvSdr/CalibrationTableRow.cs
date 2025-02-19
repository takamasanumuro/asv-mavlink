using System;
using Asv.Mavlink.AsvSdr;


namespace Asv.Mavlink;

public class CalibrationTablePod
{
    public string? Name { get; set; }
    public CalibrationTableMetadata? Metadata { get; set; }
    public CalibrationTableRow[]? Rows { get; set; }
}


public class CalibrationTableMetadata
{
    public CalibrationTableMetadata()
    {
        
    }
    public CalibrationTableMetadata(AsvSdrCalibTableUploadStartPacket updated)
    {
        Updated = MavlinkTypesHelper.FromUnixTimeUs(updated.Payload.CreatedUnixUs);
    }
    public CalibrationTableMetadata(AsvSdrCalibTablePayload result)
    {
        Updated = MavlinkTypesHelper.FromUnixTimeUs(result.CreatedUnixUs);
    }
    public CalibrationTableMetadata(DateTime updated)
    {
        Updated = updated;
    }

    

    public DateTime Updated { get; set; }

    public void Fill(AsvSdrCalibTablePayload payload)
    {
        payload.CreatedUnixUs = MavlinkTypesHelper.ToUnixTimeUs(Updated);
    }
}

public class CalibrationTableRow
{
    public CalibrationTableRow()
    {
        
    }
    public CalibrationTableRow(AsvSdrCalibTableRowPayload result)
    {
        FrequencyHz = result.RefFreq;
        RefPower = result.RefPower;
        RefValue = result.RefValue;
        Adjustment = result.Adjustment;
    }

    public CalibrationTableRow(ulong frequencyHz, float refPower, float refValue, float adjustment)
    {
        FrequencyHz = frequencyHz;
        RefPower = refPower;
        RefValue = refValue;
        Adjustment = adjustment;
    }
    public ulong FrequencyHz { get; set; }
    public float RefPower { get;set; }
    public float RefValue { get;set; }
    public float Adjustment { get; set; }

    public void Fill(AsvSdrCalibTableRowPayload args)
    {
        args.RefFreq = FrequencyHz;
        args.RefPower = RefPower;
        args.RefValue = RefValue;
        args.Adjustment = Adjustment;
    }
}