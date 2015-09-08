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
    static class CustomGUI
    {
        private static GUIStyle m_RegularStyle;
        public static GUIStyle RegularStyle
        {
            get { return m_RegularStyle; }
        }

        private static GUIStyle m_HeaderStyle;
        public static GUIStyle HeaderStyle
        {
            get { return m_HeaderStyle; }
        }

        static CustomGUI()
        {
            InitStyles();
        }

        private static readonly Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);

        // GUILayout Style
        public static void Splitter(Color rgb, float thickness = 1)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, m_RegularStyle, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = rgb;
                m_RegularStyle.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Splitter(float thickness, GUIStyle splitterStyle)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                splitterStyle.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Splitter(float thickness = 1)
        {
            Splitter(thickness, m_RegularStyle);
        }

        // GUI Style
        public static void Splitter(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                m_RegularStyle.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        private static void InitStyles()
        {
            m_RegularStyle = BasicStyleInit();
            m_RegularStyle.normal.background = EditorGUIUtility.whiteTexture;

            m_HeaderStyle = BasicStyleInit();
            m_HeaderStyle.fontSize = Mathf.RoundToInt(m_RegularStyle.fontSize * 1.5f);
            m_HeaderStyle.fontStyle = FontStyle.Bold;
        }

        private static GUIStyle BasicStyleInit()
        {
            GUIStyle style = new GUIStyle();
            //style.normal.background = EditorGUIUtility.whiteTexture;
            style.stretchWidth = true;
            style.margin = new RectOffset(0, 0, EditorConstants.EDGE_PADDING, EditorConstants.EDGE_PADDING);

            return style;
        }
    }
}