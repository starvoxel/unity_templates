/* --------------------------
 *
 * UpdateCaller.cs
 *
 * Description: Class used by non-monobehaviour classes to get a update or late update call.
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
using System.Collections.Generic;
using Action = System.Action;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.Utilities
{
	public class UpdateCaller : CustomMono
	{
		#region Fields & Properties
		//const

        //public
        public static Action UpdateAction;
        public static Action FixedUpdateAction;
        public static Action LateUpdateAction;
	
		//protected
	
		//private
        private static UpdateCaller m_Instance;

		//properties
		#endregion
	
		#region Unity Methods
        private void Update()
        {
            if (UpdateAction != null)
            {
                UpdateAction();
            }
        }

        private void LateUpdate()
        {
            if (LateUpdateAction != null)
            {
                LateUpdateAction();
            }
        }

        private void FixedUpdate()
        {
            if (FixedUpdateAction != null)
            {
                FixedUpdateAction();
            }
        }
        #endregion

        #region Constructors
        static UpdateCaller()
        {
            m_Instance = (new GameObject("UpdateCaller")).AddComponent<UpdateCaller>();
            GameObject.DontDestroyOnLoad(m_Instance.gameObject);
        }
        #endregion
    }
	
}