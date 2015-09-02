/* --------------------------
 *
 * LoggerTestingScript.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 8/28/2015 - Starvoxel
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

 namespace Starvoxel
{
	public class LoggerTestingScript : MonoBehaviour
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
        void Awake()
        {
            Logger.Log("Test Log");
        }
		#endregion
	
		#region Public Methods
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}