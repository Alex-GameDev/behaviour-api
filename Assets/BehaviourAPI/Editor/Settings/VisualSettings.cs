using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace BehaviourAPI.Editor
{
    [CreateAssetMenu(fileName = "Behaviour API Visual Settings", menuName = "BehaviourAPI/Editor/VisualSettings", order = 0)]
    public class VisualSettings : ScriptableObject
    {
        public StyleSheet VariablesStylesheet;
        public StyleSheet GraphStylesheet;

        public VisualTreeAsset InspectorLayout;
        public StyleSheet InspectorStylesheet;

        public VisualTreeAsset NodeLayout;
        public StyleSheet NodeStylesheet;

        public static VisualSettings GetOrCreateSettings()
        {
            var settings = FindSettings();
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<VisualSettings>();
                AssetDatabase.CreateAsset(settings, "Assets");
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        private static VisualSettings FindSettings()
        {
            var guids = AssetDatabase.FindAssets("t:VisualSettings");
            if (guids.Length == 0)
            {
                return null;
            }
            else
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<VisualSettings>(path);
            }
        }
    }

}