/* --------------------------
 *
 * View.cs
 *
 * Description: Class that represents a view in the game.  Has all the methods used during the change of flow.
 * 
 * Flow of a view:
 *  - OnLoaded
 *  - OnOpeningSequence
 *  - OnOpened
 *  - OnFocusLost
 *  - OnFocusGained
 *  - OnClosingSequence
 *  - OnClosed
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
#endregion

#region Other Includes
using Starvoxel.Core;
#endregion
#endregion

namespace Starvoxel.FlowManagement
{
	public class View : MonoBehaviour
    {
        #region Data Types
        public enum eViewState
        {
            LOADED,
            OPENING,
            OPENED,
            CLOSING,
            CLOSED,
            LOSING_FOCUS,
            FOCUS_LOST,
            GAINING_FOCUS
        }

        public delegate void ViewStateChangedEventHandler(View view);
        #endregion

        #region Fields & Properties
        //const

        //static
	
		//public
        public event ViewStateChangedEventHandler StateChanged;
	
		//protected
        protected eViewState m_State;
        protected Dictionary<string, object> m_Parameters;
        protected Dictionary<string, object> m_SequenceParameters;
	
		//private
	
		//properties
        public eViewState State
        {
            get { return m_State; }
        }
		#endregion
	
		#region Unity Methods
        protected virtual void Awake()
        {
            m_State = eViewState.CLOSED;

            if (!ApplicationInitializer.IsInitialized)
            {
                ApplicationInitializer.Initialize();
            }
        }
		#endregion

        #region Public API
        public object GetParameter(string key)
        {
            if (m_Parameters != null && m_Parameters.Count > 0 && m_Parameters.ContainsKey(key))
            {
                return m_Parameters[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Starts the loading sequence of the view
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public IEnumerator ViewLoaded(Dictionary<string, object> parameters)
        {
            yield return StartCoroutine(OnViewLoaded(parameters));
        }

        /// <summary>
        /// Starts opening the view.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public void OpenView(Dictionary<string, object> parameters)
        {
            m_SequenceParameters = parameters;

            OnOpeningSequenceStarted();
        }

        /// <summary>
        /// Starts closing the view.
        /// </summary>
        /// <param name="parameters"></param>
        public void CloseView(Dictionary<string, object> parameters)
        {
            m_SequenceParameters = parameters;

            OnClosingSequenceStarted();
        }

        /// <summary>
        /// Starts the losing focus sequence.
        /// </summary>
        /// <param name="parameters"></param>
        public void LoseFocus(Dictionary<string, object> parameters)
        {
            OnLosingFocus(parameters);
        }

        /// <summary>
        /// Starts regaining focus.
        /// </summary>
        /// <param name="parameters"></param>
        public void GainFocus(Dictionary<string, object> parameters)
        {
            OnGainingFocus(parameters);
        }
		#endregion

        #region Protected Methods
        /// <summary>
        /// Called when the view prefab is finished loading and can start to do any loading that it needs.
        /// NOTE: Always call the base when you inherite.
        /// </summary>
        /// <param name="parameters">Dictionary of parameters that the action that launched this view contained.</param>
        protected virtual IEnumerator OnViewLoaded(Dictionary<string, object> parameters)
        {
            ChangeState(eViewState.LOADED);
            m_Parameters = parameters;

            yield break;
        }

        /// <summary>
        /// Called when the opening sequence should be started.  Everything that has to do with the introduction of a view should be triggered from here.
        /// NOTE: Always call the base when you inherite.
        /// </summary>
        protected virtual void OnOpeningSequenceStarted()
        {
            ChangeState(eViewState.OPENING);

            OnOpeningSequeneComplete();
        }

        /// <summary>
        /// Called when the opening sequence is finished and the flow should continue.
        /// NOTE: Always call the base when you inherite.
        /// </summary>
        protected virtual void OnOpeningSequeneComplete()
        {
            OnViewOpened(m_SequenceParameters);
            m_SequenceParameters = null;
        }

        /// <summary>
        /// Called when the entire opening is completed and the view can start it's actual flow.
        /// NOTE: Always call the base when you inherite.
        /// </summary>
        protected virtual void OnViewOpened(Dictionary<string, object> parameters)
        {
            ChangeState(eViewState.OPENED);

            FlowManager.Instance.OnViewOpened(this);
        }

        /// <summary>
        /// Called when a new view is in the process is opening on top of it.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        protected virtual void OnLosingFocus(Dictionary<string, object> parameters)
        {
            ChangeState(eViewState.LOSING_FOCUS);

            OnFocusLost(parameters);
        }

        /// <summary>
        /// Called when a new view has opened on top of it.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        protected virtual void OnFocusLost(Dictionary<string, object> parameters)
        {
            ChangeState(eViewState.FOCUS_LOST);
        }

        /// <summary>
        /// Called when a view on top of it is in the process is of closing.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        protected virtual void OnGainingFocus(Dictionary<string, object> parameters)
        {
            ChangeState(eViewState.GAINING_FOCUS);

            OnFocusGained(parameters);
        }

        /// <summary>
        /// Called when a view has closed on top of it.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        protected virtual void OnFocusGained(Dictionary<string, object> parameters)
        {
            ChangeState(eViewState.OPENED);
        }

        /// <summary>
        /// Called when the closing sequence has started.  All outro animations should be triggered from here.
        /// NOTE: Always call the base when you inherite.
        /// </summary>
        protected virtual void OnClosingSequenceStarted() 
        {
            ChangeState(eViewState.CLOSING);

            OnClosingSequenceComplete();
        }

        /// <summary>
        /// Called when the closing sequence is complete and the flow should continue.
        /// NOTE: Always call the base when you inherite.
        /// </summary>
        protected virtual void OnClosingSequenceComplete()
        {
            OnViewClosed(m_SequenceParameters);
            m_SequenceParameters = null;
        }

        /// <summary>
        /// Called when the entire view closing sequence is complete.
        /// NOTE: Always call the base when you inherite.
        /// </summary>
        protected virtual void OnViewClosed(Dictionary<string, object> parameters)
        {
            ChangeState(eViewState.CLOSED);

            FlowManager.Instance.OnViewClosed(this);
        }
		#endregion
	
		#region Private Methods
        /// <summary>
        /// Changes the state to the provided one and fires the state changed event.
        /// </summary>
        /// <param name="newState">New state.</param>
        private void ChangeState(eViewState newState)
        {
            m_State = newState;

            if (StateChanged != null)
            {
                StateChanged(this);
            }
        }
		#endregion
	}
}