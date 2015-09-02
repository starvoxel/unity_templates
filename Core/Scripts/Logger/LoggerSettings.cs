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

#endregion
#endregion

namespace Starvoxel
{
    public partial class LoggerSettings : CustomScriptableObject
    {
        #region Fields & Properties
        //const
        public const string DEFAULT_FLAG = "DEFAULT";

        public static readonly string FLAG_INFO_FILENAME = "LoggerFlag.lfi";

        public static readonly string FLAG_FILENAME = "LoggerFlags";
        public static readonly string FLAG_FILE_DIRECTORY = Application.dataPath + "/Scripts/Logger";
        public static readonly string FLAG_FILE_PATH = FLAG_FILE_DIRECTORY + "/" + FLAG_FILENAME + ".cs";

        //public

        //private
        [Header("Stack information")]
        [SerializeField]
        protected bool m_IncludeClassName = true;
        [SerializeField]
        protected bool m_IncludeMethodName = true;
        [SerializeField]
        protected bool m_IncludeLineNumber = true;

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
        #endregion

        #region Public Methods
        public LoggerSettings()
        {
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion

        #region Editor Functions
        #endregion
    }
}