using BehaviourAPI.Core;
using BehaviourAPI.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviourAPI.BehaviourTrees.Decorators
{
    /// <summary>
    /// Decorator that always execute its child.
    /// </summary>
    public abstract class DirectDecoratorNode : DecoratorNode
    {
        public override void Start()
        {
            base.Start();

            if (m_childNode == null)
                throw new MissingChildException(this);

            m_childNode.Start();
        }

        public override void Stop()
        {
            base.Stop();

            if (m_childNode == null)
                throw new MissingChildException(this);

            m_childNode.Stop();
        }
    }
}
