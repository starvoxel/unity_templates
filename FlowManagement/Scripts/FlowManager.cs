/* --------------------------
 *
 * FlowManager.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 5/30/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
#endregion

#region System Includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
#endregion
#endregion

namespace Starvoxel.FlowManagement
{
    public class FlowVersionComparer : IComparer<Version>
    {
        public int Compare(Version x, Version y)
        {
            if (x == y)
            {
                return 0;
            }

            if(x.Major > y.Major)
            {
                return 1;
            }
            else if (x.Major < y.Major)
            {
                return -1;
            }
            else
            {
                if(x.Minor > y.Minor)
                {
                    return 1;
                }
                else if(x.Minor < y.Minor)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

	public partial class FlowManager : MonoBehaviour
    {
        #region Fields & Properties
        //const
        public const string DEFAULT_STARTING_VIEW = "SPLASH";
        public static readonly Version CURRENT_VERSION = new Version("1.0.0");

		//public
	
		//protected
        [SerializeField] private string m_TestXMLPath; // Temporary.  Used for testing to load a XML on Start

        // Parsed data
        private ViewNode[] m_Views;  // All the view nodes that are were parsed from the XML
        private ActionNode[] m_GeneralActions; // All the general actions that can be used anywhere in the project
        private GeneralInformation m_GeneralInformation;

        private Stack<View> m_CurrentViewStack = new Stack<View>(); // Stack of all the current views that are loaded

        private List<View> m_ClosingViews = new List<View>(); // List oif all the views that are currentely closing
        private View m_OpeningView = null; // View that is in the process of opening

        private Queue<ActionNode> m_ActionQueue = new Queue<ActionNode>(); // Queue of all actions that are going to be processed
		//private
	
		//properties
        public bool IsBusy
        {
            get { return m_ClosingViews.Count > 0 || m_OpeningView != null; }
        }
		#endregion
	
		#region Unity Methods
        // TEMPORARY jsmellie: This is only here for now.  We'll take it out once we actually have the entire flow working properly.
        private void Start()
        {
            LaunchWithFile(m_TestXMLPath);
        }
		#endregion
	
		#region Public Methods
        /// <summary>
        /// Launch the flow of the game.  It will load and parse the XML at the provided path.
        /// </summary>
        /// <param name="filePath">Path of the flow XML.</param>
        public void LaunchWithFile(string filePath)
        {
            FlowParser parser = FlowParser.Parse(m_TestXMLPath, CURRENT_VERSION);
            m_GeneralInformation = parser.GeneralInformation;
            m_GeneralActions = parser.GeneralActions;
            m_Views = parser.Views;

            if (m_GeneralInformation.IsInitialized)
            {
                PartialOnPreLaunch();

                ViewNode startingView = new ViewNode();

                //TODO: - Check to make sure a file with the starting view is in the array of views
                for (int viewIndex = 0; viewIndex < m_Views.Length; ++viewIndex)
                {
                    if (m_Views[viewIndex].ID == m_GeneralInformation.StartingView)
                    {
                        startingView = m_Views[viewIndex];
                    }
                }

                if (startingView.IsInitialzed)
                {
                    LoadView(startingView);
                }
                else
                {
                    Debug.LogErrorFormat("Starting view ({0}) not found in view list.  Starting view must be a valid view.", m_GeneralInformation.StartingView);
                }

                PartialOnPostLaunch();
            }
        }

        /// <summary>
        /// Trigger a flow action based on provided ID.  This action can change views, load additive views on top of the current scene.  Eventually they will interact with the state machines of views aswell
        /// </summary>
        /// <param name="actionID">ID for the action</param>
        public void TriggerAction(string actionID)
        {
            ActionNode action = new ActionNode();
            //TODO jsmellie: Go through all the actions for the curent view.  If nothing is found, then go through the general actions.  If still nothing found, through a warning.
            if (m_CurrentViewStack.Count > 0)
            {
                ViewNode focusedNode = GetViewNodeForView(m_CurrentViewStack.Peek());

                if (focusedNode.IsInitialzed)
                {
                    action = focusedNode.GetActionByID(actionID);
                }

                // If we still don't have action, try the general actions
                if (!action.IsInitialized && m_GeneralActions != null && m_GeneralActions.Length > 0)
                {
                    for(int actionIndex = 0; actionIndex < m_GeneralActions.Length; ++actionIndex)
                    {
                        if (m_GeneralActions[actionIndex].ID == actionID)
                        {
                            action = m_GeneralActions[actionIndex];
                            break;
                        }
                    }
                }
            }

            if (action.IsInitialized)
            {
                m_ActionQueue.Enqueue(action);

                if (!IsBusy)
                {
                    ProcessActions();
                }
            }
        }


        private void OnViewClosed(View closedView)
        {

        }

        private void OnViewOpened(View openedView)
        {

        }
		#endregion

		#region Private Methods
        private void ProcessActions()
        {
            // TODO jsmellie: This is where we'd iterate over the actions and so all the stuff we need to do!
        }

        private void LoadView(ViewNode view)
        {
            // TODO jsmellie: We should do some kind of "If this view is already open, reload it" type thing

            // If the new view isn't modal, delete all the currentely open views
            if (!view.IsModal)
            {

            }
            else
            {

            }
        }

        private ViewNode GetViewNodeForView(View view)
        {
            string sceneName = view.gameObject.name;

            for(int viewIndex = 0; viewIndex < m_Views.Length; ++viewIndex)
            {
                if (m_Views[viewIndex].SceneName.Equals(sceneName))
                {
                    return m_Views[viewIndex];
                }
            }

            return new ViewNode();
        }
        #endregion

        #region Partial Methods
        partial void PartialOnPreLaunch();
        partial void PartialOnPostLaunch();
		#endregion
	}
}
