using System.Globalization;
using Opc.UaFx;

namespace OpcUaService.Models;

[OpcDataType("ns=3;i=1809")]
[OpcDataTypeEncoding("ns=3;i=1810", Type = OpcEncodingType.Binary)]
[OpcDataTypeEncodingMask(OpcEncodingMaskKind.Auto)]
public class SimaticAssociatedAlarmValue
{
    public bool? Invalid { get; set; }
    public bool? Boolean { get; set; }
    public short? Int16 { get; set; }
    public int? Int32 { get; set; }
    public long? Int64 { get; set; }
    public sbyte? SByte { get; set; }
    public byte? Byte { get; set; }
    public ushort? UInt16 { get; set; }
    public uint? UInt32 { get; set; }
    public ulong? UInt64 { get; set; }
    public float? Float { get; set; }
    public double? Double { get; set; }
    public string? String { get; set; }

    public bool HasValue() =>
        Boolean.HasValue ||
        Int16.HasValue ||
        Int32.HasValue ||
        Int64.HasValue ||
        SByte.HasValue ||
        Byte.HasValue ||
        UInt16.HasValue ||
        UInt32.HasValue ||
        UInt64.HasValue ||
        Float.HasValue ||
        Double.HasValue ||
        !string.IsNullOrWhiteSpace(String);

    public override string ToString()
    {
        if (Boolean.HasValue && Boolean.Value)
            return Boolean.Value.ToString();
        
        if (Int16.HasValue)
            return Int16.Value.ToString();

        if (Int32.HasValue)
            return Int32.Value.ToString();
        
        if (Int64.HasValue)
            return Int64.Value.ToString();
        
        if (SByte.HasValue)
            return SByte.Value.ToString();
        
        if (Byte.HasValue)
            return Byte.Value.ToString();
        
        if (UInt16.HasValue)
            return UInt16.Value.ToString();
        
        if (UInt32.HasValue)
            return UInt32.Value.ToString();
        
        if (UInt64.HasValue)
            return UInt64.Value.ToString();
        
        if (Float.HasValue)
            return Float.Value.ToString(CultureInfo.InvariantCulture);
        
        if (Double.HasValue)
            return Double.Value.ToString(CultureInfo.InvariantCulture);
        
        if (!string.IsNullOrWhiteSpace(String))
            return String;

        return string.Empty;
    }
}