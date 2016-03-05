using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement.PolyNav
{
    [TaskDescription("Find a place to hide and move to it using PolyNav.")]
    [TaskCategory("Movement/PolyNav")]
    [HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=8")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CoverIcon.png")]
    public class Cover : PolyNavMovement
    {
        [Tooltip("The distance to search for cover")]
        public SharedFloat maxCoverDistance = 1000;
        [Tooltip("The layermask of the available cover positions")]
        public LayerMask availableLayerCovers;
        [Tooltip("The maximum number of raycasts that should be fired before the agent gives up looking for an agent to find cover behind")]
        public SharedInt maxRaycasts = 100;
        [Tooltip("How large the step should be between raycasts")]
        public SharedFloat rayStep = 1;
        [Tooltip("Once a cover point has been found, multiply this offset by the normal to prevent the agent from hugging the wall")]
        public SharedFloat coverOffset = 2;
        [Tooltip("Should the agent look at the cover point after it has arrived?")]
        public SharedBool lookAtCoverPoint = false;
        [Tooltip("The agent is done rotating to the cover point when the square magnitude is less than this value")]
        public SharedFloat rotationEpsilon = 0.5f;
        [Tooltip("Max rotation delta if lookAtCoverPoint")]
        public SharedFloat maxLookAtRotationDelta;

        // The cover position
        private Vector3 coverPoint;

        public override void OnStart()
        {
            RaycastHit hit;
            int raycastCount = 0;
            var direction = transform.forward;
            float step = 0;
            // Keep firing a ray until too many rays have been fired
            while (raycastCount < maxRaycasts.Value) {
                var ray = new Ray(transform.position, direction);
                if (Physics.Raycast(ray, out hit, maxCoverDistance.Value, availableLayerCovers.value)) {
                    // A suitable agent has been found. Find the opposite side of that agent by shooting a ray in the opposite direction from a point far away
                    if (hit.collider.Raycast(new Ray(hit.point - hit.normal * maxCoverDistance.Value, hit.normal), out hit, Mathf.Infinity)) {
                        coverPoint = hit.point;
                        break;
                    }
                }
                // Keep sweeiping along the y axis
                step += rayStep.Value;
                direction = Quaternion.Euler(0, 0, transform.eulerAngles.z + step) * Vector3.up;
                raycastCount++;
            }

            SetDestination(Target());

            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (HasArrived()) {
                var dir = (Vector3)coverPoint - transform.position;
                var zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
                var rotation = Quaternion.Euler(0f, 0f, zAngle);
                // Return success if the agent isn't going to look at hte cover point or it has completely rotated to look at the cover point
                if (!lookAtCoverPoint.Value || Quaternion.Angle(transform.rotation, rotation) < rotationEpsilon.Value) {
                    return TaskStatus.Success;
                } else {
                    // Still needs to rotate towards the target
                    zAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, zAngle, angularSpeed.Value);
                    transform.rotation = Quaternion.Euler(0f, 0f, zAngle);
                }
            }

            // Return running until the agent has arrived
            return TaskStatus.Running;
        }

        // Find a place to hide by firing a ray 
        private Vector2 Target()
        {
            RaycastHit2D hit;
            RaycastHit2D[] hitArray;
            int raycastCount = 0;
            var direction = transform.up;
            float step = 0;
            // Keep firing a ray until too many rays have been fired
            while (raycastCount < maxRaycasts.Value) {
                hit = Physics2D.Raycast(transform.position, direction, maxCoverDistance.Value, availableLayerCovers);
                if (hit.collider != null) {
                    // A suitable cover object has been found. Find the opposite side of that cover object by shooting a ray in the opposite direction from a point far away
                    hitArray = Physics2D.RaycastAll(hit.point - hit.normal * maxCoverDistance.Value, hit.normal);
                    if (hitArray.Length > 0) {
                        // A list of colldiers is returned. Ensure that the original hit collider is hit
                        for (int i = 0; i < hitArray.Length; ++i) {
                            if (hitArray[i].collider == hit.collider) {
                                coverPoint = hitArray[i].point;
                                return hitArray[i].point + hitArray[i].normal * coverOffset.Value;
                            }
                        }
                    }
                }
                // Keep sweeiping along the y axis
                step += rayStep.Value;
                direction = Quaternion.Euler(0, 0, transform.eulerAngles.z + step) * Vector3.up;
                raycastCount++;
            }
            // The agent wasn't found - return zero
            return Vector2.zero;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            maxCoverDistance = 1000;
            maxRaycasts = 100;
            rayStep = 1;
            coverOffset = 2;
            lookAtCoverPoint = false;
            rotationEpsilon = 0.5f;
        }
    }
}