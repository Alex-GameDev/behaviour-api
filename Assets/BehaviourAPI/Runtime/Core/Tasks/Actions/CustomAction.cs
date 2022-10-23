using System;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    [System.Serializable]
    public class CustomAction : ActionTask
    {
        [SerializeField] ActionFunction _function;
        Func<Status> ExecutionFunction;
        public override void Start()
        {
            ExecutionTime = 0f;
        }

        public override void Initialize(Context context)
        {
            base.Initialize(context);
            // TODO: Bind Function
            ExecutionFunction = _function.Build();
        }

        public override void Update()
        {
            var status = ExecutionFunction();
            SetStatus(status);
        }
    }
}