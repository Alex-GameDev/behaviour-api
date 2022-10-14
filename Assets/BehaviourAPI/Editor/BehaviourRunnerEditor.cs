using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    using Runtime.BehaviourTrees;
    using Runtime.UtilitySystems;

    [CustomEditor(typeof(BehaviourRunner))]
    public class BehaviourRunnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            BehaviourRunner behaviourRunner = (BehaviourRunner)target;
            if (behaviourRunner.RootGraph != null)
            {
                if (GUILayout.Button($"Edit Behaviour Graph ({behaviourRunner.RootGraph.GetType().Name})"))
                {
                    BehaviourGraphEditorWindow.OpenGraph(behaviourRunner.RootGraph);
                }
                if (GUILayout.Button($"Remove Behaviour Graph"))
                {
                    behaviourRunner.RootGraph = null;
                    EditorUtility.SetDirty(behaviourRunner);
                }
            }
            else
            {
                if (GUILayout.Button("Bind Behaviour Tree Graph"))
                {
                    behaviourRunner.RootGraph = ScriptableObject.CreateInstance(typeof(BehaviourTree)) as BehaviourEngine;
                    BehaviourGraphEditorWindow.OpenGraph(behaviourRunner.RootGraph);
                }
                if (GUILayout.Button("Bind Behaviour Tree Graph"))
                {
                    behaviourRunner.RootGraph = ScriptableObject.CreateInstance(typeof(BehaviourTree)) as BehaviourEngine;
                    BehaviourGraphEditorWindow.OpenGraph(behaviourRunner.RootGraph);
                }
                if (GUILayout.Button("Bind Utility System Graph"))
                {
                    behaviourRunner.RootGraph = ScriptableObject.CreateInstance(typeof(UtilitySystem)) as BehaviourEngine;
                    BehaviourGraphEditorWindow.OpenGraph(behaviourRunner.RootGraph);
                }
            }
        }
    }
}

