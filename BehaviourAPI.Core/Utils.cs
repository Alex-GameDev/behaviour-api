using BehaviourAPI.Core.Perceptions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.Core
{
    public static class Utils
    {
        public static Status Inverted(this Status status)
        {
            if (status == Status.Success) return Status.Failure;
            if (status == Status.Failure) return Status.Success;
            else return status;
        }

        public static Status ToStatus(this bool check, Status valueIfFalse = Status.Failure)
        {
            return check ? Status.Success : valueIfFalse;
        }

        public static void MoveAtFirst<T>(this List<T> list, T element)
        {
            if(list.Remove(element)) list.Insert(0, element);
        }

        public static StatusFlags GetFlags(this Status status)
        {
            switch(status)
            {
                case Status.Success: return StatusFlags.Success;
                case Status.Failure: return StatusFlags.Failure;
                case Status.Running: return StatusFlags.Running;
                default: return StatusFlags.None;
            }
        }

        public static List<T> GetElements<T>(this List<T> list, List<int> indexes)
        {
            List<T> selectedElements = new List<T>();
            for(int i = 0; i < indexes.Count(); i++)
            {
                selectedElements.Add(list[indexes[i]]);
            }
            return selectedElements;
        }
    }
}
