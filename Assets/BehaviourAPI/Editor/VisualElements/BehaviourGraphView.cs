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

        public BehaviourGraphView(BehaviourEngine graph, EditorWindow window)
        {
            BehaviourGraph = graph;
            m_editorWindow = window;
            nodeViews = new Dictionary<Node, NodeView>();

            AddGridBackground();
            DrawGraph();
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
                if (port.direction == startPort.direction) return;
                if (port == startPort) return;
                if (!port.portType.IsCorrelatedWith(startPort.portType)) return;
                if (port.node == startPort.node) return;

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

        private void DrawGraph()
        {
            BehaviourGraph.Nodes.ForEach(node => DrawNodeView(node));
        }

        private NodeView DrawNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.SetPosition(new Rect(node.Position, Vector2.zero));
            this.AddElement(nodeView);
            nodeViews.Add(node, nodeView);
            return nodeView;
        }

        private Edge DrawConnectionEdge(Connection connection)
        {
            return null;
        }


        #endregion

        #region GraphViewChanged Callbacks

        // https://answers.unity.com/questions/1752747/how-can-i-detect-changes-to-graphview-nodes-and-ed.html
        private GraphViewChange OnGraphViewChanged(GraphViewChange changes)
        {

            if (changes.edgesToCreate != null)
            {
                changes.edgesToCreate.ForEach(edge => OnEdgeCreated(edge));
            }

            if (changes.elementsToRemove != null)
            {
                changes.elementsToRemove.ForEach(elem => OnElementRemoved(elem));
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
            }
        }

        private void OnEdgeCreated(Edge edge)
        {
            Debug.Log("Edge created");
        }

        #endregion

    }
}