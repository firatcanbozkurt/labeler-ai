# OPC UA CSV Server

A C# OPC UA server that reads data from a CSV file and publishes it as OPC UA variables for clients to consume.

## Features

- **OPC UA Server**: Implements a full OPC UA server using the Workstation.UaServer library
- **CSV Data Source**: Reads data from CSV files and publishes as OPC UA variables
- **Real-time Updates**: Automatically updates OPC UA variables when CSV data changes
- **Multiple Data Types**: Supports temperature, humidity, pressure, and timestamp data
- **Configurable**: Command-line argument to specify CSV file path

## Prerequisites

- .NET 6.0 or later
- Visual Studio 2022 or VS Code with C# extension

## Installation

### Option 1: Using Docker (Recommended)

1. Clone or download this project
2. Navigate to the project directory
3. Build and run using Docker Compose:
   ```bash
   docker-compose up --build
   ```

This will start both the OPC UA server and a data generator service.

### Option 2: Local Development

1. Clone or download this project
2. Navigate to the project directory
3. Install .NET 6.0 or later
4. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

## Usage

### Basic Usage

Run the server with default settings (uses `data.csv`):
```bash
dotnet run
```

### Custom CSV File

Specify a custom CSV file path:
```bash
dotnet run "path/to/your/data.csv"
```

### Build and Run

Build the project:
```bash
dotnet build
```

Run the compiled executable:
```bash
dotnet run --no-build
```

## CSV File Format

The server expects CSV files with the following format:

```csv
Timestamp,Temperature,Humidity,Pressure
2024-01-01T12:00:00,25.5,60.2,1013.25
2024-01-01T12:01:00,25.7,60.5,1013.30
2024-01-01T12:02:00,25.3,59.8,1013.20
```

### CSV Columns

- **Timestamp**: DateTime value (e.g., "2024-01-01T12:00:00")
- **Temperature**: Double value in Celsius
- **Humidity**: Double value as percentage
- **Pressure**: Double value in hPa

## OPC UA Server Details

### Endpoint
- **URL**: `opc.tcp://localhost:4840/`
- **Security Policy**: Basic256Sha256
- **Authentication**: Anonymous

### Available Variables

The server publishes the following OPC UA variables under the "CSVData" folder:

1. **Temperature** (ns=2;s=temperature)
   - Data Type: Double
   - Description: Temperature value from CSV

2. **Humidity** (ns=2;s=humidity)
   - Data Type: Double
   - Description: Humidity value from CSV

3. **Pressure** (ns=2;s=pressure)
   - Data Type: Double
   - Description: Pressure value from CSV

4. **Timestamp** (ns=2;s=timestamp)
   - Data Type: DateTime
   - Description: Timestamp from CSV

### Update Frequency

The server checks for CSV updates every 5 seconds and publishes new values to OPC UA clients.

## Connecting with OPC UA Clients

### Using UaExpert (Free OPC UA Client)

1. Download and install UaExpert from Unified Automation
2. Add a new server with endpoint: `opc.tcp://localhost:4840/`
3. Browse to the "CSVData" folder to see available variables
4. Subscribe to variables for real-time updates

### Using Python

Install the required dependencies:
```bash
pip install -r requirements.txt
```

Run the Python client example:
```bash
python python_client_example.py
```

Or use the provided example code:
```python
import opcua

# Connect to the server
client = opcua.Client("opc.tcp://localhost:4840/")
client.connect()

# Get variable nodes
temp_node = client.get_node("ns=2;s=temperature")
humidity_node = client.get_node("ns=2;s=humidity")
pressure_node = client.get_node("ns=2;s=pressure")
timestamp_node = client.get_node("ns=2;s=timestamp")

# Read values
temperature = temp_node.get_value()
humidity = humidity_node.get_value()
pressure = pressure_node.get_value()
timestamp = timestamp_node.get_value()

print(f"Temperature: {temperature}°C")
print(f"Humidity: {humidity}%")
print(f"Pressure: {pressure}hPa")
print(f"Timestamp: {timestamp}")

client.disconnect()
```

## Project Structure

```
├── Program.cs                 # Main application entry point
├── CsvDataNodeManager.cs      # OPC UA node manager for CSV data
├── TestClient.cs              # C# test client for the server
├── GenerateSampleData.cs      # Sample data generator utility
├── OpcUaServer.csproj         # Project file with dependencies
├── appsettings.json           # Server configuration
├── Dockerfile                 # Docker container definition
├── docker-compose.yml         # Docker Compose setup
├── python_client_example.py   # Python client example
├── requirements.txt           # Python dependencies
├── run.bat                    # Windows batch script
├── run.sh                     # Linux/macOS shell script
└── README.md                  # This file
```

## Dependencies

- **Workstation.UaServer**: OPC UA server implementation
- **Workstation.UaClient**: OPC UA client library (for testing)
- **CsvHelper**: CSV file parsing library

## Troubleshooting

### Common Issues

1. **Port Already in Use**: If port 4840 is already in use, modify the endpoint URL in `Program.cs`
2. **CSV File Not Found**: The server will create a sample CSV file if none exists
3. **Permission Issues**: Ensure the application has read access to the CSV file

### Logs

The server outputs detailed logs to the console, including:
- Server startup information
- CSV data updates
- Error messages

## Security Considerations

- The server uses Basic256Sha256 security policy
- Anonymous authentication is enabled
- For production use, consider implementing user authentication
- Use HTTPS/TLS for secure communication

## Contributing

Feel free to submit issues and enhancement requests!

## License

This project is provided as-is for educational and development purposes.
