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
        public GraphElement SelectedElement { get; private set; }
        public ElementInspector()
        {
            AddLayout();
            AddStyles();
            AddBindActionWindow();
        }

        public void UpdateInspector(GraphElement element)
        {
            if (element == SelectedElement) return;

            elementInspectorContent.Clear();

            if (element != null)
            {
                SelectedElement = element;
                nameLabel.text = element.Name;
                descLabel.text = element.Description;

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
            else
            {
                ClearInspector();
            }
        }

        public void ClearInspector()
        {
            elementInspectorContent.Clear();
            nameLabel.text = "---";
            descLabel.text = "";
            taskInspectorContent.Clear();
            taskContainer.style.display = DisplayStyle.None;
        }

        public void BindActionToCurrentNode(Type type)
        {
            if (SelectedElement is IActionAsignable actionAsignable)
            {
                actionAsignable.Action = ScriptableObject.CreateInstance(type) as ActionTask;
                UpdateInspector(SelectedElement);
                EditorUtility.SetDirty(SelectedElement);
            }
        }

        public bool IsSelectedElementAnActionAssignable() => (SelectedElement as IActionAsignable) != null;
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
            if (SelectedElement is IActionAsignable actionAsignable)
            {
                if (actionAsignable.Action != null)
                {
                    actionAsignable.Action = null;
                    EditorUtility.SetDirty(SelectedElement);
                    UpdateInspector(SelectedElement);
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
                taskInspectorContent.Clear();
                taskContainer.style.display = DisplayStyle.Flex;
                UnityEngine.Object.DestroyImmediate(taskEditor);
                if (actionAsignable.Action != null)
                {
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