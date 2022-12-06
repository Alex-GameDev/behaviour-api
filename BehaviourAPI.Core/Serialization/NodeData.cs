using System.Collections.Generic;


namespace BehaviourAPI.Core.Serialization
{
    public struct NodeData
    {
        public Node node;
        public List<int> parentIds;
        public List<int> childIds;

        public NodeData(Node node, List<int> parentIds, List<int> childIds)
        {
            this.node = node;
            this.parentIds = parentIds;
            this.childIds = childIds;
        }

        public void BuildNode(BehaviourGraph graph)
        {
            List<Node> parents = graph.Nodes.GetElements(parentIds);
            List<Node> children = graph.Nodes.GetElements(childIds);

            node.BuildConnections(graph, parents, children);
        }
    }
}
