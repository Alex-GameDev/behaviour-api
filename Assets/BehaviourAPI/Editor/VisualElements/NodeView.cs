using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public NodeView() : base(AssetDatabase.GetAssetPath(VisualSettings.GetOrCreateSettings().NodeLayout))
        {
            SetUp();
        }

        private void SetUp()
        {
            CreateInitialPorts();
        }

        private void CreateInitialPorts()
        {
            AddPort(Direction.Input);
            AddPort(Direction.Output);
        }

        // TODO: Input port type is node type and output port type is node childs type
        private void AddPort(Direction direction)
        {
            var port = InstantiatePort(Orientation.Vertical, direction, Port.Capacity.Single, typeof(bool));
            port.portName = "";
            port.style.flexDirection = direction == Direction.Input ? FlexDirection.Column : FlexDirection.ColumnReverse;
            var container = direction == Direction.Input ? inputContainer : outputContainer;
            container.Add(port);
        }
    }
}