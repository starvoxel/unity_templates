/* --------------------------
 *
 * EditorConstants.cs
 *
 * Description: Basic constants needed for editor work.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 9/6/2015 - Starvoxel
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
	public partial class EditorConstants
	{
		#region Fields & Properties
		//const
        public const int EDITOR_HEADER_HEIGHT = 44;

        public const int EDGE_PADDING = 10;
	
		//public
	
		//protected

        //private

        //properties
        public static float CUR_VIEW_WIDTH
        {
            get { return EditorGUIUtility.currentViewWidth - (EDGE_PADDING * 2);  }
        }
		#endregion
	
		#region Public Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}