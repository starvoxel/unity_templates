/* --------------------------
 *
 * CoroutineRunner.cs
 *
 * Description: A helper class used to run coroutines more easily for non-Monobehaviour classes
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
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.Utilities
{
	public class CoroutineRunner : CustomMono
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
        static GameObject m_RunnerParent = null;
	
		//properties
		#endregion
	
		#region Public Methods
        public static CoroutineRunner FetchCoroutineRunner()
        {
            if (m_RunnerParent == null)
            {
                m_RunnerParent = new GameObject("CoroutineRunnerParent");
                DontDestroyOnLoad(m_RunnerParent);
            }

            CoroutineRunner newRunner = (new GameObject("CoroutineRunner")).AddComponent<CoroutineRunner>();
            newRunner.transform.parent = m_RunnerParent.transform;
            return newRunner;
        }

        public Coroutine CreateCoroutine(IEnumerator function)
        {
            return StartCoroutine(function);
        }

        public void CancelCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
		#endregion
	
		#region Private Methods
		#endregion
	}
	
}