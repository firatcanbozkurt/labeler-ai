using System;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace OpcUaCsvServer
{
    public class TestClient
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("OPC UA CSV Test Client");
            Console.WriteLine("Connecting to server...");

            try
            {
                // Create the client
                var client = new UaTcpSessionChannel(
                    new ApplicationDescription
                    {
                        ApplicationName = "CSV Test Client",
                        ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:csv-test-client",
                        ApplicationType = ApplicationType.Client
                    },
                    new DirectoryStore(Path.Combine(Environment.CurrentDirectory, "pki")),
                    new AnonymousIdentity(),
                    "opc.tcp://localhost:4840/");

                // Connect to the server
                await client.OpenAsync();

                Console.WriteLine("Connected to OPC UA server!");

                // Read variables
                await ReadVariables(client);

                // Subscribe to changes
                await SubscribeToChanges(client);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

                // Disconnect
                await client.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task ReadVariables(UaTcpSessionChannel client)
        {
            Console.WriteLine("\n--- Reading Current Values ---");

            try
            {
                // Define the node IDs for our variables
                var temperatureNodeId = new NodeId("temperature", 2);
                var humidityNodeId = new NodeId("humidity", 2);
                var pressureNodeId = new NodeId("pressure", 2);
                var timestampNodeId = new NodeId("timestamp", 2);

                // Read values
                var readRequest = new ReadRequest
                {
                    NodesToRead = new[]
                    {
                        new ReadValueId { NodeId = temperatureNodeId, AttributeId = AttributeIds.Value },
                        new ReadValueId { NodeId = humidityNodeId, AttributeId = AttributeIds.Value },
                        new ReadValueId { NodeId = pressureNodeId, AttributeId = AttributeIds.Value },
                        new ReadValueId { NodeId = timestampNodeId, AttributeId = AttributeIds.Value }
                    }
                };

                var readResponse = await client.ReadAsync(readRequest);

                if (readResponse.Results != null)
                {
                    Console.WriteLine($"Temperature: {readResponse.Results[0].Value}°C");
                    Console.WriteLine($"Humidity: {readResponse.Results[1].Value}%");
                    Console.WriteLine($"Pressure: {readResponse.Results[2].Value}hPa");
                    Console.WriteLine($"Timestamp: {readResponse.Results[3].Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading variables: {ex.Message}");
            }
        }

        private static async Task SubscribeToChanges(UaTcpSessionChannel client)
        {
            Console.WriteLine("\n--- Subscribing to Changes ---");

            try
            {
                // Create a subscription
                var subscriptionRequest = new CreateSubscriptionRequest
                {
                    RequestedPublishingInterval = 1000,
                    RequestedLifetimeCount = 1000,
                    RequestedMaxKeepAliveCount = 10,
                    MaxNotificationsPerPublish = 1000,
                    PublishingEnabled = true,
                    Priority = 0
                };

                var subscriptionResponse = await client.CreateSubscriptionAsync(subscriptionRequest);
                var subscriptionId = subscriptionResponse.SubscriptionId;

                Console.WriteLine($"Subscription created with ID: {subscriptionId}");

                // Add monitored items
                var temperatureNodeId = new NodeId("temperature", 2);
                var humidityNodeId = new NodeId("humidity", 2);

                var monitoredItemsRequest = new CreateMonitoredItemsRequest
                {
                    SubscriptionId = subscriptionId,
                    ItemsToCreate = new[]
                    {
                        new MonitoredItemCreateRequest
                        {
                            ItemToMonitor = new ReadValueId { NodeId = temperatureNodeId, AttributeId = AttributeIds.Value },
                            MonitoringMode = MonitoringMode.Reporting,
                            RequestedParameters = new MonitoringParameters
                            {
                                ClientHandle = 1,
                                SamplingInterval = 1000,
                                Filter = null,
                                QueueSize = 1,
                                DiscardOldest = true
                            }
                        },
                        new MonitoredItemCreateRequest
                        {
                            ItemToMonitor = new ReadValueId { NodeId = humidityNodeId, AttributeId = AttributeIds.Value },
                            MonitoringMode = MonitoringMode.Reporting,
                            RequestedParameters = new MonitoringParameters
                            {
                                ClientHandle = 2,
                                SamplingInterval = 1000,
                                Filter = null,
                                QueueSize = 1,
                                DiscardOldest = true
                            }
                        }
                    }
                };

                var monitoredItemsResponse = await client.CreateMonitoredItemsAsync(monitoredItemsRequest);
                Console.WriteLine($"Created {monitoredItemsResponse.Results?.Length} monitored items");

                // Set up notification handler
                client.NotificationReceived += (sender, e) =>
                {
                    if (e.NotificationMessage?.NotificationData != null)
                    {
                        foreach (var notification in e.NotificationMessage.NotificationData)
                        {
                            if (notification is DataChangeNotification dataChange)
                            {
                                foreach (var item in dataChange.MonitoredItems)
                                {
                                    var value = item.Value?.Value;
                                    var timestamp = item.Value?.SourceTimestamp;
                                    
                                    switch (item.ClientHandle)
                                    {
                                        case 1:
                                            Console.WriteLine($"[{timestamp}] Temperature changed to: {value}°C");
                                            break;
                                        case 2:
                                            Console.WriteLine($"[{timestamp}] Humidity changed to: {value}%");
                                            break;
                                    }
                                }
                            }
                        }
                    }
                };

                Console.WriteLine("Monitoring for changes... (Press any key to stop)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up subscription: {ex.Message}");
            }
        }
    }
}