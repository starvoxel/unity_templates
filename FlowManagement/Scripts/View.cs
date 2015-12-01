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
using System.Collections;
using System.Collections.Generic;
#endregion
#endregion

namespace Starvoxel.FlowManagement
{
	public class View : MonoBehaviour
    {
        #region Enums
        public enum eViewState
        {
            LOADED,
            OPENING,
            OPENED,
            CLOSING,
            CLOSED,
            LOSING_FOCUS,
            FOCUS_LOST,
            GAINING_FOCUS,

        }
        #endregion

        #region Fields & Properties
        //const
	
		//public
	
		//protected
        protected eViewState m_State;
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
        protected virtual void Awake()
        {
            m_State = eViewState.CLOSED;
        }
		#endregion

        #region Public API
        /// <summary>
        /// Starts the loading sequence of the view
        /// </summary>
        /// <param name="parameters"></param>
        public IEnumerator ViewLoaded(Dictionary<string, object> parameters)
        {
            m_State = eViewState.LOADED;

            yield break;
        }
        /// <summary>
        /// Starts opening the view.
        /// NOTE: Always call the base when you inherite.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public void OpenView(Dictionary<string, object> parameters)
        {

        }

        public void CloseView(Dictionary<string, object> parameters)
        {

        }

        public void LoseFocus(Dictionary<string, object> parameters)
        {

        }

        public void GainFocus(Dictionary<string, object> parameters)
        {

        }
		#endregion

        #region Protected Methods
        /// <summary>
        /// Called when the view prefab is finished loading and can start to do any loading that it needs.
        /// </summary>
        /// <param name="parameters">Dictionary of parameters that the action that launched this view contained.</param>
        protected virtual void OnViewLoaded(Dictionary<string, object> parameters)
        {
            // jsmellie: Nothing really happens here in the base class but anything that inherites from it can do what ever it wants with he params.
        }

        /// <summary>
        /// Called when the entire view loading sequence is complete.
        /// </summary>
        protected virtual void OnViewOpened()
        {
            // jsmellie: Don't think anything really needs to be done here either...  But other classes will deffinitly need this.
        }

        /// <summary>
        /// Called when the entire view closing sequence is complete.
        /// </summary>
        protected virtual void OnViewClosed()
        {

        }
		#endregion
	
		#region Private Methods
		#endregion
	}
}