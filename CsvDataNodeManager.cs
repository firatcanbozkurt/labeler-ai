using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;
using CsvHelper;
using System.Globalization;

namespace OpcUaCsvServer
{
    public class CsvDataNodeManager : NodeManager
    {
        private readonly string _csvFilePath;
        private readonly Dictionary<string, DataVariableNode> _dataNodes;
        private readonly FolderNode _csvFolder;
        private CsvRecord? _lastRecord;

        public CsvDataNodeManager(string csvFilePath) : base("http://csv-server")
        {
            _csvFilePath = csvFilePath;
            _dataNodes = new Dictionary<string, DataVariableNode>();

            // Create the CSV data folder
            _csvFolder = new FolderNode(this)
            {
                NodeId = new NodeId("csv-data", NamespaceIndex),
                BrowseName = new QualifiedName("CSVData", NamespaceIndex),
                DisplayName = new LocalizedText("CSV Data"),
                Description = new LocalizedText("CSV file data published as OPC UA variables")
            };

            // Create data variables
            CreateDataVariables();

            // Initial data load
            UpdateCsvData();
        }

        private void CreateDataVariables()
        {
            // Temperature variable
            var temperatureNode = new DataVariableNode(this)
            {
                NodeId = new NodeId("temperature", NamespaceIndex),
                BrowseName = new QualifiedName("Temperature", NamespaceIndex),
                DisplayName = new LocalizedText("Temperature"),
                Description = new LocalizedText("Temperature value from CSV"),
                DataType = new NodeId(DataTypeIds.Double),
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                MinimumSamplingInterval = 1000
            };

            // Humidity variable
            var humidityNode = new DataVariableNode(this)
            {
                NodeId = new NodeId("humidity", NamespaceIndex),
                BrowseName = new QualifiedName("Humidity", NamespaceIndex),
                DisplayName = new LocalizedText("Humidity"),
                Description = new LocalizedText("Humidity value from CSV"),
                DataType = new NodeId(DataTypeIds.Double),
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                MinimumSamplingInterval = 1000
            };

            // Pressure variable
            var pressureNode = new DataVariableNode(this)
            {
                NodeId = new NodeId("pressure", NamespaceIndex),
                BrowseName = new QualifiedName("Pressure", NamespaceIndex),
                DisplayName = new LocalizedText("Pressure"),
                Description = new LocalizedText("Pressure value from CSV"),
                DataType = new NodeId(DataTypeIds.Double),
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                MinimumSamplingInterval = 1000
            };

            // Timestamp variable
            var timestampNode = new DataVariableNode(this)
            {
                NodeId = new NodeId("timestamp", NamespaceIndex),
                BrowseName = new QualifiedName("Timestamp", NamespaceIndex),
                DisplayName = new LocalizedText("Timestamp"),
                Description = new LocalizedText("Timestamp from CSV"),
                DataType = new NodeId(DataTypeIds.DateTime),
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                MinimumSamplingInterval = 1000
            };

            // Add nodes to the folder
            _csvFolder.AddChild(temperatureNode);
            _csvFolder.AddChild(humidityNode);
            _csvFolder.AddChild(pressureNode);
            _csvFolder.AddChild(timestampNode);

            // Store references
            _dataNodes["temperature"] = temperatureNode;
            _dataNodes["humidity"] = humidityNode;
            _dataNodes["pressure"] = pressureNode;
            _dataNodes["timestamp"] = timestampNode;

            // Add the folder to the server
            AddNode(_csvFolder);
        }

        public void UpdateCsvData()
        {
            try
            {
                if (!File.Exists(_csvFilePath))
                {
                    Console.WriteLine($"CSV file not found: {_csvFilePath}");
                    return;
                }

                // Read the last line from CSV file
                var lastLine = File.ReadLines(_csvFilePath).LastOrDefault();
                if (string.IsNullOrEmpty(lastLine))
                {
                    Console.WriteLine("CSV file is empty");
                    return;
                }

                // Parse the CSV line
                using var reader = new StringReader(lastLine);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Read();
                var record = csv.GetRecord<CsvRecord>();

                if (record != null && record != _lastRecord)
                {
                    _lastRecord = record;

                    // Update OPC UA variables
                    UpdateVariable("temperature", record.Temperature);
                    UpdateVariable("humidity", record.Humidity);
                    UpdateVariable("pressure", record.Pressure);
                    UpdateVariable("timestamp", record.Timestamp);

                    Console.WriteLine($"Updated CSV data: Temp={record.Temperature}°C, Humidity={record.Humidity}%, Pressure={record.Pressure}hPa, Time={record.Timestamp}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }
        }

        private void UpdateVariable(string variableName, object value)
        {
            if (_dataNodes.TryGetValue(variableName, out var node))
            {
                node.Value = new DataValue
                {
                    Value = value,
                    SourceTimestamp = DateTime.UtcNow,
                    ServerTimestamp = DateTime.UtcNow
                };
            }
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            base.CreateAddressSpace(externalReferences);

            // Add the CSV folder to the Objects folder
            var objectsFolder = FindNodeInAddressSpace(ObjectIds.ObjectsFolder) as FolderNode;
            objectsFolder?.AddChild(_csvFolder);
        }
    }
}