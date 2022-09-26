using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BehaviourAPI.Utils
{
    public static class TypeUtilities
    {
        public static TypeNode GetHierarchyOfType(Type rootType)
        {
            return new TypeNode(rootType, GetAllTypesDerivedFrom(rootType));
        }

        public static Assembly[] GetAllAssemblies() => AppDomain.CurrentDomain.GetAssemblies();

        public static List<Type> GetAllTypesDerivedFrom(Type rootType)
        {
            List<Type> allTypes = new List<Type>();
            GetAllAssemblies().ToList().ForEach(assembly => allTypes.AddRange(assembly.GetTypes()));
            List<Type> derivedTypes = allTypes.Where((type) => type.IsSubclassOf(rootType)).ToList();
            return derivedTypes;
        }
    }

    public class TypeNode
    {
        public Type Type { get; private set; }
        public List<TypeNode> Childs { get; private set; }
        public TypeNode(Type rootType, List<Type> derivedTypes)
        {
            Type = rootType;
            Childs = new List<TypeNode>();
            var subclasses = derivedTypes.Where(t => t.BaseType == rootType);

            foreach (var c in subclasses)
            {
                var derived = derivedTypes.Where(t => t.IsSubclassOf(rootType) && t != rootType);
                Childs.Add(new TypeNode(c, derived.ToList()));
            }
        }

        public override String ToString()
        {
            string str = $"{Type.Name} (";
            Childs.ForEach(t => str += t.ToString());
            str += ")";
            return str;
        }
    }
}