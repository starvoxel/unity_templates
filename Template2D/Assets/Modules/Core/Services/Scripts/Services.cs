/* --------------------------
 *
 * Services.cs
 *
 * Description: Service pattern implementation.  Will be used for systems that need to be publicly available but can have different implementations based on various things.
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
	public static class Services
	{
		#region Fields & Properties
		//const
	
		//public

        //private
        private static Logger m_Logger;
	
		//properties
        public static Logger Logger
        {
            get { return m_Logger; }
        }
		#endregion
	
		#region Public Methods
        public static void InitializeLogger(Logger logger)
        {
            if (m_Logger == null)
            {
                m_Logger = logger;
            }
        }
		#endregion
	}
}
