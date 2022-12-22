namespace BehaviourAPI.BehaviourTrees
{
    using BehaviourAPI.BehaviourTrees.Decorators;
    using BehaviourAPI.Core.Exceptions;
    using Core;
    /// <summary>
    /// Node that execute its child node until returns a given value.
    /// </summary>
    public class LoopUntilNode : DirectDecoratorNode
    {

        #region ------------------------------------------- Fields -------------------------------------------

        public Variable<Status> TargetStatus = Status.Success;

        public Variable<int> MaxIterations = -1;

        int _currentIterations;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public LoopUntilNode SetTargetStatus(Variable<Status> status)
        {
            TargetStatus = status;
            return this;
        }

        public LoopUntilNode SetMaxIterations(Variable<int> maxIterations)
        {
            MaxIterations = maxIterations;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();
            _currentIterations = 0;
        }

        protected override Status UpdateStatus()
        {
            if (m_childNode == null)
                throw new MissingChildException(this);

            m_childNode.Update();
            var status = m_childNode.Status;
            // If child execution ends without the target value, restart until currentIterations == MaxIterations
            if (status == TargetStatus.Value.Inverted())
            {
                _currentIterations++;
                if(_currentIterations != MaxIterations.Value)
                {
                    // Restart the node execution
                    status = Status.Running;
                    m_childNode.Stop();
                    m_childNode.Start();
                }                
            }
            return status;
        }
        #endregion

    }
}