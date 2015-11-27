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
using System.Xml.Linq;
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
        #region Fields & Properties
        //const
        public const string DEFAULT_STARTING_VIEW = "SPLASH";
        public static readonly Version CURRENT_VERSION = new Version("1.0.0");

		//public
	
		//protected
        [SerializeField] protected string m_TestXMLPath;
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
        private void Start()
        {
            LaunchWithFile(m_TestXMLPath);
        }
		#endregion
	
		#region Public Methods
        /// <summary>
        /// Launch the flow of the game.  It will load and parse the XML at the provided path.
        /// </summary>
        /// <param name="filePath">Path of the flow XML.</param>
        public void LaunchWithFile(string filePath)
        {
            FlowParser parser = FlowParser.Parse(m_TestXMLPath, CURRENT_VERSION);

            int bp = 0;
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}