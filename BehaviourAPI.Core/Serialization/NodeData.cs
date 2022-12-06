using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Serialization
{
    public struct NodeData
    {
        public Node node;
        public List<Node> parents;
        public List<Node> children;

        public NodeData(Node node, List<Node> parents, List<Node> children)
        {
            this.node = node;
            this.parents = parents;
            this.children = children;
        }

        internal void Build() => node.BuildConnections(parents, children);
    }
}
