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
#endregion

#region System Includes
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
#endregion
#endregion

namespace Starvoxel
{
    public partial class LoggerSettings : CustomScriptableObject
    {
        #region Fields & Properties
        //const

        //public

        //private
        [Header("Stack information")]
        [SerializeField]
        protected bool m_IncludeClassName = true;
        [SerializeField]
        protected bool m_IncludeMethodName = true;
        [SerializeField]
        protected bool m_IncludeLineNumber = true;

        [SerializeField, HideInInspector]
        protected eLoggerFlags[] m_ActiveFlags;

        //properties
        public bool IncludeClassName
        {
            get { return m_IncludeClassName; }
        }
        public bool IncludeMethodName
        {
            get { return m_IncludeMethodName; }
        }
        public bool IncludeLineNumber
        {
            get { return m_IncludeLineNumber; }
        }

        public int ActiveFlagValue
        {
            get
            {
                int value = 0;

                if (m_ActiveFlags != null)
                {
                    for(int i = 0; i < m_ActiveFlags.Length; ++i)
                    {
                        value |= m_ActiveFlags[i].Value;
                    }
                }

                return value;
            }
        }
        #endregion

        #region Public Methods
        public LoggerSettings()
        {
            EditorConstructor();
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion

        #region Editor Methods
        partial void EditorConstructor();
        #endregion
    }
}