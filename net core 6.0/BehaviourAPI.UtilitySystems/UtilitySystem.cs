namespace BehaviourAPI.UtilitySystems
{
    using Core;
    using Core.Actions;
    /// <summary>
    /// Behaviour graph that choose between diferent <see cref="UtilitySelectableNode"/> items and executes.
    /// </summary>
    public class UtilitySystem : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(UtilityNode);

        public override bool CanRepeatConnection => false;

        public override bool CanCreateLoops => false;

        public UtilitySelectableNode? DefaultSelectedElement
        {
            get
            {
                if (_utilityCandidates.Count == 0) return null;
                else return _utilityCandidates[0];
            }
        }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public float UtilityThreshold = 0f;
        public float Inertia = 1.3f;        

        List<UtilitySelectableNode?> _utilityCandidates;

        UtilitySelectableNode? _currentBestElement;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public UtilitySystem(float inertia = 1.3f, float utilityThreshold = 0f)
        {
            Inertia = inertia;
            UtilityThreshold = utilityThreshold;
            _utilityCandidates = new List<UtilitySelectableNode?>();
        }

        public VariableFactor CreateVariableFactor(string name, Func<float> func, float min, float max)
        {
            VariableFactor variableFactor = CreateNode<VariableFactor>(name);
            variableFactor.Variable = func;
            variableFactor.min = min;
            variableFactor.max = max;
            return variableFactor;
        }

        public T CreateFunctionFactor<T>(string name, Factor child) where T : FunctionFactor, new()
        {
            T curveFactor = CreateNode<T>(name);
            Connect(curveFactor, child);
            curveFactor.SetChild(child);
            return curveFactor;
        }

        public T CreateFusionFactor<T>(string name, List<Factor> factors) where T : FusionFactor, new()
        {
            T fusionFactor = CreateNode<T>(name);
            factors.ForEach(factor =>
            {
                Connect(fusionFactor, factor);
                fusionFactor.AddFactor(factor);
            });
            return fusionFactor;
        }

        public T CreateFusionFactor<T>(string name, params Factor[] children) where T : FusionFactor, new()
        {
            return CreateFusionFactor<T>(name, children.ToList());
        }

        public UtilityAction CreateUtilityAction(string name, Factor factor, Action? action = null, bool root = true, bool finishOnComplete = false) 
        {
            UtilityAction utilityExecutable = CreateNode<UtilityAction>(name);
            utilityExecutable.FinishSystemOnComplete = finishOnComplete;
            utilityExecutable.Action = action;
            Connect(utilityExecutable, factor);
            if (root) _utilityCandidates.Add(utilityExecutable);
            utilityExecutable.SetFactor(factor);
            return utilityExecutable;
        }

        public UtilityBucket CreateUtilityBucket(string name, List<UtilitySelectableNode> elements, bool root = true,
            float utilityThreshold = .3f, float inertia = 1.3f, float bucketThreshold = 0f)
        {
            UtilityBucket bucket = CreateNode<UtilityBucket>(name);
            bucket.UtilityThreshold = utilityThreshold;
            bucket.Inertia = inertia;
            bucket.BucketThreshold = bucketThreshold;
            if (root) _utilityCandidates.Add(bucket);
            elements.ForEach(elem =>
            {
                Connect(bucket, elem);
                bucket.AddElement(elem);
            });
            return bucket;
        }

        public UtilityBucket CreateUtilityBucket(string name, bool root = true, float utilityThreshold = .3f, 
            float inertia = 1.3f, float bucketThreshold = 0f, params UtilitySelectableNode[] elements)
        {
            return CreateUtilityBucket(name, elements.ToList(), root, utilityThreshold, inertia, bucketThreshold);
        }

        public override bool SetStartNode(Node node)
        {
            bool defaultUtilityElementUpdated = base.SetStartNode(node);
            if (defaultUtilityElementUpdated) _utilityCandidates.MoveAtFirst(node as UtilitySelectableNode);
            return defaultUtilityElementUpdated;
        }

        public override void Initialize()
        {
            base.Initialize();
            Nodes.ForEach(node =>
            {
                if(node.Parents.Count == 0)
                {
                    _utilityCandidates.Add(node as UtilitySelectableNode);
                }
            });
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Execute()
        {
            var newBestAction = ComputeCurrentBestAction();
            // If the best action changes:
            if(newBestAction != _currentBestElement)
            {
                _currentBestElement?.Stop();
                _currentBestElement = newBestAction;
                _currentBestElement?.Start();
            }
            _currentBestElement?.Update();

            // If the executed action finish and the "finish on complete" flag is true, the utility systems finish too.
            if (_currentBestElement?.FinishExecutionWhenActionFinishes() ?? false && _currentBestElement.Status != Status.Running) Finish(_currentBestElement.Status);
        }

        private UtilitySelectableNode? ComputeCurrentBestAction()
        {
            float currentHigherUtility = -1f; // If value starts in 0, elems with Utility == 0 cant be executed

            UtilitySelectableNode? newBestElement = _currentBestElement;

            int i = 0;
            var currentActionIsLocked = false; // True if current action is a locked bucket.

            while (i < _utilityCandidates.Count && !currentActionIsLocked)
            {
                // Update utility
                var currentAction = _utilityCandidates[i];
                if (currentAction == null) continue;
                currentAction.UpdateUtility();

                // Compute the current action utility:
                var utility = currentAction.Utility;
                if (currentAction == _currentBestElement) utility *= Inertia;

                // If it's higher than the current max utility, update the selection.
                if(utility > currentHigherUtility)
                {
                    newBestElement = currentAction;
                    currentHigherUtility = utility;

                    // If the action is a locked bucket:
                    if (currentAction.ExecutionPriority) currentActionIsLocked = true;
                }
                i++;
            }

            // If utility is lower than the threshold, execute the default action:
            if (currentHigherUtility < UtilityThreshold) newBestElement = DefaultSelectedElement;

            return newBestElement;
        }

        public override void Stop()
        {
            _currentBestElement?.Stop();
            _currentBestElement = null;
        }

        #endregion
    }
}