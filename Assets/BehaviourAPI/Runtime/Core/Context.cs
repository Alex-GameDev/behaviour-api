using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.Runtime.Core
{
    public class Context
    {
        /// <summary>
        /// 
        /// </summary>
        public BehaviourRunner Runner { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Rigidbody RigidBody { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Collider Collider { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Rigidbody2D RigidBody2D { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Collider2D Collider2D { get; private set; }

        /// <summary>
        /// Create a new Execution context with the given Behaviour Runner
        /// </summary>
        /// <param name="runner">The context behaviour runner</param>
        public Context(BehaviourRunner runner)
        {
            Runner = runner;
            Transform = runner.transform;
            RigidBody = runner.GetComponent<Rigidbody>();
            Collider = runner.GetComponent<Collider>();
            RigidBody2D = runner.GetComponent<Rigidbody2D>();
            Collider2D = runner.GetComponent<Collider2D>();
        }
    }
}