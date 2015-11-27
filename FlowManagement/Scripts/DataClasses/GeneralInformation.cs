/* --------------------------
 *
 * GeneralInformation.cs
 *
 * Description: General information that is needed for flow management.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 11/26/2015 - Starvoxel
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

 namespace Starvoxel.FlowManagement
{
     public struct GeneralInformation
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected

        //private
        public string StartingView;

        //properties
        /// <summary>
        /// Returns true if this instance is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return !string.IsNullOrEmpty(StartingView); }
        }
		#endregion
	
		#region Constructor Methods
        public GeneralInformation(string startingView)
        {
            StartingView = startingView;
        }
		#endregion
	
		#region Public Methods
        public override string ToString()
        {
            return string.Format("Init: {0} | SV: {1}", IsInitialized, StartingView);
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}