using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.Editor
{
    using BehaviourAPI.Runtime.Core;
    public class BehaviourGraphView : GraphView
    {
        public BehaviourEngine BehaviourGraph { get; set; }

        NodeSearchWindow m_nodeSearchingWindow;
        public BehaviourGraphView(BehaviourEngine graph)
        {
            BehaviourGraph = graph;
            AddGridBackground();
            AddManipulators();
            AddCreateNodeWindow();
            AddStyles();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (port.direction == startPort.direction) return;
                if (port.portType != startPort.portType) return;
                if (port == startPort) return;
                if (port.node == startPort.node) return;

                compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        public NodeView CreateNode(Type type, Vector2 position = default)
        {
            Node node = (Node)Activator.CreateInstance(type);
            NodeView nodeView = new NodeView(node);
            nodeView.SetPosition(new Rect(position, Vector2.zero));
            this.AddElement(nodeView);
            return nodeView;
        }

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
            this.AddManipulator(new ContextualMenuManipulator(MenuBuilderProvider()));
        }

        private Action<ContextualMenuPopulateEvent> MenuBuilderProvider()
        {
            return menuEvent => menuEvent.menu.AppendAction("AddNode", actionEvent => AddElement(CreateNode(null)));
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
    }
}