#!/bin/bash

echo "Building OPC UA CSV Server..."

# Restore packages
dotnet restore

# Build the project
dotnet build

if [ $? -eq 0 ]; then
    echo "Build successful!"
    echo ""
    echo "To run the server:"
    echo "  dotnet run"
    echo ""
    echo "To run with custom CSV file:"
    echo "  dotnet run path/to/your/data.csv"
    echo ""
    echo "To run the client (in another terminal):"
    echo "  dotnet run --project OpcUaClient.cs"
else
    echo "Build failed!"
    exit 1
fi