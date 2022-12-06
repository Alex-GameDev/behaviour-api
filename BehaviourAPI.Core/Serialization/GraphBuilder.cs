using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core.Serialization
{
    public class GraphBuilder
    {
        BehaviourGraph graph;

        List<NodeData> nodes;

        public GraphBuilder(BehaviourGraph graph)
        {
            this.graph = graph;
            nodes = new List<NodeData>();
        }

        public void AddNode(NodeData nodeData)
        {
            nodes.Add(nodeData);
            graph.AddNode(nodeData.node);
        }

        public void Build()
        {
            nodes.ForEach(n => n.BuildNode(graph));
        }
    }
}
