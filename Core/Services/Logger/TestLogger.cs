/* --------------------------
 *
 * TestLogger.cs
 *
 * Description: Logger used for the creating of the logger system.  Will be deleted once no longer needed.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 12/5/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region System Includes
using System.IO;
using System.Collections;
using System.Collections.Generic;
using StackTrace = System.Diagnostics.StackTrace;
using StackFrame = System.Diagnostics.StackFrame;
#endregion

#region Other Includes
using UpdateCaller = Starvoxel.Utilities.UpdateCaller;
#endregion
#endregion

 namespace Starvoxel.Core
{
	public class TestLogger : ILogger
	{
		#region Fields & Properties
		//const
        private const string FILE_PATH = "/Logs/{0}.log";
        private const int WRITE_INTERVAL = 10;

        //struct
        private struct sLogInfo
        {
            public string[] Categories;
            public string Message;
            public string Timestamp;
            public string StackInfo;

            public override string ToString()
            {
                return string.Format("{0} | {1} {2} {3}", Timestamp, Message, StackInfo, FormatCategories());
            }

            public string FormatCategories()
            {
                string categoryInfo = string.Empty;

                if (Categories != null && Categories.Length > 0)
                {
                    categoryInfo = "{";

                    for(int i = 0; i < Categories.Length; ++i)
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
	
		//public
	
		//protected
	
		//private
        private int m_FrameCount = 0;
        private Queue<sLogInfo> m_QueuedLogs = new Queue<sLogInfo>();
        private List<sLogInfo> m_AllLogs = new List<sLogInfo>();
        private string m_FilePath = string.Empty;

		//properties
		#endregion
	
		#region Constructor Methods
        public TestLogger()
        {
            //TODO jsmellie: Do somekind of initialization of log info queue that I can use to output to some file.  Maybe spawn a seporate thread or something
            // so that it doesn't slow the game in any way.
            m_FilePath = UnityEngine.Application.persistentDataPath + string.Format(FILE_PATH, System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss"));
            UpdateCaller.LateUpdateAction += LateUpdate;
        }

        ~TestLogger()
        {
            UpdateCaller.LateUpdateAction -= LateUpdate;
        }
		#endregion

        #region Public Methods
        public void TestAllFunctions()
        {
            this.Log("Test normal log. {0}", true);
            List<string> categories = new List<string>();
            categories.Add(LoggerConstants.INPUT_CATEGORY);
            categories.Add(LoggerConstants.TESTING_CATEGORY);
            this.LogWithCategories(categories, "Test multiple categories. {0}", false);
            this.LogWithCategory(LoggerConstants.CORE_CATEGORY, "Test Core category log. {0}", true);
            this.LogWarning("Test warning log. {0}", true);
            this.LogError("Test error log. {0}", true);

            this.LogVariable("TEST_ID", 2);
            this.LogVariable("TEST_ID", 3);
            this.LogVariable("TEST_ID", 1);
        }

        public void LogWithCategory(string category, string msg, params object[] args)
        {
            LogWithCategories(new List<string>(new string[] { category }), msg, args);
        }

        public void LogWithCategories(List<string> categories, string msg, params object[] args)
        {
            StackTrace stack = new StackTrace(1, true);

            sLogInfo newLogInfo = new sLogInfo();
            newLogInfo.Message = string.Format(msg, args);
            newLogInfo.Timestamp = UnityEngine.Time.time.ToString();
            newLogInfo.StackInfo = CreateInfoLine(stack);
            newLogInfo.Categories = categories.ToArray();
            string test = newLogInfo.Message + newLogInfo.StackInfo + newLogInfo.FormatCategories() + FormatStackTrace(stack);

            m_QueuedLogs.Enqueue(newLogInfo);

            // Do a proper Debug log based off the categories passed
            if (categories.Contains(LoggerConstants.ERROR_CATEGORY))
            {
                UnityEngine.Debug.LogError(test);
            }
            else if (categories.Contains(LoggerConstants.WARNING_CATEGORY))
            {
                UnityEngine.Debug.LogWarning(test);
            }
            else
            {
                UnityEngine.Debug.Log(test);
            }
        }

        public void Log(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.GetDefaultCategory(), msg, args);
        }

        public void LogWarning(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.WARNING_CATEGORY, msg, args);
        }

        public void LogError(string msg, params object[] args)
        {
            LogWithCategory(LoggerConstants.ERROR_CATEGORY, msg, args);
        }

        public void LogVariable(string id, object variable)
        {
            // This won't be done for a good long time but I definitely want it to happen!
        }
		#endregion
	
		#region Protected Methods
		#endregion

        #region Private Methods
        private void EnqueueLog(sLogInfo log)
        {
            m_QueuedLogs.Enqueue(log);
            m_AllLogs.Add(log);
        }

        private void LateUpdate()
        {
            m_FrameCount++;

            if (m_FrameCount % WRITE_INTERVAL == 0)
            {
                WriteLogsToFile();
            }
        }

        private void WriteLogsToFile()
        {
            if (m_QueuedLogs.Count > 0)
            {
                if (!File.Exists(m_FilePath))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(m_FilePath));
                }

                UnityEngine.Debug.Log("Log Path: " + m_FilePath);

                using (FileStream fileStream = new FileStream(m_FilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        sLogInfo info;
                        while (m_QueuedLogs.Count > 0)
                        {
                            info = m_QueuedLogs.Dequeue();
                            streamWriter.WriteLine(info.ToString());
                            streamWriter.WriteLine();
                        }
                    }
                }
            }
        }

        private string CreateInfoLine(StackTrace stack)
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

        private string FormatStackTrace(StackTrace stack)
        {
            string formattedStack = "\n\n--- STACK TRACE ---";
#if UNITY_EDITOR
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

        private int GetIndexForFirstValidFrame(StackTrace stack)
        {
            int counter = -1;
            StackFrame frame;

            do
            {
                counter += 1;
                frame = stack.GetFrame(counter);
            } while (counter < stack.GetFrames().Length && frame.GetMethod().DeclaringType == this.GetType());

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
