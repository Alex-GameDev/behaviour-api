using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI
{
    using UnityEngine;
    using UnityEditor;
    using Runtime.Core;
    using System.Linq;
    using BehaviourAPI.Runtime;

    [CustomPropertyDrawer(typeof(ActionFunction))]
    public class ActionFunctionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // var sceneObjectProperty = property.FindPropertyRelative("sceneObject");
            // var componentNameProperty = property.FindPropertyRelative("componentName");

            // var sceneObject = sceneObjectProperty.objectReferenceValue as GameObject;
            // var componentName = componentNameProperty.stringValue;

            // EditorGUI.ObjectField(position, sceneObjectProperty);
            // if (sceneObjectProperty.objectReferenceValue != null)
            // {
            //     var components = sceneObject.GetComponents<Component>().ToList();
            //     var componentNames = new List<string>();
            //     components.ForEach(comp => componentNames.Add(comp.GetType().Name));
            //     componentId = componentNames.FindIndex(0, components.Count, str => componentName.Equals(str));
            //     componentId = EditorGUILayout.Popup(componentId, componentNames.ToArray());
            //     if (componentId >= 0)
            //     {
            //         componentNameProperty.stringValue = componentNames[componentId];
            //         Debug.Log($"update component name {componentName}");
            //         componentName = componentNameProperty.stringValue
            //     }

            //     if (!string.IsNullOrEmpty(componentName))
            //     {
            //         var methodNameProperty = property.FindPropertyRelative("methodName");
            //         var methodInfo = components
            //         var methodNames = new List<string>();

            //     }
            // }
            var componentProperty = property.FindPropertyRelative("component");
            if (!DisplayComponentProperty(componentProperty)) return;
            var methodNameProperty = property.FindPropertyRelative("methodName");
            DisplayMethodNameProperty(methodNameProperty, componentProperty.objectReferenceValue as Component);
        }

        private bool DisplayComponentProperty(SerializedProperty componentProperty)
        {
            EditorGUILayout.ObjectField(componentProperty);
            return componentProperty.objectReferenceValue != null;
        }

        private void DisplayMethodNameProperty(SerializedProperty methodNameProperty, Component component)
        {
            var methodName = methodNameProperty.stringValue;
            var methods = component.GetType().GetMethods().ToList()
                .FindAll(x => x.GetCustomAttributes(typeof(TaskMethodAttribute), false).Length > 0)
                .FindAll(x => x.ReturnParameter.ParameterType == typeof(Status))
                .FindAll(x => x.GetParameters().Length == 0);

            var methodNames = methods.Select(x => x.Name).ToList();

            int methodNameIndex = methodNames.FindIndex(str => str.Equals(methodName));
            methodNameIndex = EditorGUILayout.Popup(methodNameIndex, methodNames.ToArray());

            if (methodNameIndex != -1)
            {
                methodNameProperty.stringValue = methodNames[methodNameIndex];
            }
        }
    }
}
