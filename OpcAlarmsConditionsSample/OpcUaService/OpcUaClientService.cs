using dotenv.net.Utilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opc.UaFx;
using Opc.UaFx.Client;
using OpcUaService.Models;

namespace OpcUaService;

public sealed class OpcUaClientService : IHostedService, IDisposable
{
	/// <summary>
	/// Used to log messages
	/// </summary>
	private readonly ILogger<OpcUaClientService> _logger;
	
	/// <summary>
	/// Subscriptions for alarms and events
	/// </summary>
	private OpcSubscription? _eventSubscriptions;

	/// <summary>
	/// Subscriptions for nodes
	/// </summary>
	private OpcSubscription? _nodeSubscriptions;

	/// <summary>
	/// Client that is connected to the OPC UA server
	/// </summary>
	private OpcClient? _opcClient;

	/// <summary>
	/// Flag to detect redundant calls
	/// </summary>
	private bool _disposed;
	
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="logger">Logger that will be used for logging</param>
	public OpcUaClientService(ILogger<OpcUaClientService> logger)
	{
		_logger = logger;
	}

	/// <inheritdoc />
	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Opc service wird gestartet..");

		_opcClient = new OpcClient(EnvReader.GetStringValue(EnvVars.EndpointUrlEnvVar), new OpcSecurityPolicy(OpcSecurityMode.None));
		_opcClient.UseDynamic = true;
		
		_opcClient.Connected += OpcClientOnConnected;
		_opcClient.Disconnected += OpcClientOnDisconnected;
		_opcClient.Reconnected += OpcClientOnReconnected;
		
		_opcClient.Connect();
		
		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public Task StopAsync(CancellationToken cancellationToken)
	{
		Dispose();
		
		return Task.CompletedTask;
	}

#region Callbacks
	
	private void OpcClientOnConnected(object? sender, EventArgs e)
	{
		_logger.LogInformation("Verbindung zum OPC UA Server wurde hergestellt.");
		
		// Build the filter for the events
		var filter = OpcFilter.Using(_opcClient)
			.FromEvents(
				OpcEventTypes.AlarmCondition,
				OpcEventTypes.ExclusiveLimitAlarm,
				OpcEventTypes.DialogCondition)
			.Where(
				OpcFilterOperand.OfType(OpcEventTypes.AlarmCondition) |
				OpcFilterOperand.OfType(OpcEventTypes.AlarmCondition) |
				OpcFilterOperand.OfType(OpcEventTypes.AlarmCondition)).Select();

		// Subscribe at the server for events
		if (OpcNodeId.TryParse(EnvReader.GetStringValue(EnvVars.AlarmsEventNodeEnvVar), out var nodeId))
		{
			if (!OpcNodeId.IsNullOrEmpty(nodeId))
			{
				//_eventSubscriptions = _opcClient?.SubscribeEvent(nodeId, filter, OnOpcEventReceived);
				_eventSubscriptions = _opcClient?.SubscribeEvent(nodeId, OnOpcEventReceived);
				
				_eventSubscriptions?.RefreshConditions();

				_logger.LogInformation("Event subscription wurde erstellt.");	
			}
			else
			{
				_logger.LogError("Es wurde keine gültige NodeId für die Events angegeben.");
			}
		}
		else
		{
			_logger.LogError("Es wurde keine gültige NodeId für die Events angegeben.");
		}
	}
	
	private void OpcClientOnDisconnected(object? sender, EventArgs e)
	{
		_logger.LogInformation("Verbindung zum OPC UA Server wurde getrennt.");
	}
	
	private void OpcClientOnReconnected(object? sender, EventArgs e)
	{
		_logger.LogInformation("Verbindung zum OPC UA Server wurde wiederhergestellt."); 
	}
	
#endregion
	
	private void OnOpcEventReceived(object sender, OpcEventReceivedEventArgs e)
	{
		var alarmEvent = e.Event;

		switch (alarmEvent)
		{
			case GenericEvent genericEvent:
				Print(genericEvent.GetData());
				break;
			case OpcAlarmCondition opcCondition:
				_logger.LogWarning("Nachricht: {Nachricht}", opcCondition.Message);
				_logger.LogWarning("Quittiert: {Quittiert}, Steht an: {StehtAn}", opcCondition.IsAcked, opcCondition.IsActive);
				break;
		}
	}
	
	private static void Print(Dictionary<string, object>? data, int level = 0)
	{
		if (data == null) 
			throw new ArgumentNullException(nameof(data));
		
		var indent = new string('.', level * 2);

		foreach (var item in data)
		{
			switch (item.Value)
			{
				case OpcDataObject instance:
				{
					Console.WriteLine(indent + item.Key);

					var instanceData = instance.GetFields().ToDictionary(
						field => field.Name,
						field => field.Value);

					Print(instanceData, level + 1);
					break;
				}
				case OpcDataObject[] instances:
				{
					for (var index = 0; index < instances.Length; index++) 
					{
						Console.WriteLine(indent + "{0}[{1}]", item.Key, index);

						var instanceData = instances[index].GetFields().ToDictionary(
							field => field.Name,
							field => field.Value);

						Print(instanceData, level + 1);
					}

					break;
				}
				default:
					Console.WriteLine(indent + "{0} = {1}", item.Key, item.Value);
					break;
			}
		}
	}
	
	/// <inheritdoc />
	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;
		
		_eventSubscriptions?.Unsubscribe();
		_eventSubscriptions = null;
		
		_nodeSubscriptions?.Unsubscribe();
		_nodeSubscriptions = null;

		if (_opcClient != null)
		{
			_opcClient.Connected -= OpcClientOnConnected;

			_opcClient?.Disconnect();
			_opcClient?.Dispose();
		}

		_opcClient = null;
	}
}