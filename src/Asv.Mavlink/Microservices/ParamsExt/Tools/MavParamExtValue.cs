using System;
using System.Collections.Generic;
using System.Linq;
using Asv.Mavlink.V2.Common;

namespace Asv.Mavlink;

public readonly struct MavParamExtValue : IComparable<MavParamExtValue>, IComparable, IEquatable<MavParamExtValue>
{
    public const int RawValueBufferMaxLength = 128;

    private readonly byte[] _rawValue = new byte[RawValueBufferMaxLength];

    #region Constructors

    public MavParamExtValue(byte value)
    {
        Type = MavParamExtType.MavParamExtTypeUint8;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(sbyte value)
    {
        Type = MavParamExtType.MavParamExtTypeInt8;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(short value)
    {
        Type = MavParamExtType.MavParamExtTypeInt16;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(ushort value)
    {
        Type = MavParamExtType.MavParamExtTypeUint16;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(int value)
    {
        Type = MavParamExtType.MavParamExtTypeInt32;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(uint value)
    {
        Type = MavParamExtType.MavParamExtTypeUint32;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(long value)
    {
        Type = MavParamExtType.MavParamExtTypeInt32;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(ulong value)
    {
        Type = MavParamExtType.MavParamExtTypeUint32;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(float value)
    {
        Type = MavParamExtType.MavParamExtTypeReal32;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(double value)
    {
        Type = MavParamExtType.MavParamExtTypeReal64;
        CopyValueBytesToRawValue(value);
    }

    public MavParamExtValue(char[] value)
    {
        Type = MavParamExtType.MavParamExtTypeCustom;
        var tempBuf = value.Select(ch => (byte)ch).ToArray();
        Array.Copy(tempBuf, _rawValue, tempBuf.Length);
    }

    public MavParamExtValue(byte[] value)
    {
        Type = MavParamExtType.MavParamExtTypeCustom;
        var tempBuf = value.ToArray();
        Array.Copy(tempBuf, _rawValue, tempBuf.Length);
    }

    #endregion

    private void CopyValueBytesToRawValue<T>(T value) where T : struct
    {
        dynamic dynamicValue = value;
        var tempBuf = BitConverter.GetBytes(dynamicValue);
        Array.Copy(tempBuf, _rawValue, tempBuf.Length);
    }

    #region Cast to MavParamExtValue

    public static implicit operator MavParamExtValue(byte x) => new(x);

    public static implicit operator MavParamExtValue(sbyte x) => new(x);

    public static implicit operator MavParamExtValue(short x) => new(x);

    public static implicit operator MavParamExtValue(ushort x) => new(x);

    public static implicit operator MavParamExtValue(int x) => new(x);

    public static implicit operator MavParamExtValue(uint x) => new(x);

    public static implicit operator MavParamExtValue(float x) => new(x);

    public static implicit operator MavParamExtValue(double x) => new(x);

    public static implicit operator MavParamExtValue(long x) => new(x);

    public static implicit operator MavParamExtValue(ulong x) => new(x);

    public static implicit operator MavParamExtValue(char[] x) => new(x);

    public static implicit operator MavParamExtValue(byte[] x) => new(x);

    #endregion

    #region Cast from MavParamExtValue

    public static implicit operator byte(MavParamExtValue x) => x._rawValue[0];

    public static implicit operator sbyte(MavParamExtValue x) => (sbyte)x._rawValue[0];

    public static implicit operator short(MavParamExtValue x) => BitConverter.ToInt16(x._rawValue);

    public static implicit operator ushort(MavParamExtValue x) => BitConverter.ToUInt16(x._rawValue);

    public static implicit operator int(MavParamExtValue x) => BitConverter.ToInt32(x._rawValue);

    public static implicit operator uint(MavParamExtValue x) => BitConverter.ToUInt32(x._rawValue);

    public static implicit operator long(MavParamExtValue x) => BitConverter.ToInt64(x._rawValue);

    public static implicit operator ulong(MavParamExtValue x) => BitConverter.ToUInt64(x._rawValue);

    public static implicit operator float(MavParamExtValue x) => BitConverter.ToSingle(x._rawValue);

    public static implicit operator double(MavParamExtValue x) => BitConverter.ToDouble(x._rawValue);

    public static implicit operator char[](MavParamExtValue x) => x._rawValue.Select(by => (char)by).ToArray();

    public static implicit operator byte[](MavParamExtValue x) => x._rawValue;

    #endregion

    public MavParamExtType Type { get; }

    #region Comparison

    public int CompareTo(object obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is MavParamValue other ? CompareTo(other) :
            throw new ArgumentException($"Object must be of type {nameof(MavParamValue)}");
    }
    
    public int CompareTo(MavParamExtValue other)
    {
        var typeComparison = Type.CompareTo(other.Type);
        if (typeComparison != 0) return typeComparison;

        for (int i = 0; i < RawValueBufferMaxLength; i++)
        {
            var valueComparison = _rawValue[i].CompareTo(other._rawValue[i]);
            if (valueComparison != 0) return valueComparison;
        }

        return 0;
    }

    public bool Equals(MavParamExtValue other) => 
        Type == other.Type && _rawValue.SequenceEqual(other._rawValue);

    public override int GetHashCode()
    {
        var hash = (int)Type;
        return _rawValue.Aggregate(hash, (current, val) => (current * 397) ^ val);
    }

    public static bool operator ==(MavParamExtValue left, MavParamExtValue right) => left.Equals(right);
    public static bool operator !=(MavParamExtValue left, MavParamExtValue right) => !left.Equals(right);
    public static bool operator <(MavParamExtValue left, MavParamExtValue right) => left.CompareTo(right) < 0;
    public static bool operator >(MavParamExtValue left, MavParamExtValue right) => left.CompareTo(right) > 0;
    public static bool operator <=(MavParamExtValue left, MavParamExtValue right) => left.CompareTo(right) <= 0;
    public static bool operator >=(MavParamExtValue left, MavParamExtValue right) => left.CompareTo(right) >= 0;

    #endregion

    public override string ToString() =>
        Type switch
        {
            MavParamExtType.MavParamExtTypeUint8 => $"{(byte)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeInt8 => $"{(sbyte)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeUint16 => $"{(ushort)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeInt16 => $"{(short)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeUint32 => $"{(uint)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeInt32 => $"{(int)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeReal32 => $"{(float)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeUint64 => $"{(ulong)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeInt64 => $"{(long)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeReal64 => $"{(double)this}[{Type:G}]",
            MavParamExtType.MavParamExtTypeCustom => $"{{{string.Join(",", _rawValue)}}}[{Type:G}]",
            _ => throw new ArgumentOutOfRangeException()
        };
}