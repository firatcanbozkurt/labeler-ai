using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace OpcUaServer
{
    public class CsvOpcUaServer
    {
        private UaApplication? _application;
        private UaServer? _server;
        private readonly string _csvFilePath;
        private readonly Timer _updateTimer;
        private readonly Dictionary<string, object> _nodeValues = new();
        private readonly object _lockObject = new();

        public CsvOpcUaServer(string csvFilePath = "data.csv")
        {
            _csvFilePath = csvFilePath;
            _updateTimer = new Timer(UpdateCsvData, null, Timeout.Infinite, Timeout.Infinite);
        }

        public async Task StartAsync()
        {
            // Create the OPC UA application
            _application = new UaApplicationBuilder()
                .SetApplicationUri($"urn:{Dns.GetHostName()}:CSVServer")
                .SetProductUri("urn:CSVServer")
                .SetApplicationName("CSV Data Server")
                .SetApplicationType(ApplicationType.Server)
                .SetDiscoveryUrls($"opc.tcp://{Dns.GetHostName()}:4840/")
                .SetSecurityPolicies(new SecurityPolicy[] { SecurityPolicy.None })
                .SetUserTokenPolicies(new UserTokenPolicy[] { new UserTokenPolicy() })
                .Build();

            // Create the server
            _server = new UaServer(_application);

            // Add CSV data nodes
            await AddCsvDataNodes();

            // Start the server
            await _server.StartAsync();

            // Start periodic CSV updates
            _updateTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public async Task StopAsync()
        {
            _updateTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _updateTimer?.Dispose();
            
            if (_server != null)
            {
                await _server.StopAsync();
            }
            
            _application?.Dispose();
        }

        private async Task AddCsvDataNodes()
        {
            if (_server == null) return;

            // Create a folder for CSV data
            var csvFolder = new FolderState(_server)
            {
                NodeId = new NodeId("CSVData", 2),
                BrowseName = new QualifiedName("CSVData", 2),
                DisplayName = new LocalizedText("CSV Data"),
                Description = new LocalizedText("CSV file data nodes")
            };

            await _server.AddNodeAsync(csvFolder);

            // Add some sample nodes (these will be updated with CSV data)
            var nodes = new[]
            {
                new VariableState(_server)
                {
                    NodeId = new NodeId("Temperature", 2),
                    BrowseName = new QualifiedName("Temperature", 2),
                    DisplayName = new LocalizedText("Temperature"),
                    DataType = new NodeId(DataTypeIds.Double),
                    ValueRank = ValueRanks.Scalar,
                    AccessLevel = AccessLevels.CurrentRead,
                    UserAccessLevel = AccessLevels.CurrentRead,
                    Value = 0.0
                },
                new VariableState(_server)
                {
                    NodeId = new NodeId("Pressure", 2),
                    BrowseName = new QualifiedName("Pressure", 2),
                    DisplayName = new LocalizedText("Pressure"),
                    DataType = new NodeId(DataTypeIds.Double),
                    ValueRank = ValueRanks.Scalar,
                    AccessLevel = AccessLevels.CurrentRead,
                    UserAccessLevel = AccessLevels.CurrentRead,
                    Value = 0.0
                },
                new VariableState(_server)
                {
                    NodeId = new NodeId("Humidity", 2),
                    BrowseName = new QualifiedName("Humidity", 2),
                    DisplayName = new LocalizedText("Humidity"),
                    DataType = new NodeId(DataTypeIds.Double),
                    ValueRank = ValueRanks.Scalar,
                    AccessLevel = AccessLevels.CurrentRead,
                    UserAccessLevel = AccessLevels.CurrentRead,
                    Value = 0.0
                }
            };

            foreach (var node in nodes)
            {
                await _server.AddNodeAsync(node);
                await _server.AddReferenceAsync(csvFolder.NodeId, ReferenceTypeIds.Organizes, node.NodeId);
                _nodeValues[node.BrowseName.Name] = node;
            }
        }

        private async void UpdateCsvData(object? state)
        {
            try
            {
                if (!File.Exists(_csvFilePath))
                {
                    Console.WriteLine($"CSV file not found: {_csvFilePath}");
                    return;
                }

                using var reader = new StreamReader(_csvFilePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                // Read the CSV file
                var records = csv.GetRecords<dynamic>().ToList();
                
                if (records.Count == 0)
                {
                    Console.WriteLine("No data found in CSV file");
                    return;
                }

                // Get the latest record (assuming CSV has timestamp column)
                var latestRecord = records.Last();
                var recordDict = (IDictionary<string, object>)latestRecord;

                lock (_lockObject)
                {
                    foreach (var kvp in recordDict)
                    {
                        if (_nodeValues.TryGetValue(kvp.Key, out var node))
                        {
                            if (node is VariableState variable)
                            {
                                // Try to convert the value to double
                                if (double.TryParse(kvp.Value.ToString(), out double doubleValue))
                                {
                                    variable.Value = doubleValue;
                                    Console.WriteLine($"Updated {kvp.Key}: {doubleValue}");
                                }
                                else
                                {
                                    variable.Value = kvp.Value.ToString();
                                    Console.WriteLine($"Updated {kvp.Key}: {kvp.Value}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating CSV data: {ex.Message}");
            }
        }
    }
}