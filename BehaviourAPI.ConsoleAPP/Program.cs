using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using System.Text;
using System.Text.Json;

namespace BehaviourAPI.ConsoleAPP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BehaviourTree tree = new BehaviourTree();
            var time = DateTime.Now;

            List<BTNode> nodes = new List<BTNode>();
            for(int i = 0; i < 100; i++)
            {
                List<BTNode> childs = new List<BTNode>();
                for(int j = 0; j < 200; j++)
                {
                    childs.Add(tree.CreateLeafNode($"action_{i}_{j}"));
                }
                nodes.Add(tree.CreateComposite<SequencerNode>($"seq_{i}", childs));
            }
            tree.SetStartNode(tree.CreateComposite<SelectorNode>("root", nodes));
            Console.WriteLine(DateTime.Now - time);
            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);
            writer.WriteStartObject();
            tree.SerializeToJSON(writer);
            writer.WriteEndObject();
            writer.Flush();
            Console.WriteLine(DateTime.Now - time);
            // Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }
}