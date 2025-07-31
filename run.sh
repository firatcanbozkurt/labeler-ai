#!/bin/bash

echo "OPC UA CSV Server - Management Script"
echo "====================================="

while true; do
    echo
    echo "Select an option:"
    echo "1. Run OPC UA Server"
    echo "2. Run OPC UA Server with custom CSV file"
    echo "3. Run Test Client"
    echo "4. Generate Sample Data"
    echo "5. Build Project"
    echo "6. Exit"
    echo
    read -p "Enter your choice (1-6): " choice

    case $choice in
        1)
            echo
            echo "Starting OPC UA Server..."
            dotnet run
            ;;
        2)
            echo
            read -p "Enter CSV file path: " csvfile
            echo "Starting OPC UA Server with CSV file: $csvfile"
            dotnet run "$csvfile"
            ;;
        3)
            echo
            echo "Starting Test Client..."
            dotnet run --project TestClient.cs
            ;;
        4)
            echo
            echo "Starting Sample Data Generator..."
            dotnet run --project GenerateSampleData.cs
            ;;
        5)
            echo
            echo "Building project..."
            dotnet build
            echo "Build completed!"
            ;;
        6)
            echo
            echo "Goodbye!"
            exit 0
            ;;
        *)
            echo "Invalid choice. Please enter a number between 1 and 6."
            ;;
    esac
done