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

            var action_1 = tree.CreateLeafNode("Nodo 1");
            action_1.Action = new FunctionalAction(() => Status.Failure);
            var action_2 = tree.CreateLeafNode("Nodo 2");
            action_2.Action = new FunctionalAction(() => Status.Success);
            var action_3 = tree.CreateLeafNode("Nodo 3");
            action_3.Action = new FunctionalAction(() => Status.Failure);
            var composite_1 = tree.CreateComposite<SelectorNode>("Sel_1", false, action_1, action_2, action_3);

            var action_4 = tree.CreateLeafNode("Nodo 4");
            action_4.Action = new FunctionalAction(() => Status.Success);
            var action_5 = tree.CreateLeafNode("Nodo 5");
            action_5.Action = new FunctionalAction(() => Status.Success);
            var action_6 = tree.CreateLeafNode("Nodo 6");
            action_6.Action = new FunctionalAction(() => Status.Success);
            var composite_2 = tree.CreateComposite<SequencerNode>("Seq_2", false, action_4, action_5, action_6);

            var action_7 = tree.CreateLeafNode("Nodo 7");
            action_7.Action = new FunctionalAction(() => Status.Success);
            var composite_root = tree.CreateComposite<SequencerNode>("Seq", false, composite_1, composite_2, action_7);

            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);
            writer.WriteStartObject();
            tree.SerializeToJSON(writer);
            writer.WriteEndObject();
            writer.Flush();
            Console.WriteLine(DateTime.Now - time);
            Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }
}