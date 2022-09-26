using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourAPI.Editor
{
    using Utils;
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        BehaviourGraphView graphView;

        public void Initialize(BehaviourGraphView graphView)
        {
            this.graphView = graphView;
        }


        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            TypeNode rootTypeNode = TypeUtilities.GetHierarchyOfType(graphView.BehaviourGraph.NodeType);
            return CreateSubSearchTree(rootTypeNode, 0);
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 position = graphView.GetLocalMousePosition(context.screenMousePosition);
            graphView.CreateNode((Type)SearchTreeEntry.userData, position);
            return true;
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

