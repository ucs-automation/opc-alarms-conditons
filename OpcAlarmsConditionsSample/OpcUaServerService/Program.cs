// See https://aka.ms/new-console-template for more information

using Opc.UaFx.Server;
using OpcUaServerService;

var nodeManager = new SampleNodeManager();

// If the server domain name does not match localhost just replace it
// e.g. with the IP address or name of the server machine.
using var server = new OpcServer(
	"opc.tcp://localhost:4840/SampleServer",
	nodeManager);

server.Start();
//// NOTE: All AE specific code will be found in the SampleNodeManager.cs.

using var semaphore = new SemaphoreSlim(0);
var thread = new Thread(() => nodeManager.Simulate(semaphore));
thread.Start();

Console.WriteLine("OPC UA Server is running...");
Console.ReadKey(true);

semaphore.Release();
thread.Join();

server.Stop();