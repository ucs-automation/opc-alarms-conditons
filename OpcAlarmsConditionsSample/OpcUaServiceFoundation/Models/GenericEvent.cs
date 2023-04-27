using Opc.UaFx;

namespace OpcUaService.Models;

public class GenericEvent : OpcEvent
{
	public GenericEvent(IOpcReadOnlyNodeDataStore dataStore)
		: base(dataStore)
	{
	}

	public Dictionary<string, object>? GetData()
	{
		var data = default(Dictionary<string, object>);

		if (DataStore is not OpcEventNodeView eventView) 
			return data;
		
		var operands = eventView.Filter.SelectClause;
		var fields = eventView.Data.Fields;

		data = new Dictionary<string, object>();

		for (var index = 0; index < operands.Count; index++) 
		{
			var operand = operands[index];
			var value = new OpcValue(fields[index].Value).Value;

			data.Add(operand.NodePath.ToString(), value);
		}

		return data;
	}
}