using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    public class BehaviourGraphEditorWindow : EditorWindow
    {
        public static BehaviourEngine Graph;
        /// <summary>
        /// Edit an existing graph.
        /// </summary>
        public static void OpenGraph(BehaviourEngine graph)
        {
            BehaviourGraphEditorWindow wnd = GetWindow<BehaviourGraphEditorWindow>();
            wnd.titleContent = new GUIContent("BehaviourGraphEditorWindow");
            Graph = graph;
        }

        /// <summary>
        /// Create a new behaviour graph.
        /// </summary>
        [MenuItem("BehaviourAPI/EditorWindow")]
        public static void CreateGraph()
        {

        }

        public void CreateGUI()
        {
            AddVisualElements();
            AddStyles();
        }

        private void AddVisualElements()
        {
            BehaviourGraphView behaviourGraphView = new BehaviourGraphView(Graph);
            behaviourGraphView.StretchToParentSize();

            rootVisualElement.Add(behaviourGraphView);

            Inspector inspector = new Inspector();
            rootVisualElement.Add(inspector);
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = VisualSettings.GetOrCreateSettings().VariablesStylesheet;
            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }

}