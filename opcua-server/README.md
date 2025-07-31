# OPC UA CSV Server

This C# application acts as an OPC UA server that reads data from a CSV file and publishes it to OPC UA clients.

## Features

- Reads data from CSV files
- Publishes CSV data as OPC UA variables
- Updates data every 5 seconds
- Supports multiple data types (numeric and string)
- Runs on port 4840 (default OPC UA port)

## Prerequisites

- .NET 6.0 or later
- A CSV file with your data

## Installation

1. Install .NET 6.0 SDK
2. Navigate to the project directory
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

## Usage

1. Place your CSV file in the project directory (default: `data.csv`)
2. Run the server:
   ```bash
   dotnet run
   ```
3. Connect your OPC UA client to: `opc.tcp://localhost:4840`

## CSV Format

The CSV file should have headers that match the OPC UA node names. Example:

```csv
Timestamp,Temperature,Pressure,Humidity
2024-01-01T10:00:00,25.5,1013.25,60.2
2024-01-01T10:01:00,25.7,1013.30,60.5
```

## OPC UA Nodes

The server creates the following nodes under the "CSVData" folder:
- Temperature (Double)
- Pressure (Double) 
- Humidity (Double)

## Configuration

You can modify the CSV file path by changing the constructor parameter in `CsvOpcUaServer.cs`:

```csharp
var server = new CsvOpcUaServer("your-data.csv");
```

## Troubleshooting

- Make sure the CSV file exists and is readable
- Check that the CSV headers match the expected node names
- Verify that .NET 6.0 is installed
- Ensure port 4840 is not in use by another application