using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;
using CsvHelper;
using System.Globalization;

namespace OpcUaServer
{
    class Program
    {
        private static UaApplication? _application;
        private static UaTcpSessionChannel? _channel;
        private static NodeManager? _nodeManager;
        private static Timer? _updateTimer;
        private static List<CsvDataRow> _csvData = new();
        private static int _currentRowIndex = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine("OPC UA CSV Server Starting...");

            // Load CSV data
            string csvFilePath = "data.csv";
            if (args.Length > 0)
            {
                csvFilePath = args[0];
            }

            if (!File.Exists(csvFilePath))
            {
                Console.WriteLine($"CSV file not found: {csvFilePath}");
                Console.WriteLine("Creating sample CSV file...");
                CreateSampleCsvFile(csvFilePath);
            }

            LoadCsvData(csvFilePath);

            // Start OPC UA Server
            await StartOpcUaServer();

            Console.WriteLine("OPC UA Server is running. Press any key to exit.");
            Console.ReadKey();

            await StopOpcUaServer();
        }

        private static void CreateSampleCsvFile(string filePath)
        {
            var sampleData = new List<CsvDataRow>
            {
                new CsvDataRow { Timestamp = DateTime.Now, Temperature = 25.5, Humidity = 60.2, Pressure = 1013.25 },
                new CsvDataRow { Timestamp = DateTime.Now.AddSeconds(1), Temperature = 25.7, Humidity = 60.5, Pressure = 1013.30 },
                new CsvDataRow { Timestamp = DateTime.Now.AddSeconds(2), Temperature = 25.3, Humidity = 59.8, Pressure = 1013.20 },
                new CsvDataRow { Timestamp = DateTime.Now.AddSeconds(3), Temperature = 25.9, Humidity = 61.1, Pressure = 1013.35 },
                new CsvDataRow { Timestamp = DateTime.Now.AddSeconds(4), Temperature = 26.1, Humidity = 61.5, Pressure = 1013.40 }
            };

            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(sampleData);
        }

        private static void LoadCsvData(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            _csvData = csv.GetRecords<CsvDataRow>().ToList();
            Console.WriteLine($"Loaded {_csvData.Count} rows from CSV file.");
        }

        private static async Task StartOpcUaServer()
        {
            try
            {
                // Create application
                _application = new UaApplicationBuilder()
                    .SetApplicationUri($"urn:{System.Net.Dns.GetHostName()}:OpcUaCsvServer")
                    .SetProductUri("urn:OpcUaCsvServer")
                    .SetApplicationName("OPC UA CSV Server")
                    .SetApplicationType(ApplicationType.Server)
                    .SetDiscoveryUrls($"opc.tcp://{System.Net.Dns.GetHostName()}:4840/")
                    .SetServerCapabilities(new string[] { "DA" })
                    .Build();

                // Create node manager
                _nodeManager = new NodeManager(_application);

                // Start application
                await _application.StartAsync();

                // Start update timer to publish CSV data
                _updateTimer = new Timer(UpdateCsvData, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

                Console.WriteLine($"OPC UA Server started on opc.tcp://{System.Net.Dns.GetHostName()}:4840/");
                Console.WriteLine("Available nodes:");
                Console.WriteLine("  - Temperature (ns=2;s=Temperature)");
                Console.WriteLine("  - Humidity (ns=2;s=Humidity)");
                Console.WriteLine("  - Pressure (ns=2;s=Pressure)");
                Console.WriteLine("  - Timestamp (ns=2;s=Timestamp)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting OPC UA server: {ex.Message}");
                throw;
            }
        }

        private static async Task StopOpcUaServer()
        {
            _updateTimer?.Dispose();
            if (_application != null)
            {
                await _application.StopAsync();
            }
            Console.WriteLine("OPC UA Server stopped.");
        }

        private static void UpdateCsvData(object? state)
        {
            if (_csvData.Count == 0) return;

            var currentData = _csvData[_currentRowIndex];
            
            // Update node values
            _nodeManager?.UpdateNodeValue("Temperature", currentData.Temperature);
            _nodeManager?.UpdateNodeValue("Humidity", currentData.Humidity);
            _nodeManager?.UpdateNodeValue("Pressure", currentData.Pressure);
            _nodeManager?.UpdateNodeValue("Timestamp", currentData.Timestamp);

            Console.WriteLine($"Published: Temp={currentData.Temperature:F1}°C, Humidity={currentData.Humidity:F1}%, Pressure={currentData.Pressure:F2}hPa");

            // Move to next row (cycle through data)
            _currentRowIndex = (_currentRowIndex + 1) % _csvData.Count;
        }
    }

    public class CsvDataRow
    {
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
    }
}