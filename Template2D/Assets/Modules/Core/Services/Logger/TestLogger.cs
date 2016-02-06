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
	public class TestLogger : Logger
	{
		#region Fields & Properties
		//const

        //struct
	
		//public
	
		//protected
	
		//private

		//properties
		#endregion
	
		#region Constructor Methods
        public TestLogger() : this(false) { }

        public TestLogger(bool isLoggingToConsole) : base(isLoggingToConsole) { }
		#endregion

        #region Public Methods
		#endregion
	
		#region Protected Methods
		#endregion

        #region Private Methods
		#endregion
    }
}
