/* --------------------------
 *
 * ActionTrigger.cs
 *
 * Description: Fires an action that is specified in m_ActionID
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 1/31/2016 - DefaultCompany
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
using Starvoxel.Core;
#endregion
#endregion

 namespace Starvoxel.FlowManagement
{
	public class ActionTrigger : CustomMono
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
        [SerializeField] protected string m_ActionID;
	
		//private
	
		//properties
		#endregion
	
		#region Public Methods
        [ContextMenu("Fire Flow Event")]
        public virtual void FireFlowEvent()
        {
            EventMessenger.Instance.Raise<FlowEvent>(new FlowEvent(m_ActionID));
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
	
}