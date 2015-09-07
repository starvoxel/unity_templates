/* --------------------------
 *
 * LoggerIO.cs
 *
 * Description: Everything that has to do with IO for the Logger system
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
using UnityEngine;
using UnityEditor;
#endregion

#region System Includes
using System.IO;
using System.Collections;
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

 namespace Starvoxel
{
	public class LoggerIO : MonoBehaviour
	{
		#region Fields & Properties
        //const
        public static readonly char FLAG_START_CHAR = '$';
        public static readonly char FLAG_DIVIDER_CHAR = ',';

        public static readonly string FLAG_INFO_FILENAME = "LoggerFlags.lfi";
        private static readonly string LOCAL_LFI_INTERNAL_PATH = "/Other Resources/Logger/Game" + FLAG_INFO_FILENAME;

        public static readonly string FLAG_FILENAME = "LoggerFlags";
        public static readonly string FLAG_FILE_DIRECTORY = Application.dataPath + "/Scripts/Logger";
        public static readonly string FLAG_FILE_PATH = FLAG_FILE_DIRECTORY + "/" + FLAG_FILENAME + ".cs";

        public static readonly string LOCAL_LFI_PROJECT_PATH = "Assets" + LOCAL_LFI_INTERNAL_PATH;
        public static readonly string LOCAL_LFI_FULL_PATH = Application.dataPath + LOCAL_LFI_INTERNAL_PATH;

        public static readonly string LOCAL_LFI_TEMPLATE = "Game Flags\n$";
        //public
	
		//protected
	
		//private
	
		//properties
		#endregion

        #region Public Methods
        public static void CreateLocalLFI()
        {
            if (!File.Exists(LOCAL_LFI_FULL_PATH))
            {
                if (!Directory.Exists(Path.GetDirectoryName(LOCAL_LFI_FULL_PATH)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(LOCAL_LFI_FULL_PATH));
                }

                File.WriteAllText(LOCAL_LFI_FULL_PATH, LOCAL_LFI_TEMPLATE);

                AssetDatabase.Refresh();
            }
        }

        public static string[] GetFlagNamesFromLFIs()
        {
            List<string> flagNames = new List<string>();

            // -- Get all the enum files and their values
            string[] loggerInfoFiles = Directory.GetFiles(Application.dataPath, "*" + FLAG_INFO_FILENAME, SearchOption.AllDirectories);

            for (int fileIndex = 0; fileIndex < loggerInfoFiles.Length; ++fileIndex)
            {
                string[] fileFlagNames = GetFlagNamesFromLFI(loggerInfoFiles[fileIndex]);

                for (int fileFlagCounter = 0; fileFlagCounter < fileFlagNames.Length; ++fileFlagCounter)
                {
                    string curFlag = LoggerHelper.FormatFlagName(fileFlagNames[fileFlagCounter]);

                    if (!flagNames.Contains(curFlag))
                    {
                        flagNames.Add(curFlag);
                    }
                }
            }

            return flagNames.ToArray();
        }

        public static string[] GetFlagNamesFromGeneratedFile()
        {
            return eLoggerFlags.GetNames();
        }

        public static string[] GetFlagNamesFromLFI(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            List<string> flagNames = new List<string>();

            string file = File.ReadAllText(path);

            int startIndex = file.IndexOf(FLAG_START_CHAR) + 1;

            // -- Not a valid file format...  It doesn't contain the start char
            if (startIndex < 1)
            {
                return null;
            }

            string rawFlagString = file.Substring(startIndex);

            rawFlagString = rawFlagString.RemoveWhitespace();

            string[] rawFlagNames = rawFlagString.Split(FLAG_DIVIDER_CHAR);

            string curFlag = null;

            for (int rawFlagCounter = 0; rawFlagCounter < rawFlagNames.Length; ++rawFlagCounter)
            {
                curFlag = LoggerHelper.FormatFlagName(rawFlagNames[rawFlagCounter]);

                if (!flagNames.Contains(curFlag))
                {
                    flagNames.Add(curFlag);
                }
            }

            return LoggerHelper.RemoveInvalidFlags(flagNames.ToArray());
        }

        public static string AddFlagNameToLFI(string path, string flagName)
        {
            if (!File.Exists(path))
            {
                return "No file exists at provided location.  If you are seeing this error, something is really wrong.";
            }

            string file = File.ReadAllText(path);

            int startIndex = file.IndexOf(FLAG_START_CHAR) + 1;

            // -- Not a valid file format...  It doesn't contain the start char
            if (startIndex < 1)
            {
                return "Invalid file format.  No starting character found.";
            }

            string rawFlagString = file.Substring(startIndex);

            rawFlagString = rawFlagString.RemoveWhitespace();

            if (!string.IsNullOrEmpty(rawFlagString))
            {
                file += FLAG_DIVIDER_CHAR;
            }

            file += "\n" + flagName;

            File.WriteAllText(path, file);

            GenerateFlagEnum();

            return null;
        }

        public static string RemoveFlagNameFromLFI(string path, string flagName)
        {
            if (!File.Exists(path))
            {
                return "No file exists at provided location.  If you are seeing this error, something is really wrong.";
            }

            List<string> flagNames = new List<string>();

            string file = File.ReadAllText(path);

            int startIndex = file.IndexOf(FLAG_START_CHAR) + 1;

            // -- Not a valid file format...  It doesn't contain the start char
            if (startIndex < 1)
            {
                return "Invalid file format.  No starting character found.";
            }

            string rawFlagString = file.Substring(startIndex);

            rawFlagString = rawFlagString.RemoveWhitespace();

            string[] rawFlagNames = rawFlagString.Split(FLAG_DIVIDER_CHAR);

            string curFlag = null;

            for (int rawFlagCounter = 0; rawFlagCounter < rawFlagNames.Length; ++rawFlagCounter)
            {
                curFlag = LoggerHelper.FormatFlagName(rawFlagNames[rawFlagCounter]);

                if (!flagNames.Contains(curFlag))
                {
                    flagNames.Add(curFlag);
                }
            }

            if (!flagNames.Contains(flagName))
            {
                return "Specified flag was not found in the file.";
            }

            flagNames.Remove(flagName);

            file = file.Substring(0, startIndex);

            for (int flagIndex = 0; flagIndex < flagNames.Count; ++flagIndex)
            {
                if (flagIndex > 0)
                {
                    file += FLAG_DIVIDER_CHAR;
                }
                file += "\n" + flagNames[flagIndex];
            }

            File.WriteAllText(path, file);

            GenerateFlagEnum();

            return null;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void GenerateFlagEnum()
        {
            string[] enumNames = GetFlagNamesFromLFIs();

            enumNames = LoggerHelper.RemoveIncludedAndInvalidFlags(enumNames);

            UpdateFlagFile(enumNames);
        }
        #endregion

        #region Private Methods
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
    }
}