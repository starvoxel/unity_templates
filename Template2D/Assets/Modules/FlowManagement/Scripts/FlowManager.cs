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

    public struct ActionQueueElement
    {
        public ActionNode Action;
        public Dictionary<string, object> Parameters;
    }

	public partial class FlowManager : MonoBehaviour
    {
        #region Fields & Properties
        //const
        public const string DEFAULT_STARTING_VIEW = "SPLASH";
        public const int DEFAULT_MODAL_DEPTH_OFFSET = -40;
        public static readonly Version CURRENT_VERSION = new Version("1.0.0");

        public const string ACTION_NAME_KEY = "ACTION_NAME";
        public const string FROM_VIEW_NAME_KEY = "FROM_VIEW_NAME";

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

        private Queue<ActionQueueElement> m_ActionQueue = new Queue<ActionQueueElement>(); // Queue of all actions that are going to be processed

        private Coroutine m_LoadingRoutine = null;

        private static FlowManager m_Instance = null;
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
        public static FlowManager Instance
        {
            get { return m_Instance;  }
        }
		#endregion
	
		#region Unity Methods
        // TEMPORARY jsmellie: This is only here for now.  We'll take it out once we actually have the entire flow working properly.
        private void Start()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
                LaunchWithFile(m_TestXMLPath);
            }
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
        public void TriggerAction(string actionID, Dictionary<string, object> parameters = null)
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
                ActionQueueElement newElement = new ActionQueueElement();
                newElement.Action = action;
                newElement.Parameters = parameters;

                m_ActionQueue.Enqueue(newElement);

                if (!IsBusy)
                {
                    ProcessActions();
                }
            }
        }
		#endregion

		#region Private Methods
        private void ProcessActions()
        {
            if (m_ActionQueue.Count > 0)
            {
                ActionQueueElement curElement = m_ActionQueue.Dequeue();

                m_CurrentParameters = new Dictionary<string, object>();

                //Add default parameters from the action
                if (curElement.Action.Parameters != null && curElement.Action.Parameters.Count > 0)
                {
                    foreach(KeyValuePair<string, object> pair in curElement.Action.Parameters)
                    {
                        if (!m_CurrentParameters.ContainsKey(pair.Key))
                        {
                            m_CurrentParameters.Add(pair.Key, pair.Value);
                        }
                        else
                        {

                        }
                    }
                }

                // Add custom parameters passed when the action was triggered
                if (curElement.Parameters != null && curElement.Parameters.Count > 0)
                {
                    foreach (KeyValuePair<string, object> pair in curElement.Parameters)
                    {
                        if (!m_CurrentParameters.ContainsKey(pair.Key))
                        {
                            m_CurrentParameters.Add(pair.Key, pair.Value);
                        }
                        else
                        {

                        }
                    }
                }

                // Add parameters based off the action that was triggered
                if (!m_CurrentParameters.ContainsKey(FROM_VIEW_NAME_KEY))
                {
                    m_CurrentParameters.Add(FROM_VIEW_NAME_KEY, m_CurrentViewStack.Peek().name);
                }
                else
                {

                }

                if (!m_CurrentParameters.ContainsKey(ACTION_NAME_KEY))
                {
                    m_CurrentParameters.Add(ACTION_NAME_KEY, curElement.Action.ID);
                }
                else
                {

                }

                if (!string.IsNullOrEmpty(curElement.Action.ViewID))
                {
                    LoadView(GetViewNodeForID(curElement.Action.ViewID));
                }
                else if (GetViewNodeForView(m_CurrentViewStack.Peek()).IsModal)
                {
                    StartCoroutine(CloseCurrentView());
                }
            }
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
                yield return StartCoroutine(CloseAllViews());
            }
            else if (m_CurrentViewStack.Count > 0)
            {
                m_CurrentViewStack.Peek().LoseFocus(m_CurrentParameters);
            }

            PartialOnPreLoadNewScene();
            yield return StartCoroutine(LoadNewScene());
            PartialOnPreOpenView();
            yield return StartCoroutine(OpenView());
            ProcessActions();
        }

        private IEnumerator CloseCurrentView()
        {
            if (m_CurrentViewStack.Count > 0)
            {
                View closingView = m_CurrentViewStack.Pop();
                m_ClosingViews.Clear();
                m_ClosingViews.Add(closingView);
                closingView.CloseView(m_CurrentParameters);

                while (m_ClosingViews.Count > 0)
                {
                    yield return null;
                }

                if (m_CurrentViewStack.Count > 0)
                {
                    m_CurrentViewStack.Peek().GainFocus(m_CurrentParameters);
                }
            }
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

            Vector3 pos = m_OpeningView.transform.position;
            pos.z = m_CurrentViewStack.Count * m_GeneralInformation.ModalDepthOffset;

            if (m_OpeningView != null)
            {
                yield return StartCoroutine(m_OpeningView.ViewLoaded(m_CurrentParameters));
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

        public void OnViewClosed(View closedView)
        {
            if (m_ClosingViews.Contains(closedView))
            {
                m_ClosingViews.Remove(closedView);
                SceneManager.UnloadScene(closedView.gameObject.name);
            }
        }

        public void OnViewOpened(View openedView)
        {
            Debug.Log("OnViewOpened called for view: " + openedView);
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
                if ( !string.IsNullOrEmpty(m_Views[viewIndex].SceneName) && m_Views[viewIndex].SceneName.Equals(sceneName))
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

        #region Context Methods
        [SerializeField]
        private string m_ActionID;

        [ContextMenu("Run Action")]
        private void ContextRunAction()
        {
            Debug.Log("Running Action: " + m_ActionID);
            FlowManager.Instance.TriggerAction(m_ActionID);
        }
        #endregion
    }
}
