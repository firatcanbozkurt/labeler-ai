#!/usr/bin/env python3
"""
OPC UA CSV Server - Python Client Example

This script demonstrates how to connect to the OPC UA server and read CSV data.
Requires: pip install opcua
"""

import asyncio
from opcua import Client
import sys
import time

class OpcUaCsvClient:
    def __init__(self, url="opc.tcp://localhost:4840/"):
        self.url = url
        self.client = None

    async def connect(self):
        """Connect to the OPC UA server"""
        try:
            self.client = Client(self.url)
            await self.client.connect()
            print(f"Connected to OPC UA server at {self.url}")
            return True
        except Exception as e:
            print(f"Failed to connect: {e}")
            return False

    async def disconnect(self):
        """Disconnect from the server"""
        if self.client:
            await self.client.disconnect()
            print("Disconnected from server")

    async def read_variables(self):
        """Read all CSV variables from the server"""
        try:
            # Get variable nodes
            temp_node = self.client.get_node("ns=2;s=temperature")
            humidity_node = self.client.get_node("ns=2;s=humidity")
            pressure_node = self.client.get_node("ns=2;s=pressure")
            timestamp_node = self.client.get_node("ns=2;s=timestamp")

            # Read values
            temperature = await temp_node.get_value()
            humidity = await humidity_node.get_value()
            pressure = await pressure_node.get_value()
            timestamp = await timestamp_node.get_value()

            print(f"\n--- Current CSV Data ---")
            print(f"Temperature: {temperature}°C")
            print(f"Humidity: {humidity}%")
            print(f"Pressure: {pressure}hPa")
            print(f"Timestamp: {timestamp}")
            print("-" * 30)

        except Exception as e:
            print(f"Error reading variables: {e}")

    async def subscribe_to_changes(self):
        """Subscribe to variable changes"""
        try:
            # Get variable nodes
            temp_node = self.client.get_node("ns=2;s=temperature")
            humidity_node = self.client.get_node("ns=2;s=humidity")

            # Create subscription
            subscription = await self.client.create_subscription(1000, self.data_change_handler)
            
            # Add monitored items
            await subscription.subscribe_data_change(temp_node)
            await subscription.subscribe_data_change(humidity_node)

            print("Subscribed to temperature and humidity changes...")
            print("Press Ctrl+C to stop monitoring")

            # Keep the subscription alive
            while True:
                await asyncio.sleep(1)

        except Exception as e:
            print(f"Error setting up subscription: {e}")

    def data_change_handler(self, node, val, data):
        """Handle data change notifications"""
        node_name = node.get_browse_name().Name
        print(f"[{time.strftime('%H:%M:%S')}] {node_name} changed to: {val}")

async def main():
    """Main function"""
    if len(sys.argv) > 1:
        url = sys.argv[1]
    else:
        url = "opc.tcp://localhost:4840/"

    client = OpcUaCsvClient(url)
    
    if not await client.connect():
        return

    try:
        # Read current values
        await client.read_variables()

        # Ask user what to do
        print("\nOptions:")
        print("1. Read current values once")
        print("2. Subscribe to changes")
        print("3. Exit")
        
        choice = input("Enter your choice (1-3): ")

        if choice == "1":
            await client.read_variables()
        elif choice == "2":
            await client.subscribe_to_changes()
        elif choice == "3":
            print("Exiting...")
        else:
            print("Invalid choice")

    except KeyboardInterrupt:
        print("\nInterrupted by user")
    except Exception as e:
        print(f"Error: {e}")
    finally:
        await client.disconnect()

if __name__ == "__main__":
    asyncio.run(main())