/* --------------------------
 *
 * SceneAutoLoader.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 5/14/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
#endregion

#region System Includes
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes
using Starvoxel.Core;
#endregion
#endregion

namespace Starvoxel.EditorUtilities
{
    [InitializeOnLoad]
    static class SceneAutoLoader
    {
        #region Fields & Properties
        //const
        private const string EDITORPREFS_LOAD_MASTER_ON_PLAY_KEY = "SceneAutoLoader_LoadMasterOnPlay";
        private const string EDITORPREFS_MASTER_PATH_KEY = "SceneAutoLoader_MasterScene";
        private const string EDITORPREFS_MASTER_GUID_KEY = "SceneAutoLoader_MasterSceneGUID";
        private const string EDITORPREFS_PREV_PATH_KEY = "SceneAutoLoader_PreviousScene";
        private const string EDITORPREFS_PREV_SELECTION_KEY = "SceneAutoLoader_PreviousSelection";

        private const char HIERARCHY_SEPERATOR_CHAR = '|';
        private const char OBJECT_SEPERATOR_CHAR = '\t';

        //public

        //protected

        //private

        //properties
        private static bool LoadMasterOnPlay
        {
            get { return EditorPrefs.GetBool(EDITORPREFS_LOAD_MASTER_ON_PLAY_KEY, false); }
            set { EditorPrefs.SetBool(EDITORPREFS_LOAD_MASTER_ON_PLAY_KEY, value); }
        }

        private static string MasterScene
        {
            get { return EditorPrefs.GetString(EDITORPREFS_MASTER_PATH_KEY, "Assets/Scenes/AppLauncher.unity"); }
            set { EditorPrefs.SetString(EDITORPREFS_MASTER_PATH_KEY, value); }
        }

        private static string MasterSceneGUID
        {
            get { return EditorPrefs.GetString(EDITORPREFS_MASTER_GUID_KEY, null); }
            set { EditorPrefs.SetString(EDITORPREFS_MASTER_GUID_KEY, value); }
        }

        private static string PreviousScene
        {
            get { return EditorPrefs.GetString(EDITORPREFS_PREV_PATH_KEY, EditorSceneManager.GetActiveScene().path /*EditorApplication.currentScene*/); }
            set { EditorPrefs.SetString(EDITORPREFS_PREV_PATH_KEY, value); }
        }

        private static string[] PreviousSelectionHierarchy
        {
            get
            {
                string[] retVal = null;
                string savedValue = EditorPrefs.GetString(EDITORPREFS_PREV_SELECTION_KEY, null);

                if (!string.IsNullOrEmpty(savedValue))
                {
                    retVal = savedValue.Split(OBJECT_SEPERATOR_CHAR);
                }

                return retVal;
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    string combined = string.Empty;

                    for (int i = 0; i < value.Length; ++i)
                    {
                        if (i != 0)
                        {
                            combined += OBJECT_SEPERATOR_CHAR;
                        }

                        if (string.IsNullOrEmpty(value[i]))
                        {
                            EditorPrefs.SetString(EDITORPREFS_PREV_SELECTION_KEY, null);
                            return;
                        }

                        combined += value[i];
                    }

                    Services.Logger.LogWithCategory(LoggerConstants.EDITOR_UTILITY_CATEGORY, LogType.Log, "Previous Selection: " + combined);

                    EditorPrefs.SetString(EDITORPREFS_PREV_SELECTION_KEY, combined);
                }
                else
                {
                    EditorPrefs.SetString(EDITORPREFS_PREV_SELECTION_KEY, null);
                }
            }
        }
        #endregion

        #region Private Methods

        #region Constructors
        static SceneAutoLoader()
        {
            EditorApplication.playmodeStateChanged += OnPlayModeChanged;
        }
        #endregion

        private static void OnPlayModeChanged()
        {
            // Load master scene not set, exit
            if (!LoadMasterOnPlay)
            {
                return;
            }

            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // Play pressed, attempt to load master scene
                PreviousScene = EditorSceneManager.GetActiveScene().path;

                SaveSelection();

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    int index = Application.dataPath.LastIndexOf("Assets");

                    string fullMasterPath = Application.dataPath.Substring(0, index) + MasterScene;

                    if (!EditorSceneManager.OpenScene(fullMasterPath).IsValid()/*EditorApplication.OpenScene(fullMasterPath)*/)
                    {
                        Services.Logger.LogWithCategory(LoggerConstants.EDITOR_UTILITY_CATEGORY, LogType.Error, string.Format("error: scene not found: {0}", MasterScene));
                        EditorApplication.isPlaying = false;
                    }
                }
                else
                {
                    // Saving process cancelled, stop playing
                    EditorApplication.isPlaying = false;
                }
            }
            if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // Try to return to the old scene
                if (!EditorSceneManager.OpenScene(PreviousScene).IsValid()/*EditorApplication.OpenScene(PreviousScene)*/)
                {
                    Services.Logger.LogWithCategory(LoggerConstants.EDITOR_UTILITY_CATEGORY, LogType.Error, "error: scene not found: {0}", PreviousScene);
                }
                else
                {
                    // Attmept to return the selection
                    LoadSelection();
                }
            }
        }

        #region Context Menu Methods
        //Basic function that opens a file panel to allow the user to pick a master scene
        [MenuItem("Tools/Scene Autoload/Select Master Scene...", false, 50)]
        private static void SelectMasterScene()
        {
            string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");

            if (masterScene.StartsWith(Application.dataPath))
            {
                int index = Application.dataPath.LastIndexOf("Assets");
                masterScene = masterScene.Substring(index);
            }

            if (!string.IsNullOrEmpty(masterScene))
            {
                MasterScene = masterScene;
                LoadMasterOnPlay = true;
            }
        }

        [MenuItem("Tools/Scene Autoload/Load Master On Play", true, 100)]
        private static bool ShowLoadMasterOnPlay()
        {
            return !LoadMasterOnPlay;
        }
        [MenuItem("Tools/Scene Autoload/Load Master On Play", false, 100)]
        private static void EnableLoadMasterOnPlay()
        {
            LoadMasterOnPlay = true;
        }

        [MenuItem("Tools/Scene Autoload/Don't Load Master On Play", true, 101)]
        private static bool ShowDontLoadMasterOnPlay()
        {
            return LoadMasterOnPlay;
        }
        [MenuItem("Tools/Scene Autoload/Don't Load Master On Play", false, 101)]
        private static void DisableLoadMasterOnPlay()
        {
            LoadMasterOnPlay = false;
        }
        #endregion

        #region Selection IO
        private static void SaveSelection()
        {
            if (Selection.transforms != null && Selection.transforms.Length > 0)
            {
                string[] selections = new string[Selection.transforms.Length];

                for (int i = 0; i < selections.Length; ++i)
                {
                    string selection = "";

                    Transform curObject = Selection.transforms[i];

                    while (curObject != null)
                    {
                        string objectName = curObject.gameObject.name;

                        if (curObject != Selection.transforms[i])
                        {
                            objectName += HIERARCHY_SEPERATOR_CHAR;
                        }

                        selection = selection.Insert(0, objectName);

                        curObject = curObject.parent;
                    }

                    selections[i] = selection;
                }

                PreviousSelectionHierarchy = selections;
            }
        }

        private static void LoadSelection()
        {
            if (PreviousSelectionHierarchy != null)
            {
                string[] objects = PreviousSelectionHierarchy;

                List<GameObject> selection = new List<GameObject>();

                for (int oIndex = 0; oIndex < objects.Length; ++oIndex)
                {
                    if (!string.IsNullOrEmpty(objects[oIndex]))
                    {
                        string[] hierachy = objects[oIndex].Split(HIERARCHY_SEPERATOR_CHAR);

                        Transform curXform = null;

                        for (int hIndex = 0; hIndex < hierachy.Length; ++hIndex)
                        {
                            if (hierachy[hIndex] != null)
                            {
                                if (hIndex == 0)
                                {
                                    GameObject rootObject = GetRootObject(hierachy[hIndex]);
                                    if (rootObject != null)
                                    {
                                        curXform = rootObject.transform;
                                    }
                                }
                                else
                                {
                                    curXform = GetChild(hierachy[hIndex], curXform);
                                }

                                if (curXform == null)
                                {
                                    break;
                                }
                            }
                        }


                        if (curXform == null)
                        {
                            Services.Logger.LogWithCategory(LoggerConstants.EDITOR_UTILITY_CATEGORY, LogType.Error, "Unable to return old selection for: " + objects[oIndex]);
                            continue;
                        }
                        else
                        {
                            selection.Add(curXform.gameObject);
                        }
                    }
                }

                if (selection.Count > 0)
                {
                    Selection.objects = selection.ToArray();
                }

                PreviousSelectionHierarchy = null;
            }
        }
        #endregion

        #region Selection Loading Helpers
        private static GameObject GetRootObject(string name)
        {
            HierarchyProperty prop = new HierarchyProperty(HierarchyType.GameObjects);
            int[] expanded = new int[0];
            while (prop.Next(expanded))
            {
                GameObject rootObject = prop.pptrValue as GameObject;

                if (rootObject != null && rootObject.name == name)
                {
                    return rootObject;
                }
            }

            return null;
        }

        private static Transform GetChild(string name, Transform parent)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.name == name)
                {
                    return child;
                }
            }

            return null;
        }
        #endregion

        #endregion
    }
}
