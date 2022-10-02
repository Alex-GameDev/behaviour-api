using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    public class ElementInspector : VisualElement
    {
        UnityEditor.Editor editor;
        VisualElement inspectorContent;

        public ElementInspector()
        {
            AddLayout();
            AddStyles();
        }

        public void UpdateInspector(Node node)
        {
            inspectorContent.Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor && editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            inspectorContent.Add(container);
        }

        private void AddLayout()
        {
            var visualTree = VisualSettings.GetOrCreateSettings().InspectorLayout;
            var inspectorFromUXML = visualTree.Instantiate();
            Add(inspectorFromUXML);
            inspectorContent = this.Q("inspector-container");
        }

        private void AddStyles()
        {
            var styleSheet = VisualSettings.GetOrCreateSettings().InspectorStylesheet;
            styleSheets.Add(styleSheet);
        }

    }
}