using System.Collections.Generic;

namespace BehaviourAPI.Core.Serialization
{
    public class BehaviourGraphBuilder
    {
        BehaviourGraph graph;

        List<NodeData> nodes;

        public BehaviourGraphBuilder(BehaviourGraph graph)
        {
            this.graph = graph;
            this.nodes = new List<NodeData>();
        }

        public void AddNode(NodeData nodeData)
        {
            nodes.Add(nodeData);
            graph.AddNode(nodeData.node);
        }

        public void Build()
        {
            nodes.ForEach(n => n.Build());
        }
    }
}
