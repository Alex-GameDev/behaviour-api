namespace BehaviourAPI.UtilitySystems
{
    using Core;

    /// <summary>
    /// Behaviour graph that choose between diferent <see cref="UtilitySelectableNode"/> items and executes.
    /// </summary>
    public class UtilitySystem : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(UtilityNode);
        public override Type ConnectionType => typeof(UtilityConnection);

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

        float UtilityThreshold = 0f;
        float Inertia = 1.3f;        

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

        public FunctionFactor CreateFunctionFactor(string name, Factor child, UtilityFunction curve)
        {
            FunctionFactor curveFactor = CreateNode<FunctionFactor>(name);
            curveFactor.function = curve;
            curveFactor.SetChild(child);
            return curveFactor;
        }

        public T CreateFusionFactor<T>(string name, List<Factor> factors) where T : FusionFactor, new()
        {
            T fusionFactor = CreateNode<T>(name);
            factors.ForEach(factor =>
            {
                CreateConnection<UtilityConnection>(fusionFactor, factor);
                fusionFactor.AddFactor(factor);
            });
            return fusionFactor;
        }

        public T CreateFusionFactor<T>(string name, params Factor[] children) where T : FusionFactor, new()
        {
            return CreateFusionFactor<T>(name, children.ToList());
        }

        public T CreateUtilityElement<T>(string name, Factor factor) where T : UtilityElement, new()
        {
            T utilityExecutable = CreateNode<T>(name);
            CreateConnection<UtilityConnection>(utilityExecutable, factor);
            utilityExecutable.SetFactor(factor);
            return utilityExecutable;
        }

        public UtilityBucket CreateUtilityBucket(string name, List<UtilitySelectableNode> elements, 
            float utilityThreshold = .3f, float inertia = 1.3f, float bucketThreshold = 0f)
        {
            UtilityBucket bucket = CreateNode<UtilityBucket>(name);
            bucket.UtilityThreshold = utilityThreshold;
            bucket.Inertia = inertia;
            bucket.BucketThreshold = bucketThreshold;
            elements.ForEach(elem =>
            {
                CreateConnection<UtilityConnection>(bucket, elem);
                bucket.AddElement(elem);
            });
            return bucket;
        }

        public UtilityBucket CreateUtilityBucket(string name, float utilityThreshold = .3f, 
            float inertia = 1.3f, float bucketThreshold = 0f, params UtilitySelectableNode[] elements)
        {
            return CreateUtilityBucket(name, elements.ToList(), utilityThreshold, inertia, bucketThreshold);
        }

        public override bool SetStartNode(Node node)
        {
            bool defaultutilityElementUpdated = base.SetStartNode(node);
            if (defaultutilityElementUpdated) _utilityCandidates.MoveAtFirst(node as UtilitySelectableNode);
            return defaultutilityElementUpdated;
        }

        public UtilitySystem SetInertia(float inertia)
        {
            Inertia = inertia;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
        }

        public override void Update()
        {
            var newBestAction = ComputeCurrentBestAction();
            if(newBestAction != _currentBestElement)
            {
                _currentBestElement?.Stop();
                _currentBestElement = newBestAction;
                _currentBestElement?.Start();
            }
            _currentBestElement?.Update();
        }

        private UtilitySelectableNode? ComputeCurrentBestAction()
        {
            bool currentElementIsLocked = false;
            float currentHigherUtility = 0f;

            var newBestElement = _currentBestElement;

            for (int i = 0; i < _utilityCandidates.Count; i++)
            {
                var utilityElement = _utilityCandidates[i];
                if (utilityElement == null) break;

                utilityElement.UpdateUtility();
                // The current action utility is mutiplied by the Inertia
                var utility = utilityElement.Utility * (utilityElement == _currentBestElement? Inertia : 1f);

                if(!currentElementIsLocked && utility > currentHigherUtility)
                {
                    newBestElement = utilityElement;
                    currentHigherUtility = utility;
                    if (utility >= float.MaxValue) currentElementIsLocked = true;
                }
            }

            // if max utility is lower than the treshold, executes the default action.
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