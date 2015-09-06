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
        public static readonly GUIContent REMOVE_BUTTON_CONTENT = new GUIContent("X", "Clicking this button will remove this flag from the enum.  WARNING: Might take time to re-calculate the enum.");
        public static readonly float REMOVE_BUTTON_SIZE = 35;

        public static readonly GUIContent ADD_BUTTON_CONTENT = new GUIContent("+", "Clicking this button will add a new flag to this file.");
        public static readonly float ADD_BUTTON_SIZE = 200;
	
		//public
	
		//protected
	
		//private
        string m_Path;
	
		//properties
		#endregion
	
		#region Unity Methods
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

        public override void OnInspector()
        {
            bool oldStatus = GUI.enabled;

            GUI.enabled = true;

            string[] flagNames = LoggerFlagsGenerator.GetFlagNamesFromLFI(m_Path);

            if (flagNames != null && flagNames.Length > 0)
            {
                GUILayout.BeginVertical();
                {
                    for (int flagIndex = 0; flagIndex < flagNames.Length; ++flagIndex)
                    {
                        FlagElementGUI(flagNames[flagIndex]);
                    }

                    GUILayout.Space(15);

                    AddButtonGUI();
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
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(ADD_BUTTON_CONTENT, GUILayout.MaxWidth(ADD_BUTTON_SIZE), GUILayout.MinWidth(ADD_BUTTON_SIZE)))
                {
                    //TODO jsmellie: Start the flag adding process.  That will be fun!
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

                if (GUILayout.Button(REMOVE_BUTTON_CONTENT, GUILayout.MaxWidth(REMOVE_BUTTON_SIZE), GUILayout.MinWidth(REMOVE_BUTTON_SIZE)))
                {
                    //TODO jsmellie: Properly remove the flag from the file...  Not sure how I want to do that yet...
                }
            }
            GUILayout.EndHorizontal();
        }
		#endregion
	}
}