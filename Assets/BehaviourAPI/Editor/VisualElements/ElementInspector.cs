using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    using UnityEditor;

    public class ElementInspector : VisualElement
    {
        UnityEditor.Editor elementEditor, taskEditor;
        VisualElement elementInspectorContent, taskInspectorContent, taskContainer;
        Label nameLabel, descLabel;
        Button assignTaskButton;
        ActionSearchWindow m_actionSearchWindow;
        GraphElement m_selectedElement;
        public ElementInspector()
        {
            AddLayout();
            AddStyles();
            AddBindActionWindow();
        }

        public void UpdateInspector(GraphElement element)
        {
            m_selectedElement = element;
            nameLabel.text = element.Name;
            descLabel.text = element.Description;
            elementInspectorContent.Clear();
            UnityEngine.Object.DestroyImmediate(elementEditor);
            elementEditor = UnityEditor.Editor.CreateEditor(element);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (elementEditor && elementEditor.target)
                {
                    elementEditor.OnInspectorGUI();
                }
            });
            elementInspectorContent.Add(container);

            DisplayActionEditor(element as IActionAsignable);
        }

        public void BindActionToCurrentNode(Type type)
        {
            if (m_selectedElement is IActionAsignable actionAsignable)
            {
                actionAsignable.Action = ScriptableObject.CreateInstance(type) as ActionTask;
                Debug.Log("New action assigned");
                UpdateInspector(m_selectedElement);
                EditorUtility.SetDirty(m_selectedElement);
            }
        }

        public bool IsSelectedElementAnActionAssignable() => (m_selectedElement as IActionAsignable) != null;
        private void AddLayout()
        {
            var visualTree = VisualSettings.GetOrCreateSettings().InspectorLayout;
            var inspectorFromUXML = visualTree.Instantiate();
            Add(inspectorFromUXML);

            elementInspectorContent = this.Q("inspector-container");
            nameLabel = this.Q<Label>(name: "name-label");
            descLabel = this.Q<Label>(name: "description-label");

            taskContainer = this.Q("task-container");
            taskInspectorContent = this.Q("task-inspector-container");
            assignTaskButton = this.Q<Button>("assign-task-button");
            taskContainer.style.display = DisplayStyle.None;
            assignTaskButton.clicked += OnAssignTaskButtonClicked;
        }

        private void OnAssignTaskButtonClicked()
        {
            if (m_selectedElement is IActionAsignable actionAsignable)
            {
                if (actionAsignable.Action != null)
                {
                    Debug.Log("Current action removed");
                    actionAsignable.Action = null;
                    EditorUtility.SetDirty(m_selectedElement);
                    UpdateInspector(m_selectedElement);
                }
                else
                {
                    SearchWindow.Open(new SearchWindowContext(), m_actionSearchWindow);
                }
            }

        }

        private void AddStyles()
        {
            var styleSheet = VisualSettings.GetOrCreateSettings().InspectorStylesheet;
            styleSheets.Add(styleSheet);
        }

        private void DisplayActionEditor(IActionAsignable actionAsignable)
        {
            if (actionAsignable != null)
            {
                Debug.Log("Is action assignable...");
                taskInspectorContent.Clear();
                taskContainer.style.display = DisplayStyle.Flex;
                UnityEngine.Object.DestroyImmediate(taskEditor);
                if (actionAsignable.Action != null)
                {
                    Debug.Log("...and has an action assigned!");
                    assignTaskButton.text = "Remove current action";
                    taskEditor = UnityEditor.Editor.CreateEditor(actionAsignable.Action);
                    IMGUIContainer container = new IMGUIContainer(() =>
                    {
                        if (taskEditor && taskEditor.target)
                        {
                            taskEditor.OnInspectorGUI();
                        }
                    });
                    taskInspectorContent.Add(container);
                }
                else
                {
                    Debug.Log("... but doesn't have action assigned");
                    assignTaskButton.text = "Bind new action";
                }
            }
            else
            {
                taskContainer.style.display = DisplayStyle.None;
            }
        }

        private void AddBindActionWindow()
        {
            if (m_actionSearchWindow == null)
            {
                m_actionSearchWindow = ScriptableObject.CreateInstance<ActionSearchWindow>();
                m_actionSearchWindow.Initialize(this);
            }
        }
    }
}