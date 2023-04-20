using Opc.UaFx;

namespace OpcUaService.Models;

[OpcEventType("ns=3;i=0000")]
public class SimaticEventType : OpcEvent
{
	/// <inheritdoc />
	public SimaticEventType(IOpcReadOnlyNodeDataStore dataStore) : base(dataStore)
	{
	}

	public SimaticAssociatedValue AssociatedValue01 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_01");
	public SimaticAssociatedValue AssociatedValue02 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_02");
	public SimaticAssociatedValue AssociatedValue03 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_03");
	public SimaticAssociatedValue AssociatedValue04 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_04");
	public SimaticAssociatedValue AssociatedValue05 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_05");
	public SimaticAssociatedValue AssociatedValue06 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_06");
	public SimaticAssociatedValue AssociatedValue07 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_07");
	public SimaticAssociatedValue AssociatedValue08 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_08");
	public SimaticAssociatedValue AssociatedValue09 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_09");
	public SimaticAssociatedValue AssociatedValue10 => DataStore.Get<SimaticAssociatedValue>("3:AssociatedValue_10");
}