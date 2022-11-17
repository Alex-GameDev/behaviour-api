using BehaviourAPI.Core;
using System.Xml.Linq;

namespace BehaviourAPI.UtilitySystems
{
    /// <summary>
    /// Utility element that handle multiple <see cref="UtilitySelectableNode"/> itself and
    /// returns the maximum utility if its best candidate utility is higher than the threshold.
    /// </summary>
    public class UtilityBucket : UtilitySelectableNode
    {
        #region ----------------------------------------- Properties -----------------------------------------

        public override Type ChildType => typeof(UtilitySelectableNode);
        public override int MaxOutputConnections => -1;

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
        public float BucketThreshold = .3f;

        List<UtilitySelectableNode?> _utilityCandidates;

        UtilitySelectableNode? _currentBestElement;
        UtilitySelectableNode? _lastExecutedElement;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public UtilityBucket()
        {
            _utilityCandidates = new List<UtilitySelectableNode?>();
        }

        public void AddElement(UtilitySelectableNode elem) => _utilityCandidates.Add(elem);

        public override void Initialize()
        {
            base.Initialize();
            GetChildNodes().ToList().ForEach(node => _utilityCandidates.Add(node as UtilitySelectableNode));
        }

        /// <summary>
        /// For serialization reasons, default selected node must be always the first node in the list
        /// </summary>
        public bool SetDefaulSelectedElement(UtilitySelectableNode node)
        {
            if (!GetChildNodes().Contains(node) || node == DefaultSelectedElement) return false;
            _utilityCandidates.MoveAtFirst(node);
            Connection? conn = OutputConnections.Find((conn) => conn.TargetNode == node);
            if(conn != null) OutputConnections.MoveAtFirst(conn);
            return true;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        public override void Start()
        {
        }

        protected override float ComputeUtility()
        {
            _currentBestElement = ComputeCurrentBestAction();
            var maxUtility = _currentBestElement?.Utility ?? 0f;
            // if utility is higher than the bucket threshold, enable the priority.
            ExecutionPriority = maxUtility > BucketThreshold;
            return maxUtility;
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
                if (utility > currentHigherUtility)
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

        public override void Update()
        {
            if(_currentBestElement != _lastExecutedElement)
            {
                _lastExecutedElement?.Stop();
                _lastExecutedElement = _currentBestElement;
                _lastExecutedElement?.Start();
            }
            _lastExecutedElement?.Update();
        }


        public override void Stop()
        {
            _lastExecutedElement?.Stop();
            _lastExecutedElement = null;
            _currentBestElement = null;
        }

        #endregion
    }
}
