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

        public NodeView CreateNode(Type type, Vector2 position = default)
        {
            NodeView nodeView = new NodeView();
            nodeView.SetPosition(new Rect(position, Vector2.zero));
            this.AddElement(nodeView);
            return nodeView;
        }
    }
}