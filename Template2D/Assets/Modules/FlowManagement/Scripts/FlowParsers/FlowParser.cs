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
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
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
        #region File Version
        public const string FLOW_ELEMENT_KEY = "flow";
        public const string FILE_VERSION_ATTRIBUTE_KEY = "fileVersion";
        #endregion

        #region General Info
        public const string INFO_ELEMENT_KEY = "info";
        public const string STARTING_VIEW_ATTRIBUTE_KEY = "startingView";
        #endregion

        #region Actions
        #endregion

        #region Views
        #endregion

        #region Other
        public static readonly Version INVALID_VERSION = new Version("0.0.0");
        #endregion

        //public
        public static bool IsLogging = false;

        //protected
        protected XDocument m_XML = null;

        protected GeneralInformation m_GeneralInformation;
        protected ActionNode[] m_GeneralActions;
        protected  ViewNode[] m_Views;
	
		//private
	
		//properties
        public GeneralInformation GeneralInformation
        {
            get { return m_GeneralInformation; }
        }
        public ActionNode[] GeneralActions
        {
            get { return m_GeneralActions; }
        }
        public ViewNode[] Views
        {
            get { return m_Views; }
        }
		#endregion
	
		#region Constructor Methods
		private FlowParser(string xmlPath, Version currentVersion)
		{
            TextAsset flowFile = Resources.Load<TextAsset>(xmlPath);

            if (flowFile != null)
            {
                string error = string.Empty;
                XDocument doc = XDocument.Parse(flowFile.text);

                m_XML = doc;
                Version fileVersion = ParseVersion();
                Log("File Version: {0}", fileVersion);

                if (fileVersion == INVALID_VERSION)
                {
                    Debug.LogErrorFormat("FLow XML at path {0}'s version is invalid.  Either you haven't specified it or something is wrong!", xmlPath);
                    return;
                }
                else if (fileVersion < currentVersion)
                {
                    Debug.LogWarningFormat("Flow XML at path {0}'s version is smaller then the currently supported version.  Right now there's no different parsing for older versions but eventually we'll put backwards compatibility.", xmlPath);
                }
                else if (fileVersion > currentVersion)
                {
                    Debug.LogErrorFormat("FLow XML at path {0}'s version is bigger then the currently supported version.  Either this game is out of date for this file or something is really wrong.", xmlPath);
                    return;
                }

                m_GeneralInformation = ParseInfo();
                Log("General Information: " + m_GeneralInformation);

                m_GeneralActions = ParseGeneralActions();

                m_Views = ParseViews();
            }
            else
            {
                Debug.LogErrorFormat("No XML file found at {0}.", xmlPath);
            }
		}
		#endregion
	
		#region Public Methods
        /// <summary>
        /// Creates a flow parser and parses the file at the provided path.
        /// </summary>
        /// <param name="xmlPath">Path of the file to parse.</param>
        /// <param name="currentVersion">Current file version.</param>
        /// <returns>Flow parser with all needed data.</returns>
        public static FlowParser Parse(string xmlPath, Version currentVersion)
        {
            return new FlowParser(xmlPath, currentVersion);
        }
		#endregion

        #region Protected Methods
        protected Version ParseVersion()
        {
            if (m_XML.Root.HasAttributes && m_XML.Root.Attribute(FILE_VERSION_ATTRIBUTE_KEY) != null)
            {
                return new Version(m_XML.Root.Attribute(FILE_VERSION_ATTRIBUTE_KEY).Value);
            }
            else
            {
                return new Version("0.0.0");
            }
        }

        /// <summary>
        /// Parses the general information out of the XML file.
        /// </summary>
        /// <returns>Parsed information.</returns>
        protected GeneralInformation ParseInfo()
        {
            GeneralInformation info = new GeneralInformation(FlowManager.DEFAULT_STARTING_VIEW);
            XElement infoElement = m_XML.Root.Element(INFO_ELEMENT_KEY);
            if (infoElement.HasAttributes)
            {
                if (infoElement.Attribute(STARTING_VIEW_ATTRIBUTE_KEY) != null)
                {
                    info.StartingView = infoElement.Attribute(STARTING_VIEW_ATTRIBUTE_KEY).Value;
                }
            }
            return info;
        }

        protected ActionNode[] ParseGeneralActions()
        {
            List<ActionNode> generalActions = new List<ActionNode>();

            XElement generalActionElement = m_XML.Root.Element("generalActions");

            Debug.Log("Found general action element: " + (generalActionElement != null));

            if (generalActionElement != null)
            {
                IEnumerable<XElement> actionElements = generalActionElement.Elements("action");

                if (actionElements != null)
                {
                    foreach (XElement actionElement in actionElements)
                    {
                        Debug.Log(actionElement.Attribute("id"));
                        ActionNode action = ParseAction(actionElement);
                    }
                }
            }

            return generalActions.ToArray();
        }

        protected ViewNode[] ParseViews()
        {
            throw new NotImplementedException();
        }

        protected ActionNode ParseAction(XElement xmlElement)
        {
            throw new NotImplementedException();
        }

        protected ViewNode ParseView(XElement xmlElement)
        {
            throw new NotImplementedException();
        }
		#endregion

        #region Private Methods
        private void Log(string msg, params object[] args)
        {
            if (IsLogging)
            {
                Debug.LogFormat(msg, args);
            }
        }
		#endregion
	}
}