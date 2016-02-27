/* --------------------------
 *
 * Logger.cs
 *
 * Description: Base class for all loggers.  Has basic queue functionality but doesn't output anything at all, that's up to inherited classes
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 2/2/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using StackTrace = System.Diagnostics.StackTrace;
using StackFrame = System.Diagnostics.StackFrame;
#endregion

#region Other Includes
using UpdateCaller = Starvoxel.Utilities.UpdateCaller;
using LogType = UnityEngine.LogType;
using Debug = UnityEngine.Debug;
#endregion
#endregion

 namespace Starvoxel.Core
 {
     public struct sLogInfo
     {
         public string[] Categories;
         public string Message;
         public string Timestamp;
         public string StackInfo;
         public LogType SeverityLevel;

         public override string ToString()
         {
             return string.Format("{0}-{1} | {2} {3} {4}", Timestamp, SeverityLevel.ToString(), Message, StackInfo, FormatCategories());
         }

         public string FormatCategories()
         {
             string categoryInfo = string.Empty;

             if (Categories != null && Categories.Length > 0)
             {
                 categoryInfo = "{";

                 for (int i = 0; i < Categories.Length; ++i)
                 {
                     if (i > 0)
                     {
                         categoryInfo += ", ";
                     }

                     categoryInfo += Categories[i];
                 }

                 categoryInfo += "}";
             }

             return categoryInfo;
         }
     }
	public abstract class Logger
	{
		#region Fields & Properties
		//const

        //struct

        //public
        public bool IsLoggingToConsole = false;
	
		//protected
        protected List<sLogInfo> m_AllLogs = new List<sLogInfo>();
        protected List<string> m_ActiveCategories = null;
		//private

		//properties
		#endregion
	
		#region Constructor Methods
        public Logger() : this(false) { }

        public Logger(bool isLoggingToConsole)
        {
            IsLoggingToConsole = isLoggingToConsole;
        }
		#endregion

        #region Public Methods
        /// <summary>
        /// Test function that can be used in the editor to test to make sure all functions work.
        /// </summary>
        public virtual void TestAllFunctions()
        {
#if UNITY_EDITOR
            this.Log("Test normal log. {0}", 0);
            List<string> categories = new List<string>();
            categories.Add(LoggerConstants.INPUT_CATEGORY);
            categories.Add(LoggerConstants.TESTING_CATEGORY);
            this.LogWithCategories(categories, LogType.Log, "Test multiple categories. {0}", 1);
            this.LogWithCategory(LoggerConstants.CORE_CATEGORY, LogType.Log, "Test Core category log. {0}", 2);
            this.LogWarning("Test warning log. {0}", 3);
            this.LogError("Test error log. {0}", 4);

            this.LogVariable("TEST_ID", 2);
            this.LogVariable("TEST_ID", 3);
            this.LogVariable("TEST_ID", 1);
#endif
        }

        /// <summary>
        /// Clears all active categories.  When no categories are specified active, all are considered valid.
        /// </summary>
        public void ClearActiveCategories()
        {
            if (m_ActiveCategories != null)
            {
                m_ActiveCategories.Clear();
            }
        }

        /// <summary>
        /// Add specific categories to the active list.
        /// </summary>
        /// <param name="categories"></param>
        public virtual void AddCategories(params string[] categories)
        {
            if (categories != null && categories.Length > 0)
            {
                if (m_ActiveCategories == null)
                {
                    m_ActiveCategories = new List<string>();
                }

                string curCategory;

                for(int i = 0; i < categories.Length; ++i)
                {
                    curCategory = categories[i];
                    if (curCategory == null)
                    {
                        continue;
                    }

                    curCategory = curCategory.ToUpper();
                    if (!m_ActiveCategories.Contains(curCategory))
                    {
                        m_ActiveCategories.Add(curCategory);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a log with a single category for the msg provided.
        /// </summary>
        /// <param name="category">The single category</param>
        /// <param name="severityLevel">The severity of this log</param>
        /// <param name="msg">Message that will be logged</param>
        /// <param name="args">Objects that will be formatted into the msg is need be</param>
        public void LogWithCategory(string category, LogType severityLevel, string msg, params object[] args)
        {
            LogWithCategories(new List<string>(new string[] { category }), severityLevel, msg, args);
        }

        /// <summary>
        /// Creates a log with multiple ategories for the msg provided
        /// </summary>
        /// <param name="categories">The multiple categories</param>
        /// <param name="severityLevel">THe severity of this log</param>
        /// <param name="msg">Message that will be logged</param>
        /// <param name="args">Objects that will be formatted into the msg is need be</param>
        public virtual void LogWithCategories(List<string> categories, LogType severityLevel, string msg, params object[] args)
        {
            //If none of the ategories are active, skip this log
            if (!IsAnyCategoryActive(categories.ToArray()))
            {
                return;
            }

            StackTrace stack = new StackTrace(1, true);

            sLogInfo newLogInfo = new sLogInfo();
            newLogInfo.Message = string.Format(msg, args);
            newLogInfo.Timestamp = UnityEngine.Time.time.ToString();
            newLogInfo.StackInfo = CreateInfoLine(stack);
            newLogInfo.Categories = categories.ToArray();
            newLogInfo.SeverityLevel = severityLevel;

            ProcessLog(newLogInfo, stack);
        }

        /// <summary>
        /// Creates a log for the specified msg with the default category and lowest severity level.
        /// </summary>
        /// <param name="msg">Message that will be logged</param>
        /// <param name="args">Objects that will be formatted into the msg is need be</param>
        public void Log(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.GetDefaultCategory(), LogType.Log, msg, args);
        }


        /// <summary>
        /// Creates a log for the specified msg with the default category and lowest severity level.
        /// </summary>
        /// <param name="msg">Message that will be logged</param>
        /// <param name="args">Objects that will be formatted into the msg is need be</param>
        public void LogWarning(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.GetDefaultCategory(), LogType.Warning, msg, args);
        }


        /// <summary>
        /// Creates a log for the specified msg with the default category and lowest severity level.
        /// </summary>
        /// <param name="msg">Message that will be logged</param>
        /// <param name="args">Objects that will be formatted into the msg is need be</param>
        public void LogError(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.GetDefaultCategory(), LogType.Error, msg, args);
        }

        /// <summary>
        /// Logs the variable to the list of variables over time to be able to create graphs and such
        /// WARNING: THIS ISN'T IMPLEMENTED YET!!
        /// </summary>
        /// <param name="id">ID used to track this variable over time</param>
        /// <param name="variable">Objet to be tracked</param>
        public virtual void LogVariable(string id, object variable)
        {
            // This won't be done for a good long time but I definitely want it to happen!
        }
		#endregion
	
		#region Protected Methods
        protected virtual void ProcessLog(sLogInfo logInfo, StackTrace stackInfo)
        {
            m_AllLogs.Add(logInfo);

            string logString = logInfo.Message + logInfo.StackInfo + logInfo.FormatCategories() + FormatStackTrace(stackInfo);

            // Don't know if I'll keep this around for ever...  This is just used to be able to visualize the logs that are making it through the category filter
            if (IsLoggingToConsole)
            {
                // Do a proper Debug log based off the severity level
                if (logInfo.SeverityLevel == LogType.Error || logInfo.SeverityLevel == LogType.Exception)
                {
                    UnityEngine.Debug.LogError(logString);
                }
                else if (logInfo.SeverityLevel == LogType.Warning)
                {
                    UnityEngine.Debug.LogWarning(logString);
                }
                else
                {
                    UnityEngine.Debug.Log(logString);
                }
            }
        }

        /// <summary>
        /// Checks to see if any of the categories are active
        /// </summary>
        /// <param name="categories">Array of categories</param>
        /// <returns></returns>
        protected bool IsAnyCategoryActive(string[] categories)
        {
            if (categories != null && categories.Length > 0 && m_ActiveCategories != null && m_ActiveCategories.Count > 0)
            {
                bool isActive = false;
                for (int i = 0; i < categories.Length; ++i)
                {
                    isActive |= m_ActiveCategories.Contains(categories[i]);
                }

                return isActive;
            }
            //If we don't have any active categories specified, all categories are valid
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Create a formatted string containing the info based on the current frame
        /// </summary>
        /// <param name="stack">Stack trace info</param>
        /// <returns></returns>
        protected virtual string CreateInfoLine(StackTrace stack)
        {
            int frameIndex = GetIndexForFirstValidFrame(stack);
            if (frameIndex >= 0)
            {
                StackFrame frame = stack.GetFrame(GetIndexForFirstValidFrame(stack));
                return string.Format("\n[{0}:{1} @ {2}] ", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name, frame.GetFileLineNumber());
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formats the full stack trace.  Removes calls from logger classes which would just bloat the logs
        /// </summary>
        /// <param name="stack"Stack trace info></param>
        /// <returns></returns>
        protected virtual string FormatStackTrace(StackTrace stack)
        {
            string formattedStack = "\n\n--- STACK TRACE ---";
#if !UNITY_EDITOR
            int startingIndex = GetIndexForFirstValidFrame(stack);

            if (startingIndex >= 0)
            {
                StackFrame[] frames = stack.GetFrames();
                StackFrame frame;

                for (int frameIndex = startingIndex; frameIndex < frames.Length; ++frameIndex)
                {
                    frame = frames[frameIndex];
                    System.Reflection.MethodBase method = frame.GetMethod();
                    formattedStack += "\n" + method.DeclaringType.FullName + ":" + method.Name + "(";
                    System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                    if (parameters != null)
                    {
                        System.Reflection.ParameterInfo parameter;

                        for (int paramIndex = 0; paramIndex < parameters.Length; ++paramIndex)
                        {
                            parameter = parameters[paramIndex];

                            if (paramIndex != 0)
                            {
                                formattedStack += ", ";
                            }

                            formattedStack += parameter.ParameterType.Name;
                        }
                    }

                    string fileName = frame.GetFileName();
                    int assetIndex = fileName.LastIndexOf("Assets");

                    formattedStack += ") (at " + fileName.Substring(assetIndex) + ":" + frame.GetFileLineNumber() + ")";
                }

                formattedStack += "\n--- STACK TRACE END ---\n\n";
            }
#endif

            return formattedStack;
        }

        /// <summary>
        /// Returns a int indicating the first stack index for a valid frame
        /// </summary>
        /// <param name="stack">Stack trace info</param>
        /// <returns></returns>
        protected int GetIndexForFirstValidFrame(StackTrace stack)
        {
            int counter = -1;
            StackFrame frame;

            System.Type methodType;

            do
            {
                counter += 1;
                frame = stack.GetFrame(counter);
                methodType = frame.GetMethod().DeclaringType;
            } while (counter < stack.GetFrames().Length && methodType.IsAssignableFrom(typeof(Logger)));

            if (counter >= stack.GetFrames().Length)
            {
                return -1;
            }
            else
            {
                return counter;
            }
        }
		#endregion
    }
}
