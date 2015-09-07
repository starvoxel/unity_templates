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
        private static GUIStyle m_Style;

        static CustomGUI()
        {
            InitStyle();
        }

        private static readonly Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);

        // GUILayout Style
        public static void Splitter(Color rgb, float thickness = 1)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, m_Style, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = rgb;
                m_Style.Draw(position, false, false, false, false);
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
            Splitter(thickness, m_Style);
        }

        // GUI Style
        public static void Splitter(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                m_Style.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        private static void InitStyle()
        {
            m_Style = new GUIStyle();
            m_Style.normal.background = EditorGUIUtility.whiteTexture;
            m_Style.stretchWidth = true;
            m_Style.margin = new RectOffset(0, 0, EditorConstants.EDGE_PADDING, EditorConstants.EDGE_PADDING);
        }
    }
}