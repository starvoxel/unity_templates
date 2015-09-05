/* --------------------------
 *
 * ObjectInspector.cs
 *
 * Description: Override for all basic objects
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 9/5/2015 - Starvoxel
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
    [CustomEditor(typeof(Object))]
	public class ObjectInspector : Editor
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("Random label!!");
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