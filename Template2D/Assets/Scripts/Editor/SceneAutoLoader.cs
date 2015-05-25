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
#endregion

#region System Includes
using System.Collections;
#endregion
#endregion

namespace Template2D
{
    [InitializeOnLoad]
    static class SceneAutoLoader
    {
        #region Fields & Properties
        //const
        private const string EDITORPREFS_LOAD_MASTER_ON_PLAY = "SceneAutoLoader.LoadMasterOnPlay";
        private const string EDITORPREFS_MASTER_PATH = "SceneAutoLoader.MasterScene";
        private const string EDITORPREFS_PREV_PATH = "SceneAutoLoader.PreviousScene";
        private const string EDITORPREFS_PREV_SELECTION = "SceneAutoLoader.PreviousSelection";

        //public

        //protected

        //private

        //properties
        private static bool LoadMasterOnPlay
        {
            get { return EditorPrefs.GetBool(EDITORPREFS_LOAD_MASTER_ON_PLAY, false); }
            set { EditorPrefs.SetBool(EDITORPREFS_LOAD_MASTER_ON_PLAY, value); }
        }

        private static string MasterScene
        {
            get { return EditorPrefs.GetString(EDITORPREFS_MASTER_PATH, "Master.unity"); }
            set { EditorPrefs.SetString(EDITORPREFS_MASTER_PATH, value); }
        }

        private static string PreviousScene
        {
            get { return EditorPrefs.GetString(EDITORPREFS_PREV_PATH, EditorApplication.currentScene); }
            set { EditorPrefs.SetString(EDITORPREFS_PREV_PATH, value); }
        }

        private static string PreviousSelectionHierarchy
        {
            get { return EditorPrefs.GetString(EDITORPREFS_PREV_SELECTION, null); }
            set { EditorPrefs.SetString(EDITORPREFS_PREV_SELECTION, value); }
        }
        #endregion

        #region Unity Methods
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
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
            if (!LoadMasterOnPlay)
            {
                return;
            }

            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // User pressed play -- autoload master scene.
                PreviousScene = EditorApplication.currentScene;

                SaveSelection();

                if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
                {
                    if (!EditorApplication.OpenScene(MasterScene))
                    {
                        Debug.LogError(string.Format("error: scene not found: {0}", MasterScene));
                        EditorApplication.isPlaying = false;
                    }
                }
                else
                {
                    // User cancelled the save operation -- cancel play as well.
                    EditorApplication.isPlaying = false;
                }
            }
            if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // User pressed stop -- reload previous scene.
                if (!EditorApplication.OpenScene(PreviousScene))
                {
                    Debug.LogError(string.Format("error: scene not found: {0}", PreviousScene));
                }
                else
                {
                    LoadSelection();
                }
            }
        }

        #region Context Menu Functions (Hopefully I'll get rid of these soon for a real editor window)
        // Menu items to select the "master" scene and control whether or not to load it.
        [MenuItem("Editor/Scene Autoload/Select Master Scene...", false, 50)]
        private static void SelectMasterScene()
        {
            string masterScene = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
            if (!string.IsNullOrEmpty(masterScene))
            {
                MasterScene = masterScene;
                LoadMasterOnPlay = true;
            }
        }

        [MenuItem("Editor/Scene Autoload/Load Master On Play", true, 100)]
        private static bool ShowLoadMasterOnPlay()
        {
            return !LoadMasterOnPlay;
        }
        [MenuItem("Editor/Scene Autoload/Load Master On Play", false, 100)]
        private static void EnableLoadMasterOnPlay()
        {
            LoadMasterOnPlay = true;
        }

        [MenuItem("Editor/Scene Autoload/Don't Load Master On Play", true, 101)]
        private static bool ShowDontLoadMasterOnPlay()
        {
            return LoadMasterOnPlay;
        }
        [MenuItem("Editor/Scene Autoload/Don't Load Master On Play", false, 101)]
        private static void DisableLoadMasterOnPlay()
        {
            LoadMasterOnPlay = false;
        }
        #endregion

        #region Selection IO
        private static void SaveSelection()
        {
            if (Selection.activeTransform != null)
            {
                string selection = "";

                Transform curObject = Selection.activeTransform;

                while (curObject != null)
                {
                    string objectName = curObject.gameObject.name;

                    if (curObject != Selection.activeTransform)
                    {
                        objectName += "/";
                    }

                    selection = selection.Insert(0, objectName);

                    curObject = curObject.parent;
                }

                Debug.Log(selection);

                PreviousSelectionHierarchy = selection;
            }
        }

        private static void LoadSelection()
        {
            if (!string.IsNullOrEmpty(PreviousSelectionHierarchy))
            {
                string[] hierachy = PreviousSelectionHierarchy.Split('/');

                Transform curObject = null;

                for (int i = 0; i < hierachy.Length; ++i)
                {
                    if (hierachy[i] != null)
                    {
                        if (i == 0)
                        {
                            curObject = GetRootObject(hierachy[i]).transform;
                        }
                        else
                        {
                            curObject = GetChild(hierachy[i], curObject);
                        }

                        if (curObject == null)
                        {
                            Debug.LogError("Unable to return old selection.  Don't know why but I couldn't!");
                        }
                        else
                        {
                            Selection.objects = new Object[] { curObject.gameObject };
                        }
                    }
                }
            }
        }
        #endregion

        #region Selection Loading Helpers
        private static GameObject GetRootObject(string name)
        {
            var prop = new HierarchyProperty(HierarchyType.GameObjects);
            var expanded = new int[0];
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
