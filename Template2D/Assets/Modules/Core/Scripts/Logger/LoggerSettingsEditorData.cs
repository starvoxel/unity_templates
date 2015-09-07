/* --------------------------
 *
 * LoggerSettingsEditorData
 *
 * Description: All of the editor specific stuff is done here.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 9/6/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#if UNITY_EDITOR

#region Includes
#region Unity Includes
using UnityEngine;
using UnityEditor;
#endregion

#region System Includes
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel
{
    public partial class LoggerSettings
    {
        #region Fields & Properties
        //const
        private const string EDITOR_FLAG_KEY = "ActiveFlags_";

        public static readonly char FLAG_DIVIDER_CHAR = ',';

        //public

        //protected

        //private
        private eLoggerFlags[] m_ActiveEditorFlags;

        //properties
        public int EditorActiveFlagValue
        {
            get
            {
                int value = 0;

                if (m_ActiveEditorFlags != null)
                {
                    for (int i = 0; i < m_ActiveEditorFlags.Length; ++i)
                    {
                        value |= m_ActiveEditorFlags[i].Value;
                    }
                }

                return value;
            }
        }

        public eLoggerFlags[] ActiveEditorFlags
        {
            get { return m_ActiveEditorFlags; }
            set
            {
                if (value == null)
                {
                    m_ActiveEditorFlags = new eLoggerFlags[0];
                }
                else
                {
                    m_ActiveEditorFlags = value;
                }

                SaveEditorFlags();
            }
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        partial void EditorConstructor()
        {
            string editorFlagsString = EditorPrefs.GetString(EDITOR_FLAG_KEY + PlayerSettings.productName, string.Empty);
        
            if (!string.IsNullOrEmpty(editorFlagsString))
            {
                string[] flags = editorFlagsString.Split(FLAG_DIVIDER_CHAR);

                m_ActiveEditorFlags = new eLoggerFlags[flags.Length];

                for(int i = 0; i < flags.Length; ++i)
                {
                    m_ActiveEditorFlags[i] = eLoggerFlags.GetFlagForName(flags[i]);
                }
            }
            else
            {
                m_ActiveEditorFlags = new eLoggerFlags[0];
            }
        }

        private void SaveEditorFlags()
        {
            string editorFlagsString = string.Empty;

            for(int i = 0; i < m_ActiveEditorFlags.Length; ++i)
            {
                if(i != 0)
                {
                    editorFlagsString += FLAG_DIVIDER_CHAR;
                }

                editorFlagsString += eLoggerFlags.GeNameForFlag(m_ActiveEditorFlags[i]);
            }

            EditorPrefs.SetString(EDITOR_FLAG_KEY + PlayerSettings.productName, editorFlagsString);
        }
        #endregion
    }
}

#endif