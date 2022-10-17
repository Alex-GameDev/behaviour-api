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
        Label nameLabel;
        Label descLabel;

        public ElementInspector()
        {
            AddLayout();
            AddStyles();
        }

        public void UpdateInspector(GraphElement element)
        {
            nameLabel.text = element.Name;
            descLabel.text = element.Description;
            inspectorContent.Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(element);
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
            nameLabel = this.Q<Label>(name: "name-label");
            descLabel = this.Q<Label>(name: "description-label");
        }

        private void AddStyles()
        {
            var styleSheet = VisualSettings.GetOrCreateSettings().InspectorStylesheet;
            styleSheets.Add(styleSheet);
        }

    }
}