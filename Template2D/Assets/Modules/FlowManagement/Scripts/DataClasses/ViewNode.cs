/* --------------------------
 *
 * ViewNode.cs
 *
 * Description: All information for a View that is parsed from the XML.
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
using UnityEngine;
#endregion
#endregion

namespace Starvoxel.FlowManagement
{
    [System.Serializable]
    public struct ViewNode
	{
		#region Fields & Properties
		//const

        //public
        public string ID;
        public string SceneName;

        public bool IsModal;
        public bool ShowOverlay;

        public ActionNode[] Actions;
	
		//protected

        //private

        //properties
        /// <summary>
        /// Returns true if this instance is initialized.
        /// </summary>
        public bool IsInitialzed
        {
            get { return !string.IsNullOrEmpty(ID) && Actions != null; }
        }
		#endregion
	
		#region Constructor Methods
		#endregion
	
		#region Public Methods

        /// <summary>
        /// Gets an action by the ID.  Returns a empty ActionNode if none exists.
        /// </summary>
        /// <param name="actionID">ID used for fetching the ActionNode.</param>
        /// <returns></returns>
        public ActionNode GetActionByID(string actionID)
        {
            for(int i = 0; i < Actions.Length; ++i)
            {
                if (Actions[i].ID == actionID)
                {
                    return Actions[i];
                }
            }

            return new ActionNode();
        }

        /// <summary>
        /// Checks to see if a action with the specified ID already exists in the action list.
        /// </summary>
        /// <param name="actionID">ID that will be checked.</param>
        /// <returns>true if the specified ID is in the action list.</returns>
        public bool ContainActionForID(string actionID)
        {
            for(int i = 0; i < Actions.Length; ++i)
            {
                if (Actions[i].ID == actionID)
                {
                    return true;
                }
            }

            return false;
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}
