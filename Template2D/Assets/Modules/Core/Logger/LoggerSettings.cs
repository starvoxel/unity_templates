/* --------------------------
 *
 * LoggerSettings.cs
 *
 * Description: All the settings data for the Logger class.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 8/28/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

#region System Includes
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel
{
	public class LoggerSettings : CustomScriptableObject
	{
		#region Fields & Properties
		//const
	
		//public
	
		//private
        [Header("Stack information")]
        [SerializeField] protected bool m_IncludeClassName = true;
        [SerializeField] protected bool m_IncludeMethodName = true;
        [SerializeField] protected bool m_IncludeLineNumber = true;
	
		//properties
        public bool IncludeClassName
        {
            get { return m_IncludeClassName;  }
        }
        public bool IncludeMethodName
        {
            get { return m_IncludeMethodName; }
        }
        public bool IncludeLineNumber
        {
            get { return m_IncludeLineNumber; }
        }
		#endregion
	
		#region Public Methods
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	
		#region Editor Functions
		#if UNITY_EDITOR
		[MenuItem("Assets/Create/ScriptableObjects/LoggerSettings", false, 5000)]
		public static void CreateAsset()
		{
			ScriptableObjectUtility.CreateAsset<LoggerSettings>();
		}
		#endif
		#endregion
	}
}