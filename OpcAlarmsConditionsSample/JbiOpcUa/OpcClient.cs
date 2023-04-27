using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace JbiOpcUa;

public sealed class OpcClient : IDisposable
{
	/// <summary>
	/// Logger for the OpcClient
	/// </summary>
	private readonly ILogger _logger;

	/// <summary>
	/// The application configuration to use for the client.
	/// </summary>
	private readonly ApplicationConfiguration _configuration;
	
	/// <summary>
	/// Semaphore for synchronizing the session and reconnect handler.
	/// </summary>
	private readonly SemaphoreSlim _semaphore = new (1, 1);
	
	/// <summary>
	/// The session to use for the communication.
	/// </summary>
	private Session? _session;
	
	/// <summary>
	/// The reconnect handler to use for the session.
	/// </summary>
	private SessionReconnectHandler? _reconnectHandler;
	
	/// <summary>
	/// The user identity to use to connect to the server.
	/// </summary>
	public IUserIdentity UserIdentity { get; set; } = new UserIdentity();

	/// <summary>
	/// The interval in milliseconds between keep alive messages.
	/// </summary>
	public int KeepAliveInterval { get; set; } = 1000;

	/// <summary>
	/// When the server does not send a keep alive message within this time, the session will be closed.
	/// </summary>
	public uint SessionTimeout { get; set; } = 30_000;
	
	/// <summary>
	/// The period in milliseconds between reconnect attempts.
	/// </summary>
	public int ReconnectPeriod { get; set; } = 1_000;

	/// <summary>
	/// If true, the client will automatically accept the server certificate.
	/// </summary>
	public bool AutoAccept { get; set; } = false;

	/// <summary>
	/// If true, the client will check the domain of the server certificate.
	/// </summary>
	public bool CheckDomain { get; set; } = false;
	
	/// <summary>
	/// Constructor for building the OpcClient.
	/// </summary>
	/// <param name="logger">The logger used for logging the state</param>
	public OpcClient(ILogger logger)
	{
		_logger = logger;
		_configuration = new ApplicationConfiguration
		{
			ApplicationName = "OpcUaClient",
			ApplicationType = ApplicationType.Client,
			ApplicationUri = "UCS.OpcUaClient",
			ProductUri = "UCS.OpcUaClient",
			CertificateValidator = new CertificateValidator()
			{
				AutoAcceptUntrustedCertificates = AutoAccept,
			}
		};
	}

	public async Task<bool> ConnectAsync(string serverUrl, bool useSecurity = true)
	{
		if (string.IsNullOrWhiteSpace(serverUrl))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(serverUrl));
		
		if (_session is not null && _session.Connected)
			throw new InvalidOperationException("The session is already created and connected");

		try
		{
			var endpointDescription = CoreClientUtils.SelectEndpoint(_configuration, serverUrl, useSecurity);
			var endpointConfiguration = EndpointConfiguration.Create(_configuration);
			var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
			
			var session = await Session.Create(
				_configuration,
				endpoint,
				false,
				CheckDomain,
				_configuration.ApplicationName,
				SessionTimeout,
				UserIdentity,
				null 
			);

			if (_session is not null && _session.Connected)
			{
				_session = session;
				_session.KeepAliveInterval = KeepAliveInterval;
				_session.KeepAlive += SessionOnKeepAlive;

				return true;
			}
		}
		catch (Exception e)
		{
			_logger.LogCritical(e, "Error while connecting");
			throw;
		}

		return false;
	}
	
	public void Disconnect()
	{
		try
		{
			if (_session is null)
			{
				_logger.LogWarning("There is no session to disconnect from");
				return;
			}
			
			_logger.LogInformation("Disconnecting from server..");

			_semaphore.Wait();

			try
			{
				_session.KeepAlive -= SessionOnKeepAlive;
				_reconnectHandler?.Dispose();
				_reconnectHandler = null;
				_session.Close();
				_session.Dispose();
				_session = null;
				
				_logger.LogInformation( "Disconnected from server");
			}
			finally
			{
				_semaphore.Release();
			}
		}
		catch (Exception e)
		{
			_logger.LogCritical(e, "Error while disconnecting");
			throw;
		}
	}

	/// <summary>
	/// Callback that will be invoked, when the server sends a keep alive message.
	/// </summary>
	/// <param name="session">The session responsible for the Communication</param>
	/// <param name="e">The keep alive event</param>
	private void SessionOnKeepAlive(ISession session, KeepAliveEventArgs e)
	{
		// Seems to be a stale event
		if (!ReferenceEquals(session, _session))
			return;
		
		// The server is still alive
		if (ServiceResult.IsNotBad(e.Status))
			return;
		
		_semaphore.Wait();

		try
		{
			if (_reconnectHandler is not null)
			{
				_logger.LogWarning("Reconnect already in progress");
			}
			else
			{
				_reconnectHandler = new SessionReconnectHandler(true);
				_reconnectHandler.BeginReconnect(_session, ReconnectPeriod, SessionOnReconnected);	
			}
		}
		finally
		{
			_semaphore.Release();
		}
	}

	private void SessionOnReconnected(object? sender, EventArgs e)
	{
		// This callback seems to be stale
		if (!ReferenceEquals(sender, _reconnectHandler))
			return;

		_semaphore.Wait();

		try
		{
			if (_reconnectHandler?.Session is not null)
				_session = _reconnectHandler.Session as Session;
			
			_reconnectHandler?.Dispose();
			_reconnectHandler = null;
		}
		finally
		{
			_semaphore.Release();
		}
	}
	
	/// <summary>
	/// Handles the certificate validation event.
	/// This event is triggered every time an untrusted certificate is received from the server.
	/// </summary>
	protected void CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
	{
		var certificateAccepted = false;

		// TODO: Logic for accepting a certificate
		// ****
		// The certificate can be retrieved from the e.Certificate field
		// ***

		var error = e.Error;
		_logger.LogError("{Error}", error);
		
		certificateAccepted = error.StatusCode == StatusCodes.BadCertificateUntrusted && AutoAccept;

		if (certificateAccepted)
		{
			_logger.LogInformation("Untrusted Certificate accepted. Subject = {0}", e.Certificate.Subject);
			e.Accept = true;
		}
		else
		{
			_logger.LogWarning("Untrusted Certificate rejected. Subject = {0}", e.Certificate.Subject);
		}
	}

#region Subscriptions

	public async Task SubscribeEvent()
	{
		if (_session is null || !_session.Connected)
			throw new InvalidOperationException("The session is not connected");

		Subscription subscription = new()
		{
			DisplayName = null,
			PublishingInterval = 1000,
			KeepAliveCount = 10,
			LifetimeCount = 100,
			MaxNotificationsPerPublish = 1000,
			PublishingEnabled = true,
			TimestampsToReturn = TimestampsToReturn.Both
		};
		
		_session.AddSubscription(subscription);
		await subscription.CreateAsync();

		Dictionary<NodeId, NodeId> eventTypeMappings = new();
		FilterDefinition 

	}

#endregion
	
	/// <inheritdoc />
	public void Dispose()
	{
		Disconnect();
		
		_semaphore.Dispose();
		_session?.Dispose();
		_reconnectHandler?.Dispose();
	}
}