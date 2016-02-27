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
using LogType = UnityEngine.LogType;
#endregion
#endregion

 namespace Starvoxel.Core
{
	public class TextFileLogger : Logger
	{
		#region Fields & Properties
		//const
        private const int WRITE_INTERVAL = 2;
        private const string FILE_NAME_FORMAT = "HH-mm_ss";
        private const string FILE_PATH = LoggerConstants.BASE_FOLDER_STRUCTUR + "{0}/{1}/{2}/{3}.log";

        //struct
	
		//public
	
		//protected
        protected int m_LogOutputIndex = 0; // Index of the next log that needs to be written to the log file
        protected string m_FilePath = string.Empty;

        private Thread m_OutputThread;

        //private

		//properties
		#endregion
	
		#region Constructor Methods
        public TextFileLogger() : this(false) { }

        public TextFileLogger(bool isLoggingToConsole) : base(isLoggingToConsole)
        {
            //Conatenate the file path.  Always puts it in the persistentDataPath so sometimes it has weird paths but they won't be flushed.
            System.DateTime now = System.DateTime.Now;
            m_FilePath = UnityEngine.Application.persistentDataPath + string.Format(FILE_PATH, now.Year.ToString("0000"), now.Month.ToString("00"), now.Day.ToString("00"), now.ToString(FILE_NAME_FORMAT));

            // Start up a thread to output the logs to a file so that if we have a ot of logs we don't clog up the main thread at all
            ThreadStart newThreadStart = new ThreadStart(WriteLogsToFile);
            m_OutputThread = new Thread(newThreadStart);
            m_OutputThread.Start();
            while (!m_OutputThread.IsAlive) ;
        }
		#endregion

        #region Public Methods
		#endregion
	
		#region Protected Methods
		#endregion

        #region Private Methods
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
                Thread.Sleep(WRITE_INTERVAL * 1000);
            }
        }
		#endregion
    }
}
