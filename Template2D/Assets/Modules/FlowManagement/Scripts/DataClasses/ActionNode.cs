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
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.FlowManagement
{
	public struct ActionNode
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
        private string m_ID;
        private string m_ViewID;
        private Hashtable m_Parameters;
	
		//properties
        public string ID
        {
            get { return m_ID; }
        }

        public string ViewID
        {
            get { return m_ViewID; }
        }

        public Hashtable Parameters
        {
            get { return m_Parameters; }
        }

        /// <summary>
        /// Returns true if this instance is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return !string.IsNullOrEmpty(m_ID); }
        }
		#endregion
	
		#region Constructor Methods
        public ActionNode(string id, string viewID)
        {
            m_ID = id;
            m_ViewID = viewID;
            m_Parameters = new Hashtable();
        }

        public ActionNode(string id, string viewID, Hashtable parameters)
            : this(id, viewID)
        {
            m_Parameters = parameters;
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