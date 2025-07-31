using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CsvHelper;
using System.Globalization;

namespace OpcUaCsvServer
{
    public class SampleDataGenerator
    {
        private static readonly Random _random = new Random();
        private static string _csvFilePath = "data.csv";

        static void Main(string[] args)
        {
            Console.WriteLine("CSV Sample Data Generator");
            Console.WriteLine("Generating sample data for OPC UA server...");

            if (args.Length > 0)
            {
                _csvFilePath = args[0];
            }

            // Generate initial data
            GenerateInitialData();

            Console.WriteLine($"Sample data generated in: {_csvFilePath}");
            Console.WriteLine("Press any key to start continuous data generation (Ctrl+C to stop)...");
            Console.ReadKey();

            // Start continuous data generation
            GenerateContinuousData();
        }

        private static void GenerateInitialData()
        {
            var records = new List<CsvRecord>();
            var baseTime = DateTime.Now.AddMinutes(-10);

            for (int i = 0; i < 10; i++)
            {
                records.Add(new CsvRecord
                {
                    Timestamp = baseTime.AddSeconds(i * 60),
                    Temperature = GenerateTemperature(),
                    Humidity = GenerateHumidity(),
                    Pressure = GeneratePressure()
                });
            }

            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(records);
        }

        private static void GenerateContinuousData()
        {
            try
            {
                while (true)
                {
                    var newRecord = new CsvRecord
                    {
                        Timestamp = DateTime.Now,
                        Temperature = GenerateTemperature(),
                        Humidity = GenerateHumidity(),
                        Pressure = GeneratePressure()
                    };

                    // Append to CSV file
                    using var writer = new StreamWriter(_csvFilePath, true);
                    using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                    csv.WriteRecord(newRecord);
                    csv.NextRecord();

                    Console.WriteLine($"Generated: {newRecord.Timestamp:HH:mm:ss} - Temp: {newRecord.Temperature:F1}°C, Humidity: {newRecord.Humidity:F1}%, Pressure: {newRecord.Pressure:F2}hPa");

                    Thread.Sleep(5000); // Wait 5 seconds
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating data: {ex.Message}");
            }
        }

        private static double GenerateTemperature()
        {
            // Generate temperature between 20-30°C with some variation
            return 25.0 + (_random.NextDouble() - 0.5) * 10.0;
        }

        private static double GenerateHumidity()
        {
            // Generate humidity between 50-70% with some variation
            return 60.0 + (_random.NextDouble() - 0.5) * 20.0;
        }

        private static double GeneratePressure()
        {
            // Generate pressure between 1010-1020 hPa with some variation
            return 1015.0 + (_random.NextDouble() - 0.5) * 10.0;
        }
    }
}