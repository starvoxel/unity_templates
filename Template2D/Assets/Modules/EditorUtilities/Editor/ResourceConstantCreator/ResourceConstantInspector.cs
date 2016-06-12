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
using UnityEditorInternal;
#endregion

#region System Includes
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        private GUIContent TAB_GUI_CONTENT = new GUIContent("Resource Contants");
        private string FILE_EXTENSION = ".cs";
        private string ROOT_PATH = "/Scripts/ResourceConstants/";

        private const string RESOURCE_FOLDER_NAME = "/Resources/";
        private static readonly List<string> INVALID_FILE_EXTENSIONS = new List<string>() { ".meta" };
	
        // structs
        public struct sResourceConstantData
        {
            public bool IsRecursive; // If we should go recursively over all the sub-folders and include everything in those in the file we are about to create
            public List<string> Exclusions;  // List off files that won't be included in the constants

            public void Initialize()
            {
                IsRecursive = false;
                Exclusions = new List<string>();
            }
        }

		//public
	
		//protected
	
		//private

        private string m_FullPath;
        private string m_PathStub = null;
        private sResourceConstantData m_Data;

        private ReorderableList m_ExclusionReorderList;

        private List<string> m_LocalFiles;
	
		//properties
        public override GUIContent TabContent
        {
            get { return TAB_GUI_CONTENT; }
        }
		#endregion
	
		#region Constructor Methods
		#endregion

        #region Public Methods
        /// <summary>
        /// Validation function for any kind of default asset
        /// </summary>
        /// <param name="path">Path of the object that we are drawing the editor for.</param>
        /// <returns>True if the object is valid for this editor.</returns>
        public override bool IsValid(string path)
        {
            if (Directory.Exists(path) && path.Contains(RESOURCE_FOLDER_NAME))
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

        /// <summary>
        /// Called at the start of the editor to initialize all needed elements
        /// </summary>
        /// <param name="editor"></param>
        public override void Initialize(ObjectEditor editor)
        {
            base.Initialize(editor);
            LoadData();
            m_ExclusionReorderList = new ReorderableList(m_Data.Exclusions, typeof(string), true, true, true, true);
            m_ExclusionReorderList.drawElementCallback += OnDrawExclusionElement;
            m_ExclusionReorderList.onAddCallback += OnAddExclusionElement;
        }

        /// <summary>
        /// Called when it's time to render the inspector
        /// </summary>
        public override void OnInspector()
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

                    m_Data.IsRecursive = GUILayout.Toggle(m_Data.IsRecursive, "Is Recursive");

                    m_ExclusionReorderList.DoLayoutList();

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Create Constants"))
                        {
                            CreateConstants();
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

                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.EndVertical();
                GUI.enabled = oldGUIEnabled;

                SaveData();
            }
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
        /// <summary>
        /// Callback for drawing a element in the exclusion element
        /// </summary>
        /// <param name="rect">Rect size of the element</param>
        /// <param name="index">Index in the array</param>
        /// <param name="isActive">Is active element</param>
        /// <param name="isFocused">Is in focus</param>
        private void OnDrawExclusionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            string element = m_Data.Exclusions[index];
            Object orgElementObj = null;
            if (!string.IsNullOrEmpty(element))
            {
                orgElementObj = AssetDatabase.LoadAssetAtPath(element, typeof(Object));
            }
            Object newElementObj = EditorGUI.ObjectField(rect, orgElementObj, typeof(Object), false);

            if (newElementObj != orgElementObj && newElementObj != null)
            {
                string newElementPath = AssetDatabase.GetAssetPath(newElementObj);

                if (!newElementPath.StartsWith(m_FullPath))
                {
                    newElementObj = null;
                }
            }

            m_Data.Exclusions[index] = AssetDatabase.GetAssetPath(newElementObj);
        }

        /// <summary>
        /// Callback for when the plus on the re-orderable list is clicked
        /// </summary>
        /// <param name="reorderableList"></param>
        protected void OnAddExclusionElement(ReorderableList reorderableList)
        {
            reorderableList.list.Add(string.Empty);
        }

        /// <summary>
        /// Fetches all files inside a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string[] GetAllFiles(string path)
        {
            string[] allFiles = Directory.GetFiles(path);

            // If we are going recursive, call this function again for all directories
            if (m_Data.IsRecursive)
            {
                string[] directories = Directory.GetDirectories(path);

                if (directories != null && directories.Length > 0)
                {
                    //Easiest way to add multiple lists of strings together
                    List<string> allFilesList = new List<string>(allFiles);

                    for(int directoryIndex = 0; directoryIndex < directories.Length; ++directoryIndex)
                    {
                        allFilesList.AddRange(GetAllFiles(directories[directoryIndex]));
                    }

                    allFiles = allFilesList.ToArray();
                }
            }
            return allFiles;
        }

        #region Save/Load
        /// <summary>
        /// Loads data from the .meta file.  If no data is found, creates new data.
        /// </summary>
        private void LoadData()
        {
            AssetImporter folderImporter = AssetImporter.GetAtPath(m_FullPath);

            if (folderImporter != null && !string.IsNullOrEmpty(folderImporter.userData))
            {
                m_Data = JsonUtility.FromJson<sResourceConstantData>(folderImporter.userData);
            }
            else
            {
                m_Data.Initialize();
                SaveData(true);
            }
        }

        /// <summary>
        /// Save data to the .meta file if anything has changed
        /// </summary>
        /// <param name="force">If true, forces the save even no GUI has changed. </param>
        private void SaveData(bool force = false)
        {
            if (GUI.changed || force)
            {
                AssetImporter folderImporter = AssetImporter.GetAtPath(m_FullPath);

                if (folderImporter != null)
                {
                    folderImporter.userData = JsonUtility.ToJson(m_Data);
                    AssetDatabase.WriteImportSettingsIfDirty(m_FullPath);
                }
            }
        }
        #endregion

        #region File Creation
        private void CreateConstants()
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
            string[] allFiles = GetAllFiles(m_FullPath);

            if (allFiles.Length > 0)
            {
                for (int i = 0; i < allFiles.Length; ++i)
                {
                    allFiles[i] = allFiles[i].Replace('\\', '/');
                    if (IsValidPath(allFiles[i]))
                    {
                        m_LocalFiles.Add(allFiles[i]);
                    }
                }
            }

            string ns = string.Format("{0}.{1}", CodifyString(PlayerSettings.companyName), CodifyString(PlayerSettings.productName)); // TODO jsmellie: I should probably put this in something shared...  I now use this in multiple places
            string className = "TestConstants";
            string path = Application.dataPath + ROOT_PATH + m_PathStub + Path.DirectorySeparatorChar;

            ResourceConstantsGenerator generator = new ResourceConstantsGenerator();

            generator.Session = new Dictionary<string, object>();
            generator.Session["m_Namespace"] = ns;
            generator.Session["m_ClassName"] = className;
            generator.Session["m_ConstantsDictionary"] = new Dictionary<string, string[]>() 
            {
                { "Unsorted", m_LocalFiles.ToArray() }
            };

            generator.Initialize();

            string fileText = generator.TransformText();

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            System.IO.File.WriteAllText(path + className + FILE_EXTENSION, fileText);

            AssetDatabase.Refresh();
        }

        private bool IsValidPath(string path)
        {
            bool isValid = true;
            isValid &= !INVALID_FILE_EXTENSIONS.Contains(Path.GetExtension(path));

            for(int i = 0; i < m_Data.Exclusions.Count; ++i)
            {
                isValid &= !path.StartsWith(m_Data.Exclusions[i]);
            }

            return isValid;
        }
        #endregion

        private static string CodifyString(string value)
        {
            while (value.Length > 0 && value[0] >= '0' && value[0] <= '9')
            {
                value = value.Remove(0, 1);
            }

            value = value.Replace(" ", "");

            return value;
        }
        #endregion
    }
	
}