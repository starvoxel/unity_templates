using UnityEngine;
using System.Collections.Generic;
using System;

namespace BehaviorDesigner.Runtime.Tasks.Movement.PolyNav
{
    public abstract class PolyNavMovement : Movement
    {
        [Tooltip("The speed of the agent")]
        public SharedFloat speed = 5;
        [Tooltip("Angular speed of the agent")]
        public SharedFloat angularSpeed = 350;

        private bool destinationReached = false;
        // A cache of the PolyNavAgent
        private PolyNavAgent polyNavAgent;
        
        public override void OnAwake()
        {
            polyNavAgent = gameObject.GetComponent<PolyNavAgent>();
        }

        public override void OnStart()
        {
            destinationReached = false;

            polyNavAgent.maxSpeed = speed.Value;
            polyNavAgent.rotateSpeed = angularSpeed.Value;
            polyNavAgent.enabled = true;
        }

        protected override bool SetDestination(Vector3 target)
        {
            destinationReached = false;
            return polyNavAgent.SetDestination(target, OnDestinationReached);
        }

        protected override bool HasArrived()
        {
            return destinationReached;
        }

        protected override Vector3 Velocity()
        {
            return polyNavAgent.movingDirection * polyNavAgent.currentSpeed;
        }

        private void OnDestinationReached(bool arrived)
        {
            destinationReached = true;
        }

        public override void OnEnd()
        {
            // Disable the poly nav
            polyNavAgent.enabled = false;
        }

        // Reset the public variables
        public override void OnReset()
        {
            speed = 5;
            angularSpeed = 10;
        }
    }
}