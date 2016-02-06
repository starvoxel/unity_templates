/* --------------------------
 *
 * ObjectInspector.cs
 *
 * Description: Abstract base to every Object inspector that we might want.
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
#endregion

#region System Includes
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel.EditorUtilities
{
    public abstract class ObjectInspector
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
		#endregion
	
		#region Public Methods
        public abstract bool IsValid(string path);

        public abstract void OnInspector(ObjectEditor editor);
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}