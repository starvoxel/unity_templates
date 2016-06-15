/* --------------------------
 *
 * TestObjectInspector.cs
 *
 * Description: Just testing out some things
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 6/14/2016 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Inludes
using UnityEditor;
using UnityEngine;
#endregion

#region System Includes
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.EditorUtilities
{
	public class TestObjectInspector : ObjectInspector
	{
		#region Fields & Properties
        //const
        private GUIContent TAB_GUI_CONTENT = new GUIContent("Test Insp.");
	
		//public
	
		//protected
	
		//private
	
		//properties
        public override UnityEngine.GUIContent TabContent
        {
            get { return TAB_GUI_CONTENT; }
        }
		#endregion

        #region Public Methods
        public override bool IsValid(string path)
        {
            return false;
        }

        public override void OnInspector() 
        {
            UnityEditor.EditorGUILayout.LabelField("Test Object Inspector");
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
	
}