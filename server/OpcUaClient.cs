using System;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace OpcUaClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("OPC UA Client Starting...");

            var endpointUrl = "opc.tcp://localhost:4840/";
            if (args.Length > 0)
            {
                endpointUrl = args[0];
            }

            try
            {
                using var channel = new UaTcpSessionChannel(
                    new ApplicationDescription
                    {
                        ApplicationName = "OPC UA CSV Client",
                        ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:OpcUaCsvClient",
                        ApplicationType = ApplicationType.Client
                    },
                    null,
                    endpointUrl,
                    SecurityPolicyUris.None);

                await channel.OpenAsync();

                Console.WriteLine($"Connected to server: {endpointUrl}");

                // Read values from the server
                var nodeIds = new[]
                {
                    new NodeId("Temperature", 2),
                    new NodeId("Humidity", 2),
                    new NodeId("Pressure", 2),
                    new NodeId("Timestamp", 2)
                };

                while (true)
                {
                    try
                    {
                        var response = await channel.ReadAsync(nodeIds);
                        
                        Console.WriteLine("\n--- Current Values ---");
                        Console.WriteLine($"Temperature: {response[0].Value}°C");
                        Console.WriteLine($"Humidity: {response[1].Value}%");
                        Console.WriteLine($"Pressure: {response[2].Value}hPa");
                        Console.WriteLine($"Timestamp: {response[3].Value}");
                        Console.WriteLine($"Source Time: {response[0].SourceTimestamp}");
                        Console.WriteLine("------------------------");

                        await Task.Delay(2000); // Read every 2 seconds
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading values: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to server: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}