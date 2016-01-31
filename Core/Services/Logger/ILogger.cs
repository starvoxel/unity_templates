/* --------------------------
 *
 * ILogger.cs
 *
 * Description: Interface for any loggers.  Used in Services.cs to hold a ref to the currently active logger.
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
using System.Collections;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.Core
{
	public interface ILogger
	{
		#region Fields & Properties
		//properties
		#endregion

        #region Methods
        void TestAllFunctions();

        /// <summary>
        /// Log a message under a specific category.
        /// </summary>
        /// <param name="logType">What type of log is it</param>
        /// <param name="msg">The string.  The args passed in will be used to format the string.</param>
        /// <param name="args">Args used to format the string.</param>
        void LogWithCategory(string category, string msg, params object[] args);

        /// <summary>
        /// Log a message under the default category.
        /// </summary>
        /// <param name="msg">The string.  The args passed in will be used to format the string.</param>
        /// <param name="args">Args used to format the string.</param>
        void Log(string msg, params object[] args);

        /// <summary>
        /// Log a message under the warning category.  This should be used to warn the developer that something could be/go wrong.
        /// </summary>
        /// <param name="msg">The string.  The args passed in will be used to format the string.</param>
        /// <param name="args">Args used to format the string.</param>
        void LogWarning(string msg, params object[] args);

        /// <summary>
        /// Log a message under the error category.  This should only be used when something has gone completely wrong.
        /// </summary>
        /// <param name="msg">The string.  The args passed in will be used to format the string.</param>
        /// <param name="args">Args used to format the string.</param>
        void LogError(string msg, params object[] args);

        /// <summary>
        /// Adds the provided value to the proper list of values based on the ID
        /// </summary>
        /// <param name="id">ID of the particular variable.  Used to fetch data on it.</param>
        /// <param name="variable">Value of the variable when being logged.</param>
        void LogVariable(string id, object variable);
		#endregion
	}
}
