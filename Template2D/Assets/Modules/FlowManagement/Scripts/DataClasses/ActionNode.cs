/* --------------------------
 *
 * ActionNode.cs
 *
 * Description: Data class that is parsed from the flow XML.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 11/26/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.FlowManagement
{
     [System.Serializable]
	public struct ActionNode
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
        public string ID;
        public string ViewID;
        public Dictionary<string, object> Parameters;
	
		//properties
        /// <summary>
        /// Returns true if this instance is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return !string.IsNullOrEmpty(ID); }
        }
		#endregion
	
		#region Constructor Methods
        public ActionNode(string id, string viewID)
        {
            ID = id;
            ViewID = viewID;
            Parameters = new Dictionary<string, object>();
        }

        public ActionNode(string id, string viewID, Dictionary<string, object> parameters)
            : this(id, viewID)
        {
            Parameters = parameters;
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