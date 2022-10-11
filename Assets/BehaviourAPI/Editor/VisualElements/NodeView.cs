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
        /// <summary>
        /// The represented node.
        /// </summary>
        public Node node;

        ElementInspector m_elementInspector;

        /// <summary>
        /// Draw a new NodeView that represents the node.
        /// </summary>
        public NodeView(Node node) : base(AssetDatabase.GetAssetPath(VisualSettings.GetOrCreateSettings().NodeLayout))
        {
            this.node = node;
            if (node.MaxInputConnections != 0) InsertPort(Direction.Input, 0);
            if (node.MaxOutputConnections != 0) InsertPort(Direction.Output, 0);

            node.InputConnectionAdded += OnInputConnectionCreated;
            node.OutputConnectionAdded += OnOutputConnectionCreated;
            node.InputConnectionRemoved += OnInputConnectionRemoved;
            node.OutputConnectionRemoved += OnOutputConnectionRemoved;

        }

        /// <summary>
        /// Set the inspector triggered on select event.
        /// </summary>
        /// <param name="inspector"></param>
        public void SetInspector(ElementInspector inspector) => m_elementInspector = inspector;

        /// <summary>
        /// Create a new Port
        /// </summary>
        public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
        {
            return Port.Create<ConnectionView>(orientation, direction, capacity, type);
        }

        #region GUI event callbacks

        /// <summary>
        /// Called when this nodeview is selected.
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            m_elementInspector?.UpdateInspector(node);
        }

        /// <summary>
        /// Called when tis nodeview is moved.
        /// </summary>
        /// <param name="newPosition"></param>
        public void OnMoved(Vector2 newPosition)
        {
            node.Position = newPosition;
        }

        /// <summary>
        /// Connect an edge with this nodeview.
        /// </summary>
        public void Connect(ConnectionView connectionView, Direction direction)
        {
            var container = GetPortContainer(direction);
            var connection = connectionView.connection;
            int connectionIdx = direction == Direction.Input ?
               connection.TargetNode.InputConnections.IndexOf(connection) :
               connection.SourceNode.OutputConnections.IndexOf(connection);
            int portIdx = connectionIdx * 2;

            if (container.childCount < portIdx) // Si no hay puertos suficientes para conectar, se crean
            {
                for (int i = container.childCount; i < portIdx; i++)
                {
                    InsertPort(direction, i);
                }
            }

            GetPort(direction, portIdx).Connect(connectionView);

            var capacity = direction == Direction.Input ? node.MaxInputConnections : node.MaxOutputConnections;
            if (capacity != -1) return;
            InsertPort(direction, portIdx + 1);
            InsertPort(direction, portIdx);
        }

        #endregion
        public int GetIndexOfInputPort(Port port) => inputContainer.IndexOf(port);
        public int GetIndexOfOutputPort(Port port) => outputContainer.IndexOf(port);

        public void OnOutputConnectionCreated(int idx)
        {
            if (node.MaxOutputConnections != -1) return;
            idx = idx * 2;
            InsertPort(Direction.Output, idx);
            InsertPort(Direction.Output, idx + 2);
        }

        public void OnInputConnectionCreated(int idx)
        {
            if (node.MaxInputConnections != -1) return;
            idx = idx * 2;
            InsertPort(Direction.Input, idx);
            InsertPort(Direction.Input, idx + 2);
        }

        public void OnOutputConnectionRemoved(int idx)
        {
            if (node.MaxOutputConnections != -1) return;
            idx = idx * 2 + 1;
            DeletePort(Direction.Output, idx - 1);
            DeletePort(Direction.Output, idx);
        }

        public void OnInputConnectionRemoved(int idx)
        {
            if (node.MaxInputConnections != -1) return;
            idx = idx * 2 + 1;
            DeletePort(Direction.Input, idx - 1);
            DeletePort(Direction.Input, idx);
        }

        private void DeletePort(Direction direction, int idx)
        {
            var container = direction == Direction.Input ? inputContainer : outputContainer;
            container.RemoveAt(idx);
        }

        private Port GetPort(Direction dir, int idx)
        {
            var container = GetPortContainer(dir);
            return container[idx] as Port;
        }

        private VisualElement GetPortContainer(Direction direction)
        {
            return direction == Direction.Input ? inputContainer : outputContainer;
        }

        private void InsertPort(Direction direction, int idx)
        {
            Type portType = direction == Direction.Input ? node.GetType() : node.ChildType;

            var port = InstantiatePort(Orientation.Vertical, direction, Port.Capacity.Single, portType);
            port.portName = "";
            port.style.flexDirection = direction == Direction.Input ? FlexDirection.Column : FlexDirection.ColumnReverse;
            var container = direction == Direction.Input ? inputContainer : outputContainer;
            container.Insert(idx, port);
        }
    }
}