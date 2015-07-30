/* --------------------------
 *
 * FlowManager.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 5/30/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
#endregion

#region System Includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
#endregion
#endregion

namespace Starvoxel.FlowManagement
{
    public class FlowVersionComparer : IComparer<Version>
    {
        public int Compare(Version x, Version y)
        {
            if (x == y)
            {
                return 0;
            }

            if(x.Major > y.Major)
            {
                return 1;
            }
            else if (x.Major < y.Major)
            {
                return -1;
            }
            else
            {
                if(x.Minor > y.Minor)
                {
                    return 1;
                }
                else if(x.Minor < y.Minor)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

	public class FlowManager : MonoBehaviour
    {
        #region Internal Classes
        public struct ActionNode
        {
            string m_Name;
            Hashtable m_Parameters;

            public ActionNode(string name)
            {
                m_Name = name;
                m_Parameters = new Hashtable();
            }

            public ActionNode(string name, Hashtable parameters)
                : this(name)
            {
                m_Parameters = parameters;
            }

        }

        public struct ViewNode
        {
            public string m_Name;
            public string m_ViewName;

            public bool m_IsModal;

            public ActionNode[] m_Actions;
        }

        public struct FlowData
        {
            public ActionNode[] m_GlobalActions;
            public View[] m_Views;
        }
        #endregion

        #region Fields & Properties
        //const
        public static readonly Version CURRENT_VERSION = new Version("1.0.0");
        protected const string FILE_PATH = "XML/Flow";

		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
        protected virtual void Awake()
        {
            TextAsset flowFile = Resources.Load<TextAsset>(FILE_PATH);

            if (flowFile != null)
            {
                string error = string.Empty;

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(flowFile.text);

                FlowParser parser = new FlowParser(xmlDoc);
            }
        }
		#endregion
	
		#region Public Methods
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}