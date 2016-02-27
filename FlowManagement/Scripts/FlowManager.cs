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

#region Defines
#define FLOW_MANAGEMENT
#endregion

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

#region Other Includes
using Starvoxel.Utilities;
using Starvoxel.Core;
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

	public partial class FlowManager
    {
        #region Fields & Properties
        //const
        public const string DEFAULT_STARTING_VIEW = "SPLASH";
        public const int DEFAULT_MODAL_DEPTH_OFFSET = -40;
        public const int DEFAULT_OVERLAY_DEPTH_OFFSET = 10;
        public const int DEFAULT_MODAL_CANVAS_OFFSET = 40;
        public const string DEFAULT_OVERLAY_PATH = "Prefabs/DefaultOverlay";

        public const string COROUTINE_RUNNER_NAME = "FlowManagerCoroutineRunner";

        public static readonly Version CURRENT_VERSION = new Version("1.0.0");

        public const string ACTION_NAME_KEY = "ACTION_NAME";
        public const string FROM_VIEW_NAME_KEY = "FROM_VIEW_NAME";

		//public
	
		//protected

        // Parsed data
        private ViewNode[] m_Views;  // All the view nodes that are were parsed from the XML
        private ActionNode[] m_GeneralActions; // All the general actions that can be used anywhere in the project
        private GeneralInformation m_GeneralInformation;

        private Stack<View> m_CurrentViewStack = new Stack<View>(); // Stack of all the current views that are loaded

        private List<View> m_ClosingViews = new List<View>(); // List oif all the views that are currentely closing
        private View m_OpeningView = null; // View that is in the process of opening
        private ViewNode m_OpeningViewNode; // View node that is in the process of being opened
        private Dictionary<string, object> m_CurrentParameters = null; // Current parameters that are being used for what ever process is happening

        private Queue<ActionQueueElement> m_ActionQueue = new Queue<ActionQueueElement>(); // Queue of all actions that are going to be processed

        private CoroutineRunner m_CoroutineRunner = null; // Script that we run the coroutines on seeing as FlowManager isn't  monobehaviour
#pragma warning disable 0414 // Disabling the "not used anywhere" warning because this will be used later to canel the coroutine if something goes wrong
        private Coroutine m_LoadingRoutine = null; // Ref to the coroutine for the loading sequence.  Will be used to cancel the routine if we need to
#pragma warning restore

        private Overlay m_Overlay; // OVerlay that goes behind modals

        private static FlowManager m_Instance = null; // Ref to the instance of flow manager
		//private
	
		//properties
        public bool IsBusy
        {
            get { return m_ClosingViews.Count > 0 || m_OpeningView != null; }
        }

        public bool IsInitialized
        {
            get { return m_GeneralInformation.IsInitialized && m_Views.Length > 0 && m_CoroutineRunner != null; }
        }

        public static FlowManager Instance
        {
            get 
            { 
                if (m_Instance == null)
                {
                    m_Instance = new FlowManager();
                }
                return m_Instance;  
            }
        }

        public static bool IsInstanceNull
        {
            get { return m_Instance == null; }
        }
		#endregion

        #region Constructor Methods
        public FlowManager()
        {
            if (m_CoroutineRunner == null)
            {
                m_CoroutineRunner = CoroutineRunner.FetchCoroutineRunner();
                m_CoroutineRunner.name = COROUTINE_RUNNER_NAME;
            }

            EventMessenger.Instance.AddListener<FlowEvent>(OnFlowEventFired);
        }

        ~FlowManager()
        {
            EventMessenger.Instance.RemoveListener<FlowEvent>(OnFlowEventFired);
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
                FlowParser parser = FlowParser.Parse(filePath, CURRENT_VERSION);
                m_GeneralInformation = parser.GeneralInformation;
                m_GeneralActions = parser.GeneralActions;
                m_Views = parser.Views;

                if (m_GeneralInformation.IsInitialized)
                {
                    PartialOnPreLaunch();

                    LoadOverlay(m_GeneralInformation.OverlayPrefabPath);

                    if (m_Overlay == null)
                    {
                        LoadOverlay(DEFAULT_OVERLAY_PATH);
                    }

                    if (m_Overlay == null)
                    {
                        Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Error, "Default overlay not found.  Please ensure a prefab with a Overlay script exists at {0}.", DEFAULT_OVERLAY_PATH);
                    }

                    View loadedView = GameObject.FindObjectOfType<View>();
                    if (loadedView != null)
                    {
                        m_OpeningView = loadedView;
                        m_OpeningViewNode = GetViewNodeForView(m_OpeningView);

                        m_CoroutineRunner.CreateCoroutine(InitializeOpenView());
                    }
                    else
                    {
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
                            Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Error, "Starting view ({0}) not found in view list.  Starting view must be a valid view.", m_GeneralInformation.StartingView);
                        }
                    }

                    PartialOnPostLaunch();
                }
            }
        }
		#endregion

		#region Private Methods
        /// <summary>
        /// Fired when a flow event is fired.
        /// </summary>
        /// <param name="ev">Flow event</param>
        private void OnFlowEventFired(FlowEvent ev)
        {
            ActionNode action = new ActionNode();

            // Try and find the action in the current views list of actions
            if (m_CurrentViewStack.Count > 0)
            {
                ViewNode focusedNode = GetViewNodeForView(m_CurrentViewStack.Peek());

                if (focusedNode.IsInitialzed)
                {
                    action = focusedNode.GetActionByID(ev.ActionID);
                }

                // If we still don't have action, try the general actions
                if (!action.IsInitialized && m_GeneralActions != null && m_GeneralActions.Length > 0)
                {
                    for (int actionIndex = 0; actionIndex < m_GeneralActions.Length; ++actionIndex)
                    {
                        if (m_GeneralActions[actionIndex].ID == ev.ActionID)
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
                newElement.Parameters = ev.Parameters;

                m_ActionQueue.Enqueue(newElement);

                if (!IsBusy)
                {
                    ProcessActions();
                }
            }
        }

        private IEnumerator InitializeOpenView()
        {
            PartialOnPreLoadNewScene();
            yield return m_CoroutineRunner.CreateCoroutine(LoadingView());
            PartialOnPreOpenView();
            yield return m_CoroutineRunner.CreateCoroutine(OpenView());
        }

        /// <summary>
        /// Attemps to load the overlay prefab at the provided path
        /// </summary>
        /// <param name="overlayPath">PAth of the overlay to load</param>
        private void LoadOverlay(string overlayPath)
        {
            if (!string.IsNullOrEmpty(overlayPath))
            {
                Overlay overlayPrefab = Resources.Load<Overlay>(overlayPath);

                if (overlayPrefab != null)
                {
                    m_Overlay = GameObject.Instantiate<Overlay>(overlayPrefab);
                    GameObject.DontDestroyOnLoad(m_Overlay.gameObject);
                    m_Overlay.Initialize(null, null);
                    m_Overlay.Hide(true);
                }
            }
        }

        private void ProcessActions()
        {
            // Only do something if we have actions to process
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

                // If the action has a view to load, load it
                if (!string.IsNullOrEmpty(curElement.Action.ViewID))
                {
                    LoadView(GetViewNodeForID(curElement.Action.ViewID));
                }
                // Otherwise it's a closing action and close the top view
                else if (GetViewNodeForView(m_CurrentViewStack.Peek()).IsModal)
                {
                    m_CoroutineRunner.CreateCoroutine(CloseCurrentView());
                }
            }
        }

        /// <summary>
        /// Loads the provided view based on it's information
        /// </summary>
        /// <param name="view">ViewNode that should be loaded</param>
        private void LoadView(ViewNode view)
        {
            m_OpeningViewNode = view;

            m_CoroutineRunner.CreateCoroutine(LoadingSequence());
        }

        /// <summary>
        /// Loading sequence for the new view
        /// </summary>
        /// <returns>Coroutine therefor must return an IEnumerator</returns>
        private IEnumerator LoadingSequence()
        {
            // TODO jsmellie: We should do some kind of "If this view is already open, reload it" type thing
            // If the new view isn't modal and we aren't reloading, delete all the currentely open views
            if (!m_OpeningViewNode.IsModal)
            {
                PartialOnPreCloseAllViews();
                yield return m_CoroutineRunner.CreateCoroutine(CloseAllViews());
            }
            // If we have open views then we need to tell them that they are losing focus because a modal view is opening
            else if (m_CurrentViewStack.Count > 0)
            {
                m_CurrentViewStack.Peek().LoseFocus(m_CurrentParameters);
            }

            PartialOnPreLoadNewScene();
            yield return m_CoroutineRunner.CreateCoroutine(LoadNewScene());
            PartialOnPreOpenView();
            yield return m_CoroutineRunner.CreateCoroutine(OpenView());
            ProcessActions();
        }

        /// <summary>
        /// Close the top view
        /// </summary>
        /// <returns>Coroutine therefor must return an IEnumerator</returns>
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

                UpdateOverlayState(false);

                if (m_CurrentViewStack.Count > 0)
                {
                    m_CurrentViewStack.Peek().GainFocus(m_CurrentParameters);
                }
            }
        }

        /// <summary>
        /// Close all currently open views.  Only continues when all views closing sequences are complete.
        /// </summary>
        /// <returns>Coroutine therefor must return an IEnumerator</returns>
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

        /// <summary>
        /// Load the scene associated with the new view
        /// </summary>
        /// <returns>Coroutine therefor must return an IEnumerator</returns>
        private IEnumerator LoadNewScene()
        {
            string newSceneName = m_OpeningViewNode.SceneName;
            yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);

            m_OpeningView = GetViewForSceneName(newSceneName);
                        
            if (m_OpeningView != null)
            {
                yield return m_CoroutineRunner.CreateCoroutine(LoadingView());
            }
            else
            {
                Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Error, "A view with the name {0} was not found.  Make sure that the view has the same name as the scene!", newSceneName);
            }
        }

        private IEnumerator LoadingView()
        {
            // Position the view based off how many views are open
            Vector3 pos = m_OpeningView.transform.position;
            pos.z = m_CurrentViewStack.Count * m_GeneralInformation.ModalDepthOffset;
            m_OpeningView.transform.position = pos;

            Canvas[] openingViewCanvases = m_OpeningView.GetComponentsInChildren<Canvas>(true);
            for (int canvasIndex = 0; canvasIndex < openingViewCanvases.Length; ++canvasIndex)
            {
                openingViewCanvases[canvasIndex].sortingOrder += m_GeneralInformation.ModalCanvasOffset * m_CurrentViewStack.Count;
            }

            UpdateOverlayState(true);

            //Start loading the view
            yield return m_CoroutineRunner.CreateCoroutine(m_OpeningView.ViewLoaded(m_CurrentParameters));
        }

        /// <summary>
        /// Opens the new view and waits until that view is done opening before continuing
        /// </summary>
        /// <returns>Coroutine therefor must return an IEnumerator</returns>
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

        /// <summary>
        /// Callback for when a view is done with it's closing sequence
        /// </summary>
        /// <param name="closedView">View whose closing sequence is complete</param>
        public void OnViewClosed(View closedView)
        {
            if (m_ClosingViews.Contains(closedView))
            {
                m_ClosingViews.Remove(closedView);
                SceneManager.UnloadScene(closedView.gameObject.name);
            }
        }

        /// <summary>
        /// Callback for when a view is done with it's opening sequence
        /// </summary>
        /// <param name="openedView">View whose opening sequence is complete</param>
        public void OnViewOpened(View openedView)
        {
            if (openedView == m_OpeningView)
            {
                m_CurrentViewStack.Push(openedView);
                m_OpeningView = null;
            }
        }

        private void UpdateOverlayState(bool useOpeningView)
        {
            if (m_CurrentViewStack.Count > 0)
            {
                View currentView = m_CurrentViewStack.Peek();
                if (useOpeningView)
                {
                    currentView = m_OpeningView;
                }

                ViewNode currentViewNode = GetViewNodeForView(currentView);

                // If the new view is a modal view and it wants a overlay, show the overlay and position it properly
                if (currentViewNode.IsModal && currentViewNode.ShowOverlay)
                {
                    if (m_Overlay.State == Overlay.eState.HIDDEN)
                    {
                        m_Overlay.Show(false);
                    }

                    Vector3 pos = m_Overlay.transform.position;
                    pos.z = currentView.transform.position.z + DEFAULT_OVERLAY_DEPTH_OFFSET;
                    m_Overlay.transform.position = pos;
                    
                    if (m_Overlay.Canvas != null)
                    {
                        m_Overlay.Canvas.sortingOrder = (m_CurrentViewStack.Count * m_GeneralInformation.ModalCanvasOffset) - (m_GeneralInformation.ModalCanvasOffset / 2);
                    }
                }
                // Else if the overlay is showing for what ever reason, hide it
                else if (m_Overlay.State == Overlay.eState.SHOWING || m_Overlay.State == Overlay.eState.ANIMATING_IN)
                {
                    m_Overlay.Hide(false);
                }
            }
        }

        #region Helper Functions
        private View GetViewForSceneName(string viewName)
        {
            View[] views = GameObject.FindObjectsOfType<View>();

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
    }
}
