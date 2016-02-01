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
	
		#region Unity Methods
		#endregion
	
		#region Public Methods
        public virtual void TriggerAction()
        {
            FlowManager.Instance.TriggerAction(m_ActionID);
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
	
}