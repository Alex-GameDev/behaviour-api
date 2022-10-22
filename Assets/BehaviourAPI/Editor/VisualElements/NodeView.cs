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
    using Runtime.UtilitySystems;
    using UnityEditor.UIElements;

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
            AddLayout();
            SetupDataBinding();
            AddManipulators();
            OnConvertStartNode(node.IsStartNode);
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

        /// <summary>
        /// Called when a connection is created or removed.
        /// </summary>
        /// <param name="args">The event args</param>
        private void OnConnectionChanged(ConnectionChangedEventArgs args)
        {
            var dir = args.Direction == ConnectionDirection.INPUT ? Direction.Input : Direction.Output;
            var maxConnection = dir == Direction.Input ? node.MaxInputConnections : node.MaxOutputConnections;
            if (maxConnection != -1) return;

            var idx = args.Index * 2;
            if (args.EventType == ConnectionEventType.ADD)
            {
                InsertPort(dir, idx);
                InsertPort(dir, idx + 2);
            }
            else
            {
                DeletePort(dir, idx);
                DeletePort(dir, idx + 1);
            }
        }


        public void OnConvertStartNode(bool isStart)
        {
            VisualElement borderContainer = this.Q(name: "start-node-panel");
            VisualElement inputPortContainer = inputContainer;
            borderContainer.style.display = isStart ? DisplayStyle.Flex : DisplayStyle.None;
            inputPortContainer.style.display = isStart ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public Port GetPort(Direction dir, int connectionIndex, bool includeEmpty = false)
        {
            bool multiplePorts = (dir == Direction.Input && node.MaxInputConnections == -1) || (dir == Direction.Output && node.MaxOutputConnections == -1);
            if (!includeEmpty && multiplePorts) connectionIndex = connectionIndex * 2 + 1;

            var container = GetPortContainer(dir);
            return container[connectionIndex] as Port;
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
            node.ConnectionChanged += (sender, args) => OnConnectionChanged(args);
        }

        private void DeletePort(Direction direction, int idx)
        {
            var container = direction == Direction.Input ? inputContainer : outputContainer;
            container.RemoveAt(idx);
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
                List<UnityEditor.Experimental.GraphView.GraphElement> elementsToDelete = new List<UnityEditor.Experimental.GraphView.GraphElement>();
                container.Query<Port>().ForEach(elem =>
                {
                    if (elem.connected)
                        foreach (Edge c in elem.connections) elementsToDelete.Add(c);
                });
                graphview.DeleteElements(elementsToDelete);
            }
        }

        private void AddLayout()
        {
            //titleInputField.RegisterCallback<ChangeEvent<string>>((evt) => node.NodeName = titleInputField.value);
            var extensionContainer = this.Q(name: "extension");
            var border = this.Q(name: "node-border");
            DisplayUtilityHandler(node as IUtilityHandler, extensionContainer);
            DisplayStatusHandler(node as IStatusHandler, border);
            DisplayActionAsignable(node as IActionAsignable, extensionContainer);
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
                    menuEvt.menu.AppendSeparator();
                    menuEvt.menu.AppendAction("Convert to Start Node", ctx => ConvertToStartNode());
                }
            );
        }

        public void ConvertToStartNode()
        {
            DisconnectAll(Direction.Input);
            node.ConvertToStartNode();
        }

        private void DisplayUtilityHandler(IUtilityHandler utilityHandler, VisualElement extensionContainer)
        {
            if (utilityHandler == null) return;

            var utilityBar = new ProgressBar()
            {
                title = " ",
                lowValue = 0,
                highValue = 1,
            };

            utilityHandler.OnValueChanged += (value) => utilityBar.value = value;
            extensionContainer.Add(utilityBar);
        }

        private void DisplayStatusHandler(IStatusHandler statusHandler, VisualElement border)
        {
            if (statusHandler == null) return;

            statusHandler.OnValueChanged += (status) => SetColor(border, status);
            SetColor(border, statusHandler.Status);
        }

        private void DisplayActionAsignable(IActionAsignable actionAsignable, VisualElement extensionContainer)
        {
            if (actionAsignable == null) return;
            var timeTag = new Label()
            {
                text = $"{actionAsignable.Action.ExecutionTime.ToString("0.00")}s"
            };
            timeTag.style.alignSelf = new StyleEnum<Align>(Align.Center);

            actionAsignable.Action.ExecutionTimeChanged += (time) => timeTag.text = $"{time.ToString("0.00")}s";
            extensionContainer.Add(timeTag);
        }

        private void SetColor(VisualElement element, Status status)
        {
            switch (status)
            {
                case Status.Running: element.style.backgroundColor = new StyleColor(Color.yellow); break;
                case Status.Failure: element.style.backgroundColor = new StyleColor(Color.red); break;
                case Status.Sucess: element.style.backgroundColor = new StyleColor(Color.green); break;
                default: element.style.backgroundColor = new StyleColor(new Color(0.5f, 0.5f, 0.5f, 0.5f)); break;
            }
        }

        private void SetupDataBinding()
        {
            // Bindear nodename
            var titleInputField = this.Q<TextField>(name: "title-input-field");
            titleInputField.bindingPath = "NodeName";
            titleInputField.Bind(new SerializedObject(node));
        }
    }
}