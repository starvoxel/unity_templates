/* --------------------------
 *
 * LoggerSettingsInspector.cs
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
using UnityEditor;
#endregion

#region System Includes
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel
{
    [CustomEditor(typeof(LoggerSettings))]
	public class LoggerSettingsInspector : Editor
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion

        #region Public Methods
        public override void OnInspectorGUI()
        {
            LoggerSettings loggerSettings = (LoggerSettings)target;

            DrawDefaultInspector();
            
            //TODO jsmellie: Eventually I'll make a nice inspector for the flags and stuff.  For now it doesn't matter so just draw default
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}