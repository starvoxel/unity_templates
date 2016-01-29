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
using UnityEngine.SceneManagement;
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

        private ViewNode m_OpeningViewNode;
        private Dictionary<string, object> m_CurrentParameters = null;

        private Queue<ActionNode> m_ActionQueue = new Queue<ActionNode>(); // Queue of all actions that are going to be processed

        private Coroutine m_LoadingRoutine = null;
		//private
	
		//properties
        public bool IsBusy
        {
            get { return m_ClosingViews.Count > 0 || m_OpeningView != null; }
        }

        public bool IsInitialized
        {
            get { return m_GeneralInformation.IsInitialized && m_Views.Length > 0; }
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
            if (!IsInitialized)
            {
                Initialize();

                FlowParser parser = FlowParser.Parse(m_TestXMLPath, CURRENT_VERSION);
                m_GeneralInformation = parser.GeneralInformation;
                m_GeneralActions = parser.GeneralActions;
                m_Views = parser.Views;

                if (m_GeneralInformation.IsInitialized)
                {
                    PartialOnPreLaunch();

                    ViewNode startingView = new ViewNode();

                    // Grab the starting view from the list of views.  If it's not found, through a error
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
        }

        /// <summary>
        /// Trigger a flow action based on provided ID.  This action can change views, load additive views on top of the current scene.  Eventually they will interact with the state machines of views aswell
        /// </summary>
        /// <param name="actionID">ID for the action</param>
        public void TriggerAction(string actionID)
        {
            ActionNode action = new ActionNode();

            // Try and find the action in the current views list of actions
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
		#endregion

		#region Private Methods
        private void Initialize()
        {
            View.SetStaticActions(this.OnViewClosed, this.OnViewOpened);
        }

        private void ProcessActions()
        {
            // TODO jsmellie: This is where we'd iterate over the actions and so all the stuff we need to do!
        }

        private void LoadView(ViewNode view)
        {
            m_OpeningViewNode = view;

            StartCoroutine(LoadingSequence());
        }

        private IEnumerator LoadingSequence()
        {
            // TODO jsmellie: We should do some kind of "If this view is already open, reload it" type thing
            // If the new view isn't modal and we aren't reloading, delete all the currentely open views
            if (!m_OpeningViewNode.IsModal)
            {
                PartialOnPreCloseAllViews();
                yield return CloseAllViews();
            }
            PartialOnPreLoadNewScene();
            yield return LoadNewScene();
            PartialOnPreOpenView();
            yield return OpenView();
            ProcessActions();
        }

        private IEnumerator CloseAllViews()
        {
            View curClosingView = null;

            while (m_CurrentViewStack.Count > 0)
            {
                // Remove the view from the view stack and add it to the list of closing views
                curClosingView = m_CurrentViewStack.Pop();
                if (curClosingView != null)
                {
                    m_ClosingViews.Add(curClosingView);
                    curClosingView.CloseView(m_CurrentParameters);
                }
            }

            while (m_ClosingViews.Count > 0)
            {
                yield return null;
            }
        }

        private IEnumerator LoadNewScene()
        {
            string newSceneName = m_OpeningViewNode.SceneName;
            yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            m_OpeningView = GetViewForSceneName(newSceneName);

            if (m_OpeningView != null)
            {
                yield return m_OpeningView.ViewLoaded(m_CurrentParameters);
            }
            else
            {
                Debug.LogErrorFormat("A view with the name {0} was not found.  Make sure that the view has the same name as the scene!", newSceneName);
            }
        }

        private IEnumerator OpenView()
        {
            if (m_OpeningView != null)
            {
                m_OpeningView.OpenView(m_CurrentParameters);
            }

            while (m_OpeningView != null)
            {
                yield return null;
            }
        }

        private void OnViewClosed(View closedView)
        {
            if (m_ClosingViews.Contains(closedView))
            {
                m_ClosingViews.Remove(closedView);
                SceneManager.UnloadScene(closedView.gameObject.name);
            }
        }

        private void OnViewOpened(View openedView)
        {
            if (openedView == m_OpeningView)
            {
                m_CurrentViewStack.Push(openedView);
                m_OpeningView = null;
            }
        }

        #region Helper Functions
        private View GetViewForSceneName(string viewName)
        {
            View[] views = FindObjectsOfType<View>();

            for (int viewIndex = 0; viewIndex < views.Length; ++viewIndex)
            {
                if (views[viewIndex].name == m_OpeningViewNode.SceneName)
                {
                    return views[viewIndex];
                }
            }

            return null;
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

        private ViewNode GetViewNodeForID(string viewID)
        {
            for (int viewIndex = 0; viewIndex < m_Views.Length; ++viewIndex)
            {
                if (m_Views[viewIndex].ID.Equals(viewID))
                {
                    return m_Views[viewIndex];
                }
            }

            return new ViewNode();
        }
        #endregion
        #endregion
        #region Partial Methods
        partial void PartialOnPreLaunch();
        partial void PartialOnPostLaunch();

        partial void PartialOnPreCloseAllViews();
        partial void PartialOnPreLoadNewScene();
        partial void PartialOnPreOpenView();
		#endregion
	}
}
