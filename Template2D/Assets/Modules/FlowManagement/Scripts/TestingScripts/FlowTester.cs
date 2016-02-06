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
using Starvoxel.Core;
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
        [SerializeField] private string m_TestXMLPath = string.Empty; // Used for testing to load a XML on Start
	
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

        #region Context Methods
        #endregion
	}
	
}