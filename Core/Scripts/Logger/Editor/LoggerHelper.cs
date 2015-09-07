/* --------------------------
 *
 * LoggerHelper.cs
 *
 * Description: All the helper stuff needed for the Logger system
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 9/6/2015 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEditor;
using UnityEngine;
#endregion

#region System Includes
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel
{
    public class LoggerHelper
    {
        #region Fields & Properties
        //const

        //public

        //protected

        //private

        //properties
        #endregion

        #region Public Methods
        public static void SelectLocalLFI()
        {
            LoggerIO.CreateLocalLFI();

            DefaultAsset localFile = AssetDatabase.LoadAssetAtPath(LoggerIO.LOCAL_LFI_PROJECT_PATH, typeof(DefaultAsset)) as DefaultAsset;

            Selection.objects = new UnityEngine.Object[] { localFile };
        }

        public static string FormatFlagName(string name)
        {
            if (string.Compare(name.ToUpper(), name) == 0)
            {
                return name;
            }

            string newName = string.Empty;

            for (int i = 0; i < name.Length; ++i)
            {
                char curChar = name[i];

                if (i > 0 && Char.IsUpper(curChar))
                {
                    newName += "_" + curChar;
                }
                else
                {
                    newName += Char.ToUpper(curChar);
                }
            }

            return newName;
        }

        public static string[] RemoveIncludedAndInvalidFlags(string[] enumNames)
        {
            List<string> enumNameList = new List<string>(enumNames);

            string[] includedNames = eLoggerFlags.GetIncludedNames();

            for (int i = 0; i < includedNames.Length; ++i)
            {
                if (enumNameList.Contains(includedNames[i]))
                {
                    enumNameList.Remove(includedNames[i]);
                }
            }

            return RemoveInvalidFlags(enumNameList.ToArray());
        }

        public static string[] RemoveInvalidFlags(string[] enumNames)
        {
            List<string> enumNameList = new List<string>(enumNames);

            for (int i = 0; i < enumNameList.Count; ++i)
            {
                if (string.IsNullOrEmpty(enumNameList[i]))
                {
                    enumNameList.RemoveAt(i);
                    i -= 1;
                }
            }

            return enumNameList.ToArray();
        }
        #endregion

        #region Private Methods
        #endregion
    }
}