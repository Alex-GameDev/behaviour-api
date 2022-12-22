using System;

namespace BehaviourAPI.Core
{
    public class Variable<T>
    {
        public T Value;

        public static implicit operator Variable<T>(T value) => new Variable<T> { Value = value };
    }
}
