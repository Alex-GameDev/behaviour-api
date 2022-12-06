using System.Collections.Generic;

namespace BehaviourAPI.UtilitySystems
{
    using BehaviourAPI.Core.Exceptions;
    using Core;
    using Core.Actions;
    using System;
    using System.Linq;
    using System.Net.Sockets;
    using Action = Core.Actions.Action;

    /// <summary>
    /// Behaviour graph that choose between diferent <see cref="UtilitySelectableNode"/> items and executes.
    /// </summary>
    public class UtilitySystem : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(UtilityNode);

        public override bool CanRepeatConnection => false;

        public override bool CanCreateLoops => false;

        public UtilitySelectableNode DefaultSelectedElement
        {
            get
            {
                if (_utilityCandidates.Count == 0) 
                    throw new EmptyGraphException(this, "The list of utility candidates is empty.");
                
                return _utilityCandidates[0];
            }
        }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        public float UtilityThreshold = 0f;
        public float Inertia = 1.3f;        

        List<UtilitySelectableNode> _utilityCandidates;

        UtilitySelectableNode _currentBestElement;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Creates a new <see cref="UtilitySystem"/>
        /// </summary>
        /// <param name="inertia">The utility multiplier applied to the last selected element when the best element is calculated.</param>
        /// <param name="utilityThreshold">The minimum utility value an element must have to be selected.</param>
        public UtilitySystem(float inertia = 1.3f, float utilityThreshold = 0f)
        {
            Inertia = inertia;
            UtilityThreshold = utilityThreshold;
            _utilityCandidates = new List<UtilitySelectableNode>();
        }

        /// <summary>
        /// Create a new <see cref="VariableFactor"/> named <paramref name="name"/> in this <see cref="UtilitySystem"/> that computes its utility value with the result
        /// of the delegate function specified in <paramref name="func"/>, normalized between 0 and 1 using <paramref name="min"/> and <paramref name="max"/> values. 
        /// </summary>
        /// <param name="name">The name of the variable factor.</param>
        /// <param name="func">The function delegate that executes this factor.</param>
        /// <param name="min">The minimum expected value of the result of <paramref name="func"/></param>
        /// <param name="max">The maximum expected value of the result of <paramref name="func"/></param>
        /// <returns>The <see cref="VariableFactor"/> created.</returns>
        public VariableFactor CreateVariableFactor(string name, Func<float> func, float min, float max)
        {
            VariableFactor variableFactor = CreateNode<VariableFactor>(name);
            variableFactor.Variable = func;
            variableFactor.min = min;
            variableFactor.max = max;
            return variableFactor;
        }

        /// <summary>
        /// Create a new <see cref="VariableFactor"/> in this <see cref="UtilitySystem"/> that computes its utility value with the result
        /// of the delegate function specified in <paramref name="func"/>, normalized between 0 and 1 using <paramref name="min"/> and <paramref name="max"/> values. 
        /// </summary>
        /// <param name="func">The function delegate that executes this factor.</param>
        /// <param name="min">The minimum expected value of the result of <paramref name="func"/></param>
        /// <param name="max">The maximum expected value of the result of <paramref name="func"/></param>
        /// <returns>The <see cref="VariableFactor"/> created.</returns>
        public VariableFactor CreateVariableFactor(Func<float> func, float min, float max)
        {
            VariableFactor variableFactor = CreateNode<VariableFactor>();
            variableFactor.Variable = func;
            variableFactor.min = min;
            variableFactor.max = max;
            return variableFactor;
        }

        /// <summary>
        /// Create a new function factor of type <typeparamref name="T"/> named <paramref name="name"/> that computes its utility value modifying the utility of <paramref name="child"/> factor.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="name">The name of the factor.</param>
        /// <param name="child">The child factor.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFunctionFactor<T>(string name, Factor child) where T : FunctionFactor, new()
        {
            T curveFactor = CreateNode<T>(name);
            Connect(curveFactor, child);
            curveFactor.SetChild(child);
            return curveFactor;
        }

        /// <summary>
        /// Create a new function factor of type <typeparamref name="T"/> that computes its utility value modifying the utility of <paramref name="child"/> factor.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="child">The child factor.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFunctionFactor<T>(Factor child) where T : FunctionFactor, new()
        {
            T curveFactor = CreateNode<T>();
            Connect(curveFactor, child);
            curveFactor.SetChild(child);
            return curveFactor;
        }

        /// <summary>
        /// Create a new fusion factor of type <typeparamref name="T"/> named <paramref name="name"/> that combines the utility of <paramref name="factors"/>.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="name">The name of the factor.</param>
        /// <param name="factors">The list of child factors.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
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

        /// <summary>
        /// Create a new fusion factor of type <typeparamref name="T"/> that combines the utility of <paramref name="factors"/>.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="factors">The list of child factors.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFusionFactor<T>(List<Factor> factors) where T : FusionFactor, new()
        {
            T fusionFactor = CreateNode<T>();
            factors.ForEach(factor =>
            {
                Connect(fusionFactor, factor);
                fusionFactor.AddFactor(factor);
            });
            return fusionFactor;
        }

        /// <summary>
        /// Create a new fusion factor of type <typeparamref name="T"/> named <paramref name="name"/> that combines the utility of <paramref name="children"/>.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="name">The name of the factor.</param>
        /// <param name="children">The child factors.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFusionFactor<T>(string name, params Factor[] children) where T : FusionFactor, new()
        {
            return CreateFusionFactor<T>(name, children.ToList());
        }

        /// <summary>
        /// Create a new fusion factor of type <typeparamref name="T"/> that combines the utility of <paramref name="children"/>.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="children">The child factors.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFusionFactor<T>(params Factor[] children) where T : FusionFactor, new()
        {
            return CreateFusionFactor<T>(children.ToList());
        }

        /// <summary>
        /// Create a new <see cref="UtilityAction"/> named <paramref name="name"/> that computes its utility using <paramref name="factor"/> and executes the action specified in <paramref name="action"/>.
        /// To prevent the action from being added to the <see cref="UtilitySystem"/> candidate list, set <paramref name="root"/> to false (default is true).
        /// To make the <see cref="UtilitySystem"/> execution ends when the action ends, set <paramref name="finishOnComplete"/> to true (default is false).
        /// </summary>
        /// <param name="name">The name of the utility action.</param>
        /// <param name="factor">The child factor of the action.</param>
        /// <param name="action">The action executed.</param>
        /// <param name="root">true if the action is added to the selectable element list, false otherwise."</param>
        /// <param name="finishOnComplete">true of the execution of the utility system must finish when the action finish.</param>
        /// <returns>The created <see cref="UtilityAction"/></returns>
        public UtilityAction CreateUtilityAction(string name, Factor factor, Action action = null, bool root = true, bool finishOnComplete = false) 
        {
            UtilityAction utilityExecutable = CreateNode<UtilityAction>(name);
            utilityExecutable.FinishSystemOnComplete = finishOnComplete;
            utilityExecutable.Action = action;
            utilityExecutable.IsRoot = root;
            Connect(utilityExecutable, factor);
            if (root) _utilityCandidates.Add(utilityExecutable);
            utilityExecutable.SetFactor(factor);
            return utilityExecutable;
        }

        /// <summary>
        /// Create a new <see cref="UtilityAction"/> that computes its utility using <paramref name="factor"/> and executes the action specified in <paramref name="action"/>.
        /// To prevent the action from being added to the <see cref="UtilitySystem"/> candidate list, set <paramref name="root"/> to false (default is true).
        /// To make the <see cref="UtilitySystem"/> execution ends when the action ends, set <paramref name="finishOnComplete"/> to true (default is false).
        /// </summary>
        /// <param name="factor">The child factor of the action.</param>
        /// <param name="action">The action executed.</param>
        /// <param name="root">true if the action is added to the selectable element list, false otherwise."</param>
        /// <param name="finishOnComplete">true of the execution of the utility system must finish when the action finish.</param>
        /// <returns>The created <see cref="UtilityAction"/></returns>
        public UtilityAction CreateUtilityAction(Factor factor, Action action = null, bool root = true, bool finishOnComplete = false)
        {
            UtilityAction utilityExecutable = CreateNode<UtilityAction>();
            utilityExecutable.FinishSystemOnComplete = finishOnComplete;
            utilityExecutable.Action = action;
            utilityExecutable.IsRoot = root;
            Connect(utilityExecutable, factor);
            if (root) _utilityCandidates.Add(utilityExecutable);
            utilityExecutable.SetFactor(factor);
            return utilityExecutable;
        }

        /// <summary>
        /// Create a new <see cref="UtilityBucket"/> named <paramref name="name"/> in this <see cref="UtilitySystem"/> that groups the elements specified in <paramref name="elements"/>.
        /// </summary>
        /// <param name="name">The name of the bucket.</param>
        /// <param name="elements">The elements contained by the bucket.</param>
        /// <param name="root">true if the action is added to the selectable element list, false otherwise."</param>
        /// <param name="utilityThreshold">The minimum utility value an element must have to be selected.</param>
        /// <param name="inertia">The utility multiplier applied to the last selected element when the best element is calculated.</param>
        /// <param name="bucketThreshold">The minimum utility this bucket must have to get priority.</param>
        /// <returns>The <see cref="UtilityBucket"/> created.</returns>
        public UtilityBucket CreateUtilityBucket(string name, List<UtilitySelectableNode> elements, bool root = true,
            float utilityThreshold = .3f, float inertia = 1.3f, float bucketThreshold = 0f)
        {
            UtilityBucket bucket = CreateNode<UtilityBucket>(name);
            bucket.UtilityThreshold = utilityThreshold;
            bucket.Inertia = inertia;
            bucket.BucketThreshold = bucketThreshold;
            bucket.IsRoot = root;
            if (root) _utilityCandidates.Add(bucket);
            elements.ForEach(elem =>
            {
                Connect(bucket, elem);
                bucket.AddElement(elem);
            });
            return bucket;
        }

        /// <summary>
        /// Create a new <see cref="UtilityBucket"/> in this <see cref="UtilitySystem"/> that groups the elements specified in <paramref name="elements"/>.
        /// </summary>
        /// <param name="elements">The elements contained by the bucket.</param>
        /// <param name="root">true if the action is added to the selectable element list, false otherwise."</param>
        /// <param name="utilityThreshold">The minimum utility value an element must have to be selected.</param>
        /// <param name="inertia">The utility multiplier applied to the last selected element when the best element is calculated.</param>
        /// <param name="bucketThreshold">The minimum utility this bucket must have to get priority.</param>
        /// <returns>The <see cref="UtilityBucket"/> created.</returns>
        public UtilityBucket CreateUtilityBucket(List<UtilitySelectableNode> elements, bool root = true,
            float utilityThreshold = .3f, float inertia = 1.3f, float bucketThreshold = 0f)
        {
            UtilityBucket bucket = CreateNode<UtilityBucket>();
            bucket.UtilityThreshold = utilityThreshold;
            bucket.Inertia = inertia;
            bucket.BucketThreshold = bucketThreshold;
            bucket.IsRoot = root;
            if (root) _utilityCandidates.Add(bucket);
            elements.ForEach(elem =>
            {
                Connect(bucket, elem);
                bucket.AddElement(elem);
            });
            return bucket;
        }

        /// <summary>
        /// Create a new <see cref="UtilityBucket"/> named <paramref name="name"/> in this <see cref="UtilitySystem"/> that groups the elements specified in <paramref name="elements"/>.
        /// </summary>
        /// <param name="name">The name of the bucket.</param>
        /// <param name="elements">The elements contained by the bucket.</param>
        /// <param name="root">true if the action is added to the selectable element list, false otherwise."</param>
        /// <param name="utilityThreshold">The minimum utility value an element must have to be selected.</param>
        /// <param name="inertia">The utility multiplier applied to the last selected element when the best element is calculated.</param>
        /// <param name="bucketThreshold">The minimum utility this bucket must have to get priority.</param>
        /// <returns>The <see cref="UtilityBucket"/> created.</returns>
        public UtilityBucket CreateUtilityBucket(string name, bool root = true, float utilityThreshold = .3f, 
            float inertia = 1.3f, float bucketThreshold = 0f, params UtilitySelectableNode[] elements)
        {
            return CreateUtilityBucket(name, elements.ToList(), root, utilityThreshold, inertia, bucketThreshold);
        }

        /// <summary>
        /// Create a new <see cref="UtilityBucket"/> in this <see cref="UtilitySystem"/> that groups the elements specified in <paramref name="elements"/>.
        /// </summary>
        /// <param name="elements">The elements contained by the bucket.</param>
        /// <param name="root">true if the action is added to the selectable element list, false otherwise."</param>
        /// <param name="utilityThreshold">The minimum utility value an element must have to be selected.</param>
        /// <param name="inertia">The utility multiplier applied to the last selected element when the best element is calculated.</param>
        /// <param name="bucketThreshold">The minimum utility this bucket must have to get priority.</param>
        /// <returns>The <see cref="UtilityBucket"/> created.</returns>
        public UtilityBucket CreateUtilityBucket(bool root = true, float utilityThreshold = .3f,
            float inertia = 1.3f, float bucketThreshold = 0f, params UtilitySelectableNode[] elements)
        {
            return CreateUtilityBucket(elements.ToList(), root, utilityThreshold, inertia, bucketThreshold);
        }

        public void SetDefaultUtilityElement(UtilitySelectableNode node)
        {
            StartNode = node;
            _utilityCandidates.MoveAtFirst(node);
        }

        protected override void AddNode(Node node)
        {
            base.AddNode(node);

            if (node is UtilitySelectableNode selectableNode && selectableNode.IsRoot)
                _utilityCandidates.Add(selectableNode);
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
            base.Start();

            if (_utilityCandidates.Count == 0)
                throw new EmptyGraphException(this, "The list of utility candidates is empty.");
        }

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

        private UtilitySelectableNode ComputeCurrentBestAction()
        {
            float currentHigherUtility = -1f; // If value starts in 0, elems with Utility == 0 cant be executed

            UtilitySelectableNode newBestElement = _currentBestElement;

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
            base.Stop();
            _currentBestElement?.Stop();
            _currentBestElement = null;
        }

        #endregion
    }
}