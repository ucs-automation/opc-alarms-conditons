using dotenv.net.Utilities;

namespace OpcUaService;

public static class EnvVars
{
	public const string EndpointUrlEnvVar = "ENDPOINT_URL";
	public const string AlarmsEventNodeEnvVar = "ALARM_NODEID";

	public static bool Ensure()
	{
		var success = true;

		if (!EnvReader.HasValue(EndpointUrlEnvVar))
		{
			Console.WriteLine("Environment variable {0} is not set.", EndpointUrlEnvVar);
			success = false;
		}

		if (!EnvReader.HasValue(AlarmsEventNodeEnvVar))
		{
			Console.WriteLine("Environment variable {0} is not set.", AlarmsEventNodeEnvVar);
			success = false;
		}

		return success;
	}
}