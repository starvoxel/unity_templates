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
using Starvoxel.Core;
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
        public const string MODAL_DEPTH_OFFSET_ATTRIBUTE_KEY = "modalDepthOffset";
        public const string MODAL_CANVAS_OFFSET_ATTRIBUTE_KEY = "modalCanvasOffset";
        public const string OVERLAY_PREFAB_PATH_ATTRIBUTE_KEY = "overlayPath";
        #endregion

        #region Actions
        public const string GENERAL_ACTION_ELEMENT_LEY = "generalActions";
        public const string ACTION_ELEMENT_KEY = "action";
        public const string ACTION_ID_ATTRIBUTE_KEY = "id";
        public const string ACTION_VIEW_ID_ATTRIBUTE_KEY = "viewID";
        public const string ACTION_PARAM_ELEMENT_KEY = "parameter";
        public const string ACTION_PARAM_KEY_ATTRIBUTE_KEY = "key";
        public const string ACTION_PARAM_VALUE_ATTRIBUTE_KEY = "value";
        #endregion

        #region Views
        public const string VIEW_ELEMENT_KEY = "view";
        public const string VIEW_ID_ATTRIBUTE_KEY = "id";
        public const string VIEW_IS_MODAL_ATTRIBUTE_KEY = "isModal";
        public const string VIEW_SHOW_OVERLAY_ATTRIBUTE_KEY = "showOverlay";
        public const string VIEW_SCENE_ATTRIBUTE_KEY = "scene";
        public const string VIEW_PARAM_ELEMENT_KEY = "parameter";
        public const string VIEW_PARAM_KEY_ATTRIBUTE_KEY = "key";
        public const string VIEW_PARAM_VALUE_ATTRIBUTE_KEY = "value";
        #endregion

        #region Other
        public static readonly Version INVALID_VERSION = new Version("0.0.0");

        public const string CLOSE_CURRENT_VIEW = "CLOSE_CURRENT_VIEW";
        #endregion

        //public
        public static bool IsLogging = false;

        //protected
        protected XDocument m_XML = null;

        protected GeneralInformation m_GeneralInformation;
        protected ActionNode[] m_GeneralActions;
        protected ViewNode[] m_Views;
	
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

                if (fileVersion == INVALID_VERSION)
                {
                    Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Error, "FLow XML at path {0}'s version is invalid.  Either you haven't specified it or something is wrong!", xmlPath);
                    return;
                }
                else if (fileVersion < currentVersion)
                {
                    Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Warning, "Flow XML at path {0}'s version is smaller then the currently supported version.  Right now there's no different parsing for older versions but eventually we'll put backwards compatibility.", xmlPath);
                }
                else if (fileVersion > currentVersion)
                {
                    Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Error, "FLow XML at path {0}'s version is bigger then the currently supported version.  Either this game is out of date for this file or something is really wrong.", xmlPath);
                    return;
                }

                m_GeneralInformation = ParseInfo(ref error);
                if (!string.IsNullOrEmpty(error))
                {
                   throw new Exception(string.Format("An error happened while parsing the XML: {0}", error));
                }

                m_GeneralActions = ParseGeneralActions(ref error);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception(string.Format("An error happened while parsing the XML: {0}", error));
                }

                m_Views = ParseViews(ref error);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception(string.Format("An error happened while parsing the XML: {0}", error));
                }
            }
            else
            {
                Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Error, "No XML file found at {0}.", xmlPath);
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
        /// <param name="error">String that is set if an error happens.</param>
        /// <returns>Parsed information.</returns>
        protected GeneralInformation ParseInfo(ref string error)
        {
            GeneralInformation info = new GeneralInformation(FlowManager.DEFAULT_STARTING_VIEW, FlowManager.DEFAULT_MODAL_DEPTH_OFFSET, FlowManager.DEFAULT_MODAL_CANVAS_OFFSET);
            XElement infoElement = m_XML.Root.Element(INFO_ELEMENT_KEY);
            if (infoElement != null && infoElement.HasAttributes)
            {
                XAttribute curAttribute = infoElement.Attribute(STARTING_VIEW_ATTRIBUTE_KEY);
                if (curAttribute != null)
                {
                    info.StartingView = curAttribute.Value;
                }
                else
                {
                    error = string.Format("{0} does not contain the attribute {1}.  Invalid XML element.", INFO_ELEMENT_KEY, STARTING_VIEW_ATTRIBUTE_KEY);
                    return info;
                }

                curAttribute = infoElement.Attribute(MODAL_DEPTH_OFFSET_ATTRIBUTE_KEY);
                if (curAttribute != null)
                {
                    info.ModalDepthOffset = System.Convert.ToInt32(curAttribute.Value);
                }

                curAttribute = infoElement.Attribute(OVERLAY_PREFAB_PATH_ATTRIBUTE_KEY);
                if (curAttribute != null)
                {
                    info.OverlayPrefabPath = curAttribute.Value;
                }

                curAttribute = infoElement.Attribute(MODAL_CANVAS_OFFSET_ATTRIBUTE_KEY);
                if (curAttribute != null)
                {
                    info.ModalCanvasOffset = System.Convert.ToInt32(curAttribute.Value);
                }
            }
            else
            {
                error = INFO_ELEMENT_KEY + " element in the XML is invalid.";
                return info;
            }
            return info;
        }

        /// <summary>
        /// Parse all of the actions under the general action element from m_XML
        /// </summary>
        /// <param name="error">String that is set if an error happens.</param>
        /// <returns>Array of all the actions nodes found.</returns>
        protected ActionNode[] ParseGeneralActions(ref string error)
        {
            List<ActionNode> generalActions = new List<ActionNode>();

            XElement generalActionElement = m_XML.Root.Element(GENERAL_ACTION_ELEMENT_LEY);

            if (generalActionElement != null)
            {
                IEnumerable<XElement> actionElements = generalActionElement.Elements(ACTION_ELEMENT_KEY);

                if (actionElements != null)
                {
                    foreach (XElement actionElement in actionElements)
                    {
                        ActionNode action = ParseAction(actionElement, ref error);

                        if (action.IsInitialized)
                        {
                            generalActions.Add(action);
                        }
                    }
                }
            }

            return generalActions.ToArray();
        }

        protected ViewNode[] ParseViews(ref string error)
        {
            IEnumerable<XElement> viewElements = m_XML.Root.Elements(VIEW_ELEMENT_KEY);

            List<ViewNode> viewNodes = new List<ViewNode>();

            if (viewElements != null)
            {
                foreach(XElement viewElement in viewElements)
                {
                    ViewNode viewNode = ParseView(viewElement, ref error);

                    if (!string.IsNullOrEmpty(error))
                    {
                        return null;
                    }

                    if (viewNode.IsInitialzed)
                    {
                        viewNodes.Add(viewNode);
                    }
                }
            }

            return viewNodes.ToArray();
        }

        /// <summary>
        /// Parse the action data out of the specified XML element
        /// </summary>
        /// <param name="actionElement">XML element that wll be parsed</param>
        /// <param name="error">String that is set if an error happens.</param>
        /// <returns></returns>
        protected ActionNode ParseAction(XElement actionElement, ref string error)
        {
            ActionNode action = new ActionNode();

            if (actionElement != null && actionElement.HasAttributes)
            {
                action.ID = actionElement.Attribute(ACTION_ID_ATTRIBUTE_KEY).Value;
                if (actionElement.Attribute(ACTION_VIEW_ID_ATTRIBUTE_KEY) != null)
                {
                    action.ViewID = actionElement.Attribute(ACTION_VIEW_ID_ATTRIBUTE_KEY).Value;
                }
                action.Parameters = ParseParameters(actionElement, ref error);                
            }
            else
            {
                error = actionElement.Name + " is an invalid element.";
            }

            return action;
        }

        /// <summary>
        /// Parses all the contained parameter elements into a dictionary.
        /// </summary>
        /// <param name="parentElement">Parent element that contains all the parameter elements we want to parse.</param>
        /// <param name="error">String that is set if an error happens.</param>
        /// <returns></returns>
        protected Dictionary<string, object> ParseParameters(XElement parentElement, ref string error)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            //Parameters can only be there if the parent element has sub-elements
            if (parentElement != null && parentElement.HasElements)
            {
                IEnumerable<XElement> parameterElements = parentElement.Elements(ACTION_PARAM_ELEMENT_KEY);

                if (parameterElements != null)
                {
                    foreach (XElement parameter in parameterElements)
                    {
                        //Only care about elements with the right name
                        if (parameter.Name == ACTION_PARAM_ELEMENT_KEY)
                        {
                            string key = parameter.Attribute(ACTION_PARAM_KEY_ATTRIBUTE_KEY).Value;
                            string dataType = parameter.Attribute("dataType").Value;
                            string value = parameter.Attribute(ACTION_PARAM_VALUE_ATTRIBUTE_KEY).Value;

                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                            {
                                if (!string.IsNullOrEmpty(dataType))
                                {
                                    //Incase something goes wrong with the parsing, we'll put this in a try catch
                                    try
                                    {
                                        switch (dataType)
                                        {
                                            case "int":
                                                parameters.Add(key, System.Convert.ToInt32(value));
                                                break;
                                            case "float":
                                                parameters.Add(key, System.Convert.ToSingle(value));
                                                break;
                                            case "bool":
                                                parameters.Add(key, System.Convert.ToBoolean(value));
                                                break;
                                            //If we don't know what it is, put it in as a string
                                            default:
                                                parameters.Add(key, value);
                                                break;
                                        }
                                    }
                                    //While converting, something went wrong
                                    catch (Exception err)
                                    {
                                        Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Error, err.GetType().ToString() + " : " + err.Message);
                                        parameters.Add(key, value);
                                    }
                                }
                                else
                                {
                                    //Didn't specify a type so we just put it in as a string
                                    parameters.Add(key, value);
                                }
                            }
                        }
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        /// Parses the views information from the specified element
        /// </summaryprotected
        /// <param name="viewElement"></param>
        /// <returns></returns>
        ViewNode ParseView(XElement viewElement, ref string error)
        {
            ViewNode viewNode = new ViewNode();

            if (viewElement != null && viewElement.HasAttributes)
            {
                // Grab all the attributes we need
                XAttribute idAttribute = viewElement.Attribute(VIEW_ID_ATTRIBUTE_KEY);
                XAttribute sceneNameAttribute = viewElement.Attribute(VIEW_SCENE_ATTRIBUTE_KEY);
                XAttribute isModalAttribute = viewElement.Attribute(VIEW_IS_MODAL_ATTRIBUTE_KEY);
                XAttribute showOverlayAttribute = viewElement.Attribute(VIEW_SHOW_OVERLAY_ATTRIBUTE_KEY);

                List<ActionNode> actions = new List<ActionNode>();

                IEnumerable<XElement> actionElements = viewElement.Elements(ACTION_ELEMENT_KEY);

                // Parse out the actions if there are any
                if (actionElements != null)
                {
                    foreach(XElement actionElement in actionElements)
                    {
                        ActionNode actionNode = ParseAction(actionElement, ref error);

                        if (!string.IsNullOrEmpty(error))
                        {
                            return viewNode;
                        }

                        if (actionNode.IsInitialized)
                        {
                            actions.Add(actionNode);
                        }
                    }
                }

                // If we have valid info, populate the view node
                if (idAttribute != null&& !string.IsNullOrEmpty(idAttribute.Value))
                {
                    viewNode.ID = idAttribute.Value;

                    if (sceneNameAttribute != null)
                    {
                        viewNode.SceneName = sceneNameAttribute.Value;
                    }

                    if (isModalAttribute != null)
                    {
                        viewNode.IsModal = System.Convert.ToBoolean(isModalAttribute.Value);
                    }
                    else
                    {
                        viewNode.IsModal = false;
                    }

                    if (showOverlayAttribute != null)
                    {
                        viewNode.ShowOverlay = System.Convert.ToBoolean(showOverlayAttribute.Value);
                    }
                    else
                    {
                        viewNode.ShowOverlay = false;
                    }

                    viewNode.Actions = actions.ToArray();
                }
            }
            else
            {
                if (viewElement == null)
                {
                    error = "Null view element.";
                    return viewNode;
                }
                else
                {
                    error = viewElement.Name + " is an invalid view element.  It does not contain any attributes.";
                }
            }

            return viewNode;
        }
		#endregion

        #region Private Methods
        private void Log(string msg, params object[] args)
        {
            Services.Logger.LogWithCategory(LoggerConstants.FLOW_CATEGORY, LogType.Log, msg, args);
        }
		#endregion
	}
}
