using Opc.UaFx;

namespace OpcUaService.Models;

[OpcEventType("ns=3;i=1804")]

public class SimaticEventType : OpcEvent
{
	/// <inheritdoc />
	public SimaticEventType(IOpcReadOnlyNodeDataStore dataStore) : base(dataStore)
	{
	}

	private static readonly OpcName AssociatedValueName01 = new("3:AssociatedValue_01");
	private static readonly OpcName AssociatedValueName02 = new("AssociatedValue_02", 3);
	private static readonly OpcName AssociatedValueName03 = new("AssociatedValue_03", 3);
	private static readonly OpcName AssociatedValueName04 = new("AssociatedValue_04", 3);
	private static readonly OpcName AssociatedValueName05 = new("AssociatedValue_05", 3);
	private static readonly OpcName AssociatedValueName06 = new("AssociatedValue_06", 3);
	private static readonly OpcName AssociatedValueName07 = new("AssociatedValue_07", 3);
	private static readonly OpcName AssociatedValueName08 = new("AssociatedValue_08", 3);
	private static readonly OpcName AssociatedValueName09 = new("AssociatedValue_09", 3);
	private static readonly OpcName AssociatedValueName10 = new("AssociatedValue_10", 3);
	
	public SimaticAssociatedAlarmValue AssociatedAlarmValue01 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName01);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue02 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName02);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue03 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName03);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue04 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName04);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue05 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName05);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue06 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName06);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue07 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName07);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue08 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName08);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue09 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName09);
	public SimaticAssociatedAlarmValue AssociatedAlarmValue10 => DataStore.Get<SimaticAssociatedAlarmValue>(AssociatedValueName10);
}