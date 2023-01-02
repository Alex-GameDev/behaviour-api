using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.Core
{
    public abstract class BlackboardVariable
    {
        public string Id;
    }

    public class BlackboardVariable<T> : BlackboardVariable
    {
        public T Value; 

        public static implicit operator BlackboardVariable<T>(T value) => new BlackboardVariable<T> { Value = value};
    }
}
