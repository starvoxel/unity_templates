#region Inludes

#region Unity Includes
using UnityEngine;
using UnityEditor;
#endregion

#region System Includes
using System.IO;
using System.Collections;
using System.Collections.Generic;
#endregion
#endregion

namespace Starvoxel.Core
{
    public class ScriptCreationEditor : EditorWindow
    {
        #region Fields & Properties
        //const
        private const string EDITOR_TEMPLATE_TAG = "Editor";
        private const string ASSET_FOLER_STRING = "Assets/";

        private const float BUTTON_WIDTH = 120;
        private const float LABEL_WIDTH = 120;

        //classes
        class Styles
        {
            public GUIContent WarningContent = new GUIContent(string.Empty);
            public GUIStyle PreviewBox = new GUIStyle("OL Box");
            public GUIStyle PreviewTitle = new GUIStyle("OL Title");
            public GUIStyle LoweredBox = new GUIStyle("TextField");
            public GUIStyle HelpBox = new GUIStyle("helpbox");
            public Styles()
            {
                LoweredBox.padding = new RectOffset(1, 1, 1, 1);
            }
        }

        //public

        //protected

        //private
        private ScriptData m_Data = new ScriptData();
        private string m_Path = string.Empty;

        // GUI variables
        private Vector2 m_PreviewScroll = Vector2.zero;
        private Vector2 m_DataScroll = Vector2.zero;

        private Styles m_Styles = null;

        // Default options.  Will be changable in the pref menu
        private string m_DefaultTemplateName = "CustomMono";
        private string m_DefaultHeaderName = "DefaultHeader";

        //properties
        #endregion

        #region Creation Methods
        // Add menu item to the Window menu
        [MenuItem("Window/Script Creation Editor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            EditorWindow.GetWindow<ScriptCreationEditor>(false, "Script Creation Editor");
        }

        public ScriptCreationEditor()
        {
            // Large initial size
            position = new Rect(50, 50, 770, 500);
            // But allow to scale down to smaller size
            minSize = new Vector2(550, 400);
        }

        private void OnEnable()
        {
            UpdateHeaderNamesAndHeader();
            UpdateTemplateNamesAndTemplate();

            if (Selection.activeObject != null)
            {
                if (IsFolder(Selection.activeObject))
                {
                    m_Path = AssetDatabase.GetAssetPath(Selection.activeObject).Substring(ASSET_FOLER_STRING.Length);
                }
                else
                {
                    m_Path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject).Substring(ASSET_FOLER_STRING.Length));
                }
            }
        }

        /*private void OnEnable()
        {
            m_ScriptPrescription.m_Lang = (Language)1;
            UpdateHeaderNamesAndHeader();
            UpdateTemplateNamesAndTemplate();
            OnSelectionChange();
        }*/
        #endregion

        #region Private Methods
        #region Menu Item Methods
        //[MenuItem("Assets/Create/C# Script", false, 75)]
        private static void OpenFromAssetsMenu()
        {
            Init();
        }
        #endregion

        #region Template Methods
        protected virtual void UpdateTemplateNamesAndTemplate()
        {

        }
        #endregion

        #region Header Methods
        protected virtual void UpdateHeaderNamesAndHeader()
        {

        }
        #endregion

        #region GUI Methods
        protected virtual void OnGUI()
        {
            // Initialize the styles if they are null
            if (m_Styles == null)
            {
                m_Styles = new Styles();
            }

            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                {
                    //Put a bit of padding on the left side
                    GUILayout.Space(10);
                    //Render the preview view
                    PreviewGUI();

                    // Some more padding between the preview and the data
                    GUILayout.Space(10);

                    EditorGUILayout.BeginVertical();
                    {
                        CustomizationGUI();
                        GUILayout.FlexibleSpace();
                        CreateAndCancelGUI();
                    }
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(10);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Preview of what the script will look like.  Want to make this as optimized as possible so that it doesn't lag the editor window
        /// </summary>
        private void PreviewGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(Mathf.Max(position.width * 0.3f, position.width - 450f)));
            {
                // Reserve room for preview title
                Rect previewHeaderRect = GUILayoutUtility.GetRect(new GUIContent("Preview"), m_Styles.PreviewTitle);

                // Secret! Toggle curly braces on new line when double clicking the script preview title
                Event evt = Event.current;
                if (evt.type == EventType.MouseDown && evt.clickCount == 2 && previewHeaderRect.Contains(evt.mousePosition))
                {
                    EditorPrefs.SetBool("CurlyBracesOnNewLine", !EditorPrefs.GetBool("CurlyBracesOnNewLine"));
                    Repaint();
                }

                // Preview scroll view
                m_PreviewScroll = EditorGUILayout.BeginScrollView(m_PreviewScroll, m_Styles.PreviewBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        // Tiny space since style has no padding in right side
                        GUILayout.Space(5);

                        // Preview text itself
                        string previewStr = m_Data.ClassName/*new NewScriptGenerator(m_ScriptPrescription).ToString()*/;
                        Rect r = GUILayoutUtility.GetRect(
                            new GUIContent(previewStr),
                            EditorStyles.miniLabel,
                            GUILayout.ExpandWidth(true),
                            GUILayout.ExpandHeight(true));
                        EditorGUI.SelectableLabel(r, previewStr, EditorStyles.miniLabel);

                    } EditorGUILayout.EndHorizontal();
                } EditorGUILayout.EndScrollView();

                // Draw preview title after box itself because otherwise the top row
                // of pixels of the slider will overlap with the title
                GUI.Label(previewHeaderRect, new GUIContent("Preview"), m_Styles.PreviewTitle);

                GUILayout.Space(4);
            } EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// All GUI used for the customization of the new script.
        /// </summary>
        private void CustomizationGUI()
        {
            //We put it as a scroll view incase we end up having a lot of data
            m_DataScroll = EditorGUILayout.BeginScrollView(m_DataScroll);
            {
                EditorGUILayout.BeginVertical();
                {
                    m_Data.ClassName = EditorGUILayout.TextField("Name", m_Data.ClassName);
                    EditorGUILayout.LabelField(m_Path);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }

        protected void CreateAndCancelGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create", GUILayout.Width(BUTTON_WIDTH)))
                {
                    //TODO jsmellie: DO the actual creation stuff here
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(BUTTON_WIDTH)))
                {
                    Close();
                    GUIUtility.ExitGUI();
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region IO Methods
        private bool IsFolder(Object obj)
        {
            return Directory.Exists(AssetDatabase.GetAssetPath(obj));
        }
        #endregion

        #region Helper Methods
        private void SortTemplateNames(ref List<string> templateNames)
        {
            if (templateNames != null && templateNames.Count > 0)
            {
                List<string> originalList = new List<string>(templateNames);
                templateNames.Clear();

                int monoTemplateIndex = 0;

                string curName = null;
                for (int originalIndex = 0; originalIndex < originalList.Count; ++originalIndex)
                {
                    curName = originalList[originalIndex];
                    if (!string.IsNullOrEmpty(curName))
                    {
                        if (curName.Equals(m_DefaultTemplateName))
                        {
                            templateNames.Insert(0, curName);
                            ++monoTemplateIndex;
                        }
                        else if (curName.Contains(EDITOR_TEMPLATE_TAG))
                        {
                            templateNames.Add(curName);
                        }
                        else
                        {
                            templateNames.Insert(monoTemplateIndex, curName);
                            ++monoTemplateIndex;
                        }
                    }
                }
            }
        }

        private void SortHeaderNames(ref List<string> headerNames)
        {
            if (headerNames != null && headerNames.Count > 0)
            {
                List<string> originalList = new List<string>(headerNames);
                headerNames.Clear();


                string curName = null;
                for (int originalIndex = 0; originalIndex < originalList.Count; ++originalIndex)
                {
                    curName = originalList[originalIndex];
                    if (!string.IsNullOrEmpty(curName))
                    {
                        if (curName.Equals(m_DefaultHeaderName))
                        {
                            headerNames.Insert(0, curName);
                        }
                        else
                        {
                            headerNames.Add(curName);
                        }
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}