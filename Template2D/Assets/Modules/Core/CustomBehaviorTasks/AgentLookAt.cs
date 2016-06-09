/* --------------------------
 *
 * AgentLookAt.cs
 *
 * Description: Rotates the agent towards a specied object or location
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 4/2/2016 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
#endregion

#region System Includes
using System.Collections;
#endregion

#region Other Includes
#endregion
#endregion

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	public class AgentLookAt : Action
	{
		#region Fields & Properties
		//const

        //public
        [Tooltip("Target to look at")]
        public SharedGameObject target;
        [Tooltip("Location to look at.  Used if target is null.")]
        public SharedVector3 targetLocation;
        [Tooltip("Angular speed of the agent")]
        public SharedFloat angularSpeed;
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Public Methods
        public override TaskStatus OnUpdate()
        {
            Vector2 targetPos = target.Value != null ? (Vector2)target.Value.transform.position : (Vector2)targetLocation.Value;
            Vector2 wantedDirection = targetPos - (Vector2)this.transform.position;
            wantedDirection.Normalize();

            float rot = -Mathf.Atan2(wantedDirection.x, wantedDirection.y) * 180 / Mathf.PI;
            float newZ = Mathf.MoveTowardsAngle(transform.localEulerAngles.z, rot, angularSpeed.Value * Time.deltaTime);

            this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, newZ);

            if (Mathf.Approximately(rot, newZ))
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
	
}