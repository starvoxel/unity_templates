/* --------------------------
 *
 * Overlay.cs
 *
 * Description: Basic overlay that just turns on/off
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 1/30/2016 - Starvoxel
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
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.FlowManagement
{
	public class Overlay : CustomMono
	{
		#region Fields & Properties
		//const

        // enums
        public enum eState
        {
            HIDDEN,
            ANIMATING_IN,
            SHOWING,
            ANIMATING_OUT
        }
	
		//public
	
		//protected
        protected System.Action m_OnShowComplete = null;
        protected System.Action m_OnHideComplete = null;

        protected eState m_CurrentState = eState.HIDDEN;

        protected Canvas m_Canvas = null;
	
		//private
	
		//properties
        public eState State
        {
            get { return m_CurrentState; }
        }

        public Canvas Canvas
        {
            get { return m_Canvas; }
        }
		#endregion

        #region Unity Methods
        protected virtual void OnDestroy()
        {
            Destroy();
        }
        #endregion

        #region Public Methods
        public virtual void Initialize(System.Action onShowComplete, System.Action onHideComplete)
        {
            m_Canvas = GetComponentInChildren<Canvas>(true);

            m_OnShowComplete += onShowComplete;
            m_OnHideComplete += onHideComplete;

            Hide(true);
        }

        public virtual void Destroy()
        {
            m_OnShowComplete = null;
            m_OnHideComplete = null;
        }

        public virtual void Show(bool immidiate)
        {
            this.gameObject.SetActive(true);

            m_CurrentState = eState.SHOWING;
            if (m_OnShowComplete != null)
            {
                m_OnShowComplete();
            }
        }

        public virtual void Hide(bool immidiate)
        {
            this.gameObject.SetActive(false);

            m_CurrentState = eState.HIDDEN;
            if (m_OnHideComplete != null)
            {
                m_OnHideComplete();
            }
        }
		#endregion
	}
	
}