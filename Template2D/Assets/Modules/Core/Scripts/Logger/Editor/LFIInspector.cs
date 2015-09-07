/* --------------------------
 *
 * LFIInspector.cs
 *
 * Description: Inspector for all .lfi files
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 9/5/2015 - Starvoxel
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel
{
	public class LFIInspector : ObjectInspector
	{
		#region Fields & Properties
        //const
        private const string NEW_FLAG_CONTROL_NAME = "NewFlagText";
        private const string FINISHED_CONTROL_NAME = "FinishedButton";

        private static readonly GUIContent REMOVE_BUTTON_CONTENT = new GUIContent("X", "Clicking this button will remove this flag from the enum.  WARNING: Might take time to re-calculate the enum.");
        private static readonly float REMOVE_BUTTON_SIZE = 35;

        private static readonly GUIContent ADD_BUTTON_CONTENT = new GUIContent("+", "Clicking this button will start the new flag process.");
        private static readonly float ADD_BUTTON_SIZE = 200;

        private static readonly GUIContent FINISHED_EDITING_BUTTON_CONTENT = new GUIContent("Done", "Clicking this button will add the new flag to this file.");
        private static readonly float FINISHED_EDITING_BUTTON_SIZE = 50;	
        //enums
        private enum eNewFlagState
        {
            NONE,
            ADD_PRESSED,
            FOCUSING_TEXT,
            EDITING,
            SAVING
        }

		//public
	
		//protected
	
		//private
        string m_Path;

        string m_NewFlagName = null;

        eNewFlagState m_NewFlagState = eNewFlagState.NONE;
	
		//properties
		#endregion

        #region Public Methods
        public override bool IsValid(string path)
        {
            if (Path.GetExtension(path) == ".lfi")
            {
                m_Path = path;
                return true;
            }

            return false;
        }

        public override void OnInspector(ObjectEditor editor)
        {
            if (m_NewFlagState == eNewFlagState.FOCUSING_TEXT)
            {
                m_NewFlagState = eNewFlagState.EDITING;
            }
            else if (m_NewFlagState == eNewFlagState.EDITING && GUI.GetNameOfFocusedControl() != NEW_FLAG_CONTROL_NAME)
            {
                if (GUI.GetNameOfFocusedControl() == FINISHED_CONTROL_NAME)
                {
                    m_NewFlagState = eNewFlagState.SAVING;
                }
                else
                {
                    m_NewFlagState = eNewFlagState.NONE;
                    m_NewFlagName = null;
                }
            }

            bool oldStatus = GUI.enabled;

            GUI.enabled = true;

            string[] flagNames = LoggerIO.GetFlagNamesFromLFI(m_Path);

            if (flagNames != null && flagNames.Length > 0)
            {
                GUILayout.BeginVertical();
                {
                    for (int flagIndex = 0; flagIndex < flagNames.Length; ++flagIndex)
                    {
                        FlagElementGUI(flagNames[flagIndex]);
                    }

                    GUILayout.Space(5);

                    if (m_NewFlagName != null)
                    {
                        switch(m_NewFlagState)
                        {
                            case eNewFlagState.ADD_PRESSED:
                            case eNewFlagState.EDITING:
                            case eNewFlagState.FOCUSING_TEXT:
                                NewFlagGUI();

                                if (m_NewFlagState == eNewFlagState.ADD_PRESSED)
                                {
                                    EditorGUI.FocusTextInControl(NEW_FLAG_CONTROL_NAME);
                                    m_NewFlagState = eNewFlagState.FOCUSING_TEXT;
                                }
                                break;
                            case eNewFlagState.SAVING:
                                string formattedName = LoggerHelper.FormatFlagName(m_NewFlagName);

                                List<string> flagNameList = new List<string>(flagNames);

                                if (flagNameList.Contains(formattedName))
                                {
                                    //TODO jsmellie: Show an error...  The name already exists
                                }
                                else
                                {
                                    string error = LoggerIO.AddFlagNameToLFI(m_Path, formattedName);

                                    if(!string.IsNullOrEmpty(error))
                                    {
                                        //TODO jsmellie: Display this error in some way
                                    }

                                    EditorUtility.SetDirty(editor.target);
                                }

                                m_NewFlagName = null;
                                m_NewFlagState = eNewFlagState.NONE;
                                break;
                        }
                    }

                    GUILayout.Space(15);

                    AddButtonGUI();

                    GUILayout.Label("Current State: " + m_NewFlagState.ToString() + " | Focused Control: " + GUI.GetNameOfFocusedControl());
                }
                GUILayout.EndVertical();
            }

            GUI.enabled = oldStatus;
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
        private void AddButtonGUI()
        {
            bool oldEnableState = GUI.enabled;

            if (m_NewFlagName != null)
            {
                GUI.enabled = false;
            }
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(ADD_BUTTON_CONTENT, GUILayout.MaxWidth(ADD_BUTTON_SIZE), GUILayout.MinWidth(ADD_BUTTON_SIZE)))
                {
                    m_NewFlagName = string.Empty;
                    m_NewFlagState = eNewFlagState.ADD_PRESSED;
                    EditorGUI.FocusTextInControl(NEW_FLAG_CONTROL_NAME);
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            GUI.enabled = oldEnableState;
        }

        private void FlagElementGUI(string flagName)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(flagName, EditorStyles.helpBox, GUILayout.ExpandWidth(true));

                if (GUILayout.Button(REMOVE_BUTTON_CONTENT, GUILayout.MaxWidth(REMOVE_BUTTON_SIZE), GUILayout.MinWidth(REMOVE_BUTTON_SIZE)))
                {
                    LoggerIO.RemoveFlagNameFromLFI(m_Path, flagName);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void NewFlagGUI()
        {
            //TODO jsmellie: Should definitely put some checking here.  Make it so that you can't add a flag if it already exists in the file

            GUILayout.BeginHorizontal();
            {
                GUI.SetNextControlName(NEW_FLAG_CONTROL_NAME);
                m_NewFlagName = GUILayout.TextField(m_NewFlagName, GUILayout.ExpandWidth(true));

                GUI.SetNextControlName(FINISHED_CONTROL_NAME);
                if (GUILayout.Button(FINISHED_EDITING_BUTTON_CONTENT, GUILayout.MaxWidth(FINISHED_EDITING_BUTTON_SIZE), GUILayout.MinWidth(FINISHED_EDITING_BUTTON_SIZE)))
                {
                    GUI.FocusControl(FINISHED_CONTROL_NAME);
                }
            }
            GUILayout.EndHorizontal();
        }
		#endregion
	}
}