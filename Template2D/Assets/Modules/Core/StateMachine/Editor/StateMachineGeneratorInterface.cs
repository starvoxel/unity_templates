/* --------------------------
 *
 * StateMachineGeneratorInterface.cs
 *
 * Description: Interface used to generate a state machine.
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 2/9/2016 - Starvoxel
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
using System.Collections.Generic;
#endregion

#region Other Includes

#endregion
#endregion

namespace Starvoxel.Core
{
    public static class StateMachineGeneratorInterface
    {
        #region Structs
        public struct sStateInfo
        {
            public string Name;
            public string[] ValidTypes;

            public sStateInfo(string name, string[] validTypes)
            {
                Name = name;
                ValidTypes = validTypes;
            }

            public string Save()
            {
                Dictionary<string, object> saveData = new Dictionary<string, object>();
                saveData.Add("Name", Name);
                saveData.Add("ValidTypes", ValidTypes);
                return MiniJSON.Json.Serialize(saveData);
            }

            public static sStateInfo Load(object data)
            {
                if (data is string)
                {
                    return Load(data as string);
                }
                else if (data is Dictionary<string, object>)
                {
                    return Load(data as Dictionary<string, object>);
                }
                else
                {
                    return new sStateInfo();
                }
            }

            public static sStateInfo Load(Dictionary<string, object> data)
            {
                sStateInfo info = new sStateInfo();

                if (data != null)
                {
                    if (data.ContainsKey("Name"))
                    {
                        info.Name = data["Name"] as string;
                    }

                    if (data.ContainsKey("ValidTypes"))
                    {
                        List<object> validTypes = data["ValidTypes"] as List<object>;

                        if (validTypes != null)
                        {
                            info.ValidTypes = new string[validTypes.Count];

                            for (int i = 0; i < validTypes.Count; ++i)
                            {
                                info.ValidTypes[i] = validTypes[i] as string;
                            }
                        }
                    }
                }

                return info;
            }

            public static sStateInfo Load(string json)
            {
                return Load(MiniJSON.Json.Deserialize(json) as Dictionary<string, object>);
            }
        }
        #endregion

        #region Fields & Properties
        //const
        public const string FILE_EXTENSION = ".cs";
        public const string STATE_FOLDER_NAME = "States/";

        //public

        //protected

        //private

        //properties
        #endregion

        #region Public Methods
        public static void GenerateStateMachine(string path, string contextName, string[] transitionTypes, sStateInfo[] states)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path[path.Length - 1] != System.IO.Path.PathSeparator)
                {
                    path += System.IO.Path.DirectorySeparatorChar;
                }

                List<string> stateNames = new List<string>();
                List<sStateInfo> validStates = new List<sStateInfo>(states);

                List<int> invalidStateIndices = new List<int>();

                for (int i = 0; i < states.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(states[i].Name) && !stateNames.Contains(states[i].Name))
                    {
                        stateNames.Add(states[i].Name);
                    }
                    else
                    {
                        invalidStateIndices.Add(i);
                    }
                }

                if (invalidStateIndices.Count > 0)
                {
                    for (int invalidIndex = 0; invalidIndex < invalidStateIndices.Count; invalidIndex++)
                    {
                        validStates.Remove(states[invalidIndex]);
                    }
                }

                List<string> transitionNames = new List<string>();

                for (int validIndex = 0; validIndex < validStates.Count; ++validIndex)
                {
                    for (int transitionIndex = 0; transitionIndex < validStates[validIndex].ValidTypes.Length; ++transitionIndex)
                    {
                        if (!string.IsNullOrEmpty(validStates[validIndex].ValidTypes[transitionIndex]) && !transitionNames.Contains(validStates[validIndex].ValidTypes[transitionIndex]))
                        {
                            transitionNames.Add(validStates[validIndex].ValidTypes[transitionIndex]);
                        }
                    }
                }

                string ns = string.Format("{0}.{1}", CodifyString(PlayerSettings.companyName), CodifyString(PlayerSettings.productName));

                ContextGenerator contextGenerator = new ContextGenerator();

                contextGenerator.Session = new Dictionary<string, object>();

                contextGenerator.Session["m_Namespace"] = ns;
                contextGenerator.Session["m_ClassName"] = contextName;
                contextGenerator.Session["m_StateNames"] = stateNames.ToArray();
                contextGenerator.Session["m_StartingStateIndex"] = 0;
                contextGenerator.Session["m_TransitionTypes"] = transitionTypes;

                contextGenerator.Initialize();

                string test = contextGenerator.TransformText();

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                System.IO.File.WriteAllText(path + contextName + FILE_EXTENSION, test);

                AssetDatabase.Refresh();
            }
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

            return value;
        }
        #endregion
    }

}