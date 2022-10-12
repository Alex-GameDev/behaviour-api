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
            InitializePorts();
            RegisterCallbacks();
            AddManipulators();
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

        public int GetIndexOfInputPort(Port port) => inputContainer.IndexOf(port);
        public int GetIndexOfOutputPort(Port port) => outputContainer.IndexOf(port);

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
        #endregion

        private void InitializePorts()
        {
            int inputPorts = node.MaxInputConnections == -1 ? node.InputConnections.Count * 2 + 1 : node.MaxInputConnections;
            int outputPorts = node.MaxOutputConnections == -1 ? node.OutputConnections.Count * 2 + 1 : node.MaxOutputConnections;
            for (int i = 0; i < inputPorts; i++) InsertPort(Direction.Input, i);
            for (int i = 0; i < outputPorts; i++) InsertPort(Direction.Output, i);
        }

        private void RegisterCallbacks()
        {
            node.InputConnectionAdded += OnInputConnectionCreated;
            node.OutputConnectionAdded += OnOutputConnectionCreated;
            node.InputConnectionRemoved += OnInputConnectionRemoved;
            node.OutputConnectionRemoved += OnOutputConnectionRemoved;
        }

        private void DeletePort(Direction direction, int idx)
        {
            var container = direction == Direction.Input ? inputContainer : outputContainer;
            container.RemoveAt(idx);
        }

        public Port GetPort(Direction dir, int connectionIndex, bool includeEmpty = false)
        {
            bool multiplePorts = (dir == Direction.Input && node.MaxInputConnections == -1) || (dir == Direction.Output && node.MaxOutputConnections == -1);
            if (!includeEmpty && multiplePorts) connectionIndex = connectionIndex * 2 + 1;

            var container = GetPortContainer(dir);
            return container[connectionIndex] as Port;
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

        private void DisconnectAll(Direction direction)
        {
            var graphview = GetFirstAncestorOfType<GraphView>();
            if (graphview != null)
            {
                var container = GetPortContainer(direction);
                List<GraphElement> elementsToDelete = new List<GraphElement>();
                container.Query<Port>().ForEach(elem =>
                {
                    if (elem.connected)
                        foreach (Edge c in elem.connections) elementsToDelete.Add(c);
                });
                graphview.DeleteElements(elementsToDelete);
            }
        }

        private void AddManipulators()
        {
            this.AddManipulator(CreateContextMenuManipulator());
        }

        private IManipulator CreateContextMenuManipulator()
        {
            return new ContextualMenuManipulator(
                menuEvt =>
                {
                    menuEvt.menu.AppendAction("Disconnect all", ctx =>
                    {
                        DisconnectAll(Direction.Input);
                        DisconnectAll(Direction.Output);
                    });
                    menuEvt.menu.AppendAction("Disconnect inputs", ctx => DisconnectAll(Direction.Input));
                    menuEvt.menu.AppendAction("Disconnect outputs", ctx => DisconnectAll(Direction.Output));
                }
            );
        }
    }
}