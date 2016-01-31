/* --------------------------
 *
 * EditorOnly.cs
 *
 * Description: Destroys the object on runtime
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 1/29/2016 - Starvoxel
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

 namespace Starvoxel.Utilities
{
	public class EditorOnly : CustomMono
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
        private void Awake()
        {
            Destroy(this.gameObject);
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