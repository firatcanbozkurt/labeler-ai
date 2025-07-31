# OPC UA CSV Server

This C# application acts as an OPC UA server that reads data from a CSV file and publishes it to OPC UA clients.

## Features

- Reads sensor data from CSV files (Temperature, Humidity, Pressure, Timestamp)
- Publishes data as OPC UA variables
- Cycles through CSV data rows automatically
- Supports custom CSV file paths
- Creates sample data if no CSV file is provided

## Prerequisites

- .NET 6.0 or later
- Visual Studio 2022 or VS Code with C# extension

## Installation Options

### Option 1: Install .NET 6.0 and run locally

1. Install .NET 6.0:
   ```bash
   # On Ubuntu/Debian
   ./install_dotnet.sh
   
   # On Windows
   # Download from https://dotnet.microsoft.com/download/dotnet/6.0
   
   # On macOS
   brew install dotnet
   ```

2. Navigate to the server directory:
   ```bash
   cd server
   ```

3. Build and run:
   ```bash
   ./build.sh
   dotnet run
   ```

### Option 2: Run with Docker

1. Build and run with Docker Compose:
   ```bash
   docker-compose up --build
   ```

2. Or build and run manually:
   ```bash
   docker build -t opcua-csv-server .
   docker run -p 4840:4840 -v $(pwd)/sample_data.csv:/app/data.csv opcua-csv-server
   ```

## Quick Start

1. **Install .NET 6.0** (if not already installed):
   ```bash
   ./install_dotnet.sh
   ```

2. **Build and run the server**:
   ```bash
   ./build.sh
   dotnet run
   ```

3. **Test with the included client** (in another terminal):
   ```bash
   dotnet run --project OpcUaClient.cs
   ```

## Usage

### Running with default CSV file
```bash
dotnet run
```

### Running with custom CSV file
```bash
dotnet run path/to/your/data.csv
```

### Running with sample data
```bash
dotnet run sample_data.csv
```

## CSV File Format

The CSV file should have the following columns:
- `Timestamp`: DateTime in ISO format (e.g., 2024-01-01T10:00:00)
- `Temperature`: Double value in Celsius
- `Humidity`: Double value as percentage
- `Pressure`: Double value in hPa

Example:
```csv
Timestamp,Temperature,Humidity,Pressure
2024-01-01T10:00:00,25.5,60.2,1013.25
2024-01-01T10:01:00,25.7,60.5,1013.30
```

## OPC UA Server Details

- **Endpoint**: `opc.tcp://localhost:4840/`
- **Namespace**: `http://opcuacsvserver.com`
- **Update Rate**: 1 second
- **Available Nodes**:
  - `ns=2;s=Temperature` - Temperature value in Celsius
  - `ns=2;s=Humidity` - Humidity value as percentage
  - `ns=2;s=Pressure` - Pressure value in hPa
  - `ns=2;s=Timestamp` - Timestamp of the data

## Connecting with OPC UA Clients

You can connect to this server using any OPC UA client, such as:
- UaExpert
- Prosys OPC UA Browser
- Custom OPC UA client applications

### Connection Parameters:
- **Server URL**: `opc.tcp://localhost:4840/`
- **Security Mode**: None (for testing)
- **Security Policy**: None (for testing)

## Troubleshooting

1. **Port already in use**: Make sure port 4840 is not being used by another application
2. **CSV file not found**: The application will create a sample CSV file if none is provided
3. **Connection issues**: Check firewall settings and ensure the port is accessible

## Dependencies

- `Workstation.UaServer` - OPC UA server implementation
- `Workstation.UaClient` - OPC UA client implementation
- `CsvHelper` - CSV file parsing library

## License

This project is provided as-is for educational and testing purposes.