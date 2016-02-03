/* --------------------------
 *
 * TextFileLogger.cs
 *
 * Description: Logger used to output logs to simple text file.  Also has a bool to allow simple Debug.Log output aswell
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
#endregion
#endregion

 namespace Starvoxel.Core
{
	public class TextFileLogger : ILogger
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
        public bool IsLoggingToConsole = false;
	
		//protected
	
		//private
        private int m_LogOutputIndex = 0;
        private List<sLogInfo> m_AllLogs = new List<sLogInfo>();
        private string m_FilePath = string.Empty;

        private Thread m_OutputThread;

		//properties
		#endregion
	
		#region Constructor Methods
        public TextFileLogger() : this(false) { }

        public TextFileLogger(bool isLoggingToConsole)
        {
            //Conatenate the file path.  Always puts it in the persistentDataPath so sometimes it has weird paths but they won't be flushed.
            m_FilePath = UnityEngine.Application.persistentDataPath + string.Format(FILE_PATH, System.DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss"));

            IsLoggingToConsole = isLoggingToConsole;

            // Start up a thread to output the logs to a file so that if we have a ot of logs we don't clog up the main thread at all
            ThreadStart newThreadStart = new ThreadStart(WriteLogsToFile);
            m_OutputThread = new Thread(newThreadStart);
            m_OutputThread.Start();
            while (!m_OutputThread.IsAlive) ;
        }
		#endregion

        #region Public Methods
        int m_TestCounter = 0;
        public void TestAllFunctions()
        {
            this.Log("Test normal log. {0}", m_TestCounter++);
            List<string> categories = new List<string>();
            categories.Add(LoggerConstants.INPUT_CATEGORY);
            categories.Add(LoggerConstants.TESTING_CATEGORY);
            this.LogWithCategories(categories, "Test multiple categories. {0}", m_TestCounter++);
            this.LogWithCategory(LoggerConstants.CORE_CATEGORY, "Test Core category log. {0}", m_TestCounter++);
            this.LogWarning("Test warning log. {0}", m_TestCounter++);
            this.LogError("Test error log. {0}", m_TestCounter++);

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

            EnqueueLog(newLogInfo);

            if (IsLoggingToConsole)
            {
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
            m_AllLogs.Add(log);
        }

        private void WriteLogsToFile()
        {
            sLogInfo[] allLogs = null;

            while (true)
            {
                allLogs = m_AllLogs.ToArray();

                lock (this)
                {
                    if (allLogs.Length > m_LogOutputIndex)
                    {
                        if (!File.Exists(m_FilePath))
                        {
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(m_FilePath));
                        }

                        using (FileStream fileStream = new FileStream(m_FilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(fileStream))
                            {
                                sLogInfo info;
                                while (allLogs.Length > m_LogOutputIndex)
                                {
                                    info = allLogs[m_LogOutputIndex];
                                    streamWriter.WriteLine(info.ToString());
                                    streamWriter.WriteLine();
                                    m_LogOutputIndex++;
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(10000);
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
