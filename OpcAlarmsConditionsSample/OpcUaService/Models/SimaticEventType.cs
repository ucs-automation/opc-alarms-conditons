using Opc.UaFx;

namespace OpcUaService.Models;

[OpcEventType("ns=3;i=0000")]
public class SimaticEventType : OpcEvent
{
	/// <inheritdoc />
	public SimaticEventType(IOpcReadOnlyNodeDataStore dataStore) : base(dataStore)
	{
	}

	private static readonly OpcName AssociatedValueName01 = new("AssociatedValue_01", 3);
	private static readonly OpcName AssociatedValueName02 = new("AssociatedValue_02", 3);
	private static readonly OpcName AssociatedValueName03 = new("AssociatedValue_03", 3);
	private static readonly OpcName AssociatedValueName04 = new("AssociatedValue_04", 3);
	private static readonly OpcName AssociatedValueName05 = new("AssociatedValue_05", 3);
	private static readonly OpcName AssociatedValueName06 = new("AssociatedValue_06", 3);
	private static readonly OpcName AssociatedValueName07 = new("AssociatedValue_07", 3);
	private static readonly OpcName AssociatedValueName08 = new("AssociatedValue_08", 3);
	private static readonly OpcName AssociatedValueName09 = new("AssociatedValue_09", 3);
	private static readonly OpcName AssociatedValueName10 = new("AssociatedValue_10", 3);
	
	public SimaticAssociatedValue AssociatedValue01 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName01);
	public SimaticAssociatedValue AssociatedValue02 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName02);
	public SimaticAssociatedValue AssociatedValue03 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName03);
	public SimaticAssociatedValue AssociatedValue04 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName04);
	public SimaticAssociatedValue AssociatedValue05 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName05);
	public SimaticAssociatedValue AssociatedValue06 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName06);
	public SimaticAssociatedValue AssociatedValue07 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName07);
	public SimaticAssociatedValue AssociatedValue08 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName08);
	public SimaticAssociatedValue AssociatedValue09 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName09);
	public SimaticAssociatedValue AssociatedValue10 => DataStore.Get<SimaticAssociatedValue>(AssociatedValueName10);
}