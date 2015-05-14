/* --------------------------
 *
 * TestingEditor.cs
 *
 * Description: 
 *
 * Author: 
 *
 * Editors:
 *
 * 5/13/2015 - Starvoxel
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

namespace DTemplate
{
    public struct sObjectInfo
    {
        public string m_ParentScene;
        public string m_Name;
        public Object m_Object;
        public int m_ID;
        public string m_GUID;

        public override string ToString()
        {
            return "{ Name: " + m_Name + " | Instance ID: " + m_ID + " | Asset GUID:" + m_GUID + " }";
        }
    }

    [InitializeOnLoad]
	public static class TestingEditor 
	{
		#region Fields & Properties
		//const

		//public

		//protected
        private static string m_CurrentScene;
        private static sObjectInfo m_SelectedObject;

		//private

		//properties
		#endregion

		#region Unity Methods
		#endregion

		#region Public Methods
        static TestingEditor()
        {
            m_CurrentScene = EditorApplication.currentScene;
            EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            if (EditorApplication.isPlaying == false && EditorApplication.currentScene != m_CurrentScene)
            {
                OnSceneChanged();
            }
        }

        private static void OnSceneChanged()
        {
            m_CurrentScene = EditorApplication.currentScene;

            if (!string.IsNullOrEmpty(m_SelectedObject.m_Name) && !string.IsNullOrEmpty(m_SelectedObject.m_ParentScene) && m_CurrentScene == m_SelectedObject.m_ParentScene)
            {
                GameObject newObject = GameObject.Find(m_SelectedObject.m_Name);

                if (newObject != null)
                {
                    Selection.objects = new Object[] { newObject };

                    int bp = 0;
                }
            }
        }

        [MenuItem("Testing/Save Selection...")]
        public static void SaveCurrentSelection()
        {
            if(Selection.objects.Length > 0)
            {
                m_SelectedObject.m_ParentScene = EditorApplication.currentScene;
                m_SelectedObject.m_Name = Selection.gameObjects[0].name;
                m_SelectedObject.m_Object = Selection.objects[0];
                m_SelectedObject.m_ID = Selection.instanceIDs[0];

                Debug.Log("New Selected Object Info: " + m_SelectedObject.ToString());
            }
        }
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		#endregion
	}
}
