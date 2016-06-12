/* --------------------------
 *
 * ObjectInspector.cs
 *
 * Description: Override for all basic objects
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel.EditorUtilities
{
    [CustomEditor(typeof(DefaultAsset))]
    public class ObjectEditor : Editor
    {
        private List<ObjectInspector> m_ValidInspectors = new List<ObjectInspector>();
        private GUIContent[] m_TabContents = null;
        private int m_SelectedInspector = 0;

        private void OnEnable()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);

            List<Type> inspectorTypes = new List<Type>();

            Assembly curAssembly = Assembly.GetAssembly(typeof(ObjectInspector));

            foreach(Type type in curAssembly.GetTypes())
            {
                if (IsValidType(type))
                {
                    inspectorTypes.Add(type);
                }
            }

            ObjectInspector curInspector;

            for(int i = 0; i <inspectorTypes.Count; ++i)
            {
                curInspector = Activator.CreateInstance(inspectorTypes[i]) as ObjectInspector;

                if (curInspector.IsValid(path))
                {
                    curInspector.Initialize(this);
                    m_ValidInspectors.Add(curInspector);
                }
            }

            if (m_ValidInspectors.Count > 1)
            {
                List<GUIContent> tabContent = new List<GUIContent>();
                for(int i = 0; i < m_ValidInspectors.Count; ++i)
                {
                    tabContent.Add(m_ValidInspectors[i].TabContent);
                }
                m_TabContents = tabContent.ToArray();
            }
        }

        private bool IsValidType(Type type)
        {
            return !type.IsAbstract && type.IsSubclassOf(typeof(ObjectInspector));
        }

        public override void OnInspectorGUI()
        {
            if (m_ValidInspectors != null && m_ValidInspectors.Count > 0)
            {
                if (m_ValidInspectors.Count > 1)
                {
                    m_SelectedInspector = GUILayout.Toolbar(m_SelectedInspector, m_TabContents);

                    if (m_SelectedInspector < 0 || m_SelectedInspector >= m_TabContents.Length)
                    {
                        m_SelectedInspector = 0;
                    }
                }
                else
                {
                    m_SelectedInspector = 0;
                }

                m_ValidInspectors[m_SelectedInspector].OnInspector();
            }
        }
    }
}
