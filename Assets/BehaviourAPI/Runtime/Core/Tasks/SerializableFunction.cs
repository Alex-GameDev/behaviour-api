using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    [System.Serializable]
    public class ActionFunction
    {
        public Component component;
        public string methodName;

        public Func<Status> Build()
        {
            var method = component.GetType().GetMethod(methodName);
            Func<Status> result = Expression.Lambda<Func<Status>>(Expression.Call(Expression.Constant(component), method)).Compile();
            return result;
        }
    }

    [System.Serializable]
    public class PerceptionFunction
    {
        public Component component;
        public string methodName;
    }
}
