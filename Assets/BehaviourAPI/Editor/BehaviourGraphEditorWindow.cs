using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace BehaviourAPI.Editor
{
    public class BehaviourGraphEditorWindow : EditorWindow
    {
        [MenuItem("BehaviourAPI/EditorWindow")]
        public static void OpenGraph()
        {
            BehaviourGraphEditorWindow wnd = GetWindow<BehaviourGraphEditorWindow>();
            wnd.titleContent = new GUIContent("BehaviourGraphEditorWindow");
        }

        public void CreateGUI()
        {
            AddVisualElements();
            AddStyles();
        }

        private void OnEnable()
        {

        }


        private void AddVisualElements()
        {
            BehaviourGraphView behaviourGraphView = new BehaviourGraphView();
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