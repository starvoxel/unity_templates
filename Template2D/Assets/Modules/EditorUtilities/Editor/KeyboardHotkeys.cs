/* --------------------------
 *
 * KeyboardHotkeys.cs
 *
 * Description: A bunch of usefull hotkeys that we use in the editor.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 2/3/2016 - Starvoxel
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

 namespace Starvoxel.EditorUtilities
{
	public static class KeyboardHotkeys
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Private Methods
        [MenuItem("Tools/Switch Selection Active State %e")]
        private static void SwitchSelectionActiveState()
        {
            Transform[] selection = Selection.transforms;

            for (int i = 0; i < selection.Length; ++i)
            {
                selection[i].gameObject.SetActive(!selection[i].gameObject.activeSelf);
            }
        }
		#endregion
	}
	
}