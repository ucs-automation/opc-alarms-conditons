using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.UaFx.Client;
using OpcClient = JbiOpcUa.OpcClient;

namespace OpcUaService;

public class OpcUaService : BackgroundService
{
    private readonly ILogger<OpcUaService> _logger;
    private readonly OpcClient _opcClient;

    public OpcUaService(ILogger<OpcUaService> logger)
    {
        _logger = logger;
        _opcClient = new OpcClient(_logger);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using OpcClient opcClient = new (_logger)
        {
            AutoAccept = true,
            UserIdentity = new UserIdentity(new AnonymousIdentityToken())
        };
        
        await opcClient.ConnectAsync(EnvVars.EndpointUrlEnvVar, false);
        
        
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        
        _opcClient.Disconnect();
    }
}