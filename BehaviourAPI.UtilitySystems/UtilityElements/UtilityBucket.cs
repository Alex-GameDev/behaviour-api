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

        public override string Description => "Utility element that choose between multiple Utility elements.";
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
            var newBestAction = ComputeCurrentBestAction();
            var maxUtility = newBestAction?.Utility ?? 0f;
            // if utility is higher than the bucket threshold, set it to +inf to enable the bucket priority.
            return maxUtility > BucketThreshold ? float.MaxValue : maxUtility;
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
                var utility = utilityElement.Utility * (utilityElement == _currentBestElement ? Inertia : 1f);

                if (!currentElementIsLocked && utility > currentHigherUtility)
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
