using BehaviourAPI.Runtime.Core;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    /// <summary>
    /// Composite node that executes its children until one of them returns Failure.
    /// </summary>
    public class SequencerNode : CompositeNode
    {
        public SequencerNode()
        {
        }

        protected override Status GetModifiedChildStatus(Status status)
        {
            if (status == Status.Sucess)
            {
                if (TryGoToNextChild())

                    return Status.Running;
                else
                    return Status.Sucess;
            }
            else
            {
                return status;
            }
        }
    }
}