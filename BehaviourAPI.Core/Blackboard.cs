using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core
{
    public class Blackboard
    {
        Dictionary<string, BlackboardVariable> variables;

        public BlackboardVariable<T> CreateVariable<T>(string id, T value)
        {
            var variable = new BlackboardVariable<T> { Id = id, Value = value };
            variables.Add(id, variable);
            return variable;
        }

        public BlackboardVariable<T> GetVariable<T>(string id)
        {
            if(variables.TryGetValue(id, out var variable))
            {
                if(variable is BlackboardVariable<T> typedvariable)
                {
                    return typedvariable;
                }
            }
            return null;
        }
    }
}
