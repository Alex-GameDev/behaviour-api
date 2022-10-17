using BehaviourAPI.Runtime.Core;

namespace BehaviourAPI.Runtime.BehaviourTrees
{
    /// <summary>
    /// Composite node that executes its children until one of them returns Failure.
    /// </summary>
    public class SequencerNode : CompositeNode
    {
        public override string Name => "Sequencer";
        public override string Description => "Composite node that executes its childs until one of them returns Failure.";
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