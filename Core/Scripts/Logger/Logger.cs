/* --------------------------
 *
 * Logger.cs
 *
 * Description: Basic logger to replace Unity's built-in one.
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
#region UnityIncludes
using UnityEngine;
#endregion

#region System Includes
using System.Collections;
using StackTrace = System.Diagnostics.StackTrace;
using StackFrame = System.Diagnostics.StackFrame;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel
{
	public static class Logger
	{
		#region Fields & Properties
		//const
        public const string SETTINGS_PATH = "ScriptableObjects/LoggerSettings";
        public const string INFO_START = "\n[ ";
        public const string INFO_END = " ]\n";
        public const string STACKTRACE_START = "\n -- Stack Trace --\n";
	
		//public
	
		//protected

        //private
        private static LoggerSettings m_Settings = null;
	
		//properties
		#endregion
	
		#region Public Methods
        public static void Log(string msg, params object[] args)
        {
            Initialize();

            Debug.LogFormat(CreateFinalMessage(msg), args);
        }

        public static void Log(Object context, string msg, params object[] args)
        {
            Initialize();

            string info = CreateInfoLine();

            if (context == null)
            {
                Debug.LogFormat(CreateFinalMessage(msg), args);
            }
            else
            {
                Debug.LogFormat(context, CreateFinalMessage(msg), args);
            }
        }
		#endregion

        #region Private Methods
        private static void Initialize()
        {
            if (m_Settings == null)
            {
                m_Settings = Resources.Load<LoggerSettings>(SETTINGS_PATH);
            }

            if (m_Settings == null)
            {
                throw new System.NotSupportedException("Logger Settings object not found.  Please make sure it exists at the following path: " + SETTINGS_PATH);
            }
        }

        private static string CreateInfoLine()
        {
            string info = null;

            if (m_Settings.IncludeClassName)
            {
                if (string.IsNullOrEmpty(info))
                {
                    info = INFO_START;
                }
                info += GetClassName() + ".";
            }

            if (m_Settings.IncludeMethodName)
            {
                if (string.IsNullOrEmpty(info))
                {
                    info = INFO_START;
                }
                info += GetMethodName() + "()";
            }

            if (m_Settings.IncludeLineNumber)
            {
                if (string.IsNullOrEmpty(info))
                {
                    info = INFO_START;
                }
                info += " @ " + GetLineNumber();
            }

            if (!string.IsNullOrEmpty(info))
            {
                info += INFO_END;
            }

            return info;
        }

        private static string CreateFinalMessage(string msg)
        {
            return msg + CreateInfoLine() + STACKTRACE_START;
        }

        private static string GetClassName()
        {
            StackTrace stack = new StackTrace(2, true);
            StackFrame frame = stack.GetFrame(1);

            return frame.GetMethod().DeclaringType.FullName;
        }

        private static string GetMethodName()
        {
            StackTrace stack = new StackTrace(2, true);
            StackFrame frame = stack.GetFrame(1);

            return frame.GetMethod().Name;
        }

        private static int GetLineNumber()
        {
            StackTrace stack = new StackTrace(2, true);
            StackFrame frame = stack.GetFrame(1);

            return frame.GetFileLineNumber();
        }
		#endregion
	}
}