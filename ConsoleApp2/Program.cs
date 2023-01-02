using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Action = BehaviourAPI.Core.Actions.Action;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var size = 10000000;
            //var time = DateTime.Now;
            //var c = 0;

            //var iterator = new LeafNode();

            //for(int i = 0; i < size; i++)
            //{
            //    iterator.Action = new FunctionalAction(()=> c = i, ()=> Status.Success);
            //}
            //Console.WriteLine((DateTime.Now - time).TotalMilliseconds);

            //time = DateTime.Now;
            //for (int i = 0; i < size; i++)
            //{
            //    var list = iterator.GetType().GetFields().ToList().FindAll(f => f.IsPublic && f.FieldType == typeof(Action));
            //    var field = list[0];
            //    field.SetValue(iterator, new FunctionalAction(() => c = i, () => Status.Success));
            //}
            //Console.WriteLine((DateTime.Now - time).TotalMilliseconds);

            //time = DateTime.Now;
            //for (int i = 0; i < size; i++)
            //{
            //    var field = iterator.GetType().GetField("action1");
            //    field.SetValue(iterator, new FunctionalAction(() => c = i, () => Status.Success));
            //}
            //Console.WriteLine((DateTime.Now - time).TotalMilliseconds);

            var tree = new BehaviourTree();
            var root = tree.CreateComposite<SequencerNode>(true,
                tree.CreateLeafNode(new FunctionalAction(() => Console.WriteLine("A"))),
                tree.CreateLeafNode(new FunctionalAction(() => Console.WriteLine("B"))),
                tree.CreateLeafNode(new FunctionalAction(() => Console.WriteLine("C"))));

            tree.SetRootNode(root);
            tree.Start();
            tree.Update();
            tree.Update();
            Console.WriteLine("A");
        }
    }
}
