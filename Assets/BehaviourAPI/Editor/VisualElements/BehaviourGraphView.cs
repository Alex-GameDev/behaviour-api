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
        Action<Node> m_onSelectNode;

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

        public NodeView CreateNode(Type type, Vector2 position = default)
        {
            Node node = BehaviourGraph.CreateNode(type, position);
            node.Position = position;
            return DrawNodeView(node);
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition)
        {
            return contentViewContainer.WorldToLocal(mousePosition - m_editorWindow.position.position);
        }

        public void SetSelectionNodeCallback(Action<Node> eventCallback)
        {
            m_onSelectNode = eventCallback;
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

        private NodeView DrawNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.SetPosition(new Rect(node.Position, Vector2.zero));
            nodeView.SetOnSelectedCallback(m_onSelectNode);
            this.AddElement(nodeView);
            nodeViews.Add(node, nodeView);
            return nodeView;
        }

        private Edge DrawConnectionEdge(Connection connection)
        {
            Edge edge = new Edge();

            if (nodeViews.TryGetValue(connection.SourceNode, out NodeView sourceNode) &&
                nodeViews.TryGetValue(connection.TargetNode, out NodeView targetNode))
            {
                edge.input = targetNode.InputPorts[0];
                edge.output = sourceNode.OutputPorts[0];
                Debug.Log("Edge drawed");
            }
            else Debug.Log("Error creating edge");
            this.AddElement(edge);
            return edge;
        }


        #endregion

        #region GraphViewChanged Callbacks

        // https://answers.unity.com/questions/1752747/how-can-i-detect-changes-to-graphview-nodes-and-ed.html
        private GraphViewChange OnGraphViewChanged(GraphViewChange changes)
        {
            if (changes.elementsToRemove != null)
            {
                changes.elementsToRemove.ForEach(elem => OnElementRemoved(elem));
            }

            if (changes.edgesToCreate != null)
            {
                changes.edgesToCreate.ForEach(edge => OnEdgeCreated(edge));
            }

            if (changes.movedElements != null)
            {
                changes.movedElements.ForEach(elem => OnElementMoved(elem));
            }
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
            if (elem is NodeView nodeView)
            {
                nodeView.OnRemoved();
            }
            if (elem is Edge edge)
            {
                Debug.Log("Edge removed");
                NodeView sourceNodeView = edge.output.node as NodeView;
                NodeView targetNodeView = edge.input.node as NodeView;
                Node source = sourceNodeView.node;
                Node target = targetNodeView.node;
                int outputPortIdx = sourceNodeView.GetIndexOfOutputPort(edge.output);
                int inputPortIdx = targetNodeView.GetIndexOfInputPort(edge.input);
                sourceNodeView.OnOutputConnectionRemoved(outputPortIdx);
                targetNodeView.OnInputConnectionRemoved(inputPortIdx);
            }
        }

        private void OnEdgeCreated(Edge edge)
        {
            Debug.Log("Edge created");
            NodeView sourceNodeView = edge.output.node as NodeView;
            NodeView targetNodeView = edge.input.node as NodeView;
            Node source = sourceNodeView.node;
            Node target = targetNodeView.node;
            int outputPortIdx = sourceNodeView.GetIndexOfOutputPort(edge.output);
            int inputPortIdx = targetNodeView.GetIndexOfInputPort(edge.input);
            Debug.Log($"INDEX {outputPortIdx}, {inputPortIdx}");
            Connection conn = BehaviourGraph.CreateConnection(source, target, outputPortIdx / 2, inputPortIdx / 2);
            sourceNodeView.OnOutputConnectionCreated(outputPortIdx);
            targetNodeView.OnInputConnectionCreated(inputPortIdx);
        }

        #endregion

    }
}