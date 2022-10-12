using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    using Utils;
    public class BehaviourGraphView : GraphView
    {
        public BehaviourEngine BehaviourGraph { get; set; }

        Dictionary<Node, NodeView> nodeViews;
        NodeSearchWindow m_nodeSearchingWindow;
        EditorWindow m_editorWindow;
        ElementInspector m_elementInspector;

        public BehaviourGraphView(BehaviourEngine graph, EditorWindow window)
        {
            BehaviourGraph = graph;
            m_editorWindow = window;
            nodeViews = new Dictionary<Node, NodeView>();

            AddGridBackground();
            AddManipulators();
            AddCreateNodeWindow();
            AddStyles();
            graphViewChanged += OnGraphViewChanged;
            graph.StartNodeChanged += OnStartNodeChanged;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                Node startNode = (startPort.node as NodeView).node;
                Node otherNode = (port.node as NodeView).node;

                if (port.direction == startPort.direction) return;
                if (port == startPort) return;
                if (!port.portType.IsCorrelatedWith(startPort.portType)) return;
                if (port.node == startPort.node) return;
                if (otherNode.IsConnectedWith(startNode)) return;

                compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        public void CreateNode(Type type, Vector2 position = default)
        {
            Node node = BehaviourGraph.CreateNode(type, position);
            node.Position = position;
            DrawNodeView(node);
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition)
        {
            return contentViewContainer.WorldToLocal(mousePosition - m_editorWindow.position.position);
        }

        public void SetElementInspector(ElementInspector inspector)
        {
            m_elementInspector = inspector;
        }

        #region Initialization
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            Insert(0, gridBackground);

        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

        }

        private void AddStyles()
        {
            StyleSheet styleSheet = VisualSettings.GetOrCreateSettings().GraphStylesheet;
            styleSheets.Add(styleSheet);
        }

        private void AddCreateNodeWindow()
        {
            if (m_nodeSearchingWindow == null)
            {
                m_nodeSearchingWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
                m_nodeSearchingWindow.Initialize(this);
            }
            nodeCreationRequest = context => SearchWindow.Open(
                new SearchWindowContext(context.screenMousePosition), m_nodeSearchingWindow);
        }

        #endregion

        #region Draw Elements

        public void DrawGraph()
        {
            if (BehaviourGraph == null) return;
            BehaviourGraph.Nodes.ForEach(node => DrawNodeView(node));
            BehaviourGraph.Connections.ForEach(conn => DrawConnectionEdge(conn));
        }

        private void DrawNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.SetPosition(new Rect(node.Position, Vector2.zero));
            nodeView.SetInspector(m_elementInspector);
            this.AddElement(nodeView);
            nodeViews.Add(node, nodeView);
        }

        private void DrawConnectionEdge(Connection connection)
        {
            ConnectionView connectionView = new ConnectionView();

            if (nodeViews.TryGetValue(connection.SourceNode, out NodeView sourceNode) &&
                nodeViews.TryGetValue(connection.TargetNode, out NodeView targetNode))
            {
                int outputIdx = sourceNode.node.OutputConnections.IndexOf(connection);
                int inputIdx = targetNode.node.InputConnections.IndexOf(connection);
                connectionView.Connect(sourceNode.GetPort(Direction.Output, outputIdx));
                connectionView.Connect(targetNode.GetPort(Direction.Input, inputIdx));
                this.AddElement(connectionView);
                connectionView.connection = connection;
                connectionView.SetInspector(m_elementInspector);
                connectionView.MarkDirtyRepaint();
            }
            else throw new Exception("Connection port/s didn't found!");
        }


        #endregion

        #region GraphViewChanged Callbacks

        // https://answers.unity.com/questions/1752747/how-can-i-detect-changes-to-graphview-nodes-and-ed.html
        private GraphViewChange OnGraphViewChanged(GraphViewChange changes)
        {
            if (changes.elementsToRemove != null)
                changes.elementsToRemove.ForEach(elem => OnElementRemoved(elem));

            if (changes.edgesToCreate != null)
                changes.edgesToCreate.ForEach(edge => OnEdgeCreated(edge));

            if (changes.movedElements != null)
                changes.movedElements.ForEach(elem => OnElementMoved(elem));

            return changes;
        }

        private void OnElementMoved(GraphElement elem)
        {
            if (elem is NodeView nodeView)
            {
                nodeView.OnMoved(elem.GetPosition().position);
            }
        }

        private void OnElementRemoved(GraphElement elem)
        {
            if (elem is NodeView nodeView) BehaviourGraph.RemoveNode(nodeView.node);
            if (elem is ConnectionView connectionView) BehaviourGraph.RemoveConnection(connectionView.connection);
        }

        private void OnEdgeCreated(Edge edge)
        {
            if (edge is ConnectionView connectionView)
            {
                NodeView sourceNodeView = edge.output.node as NodeView;
                NodeView targetNodeView = edge.input.node as NodeView;
                Node source = sourceNodeView.node;
                Node target = targetNodeView.node;
                int outputPortIdx = sourceNodeView.GetIndexOfOutputPort(edge.output);
                int inputPortIdx = targetNodeView.GetIndexOfInputPort(edge.input);
                Connection conn = BehaviourGraph.CreateConnection(source, target, outputPortIdx / 2, inputPortIdx / 2);
                connectionView.connection = conn;
                connectionView.SetInspector(m_elementInspector);
            }
            else throw new Exception("Editor Error: Edge don't belong the correct type (ConnectionView)");
        }


        private void OnStartNodeChanged(Node oldStartNode, Node newStartNode)
        {
            if (oldStartNode != null)
            {
                if (nodeViews.TryGetValue(oldStartNode, out NodeView oldStartNodeView))
                    oldStartNodeView.OnConvertStartNode(false);
            }
            if (newStartNode != null)
            {
                if (nodeViews.TryGetValue(newStartNode, out NodeView newStartNodeView))
                    newStartNodeView.OnConvertStartNode(true);
            }
        }

        #endregion

    }

}