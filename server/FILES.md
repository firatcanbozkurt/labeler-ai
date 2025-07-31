# OPC UA CSV Server - File Structure

This directory contains a complete C# OPC UA server implementation that publishes CSV data to OPC UA clients.

## Core Files

### C# Application Files
- **`OpcUaServer.csproj`** - C# project file with dependencies
- **`Program.cs`** - Main application entry point and server logic
- **`NodeManager.cs`** - OPC UA node management and address space handling

### Data Files
- **`sample_data.csv`** - Sample CSV file with sensor data for testing

### Client Examples
- **`OpcUaClient.cs`** - C# OPC UA client example for testing
- **`test_server.py`** - Python OPC UA client for testing (requires `pip install opcua`)

### Build and Deployment
- **`build.sh`** - Build script for the C# application
- **`install_dotnet.sh`** - Script to install .NET 6.0 on Ubuntu/Debian
- **`Dockerfile`** - Docker container definition
- **`docker-compose.yml`** - Docker Compose configuration

### Documentation
- **`README.md`** - Comprehensive documentation and usage instructions
- **`FILES.md`** - This file, listing all components

## Quick Start Commands

1. **Install .NET 6.0:**
   ```bash
   ./install_dotnet.sh
   ```

2. **Build and run server:**
   ```bash
   ./build.sh
   dotnet run
   ```

3. **Test with Python client:**
   ```bash
   pip install opcua
   python3 test_server.py
   ```

4. **Or use Docker:**
   ```bash
   docker-compose up --build
   ```

## Server Features

- ✅ Reads CSV files with sensor data
- ✅ Publishes data as OPC UA variables
- ✅ Automatic data cycling (1-second intervals)
- ✅ Custom CSV file support
- ✅ Sample data generation
- ✅ Multiple client support
- ✅ Docker containerization
- ✅ Cross-platform compatibility

## OPC UA Endpoint

- **URL:** `opc.tcp://localhost:4840/`
- **Namespace:** `http://opcuacsvserver.com`
- **Available Nodes:**
  - `ns=2;s=Temperature`
  - `ns=2;s=Humidity`
  - `ns=2;s=Pressure`
  - `ns=2;s=Timestamp`