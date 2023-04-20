using Opc.UaFx;

namespace OpcUaService.Models;

[OpcDataType("ns=3;i=0001")]
[OpcDataTypeEncoding("ns=3;i=0002")]
public class SimaticAssociatedValue
{
	public object Value { get; set; }
}