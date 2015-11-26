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
        protected FlowManager.FlowData m_FlowData = new FlowManager.FlowData();
	
		//private
	
		//properties
		#endregion
	
		#region Constructor Methods
		public FlowParser(XmlReader reader, Version currentVersion)
		{

		}
		#endregion
	
		#region Public Methods
		#endregion

        #region Protected Methods
        protected FlowManager.GeneralInfo ParseInfo()
        {
            FlowManager.GeneralInfo info = new FlowManager.GeneralInfo();
            return info;
        }

        protected FlowManager.ActionNode ParseAction()
        {
            FlowManager.ActionNode action = new FlowManager.ActionNode();
            return action;
        }
		#endregion

        #region Private Methods
        protected bool IsInvalidNodeType(XmlReader reader)
        {
            return reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.SignificantWhitespace || reader.NodeType == XmlNodeType.XmlDeclaration;
        }
		#endregion
	}
}