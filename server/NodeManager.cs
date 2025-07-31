using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

namespace OpcUaServer
{
    public class NodeManager
    {
        private readonly UaApplication _application;
        private readonly Dictionary<string, NodeId> _nodes = new();
        private readonly Dictionary<string, object> _nodeValues = new();

        public NodeManager(UaApplication application)
        {
            _application = application;
            InitializeNodes();
        }

        private void InitializeNodes()
        {
            // Create namespace for our custom nodes
            var namespaceIndex = _application.AddNamespace("http://opcuacsvserver.com");

            // Create node IDs
            var temperatureNodeId = new NodeId("Temperature", namespaceIndex);
            var humidityNodeId = new NodeId("Humidity", namespaceIndex);
            var pressureNodeId = new NodeId("Pressure", namespaceIndex);
            var timestampNodeId = new NodeId("Timestamp", namespaceIndex);

            // Store node IDs
            _nodes["Temperature"] = temperatureNodeId;
            _nodes["Humidity"] = humidityNodeId;
            _nodes["Pressure"] = pressureNodeId;
            _nodes["Timestamp"] = timestampNodeId;

            // Initialize node values
            _nodeValues["Temperature"] = 0.0;
            _nodeValues["Humidity"] = 0.0;
            _nodeValues["Pressure"] = 0.0;
            _nodeValues["Timestamp"] = DateTime.Now;

            // Create folder node for organization
            var folderNodeId = new NodeId("CSVData", namespaceIndex);
            
            // Add nodes to address space
            AddVariableNode(temperatureNodeId, "Temperature", DataTypeIds.Double, folderNodeId);
            AddVariableNode(humidityNodeId, "Humidity", DataTypeIds.Double, folderNodeId);
            AddVariableNode(pressureNodeId, "Pressure", DataTypeIds.Double, folderNodeId);
            AddVariableNode(timestampNodeId, "Timestamp", DataTypeIds.DateTime, folderNodeId);
        }

        private void AddVariableNode(NodeId nodeId, string browseName, NodeId dataType, NodeId parentNodeId)
        {
            var variableNode = new VariableNode
            {
                NodeId = nodeId,
                BrowseName = new QualifiedName(browseName, nodeId.NamespaceIndex),
                DisplayName = new LocalizedText(browseName),
                DataType = dataType,
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                EventNotifier = EventNotifiers.None
            };

            // Add to address space
            _application.AddNode(variableNode);

            // Add reference to parent
            _application.AddReference(parentNodeId, ReferenceTypeIds.Organizes, nodeId, true);
        }

        public void UpdateNodeValue(string nodeName, object value)
        {
            if (_nodes.TryGetValue(nodeName, out var nodeId))
            {
                _nodeValues[nodeName] = value;
                
                // Update the node value in the address space
                var dataValue = new DataValue
                {
                    Value = value,
                    SourceTimestamp = DateTime.UtcNow,
                    ServerTimestamp = DateTime.UtcNow
                };

                _application.WriteValue(nodeId, dataValue);
            }
        }

        public object? GetNodeValue(string nodeName)
        {
            return _nodeValues.TryGetValue(nodeName, out var value) ? value : null;
        }

        public NodeId? GetNodeId(string nodeName)
        {
            return _nodes.TryGetValue(nodeName, out var nodeId) ? nodeId : null;
        }
    }
}