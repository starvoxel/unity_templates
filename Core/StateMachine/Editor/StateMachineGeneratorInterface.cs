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
        public class sStateInfo
        {
            public class sTransitionInfo
            {
                public string TransitionName;
                public string StateName;

                public sTransitionInfo(string transitionName, string stateName)
                {
                    TransitionName = transitionName;
                    StateName = stateName;
                }

                public sTransitionInfo()
                {
                    TransitionName = string.Empty;
                    StateName = string.Empty;
                }

                public string Save()
                {
                    Dictionary<string, object> saveData = new Dictionary<string, object>();
                    saveData.Add("TransitionName", TransitionName);
                    saveData.Add("StateName", StateName);

                    return MiniJSON.Json.Serialize(saveData);
                }

                public void OnGUI()
                {
                    TransitionName = EditorGUILayout.TextField("Transition Name", TransitionName);
                    StateName = EditorGUILayout.TextField("State Name", StateName);
                }

                public static sTransitionInfo Load(object data)
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
                        return new sTransitionInfo();
                    }
                }

                public static sTransitionInfo Load(string json)
                {
                    return Load(MiniJSON.Json.Deserialize(json) as Dictionary<string, object>);
                }

                public static sTransitionInfo Load(Dictionary<string, object> data)
                {
                    sTransitionInfo info = new sTransitionInfo();

                    if (data != null)
                    {
                        if (data.ContainsKey("TransitionName"))
                        {
                            info.TransitionName = data["TransitionName"] as string;
                        }

                        if (data.ContainsKey("StateName"))
                        {
                            info.StateName = data["StateName"] as string;
                        }
                    }

                    return info;
                }
            }

            public string Name;
            public sTransitionInfo[] ValidTransitions;

            public sStateInfo()
            {
                Name = string.Empty;
                ValidTransitions = null;
                foldout = false;
            }

            public sStateInfo(string name, sTransitionInfo[] validTransitions)
            {
                Name = name;
                ValidTransitions = validTransitions;
                foldout = false;
            }

            private bool foldout;
            public void OnGUI()
            {
                Name = EditorGUILayout.TextField("Name", Name);
                foldout = EditorGUILayout.Foldout(foldout, "Valid Transitions");

                if (ValidTransitions == null)
                {
                    ValidTransitions = new sTransitionInfo[0];
                }

                List<sTransitionInfo> list = new List<sTransitionInfo>(ValidTransitions);

                if (foldout)
                {
                    EditorGUI.indentLevel += 1;

                    int count = EditorGUILayout.IntField("Size", list.Count);

                    while (count > list.Count)
                    {
                        if (list.Count != 0)
                        {
                            sTransitionInfo lastElement = list[list.Count - 1];
                            list.Add(new sTransitionInfo(lastElement.TransitionName, lastElement.StateName));
                        }
                        else
                        {
                            list.Add(new sTransitionInfo());
                        }
                    }

                    while (count < list.Count)
                    {
                        list.RemoveAt(list.Count - 1);
                    }

                    for (int i = 0; i < list.Count; ++i)
                    {
                        list[i].OnGUI();
                    }
                    EditorGUI.indentLevel -= 1;
                }

                ValidTransitions = list.ToArray();
            }

            public string Save()
            {
                Dictionary<string, object> saveData = new Dictionary<string, object>();
                saveData.Add("Name", Name);
                string[] savedTransitions = new string[ValidTransitions.Length];
                for (int i = 0; i < ValidTransitions.Length; ++i)
                {
                    savedTransitions[i] = ValidTransitions[i].Save();
                }
                saveData.Add("ValidTypes", savedTransitions);
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
                        List<object> validTransitions = data["ValidTypes"] as List<object>;

                        if (validTransitions != null)
                        {
                            info.ValidTransitions = new sTransitionInfo[validTransitions.Count];

                            for (int i = 0; i < validTransitions.Count; ++i)
                            {
                                info.ValidTransitions[i] = sTransitionInfo.Load(validTransitions[i]);
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
        public static void GenerateStateMachine(string path, string contextName, sStateInfo[] states)
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
                    for (int transitionIndex = 0; transitionIndex < validStates[validIndex].ValidTransitions.Length; ++transitionIndex)
                    {
                        if (!string.IsNullOrEmpty(validStates[validIndex].ValidTransitions[transitionIndex].TransitionName) && !transitionNames.Contains(validStates[validIndex].ValidTransitions[transitionIndex].TransitionName))
                        {
                            transitionNames.Add(validStates[validIndex].ValidTransitions[transitionIndex].TransitionName);
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
                contextGenerator.Session["m_TransitionTypes"] = transitionNames;

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