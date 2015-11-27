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
    public struct ViewNode
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected

        //private
        private string m_ID;
        private string m_SceneName;

        private List<ActionNode> m_Actions;

        //properties
        /// <summary>
        /// Returns true if this instance is initialized.
        /// </summary>
        public bool IsInitialzed
        {
            get { return !string.IsNullOrEmpty(m_ID) && !string.IsNullOrEmpty(m_SceneName) && m_Actions != null; }
        }

        public string ID
        {
            get { return m_ID; }
        }

        public string SceneName
        {
            get { return m_SceneName; }
        }

        public ActionNode[] Actions
        {
            get { return m_Actions.ToArray(); }
        }
		#endregion
	
		#region Constructor Methods
		public ViewNode(string id, string sceneName)
		{
            m_ID = id;
            m_SceneName = sceneName;
            m_Actions = new List<ActionNode>();
		}
		#endregion
	
		#region Public Methods
        /// <summary>
        /// Gets an action by the ID.  Returns a empty ActionNode if none exists.
        /// </summary>
        /// <param name="actionID">ID used for fetching the ActionNode.</param>
        /// <returns></returns>
        public ActionNode GetActionByID(string actionID)
        {
            for(int i = 0; i < m_Actions.Count; ++i)
            {
                if (m_Actions[i].ID == actionID)
                {
                    return m_Actions[i];
                }
            }

            return new ActionNode();
        }

        /// <summary>
        /// Adds the action to the action list if one with the same ID doesn't already exist.
        /// </summary>
        /// <param name="action">Action to be added.</param>
        public void AddAction(ActionNode action)
        {
            if (!ContainActionForID(action.ID))
            {
                m_Actions.Add(action);
            }
            else
            {
                Debug.LogWarningFormat("ViewNode {0} already contians an action with ID {1}.  Please fix your XML because you should never have a ViewNode with 2 actions with the same name!", this.m_ID, action.ID);
            }
        }

        /// <summary>
        /// Checks to see if a action with the specified ID already exists in the action list.
        /// </summary>
        /// <param name="actionID">ID that will be checked.</param>
        /// <returns>true if the specified ID is in the action list.</returns>
        public bool ContainActionForID(string actionID)
        {
            for(int i = 0; i < m_Actions.Count; ++i)
            {
                if (m_Actions[i].ID == actionID)
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