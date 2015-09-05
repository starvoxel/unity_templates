/* --------------------------
 *
 * LoggerFlagsGeneratorHelper.cs
 *
 * Description: The rest of the auto-generated partial class that does the actual file creation and such
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel
{
    public partial class LoggerFlagsGenerator
	{
		#region Fields & Properties
        //const
        public static readonly char ENUM_START_CHAR = '$';
        public static readonly char ENUM_DIVIDER_CHAR = ',';

        public static readonly string FLAG_INFO_FILENAME = "LoggerFlag.lfi";

        public static readonly string FLAG_FILENAME = "LoggerFlags";
        public static readonly string FLAG_FILE_DIRECTORY = Application.dataPath + "/Scripts/Logger";
        public static readonly string FLAG_FILE_PATH = FLAG_FILE_DIRECTORY + "/" + FLAG_FILENAME + ".cs";

        //public
        public int StartingValue = 0;
        public string[] FlagNames = null;
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Unity Methods
		#endregion

        #region Public Methods
        public static string[] GetFlagNamesFromLFIs()
        {
            List<string> enumNames = new List<string>();

            // -- Get all the enum files and their values
            string[] loggerInfoFiles = Directory.GetFiles(Application.dataPath, "*" + FLAG_INFO_FILENAME, SearchOption.AllDirectories);

            for (int fileIndex = 0; fileIndex < loggerInfoFiles.Length; ++fileIndex)
            {
                string file = File.ReadAllText(loggerInfoFiles[fileIndex]);

                //TODO jsmellie: For now we don't give a damn about the title...  I'll put that in later

                int startIndex = file.IndexOf(ENUM_START_CHAR) + 1;

                // -- Not a valid file format...  It doesn't contain the start char
                if (startIndex < 1)
                {
                    continue;
                }

                string rawFlagString = file.Substring(startIndex);

                rawFlagString = rawFlagString.RemoveWhitespace();

                string[] rawFlagNames = rawFlagString.Split(ENUM_DIVIDER_CHAR);

                string curFlag = null;

                for (int rawFlagCounter = 0; rawFlagCounter < rawFlagNames.Length; ++rawFlagCounter)
                {
                    curFlag = FormatFlagName(rawFlagNames[rawFlagCounter]);

                    if (!enumNames.Contains(curFlag))
                    {
                        enumNames.Add(curFlag);
                    }
                }
            }

            return enumNames.ToArray();
        }

        private static string FormatFlagName(string name)
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

        public static string[] GetEnumNamesFromGeneratedFile()
        {
            return eLoggerFlags.GetNames();
        }

        //[UnityEditor.Callbacks.DidReloadScripts]
        public static void GenerateFlagEnum()
        {
            string[] enumNames = GetFlagNamesFromLFIs();

            enumNames = RemoveIncludedFlags(enumNames);

            UpdateFlagFile(enumNames);
        }

        private static string[] RemoveIncludedFlags(string[] enumNames)
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

            return enumNameList.ToArray();
        }

        private static void UpdateFlagFile(string[] enumNames)
        {
            if (enumNames != null && enumNames.Length > 0 && NeedToRegenerateFile(enumNames))
            {
                LoggerFlagsGenerator flagGenerator = new LoggerFlagsGenerator();
                flagGenerator.FlagNames = enumNames;
                flagGenerator.StartingValue = eLoggerFlags.INCLUDED_FLAGS.Length;

                if (!Directory.Exists(FLAG_FILE_DIRECTORY))
                {
                    Directory.CreateDirectory(FLAG_FILE_DIRECTORY);
                }

                File.WriteAllText(FLAG_FILE_PATH, flagGenerator.TransformText());

                AssetDatabase.Refresh();
            }
        }

        private static bool NeedToRegenerateFile(string[] newFlags)
        {
            if (!File.Exists(FLAG_FILE_PATH))
            {
                return true;
            }

            string[] currentFlags = eLoggerFlags.GetCustomNames();

            if (currentFlags == null || currentFlags.Length == 0 || newFlags.Length != currentFlags.Length)
            {
                return true;
            }


            for (int newFlagsIndex = 0; newFlagsIndex < newFlags.Length; ++newFlagsIndex)
            {
                bool isIncluded = false;

                for (int currentFlagsIndex = 0; currentFlagsIndex < currentFlags.Length; ++currentFlagsIndex)
                {
                    if (string.Compare(newFlags[newFlagsIndex], currentFlags[currentFlagsIndex]) == 0)
                    {
                        isIncluded = true;
                        break;
                    }
                }

                if (!isIncluded)
                {
                    return true;
                }
            }

            return false;
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	}
}