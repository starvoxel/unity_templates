/* --------------------------
 *
 * TextTemplateUtilities.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 5/12/2015 - Starvoxel
 * 
 * All rights reserved
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
#endregion

namespace EditorUtilities
{
    public class TextTemplateMacroReplacer : UnityEditor.AssetModificationProcessor
    {
        #region Fields & Properties
        //const
        private const string LICENSE_FILE_KEY = "TextTemplateMacroReplacer_LicenseFile";
        private const string AUTHOR_KEY = "TextTemplateMacroReplacer_Author";
        private const string USE_NAMESPACE_KEY = "TextTemplateMacroReplacer_UseNamespace";

        private const string SCRIPT_TEMPLATE_PATH = "Data/Resources/ScriptTemplates/";
        private const string CSHARP_TEMPLATE_FILENAME = "81-C# Script-NewBehaviourScript.cs.text";

        private const string DEFAULT_LICENSE = "All rights reserved.";

        //properties
        private static string LicenseFilePath
        {
            get { return EditorPrefs.GetString(LICENSE_FILE_KEY, null); }
            set { EditorPrefs.SetString(LICENSE_FILE_KEY, value); }
        }

        private static string Author
        {
            get { return EditorPrefs.GetString(AUTHOR_KEY, null); }
            set { EditorPrefs.SetString(AUTHOR_KEY, value); }
        }

        private static bool UseNamespace
        {
            get { return EditorPrefs.GetBool(USE_NAMESPACE_KEY, false); }
            set { EditorPrefs.GetBool(USE_NAMESPACE_KEY, value); }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        [PreferenceItem("Text Template")]
        private static void OnPreferenceItem()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("License File:", GUILayout.Width(100));

                GUI.enabled = !string.IsNullOrEmpty(LicenseFilePath);
                {
                    string compressedPath = LicenseFilePath.Replace(Application.dataPath, "");

                    GUILayout.Label(compressedPath, GUI.skin.textField, GUILayout.MaxWidth(218));
                }
                GUI.enabled = true;

                if (GUILayout.Button("Set", GUILayout.ExpandWidth(false)))
                {
                    LicenseFilePath = EditorUtility.OpenFilePanel("Select License File", string.IsNullOrEmpty(LicenseFilePath) ? Application.dataPath : LicenseFilePath, "txt");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Author: ", GUILayout.Width(100));
                Author = EditorGUILayout.TextField(Author);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            UseNamespace = EditorGUILayout.Toggle("Use Namespace: ", UseNamespace);

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply default Text Template"))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "WARNING! Applying the default template will remove all custome changes you have made!", "I'm sure", "Cancel"))
                {
                    TextTemplateMacroReplacer.ApplyDefaultTemplate();
                }
            }
        }

        private static void ApplyDefaultTemplate()
        {
            string editorPath = Path.GetDirectoryName(EditorApplication.applicationPath);
            editorPath = Path.Combine(editorPath, SCRIPT_TEMPLATE_PATH);

            editorPath = editorPath.Replace('/', Path.DirectorySeparatorChar);

            editorPath += CSHARP_TEMPLATE_FILENAME;

            File.WriteAllText(editorPath, TextTemplateUtilities.DEFAULT_CSHARP_TEMPLATE);
            Debug.Log(editorPath);
        }

        private static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            int index = path.LastIndexOf(".");

            if (index < 0)
            {
                return;
            }

            string file = path.Substring(index);
            if (file != ".cs" && file != ".js" && file != ".boo") return;
            index = Application.dataPath.LastIndexOf("Assets");
            path = Application.dataPath.Substring(0, index) + path;
            file = System.IO.File.ReadAllText(path);

            file = file.Replace("#CREATIONDATE#", System.DateTime.Now.ToShortDateString());
            file = file.Replace("#PROJECTNAME#", PlayerSettings.productName);

            if (string.IsNullOrEmpty(PlayerSettings.productName))
            {
                PlayerSettings.productName = "DefaultProject";
            }
            file = file.Replace("#CODE-PROJECTNAME#", TextTemplateMacroReplacer.CodifyString(PlayerSettings.productName));
            file = file.Replace("#COMPANYNAME#", PlayerSettings.companyName);
            file = file.Replace("#AUTHOR#", string.IsNullOrEmpty(Author) ? "" : Author);
            file = file.Replace("#CODE-COMPANYNAME#", TextTemplateMacroReplacer.CodifyString(PlayerSettings.companyName));

            string licenseMsg = DEFAULT_LICENSE;
            if (!string.IsNullOrEmpty(LicenseFilePath))
            {
                licenseMsg = System.IO.File.ReadAllText(LicenseFilePath);
                licenseMsg = licenseMsg.Replace("\n", "\n * ");
            }

            file = file.Replace("#SOURCELICENSE#", licenseMsg);

            System.IO.File.WriteAllText(path, file);
            AssetDatabase.Refresh();
        }

        private static string CodifyString(string value)
        {
            while (value.Length > 0 && value[0] >= '0' && value[0] <= '9')
            {
                value = value.Remove(0, 1);
            }

            value = value.Replace(" ", "");
            value = value.Replace(".", "");

            return value;
        }
        #endregion
    }

    public class TextTemplateUtilities
    {
        public const string DEFAULT_CSHARP_TEMPLATE =
@"/* --------------------------
 *
 * #SCRIPTNAME#.cs
 *
 * Description: 
 *
 * Author: #AUTHOR#
 *
 * Editors:
 *
 * #CREATIONDATE# - #COMPANYNAME#
 *
 * #SOURCELICENSE#
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
#endregion

#region System Includes
using System.Collections;
#endregion
#endregion
        
namespace #CODE-PROJECTNAME#
{
	public class #SCRIPTNAME# : MonoBehaviour 
	{
		#region Fields & Properties
		//const

		//public

		//protected

		//private

		//properties
		#endregion

		#region Unity Methods
		#endregion

		#region Public Methods
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
		#endregion
	}
}";
    }
}
