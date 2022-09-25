using UnityEngine;
using System.Collections.Generic;
using System;

namespace BehaviourAPI.Runtime.Core
{
    public abstract class BehaviourEngine
    {

        /// <summary>
        /// The base type of the <see cref="Node"/> elements that this <see cref="BehaviourEngine"/> can contain.
        /// </summary>
        public abstract System.Type NodeType { get; }

        /// <summary>
        /// The list of all <see cref="Node"/> elements in this <see cref="BehaviourEngine"/>.
        /// </summary>
        public List<Node> Nodes { get; }

        /// <summary>
        /// The default entry point of the graph
        /// </summary>
        public Node StartNode { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public BehaviourEngine()
        {
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Create a new node in the graph
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Node AddNode(System.Type type, Vector2 pos = default)
        {
            if (type.IsSubclassOf(NodeType))
            {
                Node node = (Node)System.Activator.CreateInstance(type);
                node.Position = pos;
                Nodes.Add(node);
                return node;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Remove a specific node from the graph.
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
        }
    }
}
