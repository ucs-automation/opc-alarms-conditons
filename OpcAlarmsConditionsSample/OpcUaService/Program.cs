using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpcUaService;

// read .env file
DotEnv.Fluent()
	.WithoutExceptions()
	.WithEnvFiles()
	.WithTrimValues()
	.WithDefaultEncoding()
	.Load();

if (!EnvVars.Ensure())
{
	Console.WriteLine("Environment variables are not set correctly. Shutting down..");
	return;
}

// Host builder
IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
		services.AddLogging(options =>
		{
			options.AddSimpleConsole(configure =>
			{
				configure.TimestampFormat = "[HH:mm:ss] ";
				configure.SingleLine = true;
			});
		});
		
		services.AddHostedService<OpcUaClientService>();
	})
	.Build();

host.Run();