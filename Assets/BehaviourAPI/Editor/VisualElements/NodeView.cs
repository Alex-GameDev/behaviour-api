using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node;

        public List<Port> InputPorts = new List<Port>();
        public List<Port> OutputPorts = new List<Port>();

        public NodeView(Node node) : base(AssetDatabase.GetAssetPath(VisualSettings.GetOrCreateSettings().NodeLayout))
        {
            this.node = node;
            SetUp();
        }

        private void SetUp()
        {
            CreateInitialPorts();
        }

        private void CreateInitialPorts()
        {

            if (node.MaxInputConnections != 0) AddPort(Direction.Input);
            if (node.MaxOutputConnections != 0) AddPort(Direction.Output);
        }

        private void AddPort(Direction direction)
        {
            Type portType = direction == Direction.Input ? node.GetType() : node.ChildType;
            var port = InstantiatePort(Orientation.Vertical, direction, Port.Capacity.Single, portType);
            port.portName = "";
            port.style.flexDirection = direction == Direction.Input ? FlexDirection.Column : FlexDirection.ColumnReverse;
            var container = direction == Direction.Input ? inputContainer : outputContainer;
            container.Add(port);
            if (direction == Direction.Input) InputPorts.Add(port);
            else OutputPorts.Add(port);
        }

        private void InsertPort()
        {

        }

        public void OnMoved(Vector2 newPosition) => node.Position = newPosition;
        public void OnRemoved() => node.OnRemoved();

        public int GetIndexOfInputPort(Port port) => inputContainer.IndexOf(port);
        public int GetIndexOfOutputPort(Port port) => outputContainer.IndexOf(port);

    }
}