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
using System.IO;
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

	public partial class FlowManager : MonoBehaviour
    {
        #region Internal Classes
        public struct ActionNode
        {
            string m_ID;
            string m_View;
            Hashtable m_Parameters;

            public ActionNode(string id, string view)
            {
                m_ID = id;
                m_View = view;
                m_Parameters = new Hashtable();
            }

            public ActionNode(string id, string view, Hashtable parameters)
                : this(id, view)
            {
                m_Parameters = parameters;
            }

        }

        public struct ViewNode
        {
            public string m_ID;
            public string m_SceneName;

            public ActionNode[] m_Actions;
        }

        public class ViewGroup
        {
            private string m_Name;
            private string m_Note;
            private List<int> m_ViewIndices = new List<int>();

            public string Name
            {
                get { return m_Name;  }
            }

            public string Note
            {
                get { return m_Note; }
            }

            public int[] ViewIndices
            {
                get { return m_ViewIndices.ToArray(); }
            }

            public ViewGroup(string name, string note)
            {
                Initialize(name, note);
            }

            public void Initialize(string name, string note)
            {
                m_Name = name;
                m_Note = note;
                m_ViewIndices.Clear();
            }

            public void AddIndex(int index)
            {
                if (index >= 0 && !m_ViewIndices.Contains(index))
                {
                    m_ViewIndices.Add(index);
                }
            }

            public bool DoesContainIndex(int index)
            {
                return m_ViewIndices.Contains(index);
            }
        }

        public struct GeneralInfo
        {
            private string m_StartingView;

            public GeneralInfo(string startingView)
            {
                m_StartingView = startingView;
            }
        }

        public class FlowData
        {
            public ActionNode[] GlobalActions;
            public ViewNode[] Views;
            public ViewGroup[] ViewGroups;
            public GeneralInfo Information;
        }
        #endregion

        #region Fields & Properties
        //const
        public static readonly Version CURRENT_VERSION = new Version("1.0.0");

		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
		#endregion
	
		#region Public Methods
        public void LaunchWithFile(string filePath)
        {
            TextAsset flowFile = Resources.Load<TextAsset>(filePath);

            if (flowFile != null)
            {
                string error = string.Empty;
                XmlReader reader = XmlReader.Create(new StringReader(flowFile.text));
                FlowParser parser = new FlowParser(reader, CURRENT_VERSION);
            }
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}