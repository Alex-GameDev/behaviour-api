using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.Editor
{
    using Runtime.Core;
    using Utils;
    public class ActionSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        ElementInspector m_elementInspector;

        public void Initialize(ElementInspector elementInspector) => m_elementInspector = elementInspector;
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            TypeNode rootTypeAction = TypeUtilities.GetHierarchyOfType(typeof(ActionTask));
            return CreateSubSearchTree(rootTypeAction, 0);
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (m_elementInspector.IsSelectedElementAnActionAssignable())
            {
                m_elementInspector.BindActionToCurrentNode(SearchTreeEntry.userData as Type);
                return true;
            }
            else
            {
                return false;
            }

        }

        private List<SearchTreeEntry> CreateSubSearchTree(TypeNode rootNode, int level)
        {
            List<SearchTreeEntry> list = new List<SearchTreeEntry>();
            if (rootNode.Childs == null || rootNode.Childs.Count == 0)
            {
                list.Add(new SearchTreeEntry(new GUIContent(rootNode.Type.Name))
                {
                    level = level,
                    userData = rootNode.Type
                });
            }
            else
            {
                list.Add(new SearchTreeGroupEntry(new GUIContent(rootNode.Type.Name), level));
                foreach (var child in rootNode.Childs)
                {
                    list.AddRange(CreateSubSearchTree(child, level + 1));
                }

            }
            return list;
        }
    }
}

