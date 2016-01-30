/* --------------------------
 *
 * FlowTester.cs
 *
 * Description: Simple monobehaviour that makes it easy to test the flow manager.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 1/30/2016 - Starvoxel
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
	public class FlowTester : CustomMono
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
        [SerializeField] private string m_TestXMLPath; // Used for testing to load a XML on Start
        [SerializeField] private string m_ActionID; // Action ID of the action you want to run
	
		//properties
		#endregion
	
		#region Unity Methods
        private void Awake()
        {
            if (!string.IsNullOrEmpty(m_TestXMLPath) && !FlowManager.Instance.IsInitialized)
            {
                FlowManager.Instance.LaunchWithFile(m_TestXMLPath);
            }
        }
		#endregion
	
		#region Public Methods
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
        #endregion

        #region Context Methods
        [ContextMenu("Run Action")] private void ContextRunAction()
        {
            Debug.Log("Running Action: " + m_ActionID);
            FlowManager.Instance.TriggerAction(m_ActionID);
        }
        #endregion
	}
	
}