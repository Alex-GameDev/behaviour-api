using System.Collections.Generic;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    using System;
    using System.Linq;
    using Core;
    /// <summary>
    /// Composite node that executes its children until one of them returns Succeded.
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        public bool IsRandomOrder { get; set; }
        public SelectorNode()
        {

        }

        protected override Status GetModifiedChildStatus(Status status)
        {
            if (status == Status.Failure)
            {
                if (TryGoToNextChild())
                    return Status.Running;
                else
                    return Status.Failure;
            }
            else
            {
                return status;
            }
        }

        protected override void InitializeList()
        {
            base.InitializeList();
            if (IsRandomOrder) GetChilds().OrderBy((guid) => Guid.NewGuid());
        }
    }

}