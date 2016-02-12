/* --------------------------
 *
 * StateMachineGeneratorTester.cs
 *
 * Description: 
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 2/10/2016 - Starvoxel
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
	public class StateMachineGeneratorEditor : EditorWindow
	{
		#region Fields & Properties
		//const
        private const string CONTEXT_NAME_KEY = "ContextName";
        private const string TRANSITION_TYPES_KEY = "TransitionTypes";
        private const string STATE_INFO_KEY = "StateInfo";

        private static readonly string PATH = Application.dataPath + "/TestFolder/";
        private const string CONTEXT_NAME = "TestContext";
        private static readonly string[] TRANSITION_NAMES = new string[]
        {
            "Seen",
            "Heard",
            "Next"
        };

        private static readonly StateMachineGeneratorInterface.sStateInfo[] STATE_INFO = new StateMachineGeneratorInterface.sStateInfo[]
        {
            new StateMachineGeneratorInterface.sStateInfo("Idle", new string[]
                {
                    TRANSITION_NAMES[0],
                    TRANSITION_NAMES[1],
                    TRANSITION_NAMES[2],
                }),
            new StateMachineGeneratorInterface.sStateInfo("Patrol", new string[]
                {
                    TRANSITION_NAMES[0],
                    TRANSITION_NAMES[1],
                    TRANSITION_NAMES[2],
                }),
            new StateMachineGeneratorInterface.sStateInfo("Chase", new string[]
                {
                    TRANSITION_NAMES[2]
                }),
            new StateMachineGeneratorInterface.sStateInfo("Fire", new string[]
                {
                    TRANSITION_NAMES[2]
                }),
        };

		//public
	
		//protected
	
		//private
        private string m_ContextName = string.Empty;
        private bool m_TransitionTypesFoldout;
        private string[] m_TransitionTypes = null;
        private StateMachineGeneratorInterface.sStateInfo[] m_StateInfo;
	
		//properties
		#endregion
	
		#region Public Methods
		#endregion
	
		#region Private Methods
        [MenuItem("Window/State Machine Generator Tester")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            StateMachineGeneratorEditor window = (StateMachineGeneratorEditor)EditorWindow.GetWindow(typeof(StateMachineGeneratorEditor));
            window.Show();
        }

        private void GenerationTest()
        {
            string outputPath = EditorUtility.SaveFolderPanel(title: "Save Location",
                defaultName: "StateMachine",
                folder: "/Assets/");

            StateMachineGeneratorInterface.GenerateStateMachine(outputPath, CONTEXT_NAME, TRANSITION_NAMES, STATE_INFO);
        }

        private void OnGUI()
        {
            m_ContextName = EditorGUILayout.TextField("Context Name", m_ContextName);
            m_TransitionTypes = OnStringArrayGUI("Transition Types", m_TransitionTypes, ref m_TransitionTypesFoldout);

            EditorGUILayout.Space();

            if (GUILayout.Button("Test Generation"))
            {
                GenerationTest();
            }
            if (GUILayout.Button("Save"))
            {
                Save();
            }
            if (GUILayout.Button("Load"))
            {
                Load();
            }
        }

        private void Save()
        {
            string outputPath = EditorUtility.SaveFilePanelInProject(title: "Save Location",
                defaultName: "StateMachine",
                extension: "json",
                message: "Where would you like to save your state machine JSON?");

            if (!string.IsNullOrEmpty(outputPath))
            {
                Dictionary<string, object> saveData = new Dictionary<string, object>();
                saveData.Add(CONTEXT_NAME_KEY, CONTEXT_NAME);
                saveData.Add(TRANSITION_TYPES_KEY, TRANSITION_NAMES);
                List<string> stateValueData = new List<string>();
                if (STATE_INFO != null)
                {
                    for (int i = 0; i < STATE_INFO.Length; ++i)
                    {
                        stateValueData.Add(STATE_INFO[i].Save());
                    }
                }
                saveData.Add(STATE_INFO_KEY, stateValueData);

                string json = MiniJSON.Json.Serialize(saveData);

                System.IO.File.WriteAllText(outputPath, json);

                AssetDatabase.Refresh();
            }
        }

        private void Load()
        {
            string jsonPath = EditorUtility.OpenFilePanel(title: "JSON File Selection", directory: "", extension: "json");

            if (jsonPath != null)
            {
                Dictionary<string, object> data = MiniJSON.Json.Deserialize(System.IO.File.ReadAllText(jsonPath)) as Dictionary<string, object>;

                if (data != null)
                {
                    if (data.ContainsKey(CONTEXT_NAME_KEY))
                    {
                        m_ContextName = data[CONTEXT_NAME_KEY] as string;
                    }

                    if (data.ContainsKey(TRANSITION_TYPES_KEY))
                    {
                        List<object> transitionTypes = data[TRANSITION_TYPES_KEY] as List<object>;

                        if (transitionTypes != null)
                        {
                            m_TransitionTypes = new string[transitionTypes.Count];

                            for(int i = 0; i < transitionTypes.Count; ++i)
                            {
                                m_TransitionTypes[i] = transitionTypes[i] as string;
                            }
                        }
                    }

                    if (data.ContainsKey(STATE_INFO_KEY))
                    {
                        List<object> stateInfo = data[STATE_INFO_KEY] as List<object>;

                        if (stateInfo != null)
                        {
                            m_StateInfo = new StateMachineGeneratorInterface.sStateInfo[stateInfo.Count];

                            for(int i = 0; i < stateInfo.Count; ++i)
                            {
                                m_StateInfo[i] = StateMachineGeneratorInterface.sStateInfo.Load(stateInfo[i]);
                            }
                        }
                    }
                }
            }
        }
		#endregion
        private string[] OnStringArrayGUI(string name, string[] array, ref bool isShowing)
        {
            return OnStringArrayGUI(new GUIContent(name), array, ref isShowing);
        }


        private string[] OnStringArrayGUI(GUIContent content, string[] array, ref bool isShowing)
        {
            isShowing = EditorGUILayout.Foldout(isShowing, content);

            if (array == null)
            {
                array = new string[0];
            }

            List<string> list = new List<string>(array);

            if (isShowing)
            {
                EditorGUI.indentLevel += 1;

                int count = EditorGUILayout.IntField("Size", list.Count);

                while (count > list.Count)
                {
                    if (list.Count != 0)
                    {
                        list.Add(list[list.Count - 1]);
                    }
                    else
                    {
                        list.Add(string.Empty);
                    }
                }

                while (count < list.Count)
                {
                    list.RemoveAt(list.Count - 1);
                }

                for (int i = 0; i < list.Count; ++i)
                {
                    list[i] = EditorGUILayout.TextField("Element " + i.ToString(), list[i]);
                }
                EditorGUI.indentLevel -= 1;
            }

            return list.ToArray();
        }

        private int[] OnIntArrayGUI(string name, int[] array)
        {
            return OnIntArrayGUI(new GUIContent(name), array);
        }

        private bool foldout = false;
        private int[] OnIntArrayGUI(GUIContent content, int[] array)
        {
            foldout = EditorGUILayout.Foldout(foldout, content);

            if (array == null)
            {
                array = new int[0];
            }

            List<int> list = new List<int>(array);

            if (foldout)
            {
                EditorGUI.indentLevel += 1;

                int count = EditorGUILayout.IntField("Size", list.Count);

                while (count > list.Count)
                {
                    if (list.Count != 0)
                    {
                        list.Add(list[list.Count - 1]);
                    }
                    else
                    {
                        list.Add(0);
                    }
                }

                while (count < list.Count)
                {
                    list.RemoveAt(list.Count - 1);
                }

                for (int i = 0; i < list.Count; ++i)
                {
                    list[i] = EditorGUILayout.IntField("Element " + i.ToString(), list[i]);
                }
                EditorGUI.indentLevel -= 1;
            }

            return list.ToArray();
        }
	}
	
}