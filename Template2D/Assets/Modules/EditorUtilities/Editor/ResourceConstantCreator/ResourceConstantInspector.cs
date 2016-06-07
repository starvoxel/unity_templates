/* --------------------------
 *
 * ResourceConstantInspector.cs
 *
 * Description: Object inspector for creating resource constants
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 6/6/2016 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEditor;
using UnityEngine;
#endregion

#region System Includes
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel.EditorUtilities
{
	public class ResourceConstantInspector : ObjectInspector
	{
		#region Fields & Properties
		//const
        private const string RESOURCE_FOLDER_NAME = "/Resources/";
	
		//public
	
		//protected
	
		//private
        private string m_FullPath;
        private string m_PathStub = null;

        //TODO jsmellie: Put all this info into something persistant (Editor Prefs are too easy to clear...  maybe a hidden scriptable object of some sort?)
        // -----------------------------
        private bool m_IsRecursive = false; // If we should go recursively over all the sub-folders and include everything in those in the file we are about to create
        private string[] m_Exclusions = null;  // List off files that won't be included in the constants
        // -----------------------------

        private List<string> m_LocalFiles;
	
		//properties
		#endregion
	
		#region Constructor Methods
		#endregion

        #region Public Methods
        public override bool IsValid(string path)
        {
            if (path.Contains(RESOURCE_FOLDER_NAME))
            {
                m_FullPath = path;
                m_PathStub = path.Substring(path.LastIndexOf(RESOURCE_FOLDER_NAME) + RESOURCE_FOLDER_NAME.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void OnInspector(ObjectEditor editor)
        {
            if (m_PathStub != null)
            {
                bool oldGUIEnabled = GUI.enabled;
                GUI.enabled = true;

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Stub path: ", EditorStyles.boldLabel);
                        GUILayout.Label(m_PathStub);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    m_IsRecursive = GUILayout.Toggle(m_IsRecursive, "Is Recursive");

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Create Constants"))
                        {
                            if (m_LocalFiles == null)
                            {
                                m_LocalFiles = new List<string>();
                            }
                            else
                            {
                                m_LocalFiles.Clear();
                            }

                            // Grab all the files
                            string[] allFiles = System.IO.Directory.GetFiles(m_FullPath);

                            if (allFiles.Length > 0)
                            {
                                for(int i = 0; i < allFiles.Length; ++i)
                                {
                                    if (!System.IO.Path.GetExtension(allFiles[i]).Equals(".meta"))
                                    {
                                        m_LocalFiles.Add(allFiles[i]);
                                    }
                                }
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Local Files:");
                    EditorGUI.indentLevel += 1;
                    if (m_LocalFiles != null)
                    {
                        for(int fileIndex = 0; fileIndex < m_LocalFiles.Count; ++fileIndex)
                        {
                            EditorGUILayout.LabelField(m_LocalFiles[fileIndex]);
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("NULL", EditorStyles.boldLabel);
                    }
                }
                EditorGUILayout.EndVertical();
                GUI.enabled = oldGUIEnabled;
            }
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
	
}