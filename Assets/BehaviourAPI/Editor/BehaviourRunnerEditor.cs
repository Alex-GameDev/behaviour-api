using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    using Runtime.BehaviourTrees;

    [CustomEditor(typeof(BehaviourRunner))]
    public class BehaviourRunnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            BehaviourRunner behaviourRunner = target as BehaviourRunner;

            if (behaviourRunner.RootGraph != null)
            {
                if (GUILayout.Button($"Edit Behaviour Graph ({behaviourRunner.RootGraph.GetType().Name})"))
                {
                    BehaviourGraphEditorWindow.OpenGraph(behaviourRunner.RootGraph);
                }
            }
            else
            {
                if (GUILayout.Button("Create Behaviour Graph"))
                {
                    behaviourRunner.RootGraph = new BehaviourTree();
                    BehaviourGraphEditorWindow.OpenGraph(behaviourRunner.RootGraph);
                }
            }
        }
    }
}

