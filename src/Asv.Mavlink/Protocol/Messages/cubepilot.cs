// MIT License
//
// Copyright (c) 2023 asv-soft (https://github.com/asv-soft)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// This code was generate by tool Asv.Mavlink.Shell version 3.2.5-alpha-11

using System;
using System.Text;
using Asv.IO;

namespace Asv.Mavlink.V2.Cubepilot
{

    public static class CubepilotHelper
    {
        public static void RegisterCubepilotDialect(this IPacketDecoder<IPacketV2<IPayload>> src)
        {
            src.Register(()=>new CubepilotRawRcPacket());
            src.Register(()=>new HerelinkVideoStreamInformationPacket());
            src.Register(()=>new HerelinkTelemPacket());
            src.Register(()=>new CubepilotFirmwareUpdateStartPacket());
            src.Register(()=>new CubepilotFirmwareUpdateRespPacket());
        }
    }

#region Enums


#endregion

#region Messages

    /// <summary>
    /// Raw RC Data
    ///  CUBEPILOT_RAW_RC
    /// </summary>
    public class CubepilotRawRcPacket: PacketV2<CubepilotRawRcPayload>
    {
	    public const int PacketMessageId = 50001;
        public override int MessageId => PacketMessageId;
        public override byte GetCrcEtra() => 246;
        public override bool WrapToV2Extension => false;

        public override CubepilotRawRcPayload Payload { get; } = new CubepilotRawRcPayload();

        public override string Name => "CUBEPILOT_RAW_RC";
    }

    /// <summary>
    ///  CUBEPILOT_RAW_RC
    /// </summary>
    public class CubepilotRawRcPayload : IPayload
    {
        public byte GetMaxByteSize() => 32; // Sum of byte sized of all fields (include extended)
        public byte GetMinByteSize() => 32; // of byte sized of fields (exclude extended)
        public int GetByteSize()
        {
            var sum = 0;
            sum+=RcRaw.Length; //RcRaw
            return (byte)sum;
        }



        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var arraySize = 0;
            var payloadSize = buffer.Length;
            arraySize = /*ArrayLength*/32 - Math.Max(0,((/*PayloadByteSize*/32 - payloadSize - /*ExtendedFieldsLength*/0)/1 /*FieldTypeByteSize*/));
            RcRaw = new byte[arraySize];
            for(var i=0;i<arraySize;i++)
            {
                RcRaw[i] = (byte)BinSerialize.ReadByte(ref buffer);
            }

        }

        public void Serialize(ref Span<byte> buffer)
        {
            for(var i=0;i<RcRaw.Length;i++)
            {
                BinSerialize.WriteByte(ref buffer,(byte)RcRaw[i]);
            }
            /* PayloadByteSize = 32 */;
        }
        
        



        /// <summary>
        /// 
        /// OriginName: rc_raw, Units: , IsExtended: false
        /// </summary>
        public byte[] RcRaw { get; set; } = new byte[32];
        public byte GetRcRawMaxItemsCount() => 32;
    }
    /// <summary>
    /// Information about video stream
    ///  HERELINK_VIDEO_STREAM_INFORMATION
    /// </summary>
    public class HerelinkVideoStreamInformationPacket: PacketV2<HerelinkVideoStreamInformationPayload>
    {
	    public const int PacketMessageId = 50002;
        public override int MessageId => PacketMessageId;
        public override byte GetCrcEtra() => 181;
        public override bool WrapToV2Extension => false;

        public override HerelinkVideoStreamInformationPayload Payload { get; } = new HerelinkVideoStreamInformationPayload();

        public override string Name => "HERELINK_VIDEO_STREAM_INFORMATION";
    }

    /// <summary>
    ///  HERELINK_VIDEO_STREAM_INFORMATION
    /// </summary>
    public class HerelinkVideoStreamInformationPayload : IPayload
    {
        public byte GetMaxByteSize() => 246; // Sum of byte sized of all fields (include extended)
        public byte GetMinByteSize() => 246; // of byte sized of fields (exclude extended)
        public int GetByteSize()
        {
            var sum = 0;
            sum+=4; //Framerate
            sum+=4; //Bitrate
            sum+=2; //ResolutionH
            sum+=2; //ResolutionV
            sum+=2; //Rotation
            sum+=1; //CameraId
            sum+=1; //Status
            sum+=Uri.Length; //Uri
            return (byte)sum;
        }



        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var arraySize = 0;
            var payloadSize = buffer.Length;
            Framerate = BinSerialize.ReadFloat(ref buffer);
            Bitrate = BinSerialize.ReadUInt(ref buffer);
            ResolutionH = BinSerialize.ReadUShort(ref buffer);
            ResolutionV = BinSerialize.ReadUShort(ref buffer);
            Rotation = BinSerialize.ReadUShort(ref buffer);
            CameraId = (byte)BinSerialize.ReadByte(ref buffer);
            Status = (byte)BinSerialize.ReadByte(ref buffer);
            arraySize = /*ArrayLength*/230 - Math.Max(0,((/*PayloadByteSize*/246 - payloadSize - /*ExtendedFieldsLength*/0)/1 /*FieldTypeByteSize*/));
            Uri = new char[arraySize];
            unsafe
            {
                fixed (byte* bytePointer = buffer)
                fixed (char* charPointer = Uri)
                {
                    Encoding.ASCII.GetChars(bytePointer, arraySize, charPointer, Uri.Length);
                }
            }
            buffer = buffer.Slice(arraySize);
           

        }

        public void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteFloat(ref buffer,Framerate);
            BinSerialize.WriteUInt(ref buffer,Bitrate);
            BinSerialize.WriteUShort(ref buffer,ResolutionH);
            BinSerialize.WriteUShort(ref buffer,ResolutionV);
            BinSerialize.WriteUShort(ref buffer,Rotation);
            BinSerialize.WriteByte(ref buffer,(byte)CameraId);
            BinSerialize.WriteByte(ref buffer,(byte)Status);
            unsafe
            {
                fixed (byte* bytePointer = buffer)
                fixed (char* charPointer = Uri)
                {
                    Encoding.ASCII.GetBytes(charPointer, Uri.Length, bytePointer, Uri.Length);
                }
            }
            buffer = buffer.Slice(Uri.Length);
            
            /* PayloadByteSize = 246 */;
        }
        
        



        /// <summary>
        /// Frame rate.
        /// OriginName: framerate, Units: Hz, IsExtended: false
        /// </summary>
        public float Framerate { get; set; }
        /// <summary>
        /// Bit rate.
        /// OriginName: bitrate, Units: bits/s, IsExtended: false
        /// </summary>
        public uint Bitrate { get; set; }
        /// <summary>
        /// Horizontal resolution.
        /// OriginName: resolution_h, Units: pix, IsExtended: false
        /// </summary>
        public ushort ResolutionH { get; set; }
        /// <summary>
        /// Vertical resolution.
        /// OriginName: resolution_v, Units: pix, IsExtended: false
        /// </summary>
        public ushort ResolutionV { get; set; }
        /// <summary>
        /// Video image rotation clockwise.
        /// OriginName: rotation, Units: deg, IsExtended: false
        /// </summary>
        public ushort Rotation { get; set; }
        /// <summary>
        /// Video Stream ID (1 for first, 2 for second, etc.)
        /// OriginName: camera_id, Units: , IsExtended: false
        /// </summary>
        public byte CameraId { get; set; }
        /// <summary>
        /// Number of streams available.
        /// OriginName: status, Units: , IsExtended: false
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// Video stream URI (TCP or RTSP URI ground station should connect to) or port number (UDP port ground station should listen to).
        /// OriginName: uri, Units: , IsExtended: false
        /// </summary>
        public char[] Uri { get; set; } = new char[230];
        public byte GetUriMaxItemsCount() => 230;
    }
    /// <summary>
    /// Herelink Telemetry
    ///  HERELINK_TELEM
    /// </summary>
    public class HerelinkTelemPacket: PacketV2<HerelinkTelemPayload>
    {
	    public const int PacketMessageId = 50003;
        public override int MessageId => PacketMessageId;
        public override byte GetCrcEtra() => 62;
        public override bool WrapToV2Extension => false;

        public override HerelinkTelemPayload Payload { get; } = new HerelinkTelemPayload();

        public override string Name => "HERELINK_TELEM";
    }

    /// <summary>
    ///  HERELINK_TELEM
    /// </summary>
    public class HerelinkTelemPayload : IPayload
    {
        public byte GetMaxByteSize() => 19; // Sum of byte sized of all fields (include extended)
        public byte GetMinByteSize() => 19; // of byte sized of fields (exclude extended)
        public int GetByteSize()
        {
            var sum = 0;
            sum+=4; //RfFreq
            sum+=4; //LinkBw
            sum+=4; //LinkRate
            sum+=2; //Snr
            sum+=2; //CpuTemp
            sum+=2; //BoardTemp
            sum+=1; //Rssi
            return (byte)sum;
        }



        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            RfFreq = BinSerialize.ReadUInt(ref buffer);
            LinkBw = BinSerialize.ReadUInt(ref buffer);
            LinkRate = BinSerialize.ReadUInt(ref buffer);
            Snr = BinSerialize.ReadShort(ref buffer);
            CpuTemp = BinSerialize.ReadShort(ref buffer);
            BoardTemp = BinSerialize.ReadShort(ref buffer);
            Rssi = (byte)BinSerialize.ReadByte(ref buffer);

        }

        public void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteUInt(ref buffer,RfFreq);
            BinSerialize.WriteUInt(ref buffer,LinkBw);
            BinSerialize.WriteUInt(ref buffer,LinkRate);
            BinSerialize.WriteShort(ref buffer,Snr);
            BinSerialize.WriteShort(ref buffer,CpuTemp);
            BinSerialize.WriteShort(ref buffer,BoardTemp);
            BinSerialize.WriteByte(ref buffer,(byte)Rssi);
            /* PayloadByteSize = 19 */;
        }
        
        



        /// <summary>
        /// 
        /// OriginName: rf_freq, Units: , IsExtended: false
        /// </summary>
        public uint RfFreq { get; set; }
        /// <summary>
        /// 
        /// OriginName: link_bw, Units: , IsExtended: false
        /// </summary>
        public uint LinkBw { get; set; }
        /// <summary>
        /// 
        /// OriginName: link_rate, Units: , IsExtended: false
        /// </summary>
        public uint LinkRate { get; set; }
        /// <summary>
        /// 
        /// OriginName: snr, Units: , IsExtended: false
        /// </summary>
        public short Snr { get; set; }
        /// <summary>
        /// 
        /// OriginName: cpu_temp, Units: , IsExtended: false
        /// </summary>
        public short CpuTemp { get; set; }
        /// <summary>
        /// 
        /// OriginName: board_temp, Units: , IsExtended: false
        /// </summary>
        public short BoardTemp { get; set; }
        /// <summary>
        /// 
        /// OriginName: rssi, Units: , IsExtended: false
        /// </summary>
        public byte Rssi { get; set; }
    }
    /// <summary>
    /// Start firmware update with encapsulated data.
    ///  CUBEPILOT_FIRMWARE_UPDATE_START
    /// </summary>
    public class CubepilotFirmwareUpdateStartPacket: PacketV2<CubepilotFirmwareUpdateStartPayload>
    {
	    public const int PacketMessageId = 50004;
        public override int MessageId => PacketMessageId;
        public override byte GetCrcEtra() => 240;
        public override bool WrapToV2Extension => false;

        public override CubepilotFirmwareUpdateStartPayload Payload { get; } = new CubepilotFirmwareUpdateStartPayload();

        public override string Name => "CUBEPILOT_FIRMWARE_UPDATE_START";
    }

    /// <summary>
    ///  CUBEPILOT_FIRMWARE_UPDATE_START
    /// </summary>
    public class CubepilotFirmwareUpdateStartPayload : IPayload
    {
        public byte GetMaxByteSize() => 10; // Sum of byte sized of all fields (include extended)
        public byte GetMinByteSize() => 10; // of byte sized of fields (exclude extended)
        public int GetByteSize()
        {
            var sum = 0;
            sum+=4; //Size
            sum+=4; //Crc
            sum+=1; //TargetSystem
            sum+=1; //TargetComponent
            return (byte)sum;
        }



        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Size = BinSerialize.ReadUInt(ref buffer);
            Crc = BinSerialize.ReadUInt(ref buffer);
            TargetSystem = (byte)BinSerialize.ReadByte(ref buffer);
            TargetComponent = (byte)BinSerialize.ReadByte(ref buffer);

        }

        public void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteUInt(ref buffer,Size);
            BinSerialize.WriteUInt(ref buffer,Crc);
            BinSerialize.WriteByte(ref buffer,(byte)TargetSystem);
            BinSerialize.WriteByte(ref buffer,(byte)TargetComponent);
            /* PayloadByteSize = 10 */;
        }
        
        



        /// <summary>
        /// FW Size.
        /// OriginName: size, Units: bytes, IsExtended: false
        /// </summary>
        public uint Size { get; set; }
        /// <summary>
        /// FW CRC.
        /// OriginName: crc, Units: , IsExtended: false
        /// </summary>
        public uint Crc { get; set; }
        /// <summary>
        /// System ID.
        /// OriginName: target_system, Units: , IsExtended: false
        /// </summary>
        public byte TargetSystem { get; set; }
        /// <summary>
        /// Component ID.
        /// OriginName: target_component, Units: , IsExtended: false
        /// </summary>
        public byte TargetComponent { get; set; }
    }
    /// <summary>
    /// offset response to encapsulated data.
    ///  CUBEPILOT_FIRMWARE_UPDATE_RESP
    /// </summary>
    public class CubepilotFirmwareUpdateRespPacket: PacketV2<CubepilotFirmwareUpdateRespPayload>
    {
	    public const int PacketMessageId = 50005;
        public override int MessageId => PacketMessageId;
        public override byte GetCrcEtra() => 152;
        public override bool WrapToV2Extension => false;

        public override CubepilotFirmwareUpdateRespPayload Payload { get; } = new CubepilotFirmwareUpdateRespPayload();

        public override string Name => "CUBEPILOT_FIRMWARE_UPDATE_RESP";
    }

    /// <summary>
    ///  CUBEPILOT_FIRMWARE_UPDATE_RESP
    /// </summary>
    public class CubepilotFirmwareUpdateRespPayload : IPayload
    {
        public byte GetMaxByteSize() => 6; // Sum of byte sized of all fields (include extended)
        public byte GetMinByteSize() => 6; // of byte sized of fields (exclude extended)
        public int GetByteSize()
        {
            var sum = 0;
            sum+=4; //Offset
            sum+=1; //TargetSystem
            sum+=1; //TargetComponent
            return (byte)sum;
        }



        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Offset = BinSerialize.ReadUInt(ref buffer);
            TargetSystem = (byte)BinSerialize.ReadByte(ref buffer);
            TargetComponent = (byte)BinSerialize.ReadByte(ref buffer);

        }

        public void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteUInt(ref buffer,Offset);
            BinSerialize.WriteByte(ref buffer,(byte)TargetSystem);
            BinSerialize.WriteByte(ref buffer,(byte)TargetComponent);
            /* PayloadByteSize = 6 */;
        }
        
        



        /// <summary>
        /// FW Offset.
        /// OriginName: offset, Units: bytes, IsExtended: false
        /// </summary>
        public uint Offset { get; set; }
        /// <summary>
        /// System ID.
        /// OriginName: target_system, Units: , IsExtended: false
        /// </summary>
        public byte TargetSystem { get; set; }
        /// <summary>
        /// Component ID.
        /// OriginName: target_component, Units: , IsExtended: false
        /// </summary>
        public byte TargetComponent { get; set; }
    }


#endregion


}
