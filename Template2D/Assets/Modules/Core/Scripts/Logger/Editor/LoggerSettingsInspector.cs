/* --------------------------
 *
 * LoggerSettingsInspector.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 8/28/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
using UnityEditor;
#endregion

#region System Includes
using System.Collections;
using System.Collections.Generic;
using System.IO;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel
{
    [CustomEditor(typeof(LoggerSettings))]
	public class LoggerSettingsInspector : Editor
	{
		#region Fields & Properties
		//const
        public static readonly string INSTANCE_PATH = "Assets/Resources/ScriptableObjects/";
        public static readonly string INSTANCE_NAME = "LoggerSettings";
        public static readonly string INSTANCE_FILENAME = INSTANCE_PATH + INSTANCE_NAME + ".asset";

        public static readonly GUIContent FLAG_HEADER_CONTENT = new GUIContent("Flags");

        public static readonly GUIContent REMOVE_BUTTON_CONTENT = new GUIContent("X", "Clicking this button will remove this flag from the enum.  WARNING: Might take time to re-calculate the enum.");
        public static readonly float REMOVE_BUTTON_SIZE = 35;

        public static readonly GUIContent GENERATE_BUTTON_CONTENT = new GUIContent("Generate", "Force generates the logger flag enum based on the flag info files.  Mostly will just be used for debugging and testing.");
        public static readonly float GENERATE_BUTTON_SIZE = 300;

        public static readonly GUIContent ADD_BUTTON_CONTENT = new GUIContent("Add Flag", "Adds a new flag to the local .lfi file.  If no local .lfi file exists, creates a new one.");
        public static readonly float ADD_BUTTON_SIZE = 200;
	
		//public
	
		//protected
        protected LoggerSettings m_Target;

        protected string[] m_EnumNames;
	
		//private
	
		//properties
		#endregion

        #region Public Methods
        public override void OnInspectorGUI()
        {
            m_Target = (LoggerSettings)target;

            DrawDefaultInspector();

            FlagGUI();
        }

        public LoggerSettingsInspector()
        {
            m_EnumNames = LoggerFlagsGenerator.GetEnumNamesFromGeneratedFile();
        }
		#endregion

        #region Private Methods
        private void FlagGUI()
        {
            GUILayout.Label(FLAG_HEADER_CONTENT, EditorStyles.boldLabel);

            GUILayout.BeginVertical();
            {
                m_EnumNames = LoggerFlagsGenerator.GetEnumNamesFromGeneratedFile();

                for (int i = 0; i < m_EnumNames.Length; ++i)
                {
                    FlagElementGUI(m_EnumNames[i]);
                }

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    //if (GUILayout.Button())
                    {

                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(GENERATE_BUTTON_CONTENT, GUILayout.MaxWidth(GENERATE_BUTTON_SIZE), GUILayout.MinWidth(GENERATE_BUTTON_SIZE)))
                {
                    LoggerFlagsGenerator.GenerateFlagEnum();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void FlagElementGUI(string flagName)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(flagName, EditorStyles.helpBox, GUILayout.ExpandWidth(true));

                bool oldGUIValue = GUI.enabled;

                GUILayout.Button(REMOVE_BUTTON_CONTENT, GUILayout.MaxWidth(REMOVE_BUTTON_SIZE), GUILayout.MinWidth(REMOVE_BUTTON_SIZE));

                GUI.enabled = oldGUIValue;
            }
            GUILayout.EndHorizontal();
        }

        [MenuItem("Assets/Create/ScriptableObjects/LoggerSettings", false, 5000)]
        private static void CreateAsset()
        {
            LoggerSettings asset = ScriptableObject.CreateInstance<LoggerSettings>();

            //string otherPath = ScriptableObjectUtility.ValidObjectPath<LoggerSettings>();

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(INSTANCE_FILENAME);

            if (string.IsNullOrEmpty(assetPathAndName))
            {
                return;
            }

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
		#endregion
	}
}