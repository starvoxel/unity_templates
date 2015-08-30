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
        public static readonly char ENUM_START_CHAR = '$';
        public static readonly char ENUM_DIVIDER_CHAR = ',';

        public static readonly string TEMPLATE_PATH = Application.dataPath + "/Modules/Core/Scripts/Logger/OtherResources/LoggerFlagTemplate.txt";
        public static readonly string TEMPLATE_REPLACE_MACRO = "%FLAGS%";

        public static readonly GUIContent FLAG_HEADER_CONTENT = new GUIContent("Flags");

        public static readonly GUIContent REMOVE_BUTTON_CONTENT = new GUIContent("X", "Clicking this button will remove this flag from the enum.  WARNING: Might take time to re-calculate the enum.");
        public static readonly float REMOVE_BUTTON_SIZE = 35;

        public static readonly GUIContent GENERATE_BUTTON_CONTENT = new GUIContent("Generate", "Force generates the logger flag enum based on the flag info files.  Mostly will just be used for debugging and testing.");
        public static readonly float GENERATE_BUTTON_SIZE = 300;
	
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
            m_EnumNames = GetEnumNamesFromLFIFiles();
        }
		#endregion

        #region Private Methods
        private void FlagGUI()
        {
            GUILayout.Label(FLAG_HEADER_CONTENT, EditorStyles.boldLabel);

            GUILayout.BeginVertical();
            {
                for (int i = 0; i < m_EnumNames.Length; ++i)
                {
                    FlagElementGUI(m_EnumNames[i]);
                }
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(GENERATE_BUTTON_CONTENT, GUILayout.MaxWidth(GENERATE_BUTTON_SIZE), GUILayout.MinWidth(GENERATE_BUTTON_SIZE)))
                {
                    GenerateFlagEnum();
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
                if (flagName == LoggerSettings.DEFAULT_FLAG)
                {
                    GUI.enabled = false;
                }

                GUILayout.Button(REMOVE_BUTTON_CONTENT, GUILayout.MaxWidth(REMOVE_BUTTON_SIZE), GUILayout.MinWidth(REMOVE_BUTTON_SIZE));

                GUI.enabled = oldGUIValue;
            }
            GUILayout.EndHorizontal();
        }

        [MenuItem("Assets/Create/ScriptableObjects/LoggerSettings", false, 5000)]
        private static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<LoggerSettings>();
        }

        private void GenerateFlagEnum()
        {
            string[] enumNames = GetEnumNamesFromLFIFiles();

            UpdateFlagFile(enumNames);
        }

        private string[] GetEnumNamesFromLFIFiles()
        {
            List<string> enumNames = new List<string>();

            enumNames.Add(LoggerSettings.DEFAULT_FLAG);

            // -- Get all the enum files and their values
            string[] loggerInfoFiles = Directory.GetFiles(Application.dataPath, "*" + LoggerSettings.FLAG_INFO_FILENAME, SearchOption.AllDirectories);

            for (int fileIndex = 0; fileIndex < loggerInfoFiles.Length; ++fileIndex)
            {
                string file = File.ReadAllText(loggerInfoFiles[fileIndex]);

                //TODO jsmellie: For now we don't give a damn about the title...  I'll put that in later

                int startIndex = file.IndexOf(ENUM_START_CHAR) + 1;

                // -- Not a valid file format...  It doesn't contain the start char
                if (startIndex < 1)
                {
                    continue;
                }

                string rawEnumString = file.Substring(startIndex);

                rawEnumString = rawEnumString.RemoveWhitespace();

                string[] rawEnumNames = rawEnumString.Split(ENUM_DIVIDER_CHAR);

                for(int rawEnumCounter = 0; rawEnumCounter < rawEnumNames.Length; ++rawEnumCounter)
                {
                    if (!enumNames.Contains(rawEnumNames[rawEnumCounter]))
                    {
                        enumNames.Add(rawEnumNames[rawEnumCounter]);
                    }
                }
            }

            return enumNames.ToArray();
        }

        private string[] GetCurrentEnumNames()
        {
            string[] enums = null;
#if LOGGER_FLAGS
            enums = System.Enum.GetNames(typeof(LoggerSettings.eLoggerFlags));
#else
            GenerateFlagEnum();
            //enums = GetCurrentEnumNames();
#endif
            return enums;
        }

        private void UpdateFlagFile(string[] enumNames)
        {
            if (enumNames != null && enumNames.Length > 0)
            {
                if (File.Exists(TEMPLATE_PATH))
                {
                    string template = File.ReadAllText(TEMPLATE_PATH);

                    if (template.Contains(TEMPLATE_REPLACE_MACRO))
                    {
                        string concatenationEnum = string.Empty;

                        for (int i = 0; i < enumNames.Length; ++i)
                        {
                            if (!string.IsNullOrEmpty(enumNames[i]))
                            {
                                if (i != 0)
                                {
                                    concatenationEnum += "\n";
                                }

                                concatenationEnum += "            " + enumNames[i] + " = 1 << " + i + ",";
                            }
                        }

                        template = template.Replace(TEMPLATE_REPLACE_MACRO, concatenationEnum);

                        if (!Directory.Exists(LoggerSettings.FLAG_FILE_DIRECTORY))
                        {
                            Directory.CreateDirectory(LoggerSettings.FLAG_FILE_DIRECTORY);
                        }

                        File.WriteAllText(LoggerSettings.FLAG_FILE_PATH, template);

                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    Logger.Log("No file exists at template path: " + TEMPLATE_PATH);
                }
            }
        }
		#endregion
	}
}