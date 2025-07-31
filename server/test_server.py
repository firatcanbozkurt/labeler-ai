#!/usr/bin/env python3
"""
Simple Python OPC UA client to test the CSV server
Requires: pip install opcua
"""

import sys
import time
from opcua import Client

def test_opcua_server():
    """Test connection to the OPC UA CSV server"""
    
    # Server endpoint
    url = "opc.tcp://localhost:4840/"
    
    try:
        # Create client
        client = Client(url)
        
        # Connect to server
        print(f"Connecting to {url}...")
        client.connect()
        print("Connected successfully!")
        
        # Get root node
        root = client.get_root_node()
        print(f"Root node: {root}")
        
        # Browse available nodes
        print("\nBrowsing available nodes...")
        objects = client.get_objects_node()
        print(f"Objects node: {objects}")
        
        # Try to read specific nodes
        node_ids = [
            "ns=2;s=Temperature",
            "ns=2;s=Humidity", 
            "ns=2;s=Pressure",
            "ns=2;s=Timestamp"
        ]
        
        print("\nReading node values...")
        for i in range(5):  # Read 5 times
            print(f"\n--- Reading {i+1}/5 ---")
            
            for node_id in node_ids:
                try:
                    node = client.get_node(node_id)
                    value = node.get_value()
                    print(f"{node_id}: {value}")
                except Exception as e:
                    print(f"Error reading {node_id}: {e}")
            
            time.sleep(2)  # Wait 2 seconds between reads
        
        # Disconnect
        client.disconnect()
        print("\nDisconnected from server")
        
    except Exception as e:
        print(f"Error: {e}")
        return False
    
    return True

if __name__ == "__main__":
    print("OPC UA CSV Server Test Client")
    print("=" * 40)
    
    success = test_opcua_server()
    
    if success:
        print("\n✅ Test completed successfully!")
    else:
        print("\n❌ Test failed!")
        sys.exit(1)