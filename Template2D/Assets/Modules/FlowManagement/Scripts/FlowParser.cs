/* --------------------------
 *
 * FlowParser.cs
 *
 * Description: Basic parser for the flow manager
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 6/9/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Inludes
using UnityEngine;
#endregion

#region System Includes
using System;
using System.Collections;
using System.Xml;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.FlowManagement
{
	public class FlowParser
	{
		#region Fields & Properties
        //const
	
		//public

        //protected
        protected Version m_FileVersion;
	
		//private
	
		//properties
		#endregion
	
		#region Constructor Methods
		public FlowParser(XmlDocument doc)
		{

		}
		#endregion
	
		#region Public Methods
        public void ParseGeneralInfo()
        {
            if (attributes["version"] != null)
            {
                m_FileVersion = new Version(attributes["version"].Value);
            }
        }
		#endregion

        #region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}