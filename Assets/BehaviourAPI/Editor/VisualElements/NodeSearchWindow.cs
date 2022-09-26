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
        public Type RootType { get; set; }
        BehaviourGraphView graphView;

        public void Initialize(BehaviourGraphView graphView)
        {
            this.graphView = graphView;
            RootType = graphView.BehaviourGraph.NodeType;
        }


        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            TypeNode rootTypeNode = TypeUtilities.GetHierarchyOfType(RootType);
            return CreateSubSearchTree(rootTypeNode, 0);

        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            graphView.CreateNode((Type)SearchTreeEntry.userData, context.screenMousePosition);
            return true;
        }

        private List<SearchTreeEntry> CreateSubSearchTree(TypeNode rootNode, int level)
        {
            List<SearchTreeEntry> list = new List<SearchTreeEntry>();
            if (rootNode.Childs.Count == 0)
            {
                list.Add(new SearchTreeGroupEntry(new GUIContent(rootNode.Type.Name), level));
                rootNode.Childs.ForEach((child) => list.AddRange(CreateSubSearchTree(child, level + 1)));
            }
            else
            {
                list.Add(new SearchTreeEntry(new GUIContent(rootNode.Type.Name))
                {
                    level = level,
                    userData = rootNode.Type
                });
            }
            return list;
        }
    }
}
