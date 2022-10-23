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

        public void UpdateInspector(GraphElement element, bool refresh = false)
        {
            if (element == SelectedElement && !refresh) return;

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

                if (DisplayTaskEditor(element as ITaskHandler<ActionTask>)) return;
                if (DisplayTaskEditor(element as ITaskHandler<Perception>)) return;
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
            if (SelectedElement is ITaskHandler<ActionTask> actionAsignable)
            {
                actionAsignable.Task = ScriptableObject.CreateInstance(type) as ActionTask;
                UpdateInspector(SelectedElement, true);
                EditorUtility.SetDirty(SelectedElement);
            }
        }

        public bool IsSelectedElementAnActionAssignable() => (SelectedElement as ITaskHandler<ActionTask>) != null;
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
            if (SelectedElement is ITaskHandler<ActionTask> actionAsignable)
            {
                if (actionAsignable.Task != null)
                {
                    actionAsignable.Task = null;
                    EditorUtility.SetDirty(SelectedElement);
                    UpdateInspector(SelectedElement, true);
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

        private bool DisplayTaskEditor<T>(ITaskHandler<T> actionAsignable) where T : Task
        {
            if (actionAsignable != null)
            {
                taskInspectorContent.Clear();
                taskContainer.style.display = DisplayStyle.Flex;
                UnityEngine.Object.DestroyImmediate(taskEditor);
                if (actionAsignable.Task != null)
                {
                    assignTaskButton.text = $"Remove current {typeof(T).Name}";
                    taskEditor = UnityEditor.Editor.CreateEditor(actionAsignable.Task);
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
                    assignTaskButton.text = $"Bind new {typeof(T).Name}";
                }
                return true;
            }
            else
            {
                taskContainer.style.display = DisplayStyle.None;
                return false;
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