using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class PatrolAction : ActionTask
    {
        [SerializeField] List<Vector3> positions;
        [SerializeField] float speed;
        [SerializeField] float distanceThreshold = .1f;

        int currentTargetPosId;

        public override void Start()
        {
            base.Start();
            currentTargetPosId = 0;
        }

        public override void Update()
        {
            base.Update();
            if (positions.Count == 0) return;

            if (Vector3.Distance(ExecutionContext.Transform.position, positions[currentTargetPosId]) < distanceThreshold)
            {
                currentTargetPosId++;
                if (currentTargetPosId >= positions.Count)
                {
                    Success();
                    currentTargetPosId = 0;
                }
            }

            var currentPos = ExecutionContext.Transform.position;
            var rawMovement = positions[currentTargetPosId] - currentPos;
            var maxDistance = rawMovement.magnitude;
            var movement = rawMovement.normalized * speed * Time.deltaTime;
            ExecutionContext.Transform.position = Vector3.MoveTowards(currentPos, currentPos + movement, maxDistance);
        }
    }
}
