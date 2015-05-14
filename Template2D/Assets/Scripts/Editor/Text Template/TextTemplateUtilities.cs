/* --------------------------
 *
 * ScriptCreationUtilities.cs
 *
 * Description: 
 *
 * Author: 
 *
 * Editors:
 *
 * 5/12/2015 9:10:06 PM - Starvoxel - All rights reserved
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
    public class TextTemplateMacroReplacer : UnityEditor.AssetModificationProcessor
    {
        #region Fields & Properties
		//const
        private const string LICENSE_FILE_KEY = "ScriptKeywordReplacer_LicenseFile";
        private const string DEFAULT_LICENSE = "All rights reserved.";
        
		//properties
        private static string licenseFilePath
        {
            get { return EditorPrefs.GetString(LICENSE_FILE_KEY, null); }
            set { EditorPrefs.SetString(LICENSE_FILE_KEY, value); }
        }
		#endregion

		#region Public Methods
        [MenuItem("Editor/Text Template/Select License File...", false, 50)]
        public static void SetLicenseLocation()
        {
            licenseFilePath = EditorUtility.OpenFilePanel("Select License File", string.IsNullOrEmpty(licenseFilePath) ? Application.dataPath : licenseFilePath, "txt");
        }

        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            int index = path.LastIndexOf(".");
            string file = path.Substring(index);
            if (file != ".cs" && file != ".js" && file != ".boo") return;
            index = Application.dataPath.LastIndexOf("Assets");
            path = Application.dataPath.Substring(0, index) + path;
            file = System.IO.File.ReadAllText(path);

            file = file.Replace("#CREATIONDATE#", System.DateTime.Now.ToShortDateString());
            file = file.Replace("#PROJECTNAME#", PlayerSettings.productName);
            file = file.Replace("#CODE-PROJECTNAME#", TextTemplateMacroReplacer.CodifyString(PlayerSettings.productName));
            file = file.Replace("#COMPANYNAME#", PlayerSettings.companyName);
            file = file.Replace("#CODE-COMPANYNAME#", TextTemplateMacroReplacer.CodifyString(PlayerSettings.companyName));

            string licenseMsg = DEFAULT_LICENSE;
            if (!string.IsNullOrEmpty(licenseFilePath))
            {
                licenseMsg = System.IO.File.ReadAllText(licenseFilePath);
                licenseMsg = licenseMsg.Replace("\n", "\n * ");
            }

            file = file.Replace("#SOURCELICENSE#", licenseMsg);

            /* TODO:
             * - Somehow figure out the author thing like we do at work...  I have no fucking clue how that works
             * */

            System.IO.File.WriteAllText(path, file);
            AssetDatabase.Refresh();
        }
		#endregion

		#region Protected Methods
		#endregion

		#region Private Methods
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

}
