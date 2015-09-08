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

        public static readonly GUIContent GENERATE_BUTTON_CONTENT = new GUIContent("Generate", "Force generates the logger flag enum based on the flag info files.  Mostly will just be used for debugging and testing.");
        public static readonly float GENERATE_BUTTON_SIZE = 300;

        public static readonly GUIContent ADD_BUTTON_CONTENT = new GUIContent("Add Flag", "Selects the local .lfi file.  If none exists, creates one.");
        public static readonly float ADD_BUTTON_SIZE = 200;
	
		//public
	
		//protected
        protected LoggerSettings m_Target;

        protected LoggerIO.sLFIInfo[] m_LFIInfo;
	
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
        }
		#endregion

        #region Private Methods
        private void FlagGUI()
        {
            GUILayout.Label(FLAG_HEADER_CONTENT, CustomGUI.HeaderStyle);

            //TODO jsmellie: Instead of just showing all the flags in a row without saying what file they are in and stuff,
            // we should divide it up into the files they are in and make a button so that when you click on it, it selects
            // that file so you can edit the flags from there.

            GUILayout.BeginVertical();
            {
                m_LFIInfo = LoggerIO.GetLFIInfos();

                for (int i = 0; i < m_LFIInfo.Length; ++i)
                {
                    if (i > 0)
                    {
                        CustomGUI.Splitter();
                    }
                    LFIInfoGUI(m_LFIInfo[i]);
                }

                // Button that will create a .lfi file beside the scriptable object.  If it already exists, select it.
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Test Button"))
                    {
                        LoggerHelper.SelectLocalLFI();
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
                    LoggerIO.GenerateFlagEnum();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        [MenuItem("Assets/Create/ScriptableObjects/LoggerSettings", false, 5000)]
        private static void CreateAsset()
        {
            LoggerSettings asset = ScriptableObject.CreateInstance<LoggerSettings>();

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

        private void LFIInfoGUI(LoggerIO.sLFIInfo info)
        {
            bool isClicked = false;

            isClicked |= GUILayout.Button(info.Name, EditorStyles.boldLabel);

            for(int i = 0; i < info.Flags.Length; ++i)
            {
                isClicked |= FlagElementGUI(info.Flags[i]);
            }

            if(isClicked)
            {
                LoggerHelper.SelectLFI(info.Path);
            }
        }

        private bool FlagElementGUI(string flagName)
        {
            return GUILayout.Button(flagName, EditorStyles.label, GUILayout.ExpandWidth(true));
        }
		#endregion
	}
}