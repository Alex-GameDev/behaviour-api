using System;

namespace BehaviourAPI.Runtime.Core
{
    [System.Serializable]
    public class CustomAction : ActionTask
    {
        Func<Status> ExecutionFunction;
        public override void Start()
        {
            ExecutionTime = 0f;
        }

        public override void Initialize(Context context)
        {
            base.Initialize(context);
        }

        public override void Update()
        {
            var status = ExecutionFunction();
            SetStatus(status);
        }
    }
}