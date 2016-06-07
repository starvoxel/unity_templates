using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement.PolyNav
{
    [TaskDescription("Wander using PolyNav.")]
    [TaskCategory("Movement/PolyNav")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=9")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WanderIcon.png")]
    public class Wander : PolyNavMovement
    {
        [Tooltip("How far ahead of the current position to look ahead for a wander")]
        public SharedFloat wanderDistance = 20;
        [Tooltip("The amount that the agent rotates direction")]
        public SharedFloat wanderRate = 2;
        [Tooltip("Minimum amount of time that the agent will wait before trying to wander again.")]
        public SharedFloat minWaitTime = 0;
        [Tooltip("Maximum amount of time that the agent will wait before trying to wander again.")]
        public SharedFloat maxWaitTime = 0;
        [Tooltip("Should wait on start")]
        public SharedBool waitOnStart = true;

        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 10;

        // The time to wait
        private float waitDuration;
        // The time that the task started to wait.
        private float startTime;
        // Remember the time that the task is paused so the time paused doesn't contribute to the wait time.
        private float pauseTime;

        public override void OnStart()
        {
            base.OnStart();

            if (waitOnStart.Value)
            {
                startTime = Time.time;
                waitDuration = Random.Range(minWaitTime.Value, maxWaitTime.Value);
            }
            else
            {
                SetDestination(Target());
            }
        }

        public override TaskStatus OnUpdate()
        { 
            if (HasArrived() && float.IsNaN(startTime))
            {
                startTime = Time.time;
                waitDuration = Random.Range(minWaitTime.Value, maxWaitTime.Value);
            }
            // The task is done waiting if the time waitDuration has elapsed since the task was started.
            else if (startTime + waitDuration < Time.time)
            {
                startTime = float.NaN;
                SetDestination(Target());
            }
            // Otherwise we are still waiting or wandering
            return TaskStatus.Running;
        }

        private Vector3 Target()
        {
            // point in a new random direction and then multiply that by the wander distance
            var direction = transform.forward + Random.insideUnitSphere * wanderRate.Value;
            return transform.position + direction.normalized * wanderDistance.Value;
        }

        // Reset the public variables
        public override void OnReset()
        {
            wanderDistance = 20;
            wanderRate = 2;
            minWaitTime = maxWaitTime = 0;
        }

        public override void OnPause(bool paused)
        {
            if (!float.IsNaN(startTime))
            {
                if (paused)
                {
                    // Remember the time that the behavior was paused.
                    pauseTime = Time.time;
                }
                else
                {
                    // Add the difference between Time.time and pauseTime to figure out a new start time.
                    startTime += (Time.time - pauseTime);
                }
            }
        }
    }
}