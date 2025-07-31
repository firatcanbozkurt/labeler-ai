using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;
using CsvHelper;
using System.Globalization;

namespace OpcUaCsvServer
{
    class Program
    {
        private static UaApplication? _application;
        private static CsvDataNodeManager? _nodeManager;
        private static Timer? _updateTimer;
        private static string _csvFilePath = "data.csv";

        static async Task Main(string[] args)
        {
            Console.WriteLine("OPC UA CSV Server Starting...");

            // Parse command line arguments
            if (args.Length > 0)
            {
                _csvFilePath = args[0];
            }

            if (!File.Exists(_csvFilePath))
            {
                Console.WriteLine($"CSV file not found: {_csvFilePath}");
                Console.WriteLine("Creating sample CSV file...");
                CreateSampleCsvFile();
            }

            try
            {
                await StartServer();
                
                Console.WriteLine("OPC UA Server is running. Press any key to exit.");
                Console.ReadKey();
                
                await StopServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void CreateSampleCsvFile()
        {
            var sampleData = new List<CsvRecord>
            {
                new CsvRecord { Timestamp = DateTime.Now, Temperature = 25.5, Humidity = 60.2, Pressure = 1013.25 },
                new CsvRecord { Timestamp = DateTime.Now.AddSeconds(1), Temperature = 25.7, Humidity = 60.5, Pressure = 1013.30 },
                new CsvRecord { Timestamp = DateTime.Now.AddSeconds(2), Temperature = 25.3, Humidity = 59.8, Pressure = 1013.20 }
            };

            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(sampleData);
            
            Console.WriteLine($"Sample CSV file created: {_csvFilePath}");
        }

        private static async Task StartServer()
        {
            // Create the node manager
            _nodeManager = new CsvDataNodeManager(_csvFilePath);

            // Create the application
            _application = new UaApplicationBuilder()
                .SetApplicationUri($"urn:{System.Net.Dns.GetHostName()}:csv-server")
                .SetProductUri("urn:csv-server")
                .SetApplicationName("CSV Data Server")
                .SetApplicationType(ApplicationType.Server)
                .SetDirectoryStore(Path.Combine(Environment.CurrentDirectory, "pki"))
                .SetIdentity(new AnonymousIdentity())
                .AddMappedEndpoint("csv-server", "opc.tcp://localhost:4840/", SecurityPolicyUris.Basic256Sha256)
                .AddNodeManager(_nodeManager)
                .Build();

            // Start the application
            await _application.RunAsync();

            // Start the update timer
            _updateTimer = new Timer(UpdateCsvData, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            Console.WriteLine("OPC UA Server started successfully!");
            Console.WriteLine($"Endpoint: opc.tcp://localhost:4840/");
            Console.WriteLine($"CSV File: {_csvFilePath}");
        }

        private static async Task StopServer()
        {
            _updateTimer?.Dispose();
            if (_application != null)
            {
                await _application.DisposeAsync();
            }
            Console.WriteLine("OPC UA Server stopped.");
        }

        private static void UpdateCsvData(object? state)
        {
            try
            {
                _nodeManager?.UpdateCsvData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating CSV data: {ex.Message}");
            }
        }
    }

    public class CsvRecord
    {
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
    }
}