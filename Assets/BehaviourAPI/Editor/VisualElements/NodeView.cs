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

        Action<Node> m_onSelect = delegate { };

        public NodeView(Node node) : base(AssetDatabase.GetAssetPath(VisualSettings.GetOrCreateSettings().NodeLayout))
        {
            this.node = node;
            SetUp();
        }

        /// <summary>
        /// Called when this nodeview is selected.
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            m_onSelect(node);
        }

        public void SetOnSelectedCallback(Action<Node> callback) => m_onSelect = callback;

        private void SetUp()
        {
            CreateInitialPorts();
        }

        private void CreateInitialPorts()
        {

            if (node.MaxInputConnections != 0) InsertPort(Direction.Input, 0);
            if (node.MaxOutputConnections != 0) InsertPort(Direction.Output, 0);
        }

        private void InsertPort(Direction direction, int idx)
        {
            Type portType = direction == Direction.Input ? node.GetType() : node.ChildType;

            var port = InstantiatePort(Orientation.Vertical, direction, Port.Capacity.Single, portType);
            port.portName = "";
            port.style.flexDirection = direction == Direction.Input ? FlexDirection.Column : FlexDirection.ColumnReverse;
            var container = direction == Direction.Input ? inputContainer : outputContainer;
            container.Insert(idx, port);
            if (direction == Direction.Input) InputPorts.Add(port);
            else OutputPorts.Add(port);
        }




        // public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        // {
        //     PortView port = new PortView(orientation, direction, capacity, type);
        // }

        private void DeletePort(Direction direction, int idx)
        {
            var container = direction == Direction.Input ? inputContainer : outputContainer;
            container.RemoveAt(idx);
        }

        public void OnMoved(Vector2 newPosition) => node.Position = newPosition;
        public void OnRemoved() => node.OnRemoved();
        public int GetIndexOfInputPort(Port port) => inputContainer.IndexOf(port);
        public int GetIndexOfOutputPort(Port port) => outputContainer.IndexOf(port);

        public void OnOutputConnectionCreated(int idx)
        {
            if (node.MaxOutputConnections != -1) return;
            InsertPort(Direction.Output, idx + 1);
            InsertPort(Direction.Output, idx);
        }

        public void OnInputConnectionCreated(int idx)
        {
            if (node.MaxInputConnections != -1) return;
            InsertPort(Direction.Input, idx + 1);
            InsertPort(Direction.Input, idx);
        }

        public void OnOutputConnectionRemoved(int idx)
        {
            if (node.MaxOutputConnections != -1 || node.OutputConnections.Count < 1) return;
            DeletePort(Direction.Output, idx);
            DeletePort(Direction.Output, idx);
        }

        public void OnInputConnectionRemoved(int idx)
        {
            if (node.MaxInputConnections != -1 || node.InputConnections.Count < 1) return;
            DeletePort(Direction.Input, idx);
            DeletePort(Direction.Input, idx);
        }
    }
}