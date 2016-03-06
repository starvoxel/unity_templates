using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;
using Starvoxel.Core;

public class NewScriptWindow : EditorWindow
{
    #region Fields & Properties
    //classes
    class Styles
    {
        public GUIContent m_WarningContent = new GUIContent(string.Empty);
        public GUIStyle m_PreviewBox = new GUIStyle("OL Box");
        public GUIStyle m_PreviewTitle = new GUIStyle("OL Title");
        public GUIStyle m_LoweredBox = new GUIStyle("TextField");
        public GUIStyle m_HelpBox = new GUIStyle("helpbox");
        public Styles()
        {
            m_LoweredBox.padding = new RectOffset(1, 1, 1, 1);
        }
    }

    // Custom comparer to sort templates alphabetically,
    // but put MonoBehaviour and Plain Class as the first two
    private class TemplateNameComparer : IComparer<string>
    {
        private int GetRank(string s)
        {
            for (int i = 0; i < PRIORITY_TEMPLATE_NAMES.Length; ++i)
            {
                if (string.Compare(s, PRIORITY_TEMPLATE_NAMES[i]) == 0)
                {
                    return i;
                }
            }
            if (s.StartsWith(COMPARER_EDITOR_CLASS_PREFIX))
            {
                return PRIORITY_TEMPLATE_NAMES.Length + 100;
            }
            return PRIORITY_TEMPLATE_NAMES.Length;
        }

        public int Compare(string x, string y)
        {
            int rankX = GetRank(x);
            int rankY = GetRank(y);
            if (rankX == rankY)
                return x.CompareTo(y);
            else
                return rankX.CompareTo(rankY);
        }
    }

    //enums
    public enum eNamespaceType
    {
        CompagnyName = 0,
        ProjectName = 1,
        Directory = 2
    }

    public enum eScriptType
    {
        Project,
        Module
    }

    //const
    private const int BUTTON_WIDTH = 120;
    private const int LABEL_WIDTH = 120;
    private const string MODULE_TEMPLATE_PATH = "Modules/Core/NewScriptDialog/OtherResources/ScriptTemplates";
    private const string MODULE_HEADER_PATH = "Modules/Core/NewScriptDialog/OtherResources/HeaderTemplates";
    private const string PROJECT_TEMPLATE_PATH = "OtherResources/NewScriptDialog/ScriptTemplates";
    private const string PROJECT_HEADER_PATH = "OtherResources/NewScriptDialog/HeaderTemplates";
    private const string RESOURCES_TEMPLATE_PATH = "Resources/CustomScriptTemplates";
    private const string CUSTOM_EDITOR_NAME = "Editor";

    //Over this list in the order that you want specific templates
    private static readonly string[] PRIORITY_TEMPLATE_NAMES = new string[]
    {
        "Custom MonoBehaviour",
        "Empty Class",
        "Empty Interface",
        "Custom Scriptable Object"
    };

    private const string COMPARER_EDITOR_CLASS_PREFIX = "E:";
    private const string NO_TEMPLATE_FOUND_STRING = "No Template Found";
    private const string NO_HEADER_FOUND_STRING = "";
    // char array can't be const for compiler reasons but this should still be treated as such.
    private readonly char[] INVALID_PATH_CHARS = new char[] { '<', '>', ':', '"', '|', '?', '*', (char)0 };
    private readonly char[] PATH_SEPERATOR_CHARS = new char[] { '/', '\\' };

    //private
    private ScriptPrescription m_ScriptPrescription;
    private string m_BaseClass;
    private string m_CustomEditorTargetClassName = string.Empty;
    private bool m_IsEditorClass = false;
    private bool m_IsCustomEditor = false;
    private bool m_FocusTextFieldNow = true;
    private GameObject m_GameObjectToAddTo;
    private string m_Directory = string.Empty;
    private Vector2 m_PreviewScroll;
    private Vector2 m_OptionsScroll;
    private bool m_ClearKeyboardControl = false;
    private bool m_IsUsingHeader = true;

    private bool m_UsingDescription = false;
    private string m_Description = string.Empty;

    private bool m_UsingIncludes = false;
    private string m_Includes = string.Empty;

    private int m_TemplateIndex;
    private string[] m_TemplateNames;

    private int m_HeaderIndex;
    private string[] m_HeaderNames;

    private static Styles m_Styles;

    //properties
    private string extension
    {
        get
        {
            switch (m_ScriptPrescription.m_Lang)
            {
                case Language.CSharp:
                    return "cs";
                case Language.JavaScript:
                    return "js";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    #region Header Variables
    //const
    private const string LICENSE_FILE_KEY = "TextTemplateMacroReplacer_LicenseFile";
    private const string AUTHOR_KEY = "TextTemplateMacroReplacer_Author";

    private const string PROJECT_NAMESPACE_COUNT_KEY = "TextTemplateMacroReplacer_ProjectNamespaceCout";
    private const string PROJECT_NAMESPACE_TYPE_KEY = "TextTemplateMacroReplacer_ProjectNamespaceType";
    private const string PROJECT_USE_LONG_NAMESPACE_KEY = "TextTemplateMacroReplacer_ProjectUseLongNamespace";

    private const string MODULE_NAMESPACE_COUNT_KEY = "TextTemplateMacroReplacer_ModuleNamespaceCout";
    private const string MODULE_NAMESPACE_TYPE_KEY = "TextTemplateMacroReplacer_ModuleNamespaceType";
    private const string MODULE_USE_LONG_NAMESPACE_KEY = "TextTemplateMacroReplacer_ModuleUseLongNamespace";

    private const string DEFAULT_LICENSE = "All rights reserved.";

    private const char NAMESPACE_SEPERATOR_CHAR = ',';

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

    private static eNamespaceType[] ProjectNamespaceTypes
    {
        get
        {
            string[] typesStrArray = EditorPrefs.GetString(PROJECT_NAMESPACE_TYPE_KEY, string.Empty).Split(NAMESPACE_SEPERATOR_CHAR);

            if (typesStrArray.Length == 0 || string.IsNullOrEmpty(typesStrArray[0]))
            {
                ProjectNamespaceTypes = new eNamespaceType[] { eNamespaceType.CompagnyName };
                return ProjectNamespaceTypes;
            }
            else
            {
                eNamespaceType[] retVal = new eNamespaceType[typesStrArray.Length];

                for (int i = 0; i < retVal.Length; ++i)
                {
                    retVal[i] = (eNamespaceType)int.Parse(typesStrArray[i]);
                }

                return retVal;
            }
        }
        set
        {
            string types = string.Empty;

            for (int i = 0; i < value.Length; ++i)
            {
                if (i != 0)
                {
                    types += NAMESPACE_SEPERATOR_CHAR;
                }

                types += (int)value[i];
            }
            EditorPrefs.SetString(PROJECT_NAMESPACE_TYPE_KEY, types);
        }
    }
    private static eNamespaceType[] ModuleNamespaceTypes
    {
        get
        {
            string[] typesStrArray = EditorPrefs.GetString(MODULE_NAMESPACE_TYPE_KEY, string.Empty).Split(NAMESPACE_SEPERATOR_CHAR);

            if (typesStrArray.Length == 0 || string.IsNullOrEmpty(typesStrArray[0]))
            {
                ModuleNamespaceTypes = new eNamespaceType[] { eNamespaceType.CompagnyName };
                return ProjectNamespaceTypes;
            }
            else
            {
                eNamespaceType[] retVal = new eNamespaceType[typesStrArray.Length];

                for (int i = 0; i < retVal.Length; ++i)
                {
                    retVal[i] = (eNamespaceType)int.Parse(typesStrArray[i]);
                }

                return retVal;
            }
        }
        set
        {
            string types = string.Empty;

            for (int i = 0; i < value.Length; ++i)
            {
                if (i != 0)
                {
                    types += NAMESPACE_SEPERATOR_CHAR;
                }

                types += (int)value[i];
            }
            EditorPrefs.SetString(MODULE_NAMESPACE_TYPE_KEY, types);
        }
    }
    #endregion
    #endregion

    #region Private Methods
    #region Path Concatenation Methods
    private string GetAbsoluteBuiltinTemplatePath()
    {
        return Path.Combine(EditorApplication.applicationContentsPath, RESOURCES_TEMPLATE_PATH);
    }

    private string GetAbsoluteModuleTemplatePath()
    {
        return Application.dataPath + "/" + MODULE_TEMPLATE_PATH;
    }

    private string GetAbsoluteModuleHeaderPath()
    {
        return Application.dataPath + "/" + MODULE_HEADER_PATH;
    }

    private string TargetPath()
    {
        return Path.Combine(TargetDir(), m_ScriptPrescription.m_ClassName + "." + extension);
    }

    private string TargetDir()
    {
        return Path.Combine("Assets", m_Directory.Trim(PATH_SEPERATOR_CHARS));
    }
    #endregion

    #region Template Methods
    private void UpdateTemplateNamesAndTemplate()
    {
        // Remember old selected template name
        string oldSelectedTemplateName = null;
        if (m_TemplateNames != null && m_TemplateNames.Length > 0)
            oldSelectedTemplateName = m_TemplateNames[m_TemplateIndex];

        // Get new template names
        m_TemplateNames = GetTemplateNames();

        // Select template
        if (m_TemplateNames.Length == 0)
        {
            m_ScriptPrescription.m_Template = NO_TEMPLATE_FOUND_STRING;
            m_BaseClass = null;
        }
        else
        {
            if (oldSelectedTemplateName != null && m_TemplateNames.Contains(oldSelectedTemplateName))
                m_TemplateIndex = m_TemplateNames.ToList().IndexOf(oldSelectedTemplateName);
            else
                m_TemplateIndex = 0;
            m_ScriptPrescription.m_Template = GetTemplate(m_TemplateNames[m_TemplateIndex]);
        }

        m_UsingIncludes = m_ScriptPrescription.m_Template.Contains("$Includes");

        if (m_ScriptPrescription.m_StringReplacements.ContainsKey("$Includes") == false)
        {
            m_ScriptPrescription.m_StringReplacements.Add("$Includes", m_Includes);
        }

        if (!m_UsingIncludes)
        {
            m_Includes = string.Empty;
        }

        m_ScriptPrescription.m_StringReplacements["$Includes"] = m_Includes;

        HandleBaseClass();

        UpdateNamespace();
    }

    private void AutomaticHandlingOnChangeTemplate()
    {
        // Add or remove "Editor" from directory path
        if (m_IsEditorClass)
        {
            if (InvalidTargetPathForEditorScript())
                m_Directory = Path.Combine(m_Directory, "Editor");
        }
        else if (m_Directory.EndsWith("Editor"))
        {
            m_Directory = m_Directory.Substring(0, m_Directory.Length - 6).TrimEnd(PATH_SEPERATOR_CHARS);
        }

        // Move keyboard focus to relevant field
        if (m_IsCustomEditor)
            m_FocusTextFieldNow = true;
    }

    private string GetBaseClass(string templateContent)
    {
        string firstLine = templateContent.Substring(0, templateContent.IndexOf("\n"));
        if (firstLine.Contains("BASECLASS"))
        {
            string baseClass = firstLine.Substring(10).Trim();
            if (baseClass != string.Empty)
                return baseClass;
        }
        return null;
    }

    private bool GetFunctionIsIncluded(string baseClassName, string functionName, bool includeByDefault)
    {
        string prefName = "FunctionData_" + (baseClassName != null ? baseClassName + "_" : string.Empty) + functionName;
        return EditorPrefs.GetBool(prefName, includeByDefault);
    }

    private void SetFunctionIsIncluded(string baseClassName, string functionName, bool include)
    {
        string prefName = "FunctionData_" + (baseClassName != null ? baseClassName + "_" : string.Empty) + functionName;
        EditorPrefs.SetBool(prefName, include);
    }

    private void HandleBaseClass()
    {
        if (m_TemplateNames == null || m_HeaderNames == null)
        {
            return;
        }

        if (m_TemplateNames.Length == 0)
        {
            m_BaseClass = null;
            return;
        }

        // Get base class
        m_BaseClass = GetBaseClass(m_ScriptPrescription.m_Template);

        // If base class was found, strip first line from template
        if (m_BaseClass != null)
            m_ScriptPrescription.m_Template =
                m_ScriptPrescription.m_Template.Substring(m_ScriptPrescription.m_Template.IndexOf("\n") + 1);

        m_IsEditorClass = IsEditorClass(m_BaseClass);
        m_IsCustomEditor = (m_BaseClass == CUSTOM_EDITOR_NAME);
        m_ScriptPrescription.m_StringReplacements.Clear();

        if (m_IsUsingHeader)
        {
            m_ScriptPrescription.m_StringReplacements.Add("$Header", GetHeader());
        }

        // Try to find function file first in custom templates folder and then in built-in
        string functionDataFilePath = Path.Combine(GetAbsoluteModuleTemplatePath(), m_BaseClass + ".functions.txt");
        if (!File.Exists(functionDataFilePath))
            functionDataFilePath = Path.Combine(GetAbsoluteBuiltinTemplatePath(), m_BaseClass + ".functions.txt");

        if (!File.Exists(functionDataFilePath))
        {
            m_ScriptPrescription.m_Functions = null;
        }
        else
        {
            StreamReader reader = new StreamReader(functionDataFilePath);
            List<FunctionData> functionList = new List<FunctionData>();
            int lineNr = 1;
            while (!reader.EndOfStream)
            {
                string functionLine = reader.ReadLine();
                string functionLineWhole = functionLine;
                try
                {
                    if (functionLine.Substring(0, 7).ToLower() == "header ")
                    {
                        functionList.Add(new FunctionData(functionLine.Substring(7)));
                        continue;
                    }

                    FunctionData function = new FunctionData();

                    bool defaultInclude = false;
                    if (functionLine.Substring(0, 8) == "DEFAULT ")
                    {
                        defaultInclude = true;
                        functionLine = functionLine.Substring(8);
                    }

                    if (functionLine.Substring(0, 9) == "override ")
                    {
                        function.isVirtual = true;
                        functionLine = functionLine.Substring(9);
                    }

                    string returnTypeString = GetStringUntilSeperator(ref functionLine, " ");
                    function.returnType = (returnTypeString == "void" ? null : returnTypeString);
                    function.name = GetStringUntilSeperator(ref functionLine, "(");
                    string parameterString = GetStringUntilSeperator(ref functionLine, ")");
                    if (function.returnType != null)
                        function.returnDefault = GetStringUntilSeperator(ref functionLine, ";");
                    function.comment = functionLine;

                    string[] parameterStrings = parameterString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<ParameterData> parameterList = new List<ParameterData>();
                    for (int i = 0; i < parameterStrings.Length; i++)
                    {
                        string[] paramSplit = parameterStrings[i].Trim().Split(' ');
                        parameterList.Add(new ParameterData(paramSplit[1], paramSplit[0]));
                    }
                    function.parameters = parameterList.ToArray();

                    function.include = GetFunctionIsIncluded(m_BaseClass, function.name, defaultInclude);

                    functionList.Add(function);
                }
                catch (Exception e)
                {
                    Services.Logger.LogWithCategory(LoggerConstants.CORE_CATEGORY, LogType.Log, "Malformed function line: \"" + functionLineWhole + "\"\n  at " + functionDataFilePath + ":" + lineNr + "\n" + e);
                }
                lineNr++;
            }
            m_ScriptPrescription.m_Functions = functionList.ToArray();
        }
    }

    private string GetStringUntilSeperator(ref string source, string sep)
    {
        int index = source.IndexOf(sep);
        string result = source.Substring(0, index).Trim();
        source = source.Substring(index + sep.Length).Trim(' ');
        return result;
    }

    private string GetTemplate(string nameWithoutExtension)
    {
        string path = Path.Combine(GetAbsoluteModuleTemplatePath(), nameWithoutExtension + "." + extension + ".txt");
        if (File.Exists(path))
            return File.ReadAllText(path);

        path = Path.Combine(GetAbsoluteBuiltinTemplatePath(), nameWithoutExtension + "." + extension + ".txt");
        if (File.Exists(path))
            return File.ReadAllText(path);

        return NO_TEMPLATE_FOUND_STRING;
    }

    private string GetTemplateName()
    {
        if (m_TemplateNames.Length == 0)
            return NO_TEMPLATE_FOUND_STRING;
        return m_TemplateNames[m_TemplateIndex];
    }

    private string[] GetTemplateNames()
    {
        List<string> templates = new List<string>();

        // Get all file names of custom templates
        if (Directory.Exists(GetAbsoluteModuleTemplatePath()))
            templates.AddRange(Directory.GetFiles(GetAbsoluteModuleTemplatePath()));

        // Get all file names of built-in templates
        if (Directory.Exists(GetAbsoluteBuiltinTemplatePath()))
            templates.AddRange(Directory.GetFiles(GetAbsoluteBuiltinTemplatePath()));

        if (templates.Count == 0)
            return new string[0];

        // Filter and clean up list
        templates = templates
            .Distinct()
            .Where(f => (f.EndsWith("." + extension + ".txt")))
            .Select(f => Path.GetFileNameWithoutExtension(f.Substring(0, f.Length - 4)))
            .ToList();

        // Determine which scripts have editor class base class
        for (int i = 0; i < templates.Count; i++)
        {
            string templateContent = GetTemplate(templates[i]);
            if (IsEditorClass(GetBaseClass(templateContent)))
                templates[i] = COMPARER_EDITOR_CLASS_PREFIX + templates[i];
        }

        // Order list
        templates = templates
            .OrderBy(f => f, new TemplateNameComparer())
            .ToList();

        // Insert separator before first editor script template
        bool inserted = false;
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].StartsWith(COMPARER_EDITOR_CLASS_PREFIX))
            {
                templates[i] = templates[i].Substring(COMPARER_EDITOR_CLASS_PREFIX.Length);
                if (!inserted)
                {
                    templates.Insert(i, string.Empty);
                    inserted = true;
                }
            }
        }

        // Return list
        return templates.ToArray();
    }
    #endregion

    #region Header Methods
    private void UpdateHeaderNamesAndHeader()
    {
        // Remember old selected template name
        string oldSelectedHeaderName = null;
        if (m_HeaderNames != null && m_HeaderNames.Length > 0)
            oldSelectedHeaderName = m_HeaderNames[m_HeaderIndex];

        // Get new template names
        m_HeaderNames = GetHeaderNames();

        // Select template
        if (m_HeaderNames.Length == 0)
        {
            if (m_ScriptPrescription.m_StringReplacements.ContainsKey("$Header"))
            {
                m_ScriptPrescription.m_StringReplacements["$Header"] = GetHeader();
            }
            else
            {
                m_ScriptPrescription.m_StringReplacements.Add("$Header", GetHeader());
            }
            m_IsUsingHeader = false;
        }
        else
        {
            if (oldSelectedHeaderName != null && m_HeaderNames.Contains(oldSelectedHeaderName))
            {
                m_HeaderIndex = m_HeaderNames.ToList().IndexOf(oldSelectedHeaderName);
            }
            else
            {
                m_HeaderIndex = 0;
            }
        }

        HandleBaseClass();
    }

    private void RefreshHeader()
    {
        if (m_IsUsingHeader)
        {
            if (m_ScriptPrescription.m_StringReplacements.ContainsKey("$Header"))
            {
                m_ScriptPrescription.m_StringReplacements["$Header"] = GetHeader();
            }
            else
            {
                m_ScriptPrescription.m_StringReplacements.Add("$Header", GetHeader());
            }
        }
        else if (m_ScriptPrescription.m_StringReplacements.ContainsKey("$Header"))
        {
            m_ScriptPrescription.m_StringReplacements.Remove("$Header");
        }
    }

    private string GetHeader()
    {
        if (m_HeaderNames.Length > 0)
        {
            string path = Path.Combine(GetAbsoluteModuleHeaderPath(), GetHeaderName() + ".txt");
            if (File.Exists(path))
            {
                string file = File.ReadAllText(path);

                file = PopulateHeader(file);

                return file;
            }
        }
        return NO_HEADER_FOUND_STRING;
    }

    private string[] GetHeaderNames()
    {
        List<string> headers = new List<string>();

        // Get all file names of custom templates
        if (Directory.Exists(GetAbsoluteModuleHeaderPath()))
        {
            headers.AddRange(Directory.GetFiles(GetAbsoluteModuleHeaderPath()));
        }

        for (int i = 0; i < headers.Count; ++i)
        {
            if (Path.GetExtension(headers[i]).Contains("meta"))
            {
                headers.RemoveAt(i);
                i -= 1;
            }
        }

        if (headers.Count == 0)
            return new string[0];

        // Filter and clean up list
        headers = headers
            .Distinct()
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToList();

        // Order list
        headers.Sort();

        // Return list
        return headers.ToArray();
    }

    private string GetHeaderName()
    {
        if (m_HeaderNames.Length == 0)
        {
            return NO_HEADER_FOUND_STRING;
        }
        else
        {
            return m_HeaderNames[m_HeaderIndex];
        }
    }


    private string PopulateHeader(string header)
    {
        if (!string.IsNullOrEmpty(header))
        {
            header = header.Replace("$ScriptName", m_ScriptPrescription.m_ClassName);
            header = header.Replace("$CreationData", System.DateTime.Now.ToShortDateString());
            header = header.Replace("$ProjectName", PlayerSettings.productName);

            if (string.IsNullOrEmpty(PlayerSettings.productName))
            {
                PlayerSettings.productName = "DefaultProject";
            }
            header = header.Replace("$CompanyName", PlayerSettings.companyName);
            header = header.Replace("$Author", string.IsNullOrEmpty(Author) ? "" : Author);

            string licenseMsg = DEFAULT_LICENSE;
            if (!string.IsNullOrEmpty(LicenseFilePath))
            {
                licenseMsg = System.IO.File.ReadAllText(LicenseFilePath);
                licenseMsg = licenseMsg.Replace("\n", "\n * ");
            }

            header = header.Replace("$SourceLicense", licenseMsg);

            m_UsingDescription = header.Contains("$Description");
            header = header.Replace("$Description", m_Description);
        }
        return header;
    }
    #endregion

    #region MenuItem Methods
    [MenuItem("Component/Scripts/New Script...", false, 0)]
    private static void OpenFromComponentMenu()
    {
        Init();
    }

    [MenuItem("Component/Scripts/New Script...", true, 0)]
    private static bool OpenFromComponentMenuValidation()
    {
        return (Selection.activeObject is GameObject);
    }

    [MenuItem("Assets/Create/C# Script", false, 75)]
    private static void OpenFromAssetsMenu()
    {
        Init();
    }
    #endregion

    #region Initialization
    private static void Init()
    {
        GetWindow<NewScriptWindow>(true, "Create Script");
    }

    public NewScriptWindow()
    {
        // Large initial size
        position = new Rect(50, 50, 770, 500);
        // But allow to scale down to smaller size
        minSize = new Vector2(550, 400);

        m_ScriptPrescription = new ScriptPrescription();
    }

    private void OnEnable()
    {
        m_ScriptPrescription.m_Lang = (Language)1;
        UpdateHeaderNamesAndHeader();
        UpdateTemplateNamesAndTemplate();
        OnSelectionChange();
    }
    #endregion

    #region GUI Methods
    [PreferenceItem("New Script")]
    private static void OnPreferenceItem()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("License File:", GUILayout.Width(LABEL_WIDTH));

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
            GUILayout.Label("Author: ", GUILayout.Width(LABEL_WIDTH));
            Author = EditorGUILayout.TextField(Author);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnGUI()
    {
        if (m_Styles == null)
            m_Styles = new Styles();

        EditorGUIUtility.labelWidth = LABEL_WIDTH;

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && CanCreate())
            Create();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(10);

            PreviewGUI();
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            {
                OptionsGUI();

                GUILayout.FlexibleSpace();

                CreateAndCancelButtonsGUI();
            } EditorGUILayout.EndVertical();

            GUILayout.Space(10);
        } EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Clear keyboard focus if clicking a random place inside the dialog,
        // or if ClearKeyboardControl flag is set.
        if (m_ClearKeyboardControl || (Event.current.type == EventType.MouseDown && Event.current.button == 0))
        {
            GUIUtility.keyboardControl = 0;
            m_ClearKeyboardControl = false;
            Repaint();
        }
    }

    private void IncludeGUI()
    {
        if (!m_ScriptPrescription.m_StringReplacements.ContainsKey("$Includes"))
        {
            m_ScriptPrescription.m_StringReplacements.Add("$Includes", string.Empty);
        }

        if (m_UsingIncludes)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Extra Includes", GUILayout.Width(LABEL_WIDTH - 4));

                string oldIncludes = m_Includes;
                m_Includes = EditorGUILayout.TextArea(m_Includes, GUILayout.MaxWidth(450f - LABEL_WIDTH - 25));

                if (oldIncludes != m_Includes)
                {
                    m_ScriptPrescription.m_StringReplacements["$Includes"] = m_Includes;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (m_Includes != string.Empty)
            {
                m_Includes = string.Empty;
                m_ScriptPrescription.m_StringReplacements["$Includes"] = m_Includes;
            }
        }
    }

    private void NamespaceGUI(eScriptType scriptType)
    {
        eNamespaceType[] oldNamespaceTypes = scriptType == eScriptType.Module ? ModuleNamespaceTypes : ProjectNamespaceTypes;

        EditorGUI.indentLevel = 1;
        List<eNamespaceType> namespaceTypes = new List<eNamespaceType>(oldNamespaceTypes);

        int size;
        GUILayout.BeginHorizontal();
        {
            size = EditorGUILayout.IntField("Size: ", namespaceTypes.Count);
        }
        GUILayout.EndHorizontal();

        if (size > 5)
        {
            size = 5;
        }
        else if (size < 1)
        {
            size = 1;
        }

        if (size != namespaceTypes.Count)
        {
            int dif = size - namespaceTypes.Count;

            while (dif < 0)
            {
                namespaceTypes.RemoveAt(namespaceTypes.Count - 1);
                dif += 1;
            }

            while (dif > 0)
            {
                namespaceTypes.Add(eNamespaceType.CompagnyName);
                dif -= 1;
            }
        }

        for (int i = 0; i < namespaceTypes.Count; ++i)
        {
            namespaceTypes[i] = (eNamespaceType)EditorGUILayout.EnumPopup("Namespace Type " + i + ": ", namespaceTypes[i]);

            if (namespaceTypes[i] == eNamespaceType.Directory)
            {
                GUI.enabled = false;
            }
        }

        if (scriptType == eScriptType.Module)
        {
            ModuleNamespaceTypes = namespaceTypes.ToArray();
        }
        else
        {
            ProjectNamespaceTypes = namespaceTypes.ToArray();
        }

        GUI.enabled = true;

        UpdateNamespace();

        EditorGUI.indentLevel = 0;
    }

    private void HeaderGUI()
    {
        m_IsUsingHeader = EditorGUILayout.Toggle("Is Header", m_IsUsingHeader);

        if (!m_IsUsingHeader && m_ScriptPrescription.m_StringReplacements.ContainsKey("$Header"))
        {
            m_ScriptPrescription.m_StringReplacements.Remove("$Header");
        }
        else if (m_IsUsingHeader && !m_ScriptPrescription.m_StringReplacements.ContainsKey("$Header"))
        {
            m_ScriptPrescription.m_StringReplacements.Add("$Header", GetHeader());
        }

        bool oldGUI = GUI.enabled;
        GUI.enabled = m_IsUsingHeader;

        m_HeaderIndex = Mathf.Clamp(m_HeaderIndex, 0, m_HeaderNames.Length - 1);
        int headerIndexNew = EditorGUILayout.Popup("Header", m_HeaderIndex, m_HeaderNames);
        if (headerIndexNew != m_HeaderIndex)
        {
            m_HeaderIndex = headerIndexNew;
            UpdateHeaderNamesAndHeader();
        }

        if (m_UsingDescription)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Description", GUILayout.Width(LABEL_WIDTH - 4));
                string oldDescription = m_Description;
                m_Description = EditorGUILayout.TextArea(m_Description, GUILayout.MaxWidth(450f - LABEL_WIDTH - 25));

                if (oldDescription != m_Description)
                {
                    RefreshHeader();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUI.enabled = oldGUI;
    }

    private bool CanCreate()
    {
        return m_ScriptPrescription.m_ClassName.Length > 0 &&
            !File.Exists(TargetPath()) &&
            !ClassAlreadyExists() &&
            !ClassNameIsInvalid() &&
            !TargetClassDoesNotExist() &&
            !TargetClassIsNotValidType() &&
            !InvalidTargetPath() &&
            !InvalidTargetPathForEditorScript();
    }

    private void Create()
    {
        CreateScript();

        Close();
        GUIUtility.ExitGUI();
    }

    private void CreateAndCancelButtonsGUI()
    {
        bool canCreate = CanCreate();

        // Create string to tell the user what the problem is
        string blockReason = string.Empty;
        if (!canCreate && m_ScriptPrescription.m_ClassName != string.Empty)
        {
            if (File.Exists(TargetPath()))
                blockReason = "A script called \"" + m_ScriptPrescription.m_ClassName + "\" already exists at that path.";
            else if (ClassAlreadyExists())
                blockReason = "A class called \"" + m_ScriptPrescription.m_ClassName + "\" already exists.";
            else if (ClassNameIsInvalid())
                blockReason = "The script name may only consist of a-z, A-Z, 0-9, _.";
            else if (TargetClassDoesNotExist())
                if (m_CustomEditorTargetClassName == string.Empty)
                    blockReason = "Fill in the script component to make an editor for.";
                else
                    blockReason = "A class called \"" + m_CustomEditorTargetClassName + "\" could not be found.";
            else if (TargetClassIsNotValidType())
                blockReason = "The class \"" + m_CustomEditorTargetClassName + "\" is not of type UnityEngine.Object.";
            else if (InvalidTargetPath())
                blockReason = "The folder path contains invalid characters.";
            else if (InvalidTargetPathForEditorScript())
                blockReason = "Editor scripts should be stored in a folder called Editor.";
        }

        // Warning about why the script can't be created
        if (blockReason != string.Empty)
        {
            m_Styles.m_WarningContent.text = blockReason;
            GUILayout.BeginHorizontal(m_Styles.m_HelpBox);
            {
                GUILayout.Label(m_Styles.m_WarningContent, EditorStyles.wordWrappedMiniLabel);
            } GUILayout.EndHorizontal();
        }

        // Cancel and create buttons
        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(BUTTON_WIDTH)))
            {
                Close();
                GUIUtility.ExitGUI();
            }

            bool guiEnabledTemp = GUI.enabled;
            GUI.enabled = canCreate;
            if (GUILayout.Button(GetCreateButtonText(), GUILayout.Width(BUTTON_WIDTH)))
            {
                Create();
            }
            GUI.enabled = guiEnabledTemp;
        } GUILayout.EndHorizontal();
    }

    private bool CanAddComponent()
    {
        return false;// (m_GameObjectToAddTo != null && m_BaseClass == MONOBEHAVIOUR_NAME);
    }

    private void OptionsGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        {
            GUILayout.BeginHorizontal();
            {
                NameGUI();
                LanguageGUI();
            } GUILayout.EndHorizontal();

            GUILayout.Space(10);

            TargetPathGUI();

            GUILayout.Space(10);

            NamespaceGUI(GetScriptType());

            GUILayout.Space(20);

            HeaderGUI();

            GUILayout.Space(20);

            IncludeGUI();

            GUILayout.Space(20);

            TemplateSelectionGUI();

            if (m_IsCustomEditor)
            {
                GUILayout.Space(10);
                CustomEditorTargetClassNameGUI();
            }

            GUILayout.Space(10);

        } EditorGUILayout.EndVertical();
    }

    private bool FunctionHeader(string header, bool expandedByDefault)
    {
        GUILayout.Space(5);
        bool expanded = GetFunctionIsIncluded(m_BaseClass, header, expandedByDefault);
        bool expandedNew = GUILayout.Toggle(expanded, header, EditorStyles.foldout);
        if (expandedNew != expanded)
            SetFunctionIsIncluded(m_BaseClass, header, expandedNew);
        return expandedNew;
    }

    private void FunctionsGUI()
    {
        if (m_ScriptPrescription.m_Functions == null)
        {
            GUILayout.FlexibleSpace();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Functions", GUILayout.Width(LABEL_WIDTH - 4));

            EditorGUILayout.BeginVertical(m_Styles.m_LoweredBox);
            m_OptionsScroll = EditorGUILayout.BeginScrollView(m_OptionsScroll);
            {
                bool expanded = FunctionHeader("General", true);

                for (int i = 0; i < m_ScriptPrescription.m_Functions.Length; i++)
                {
                    FunctionData func = m_ScriptPrescription.m_Functions[i];

                    if (func.name == null)
                    {
                        expanded = FunctionHeader(func.comment, false);
                    }
                    else if (expanded)
                    {
                        Rect toggleRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.toggle);
                        toggleRect.x += 15;
                        toggleRect.width -= 15;
                        bool include = GUI.Toggle(toggleRect, func.include, new GUIContent(func.name, func.comment));
                        if (include != func.include)
                        {
                            m_ScriptPrescription.m_Functions[i].include = include;
                            SetFunctionIsIncluded(m_BaseClass, func.name, include);
                        }
                    }
                }
            } EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

        } EditorGUILayout.EndHorizontal();
    }

    private void AttachToGUI()
    {
        GUILayout.BeginHorizontal();
        {
            m_GameObjectToAddTo = EditorGUILayout.ObjectField("Attach to", m_GameObjectToAddTo, typeof(GameObject), true) as GameObject;

            if (ClearButton())
                m_GameObjectToAddTo = null;
        } GUILayout.EndHorizontal();

        HelpField("Click a GameObject or Prefab to select.");
    }

    private void SetClassNameBasedOnTargetClassName()
    {
        if (m_CustomEditorTargetClassName == string.Empty)
            m_ScriptPrescription.m_ClassName = string.Empty;
        else
            m_ScriptPrescription.m_ClassName = m_CustomEditorTargetClassName + "Editor";
    }

    private void CustomEditorTargetClassNameGUI()
    {
        GUI.SetNextControlName("CustomEditorTargetClassNameField");

        string newName = EditorGUILayout.TextField("Editor for", m_CustomEditorTargetClassName);
        m_ScriptPrescription.m_StringReplacements["$TargetClassName"] = newName;
        if (newName != m_CustomEditorTargetClassName)
        {
            m_CustomEditorTargetClassName = newName;
            SetClassNameBasedOnTargetClassName();
        }

        if (m_FocusTextFieldNow && Event.current.type == EventType.repaint)
        {
            GUI.FocusControl("CustomEditorTargetClassNameField");
            m_FocusTextFieldNow = false;
            Repaint();
        }

        HelpField("Script component to make an editor for.");
    }

    private void TargetPathGUI()
    {
        m_Directory = EditorGUILayout.TextField("Save in", m_Directory, GUILayout.ExpandWidth(true));

        HelpField("Click a folder in the Project view to select.");
    }

    private bool ClearButton()
    {
        return GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(40));
    }

    private void TemplateSelectionGUI()
    {
        m_TemplateIndex = Mathf.Clamp(m_TemplateIndex, 0, m_TemplateNames.Length - 1);
        int templateIndexNew = EditorGUILayout.Popup("Template", m_TemplateIndex, m_TemplateNames);
        if (templateIndexNew != m_TemplateIndex)
        {
            m_TemplateIndex = templateIndexNew;
            UpdateTemplateNamesAndTemplate();
            AutomaticHandlingOnChangeTemplate();
        }
    }

    private void NameGUI()
    {
        GUI.SetNextControlName("ScriptNameField");
        string oldClassName = m_ScriptPrescription.m_ClassName;
        m_ScriptPrescription.m_ClassName = EditorGUILayout.TextField("Name", m_ScriptPrescription.m_ClassName);

        if (m_FocusTextFieldNow && !m_IsCustomEditor && Event.current.type == EventType.repaint)
        {
            GUI.FocusControl("ScriptNameField");
            m_FocusTextFieldNow = false;
        }

        if (oldClassName != m_ScriptPrescription.m_ClassName)
        {
            RefreshHeader();
        }
    }

    private void LanguageGUI()
    {
        var langNew = Language.CSharp;

        if (langNew != m_ScriptPrescription.m_Lang)
        {
            m_ScriptPrescription.m_Lang = langNew;
            UpdateTemplateNamesAndTemplate();
            AutomaticHandlingOnChangeTemplate();
        }
    }

    private void HelpField(string helpText)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(LABEL_WIDTH);
            GUILayout.Label(helpText, m_Styles.m_HelpBox);
        }
        GUILayout.EndHorizontal();
    }

    private void PreviewGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(Mathf.Max(position.width * 0.3f, position.width - 450f)));
        {
            // Reserve room for preview title
            Rect previewHeaderRect = GUILayoutUtility.GetRect(new GUIContent("Preview"), m_Styles.m_PreviewTitle);

            // Secret! Toggle curly braces on new line when double clicking the script preview title
            Event evt = Event.current;
            if (evt.type == EventType.MouseDown && evt.clickCount == 2 && previewHeaderRect.Contains(evt.mousePosition))
            {
                EditorPrefs.SetBool("CurlyBracesOnNewLine", !EditorPrefs.GetBool("CurlyBracesOnNewLine"));
                Repaint();
            }

            // Preview scroll view
            m_PreviewScroll = EditorGUILayout.BeginScrollView(m_PreviewScroll, m_Styles.m_PreviewBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    // Tiny space since style has no padding in right side
                    GUILayout.Space(5);

                    // Preview text itself
                    string previewStr = new NewScriptGenerator(m_ScriptPrescription).ToString();
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
            GUI.Label(previewHeaderRect, new GUIContent("Preview"), m_Styles.m_PreviewTitle);

            GUILayout.Space(4);
        } EditorGUILayout.EndVertical();
    }
    #endregion

    #region Helper Methods
    private void UpdateNamespace()
    {
        string ns = string.Empty;
        string directory = null;

        eNamespaceType[] namespaceTypes = GetScriptType() == eScriptType.Module ? ModuleNamespaceTypes : ProjectNamespaceTypes;

        string[] folders = m_Directory.Split(PATH_SEPERATOR_CHARS);


        int folderIndex = 0;

        while (folderIndex < folders.Length && (folders[folderIndex] == "Modules" || folders[folderIndex] == "Scripts"))
        {
            folderIndex += 1;
        }

        if (folderIndex >= folders.Length)
        {
            Services.Logger.LogWithCategory(LoggerConstants.EDITOR_UTILITY_CATEGORY, LogType.Error, "Couldn't find proper directory namespace name...");
        }
        else
        {
            directory = folders[folderIndex];
            directory = directory.Replace(" ", "");
        }

        if (namespaceTypes != null && namespaceTypes.Length > 0)
        {
            for (int i = 0; i < namespaceTypes.Length; ++i)
            {
                if (i != 0)
                {
                    ns += '.';
                }

                bool breakOut = false;

                switch (namespaceTypes[i])
                {
                    case eNamespaceType.CompagnyName:
                        ns += CodifyString(PlayerSettings.companyName);
                        break;
                    case eNamespaceType.ProjectName:
                        ns += CodifyString(PlayerSettings.productName);
                        break;
                    case eNamespaceType.Directory:
                        breakOut = true;
                        if (!string.IsNullOrEmpty(directory))
                        {
                            ns += directory;
                        }
                        break;
                }

                if (breakOut)
                {
                    break;
                }
            }

            if (!string.IsNullOrEmpty(ns))
            {
                m_ScriptPrescription.m_Namespace = " namespace " + ns + "\n{";
            }
            else
            {
                m_ScriptPrescription.m_Namespace = string.Empty;
            }
        }
    }

    private static string CodifyString(string value)
    {
        while (value.Length > 0 && value[0] >= '0' && value[0] <= '9')
        {
            value = value.Remove(0, 1);
        }

        value = value.Replace(" ", "");

        return value;
    }

    private eScriptType GetScriptType()
    {
        eScriptType scriptType = eScriptType.Project;

        if (!InvalidTargetPath())
        {
            if (TargetDir().Split(PATH_SEPERATOR_CHARS, StringSplitOptions.None).Contains("Modules"))
            {
                scriptType = eScriptType.Module;
            }
        }

        return scriptType;
    }

    private bool InvalidTargetPath()
    {
        if (m_Directory.IndexOfAny(INVALID_PATH_CHARS) >= 0)
            return true;
        if (TargetDir().Split(PATH_SEPERATOR_CHARS, StringSplitOptions.None).Contains(string.Empty))
            return true;
        return false;
    }

    private bool InvalidTargetPathForEditorScript()
    {
        return m_IsEditorClass && !m_Directory.ToLower().Split(PATH_SEPERATOR_CHARS).Contains("editor");
    }

    private bool IsFolder(Object obj)
    {
        return Directory.Exists(AssetDatabase.GetAssetPath(obj));
    }

    private bool ClassNameIsInvalid()
    {
        return !System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(
            m_ScriptPrescription.m_ClassName);
    }

    private bool ClassExists(string className)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.GetType(className, false) != null);
    }

    private bool ClassAlreadyExists()
    {
        if (m_ScriptPrescription.m_ClassName == string.Empty)
            return false;
        return ClassExists(m_ScriptPrescription.m_ClassName);
    }

    private bool TargetClassDoesNotExist()
    {
        if (!m_IsCustomEditor)
            return false;
        if (m_CustomEditorTargetClassName == string.Empty)
            return true;
        return !ClassExists(m_CustomEditorTargetClassName);
    }

    private bool TargetClassIsNotValidType()
    {
        if (!m_IsCustomEditor)
            return false;
        if (m_CustomEditorTargetClassName == string.Empty)
            return true;
        return AppDomain.CurrentDomain.GetAssemblies()
            .All(a => !typeof(UnityEngine.Object).IsAssignableFrom(a.GetType(m_CustomEditorTargetClassName, false)));
    }

    private string GetCreateButtonText()
    {
        return /*CanAddComponent() ? "Create and Attach" :*/ "Create";
    }
    #endregion

    #region IO Methods
    private void CreateScript()
    {
        if (!Directory.Exists(TargetDir()))
            Directory.CreateDirectory(TargetDir());

        var writer = new StreamWriter(TargetPath());
        writer.Write(new NewScriptGenerator(m_ScriptPrescription).ToString());
        writer.Close();
        writer.Dispose();
        AssetDatabase.Refresh();
    }

    private string LoadFile(string path)
    {
        string file = null;
        if (Directory.Exists(Path.GetDirectoryName(path)))
        {
            file = System.IO.File.ReadAllText(path);
        }

        return file;
    }
    #endregion

    private void OnSelectionChange()
    {
        m_ClearKeyboardControl = true;

        if (Selection.activeObject == null)
            return;

        if (IsFolder(Selection.activeObject))
        {
            m_Directory = AssetPathWithoutAssetPrefix(Selection.activeObject);
            if (m_IsEditorClass && InvalidTargetPathForEditorScript())
            {
                m_Directory = Path.Combine(m_Directory, "Editor");
            }
        }
        else if (Selection.activeGameObject != null)
        {
            m_GameObjectToAddTo = Selection.activeGameObject;
        }
        else if (m_IsCustomEditor && Selection.activeObject is MonoScript)
        {
            m_CustomEditorTargetClassName = Selection.activeObject.name;
            SetClassNameBasedOnTargetClassName();
        }

        Repaint();
    }

    private string AssetPathWithoutAssetPrefix(Object obj)
    {
        return AssetDatabase.GetAssetPath(obj).Substring(7);
    }

    private bool IsEditorClass(string className)
    {
        if (className == null)
            return false;
        return GetAllClasses("UnityEditor").Contains(className);
    }
    #endregion

    /// Method to populate a list with all the class in the namespace provided by the user
    static List<string> GetAllClasses(string nameSpace)
    {
        // Get the UnityEditor assembly
        Assembly asm = Assembly.GetAssembly(typeof(Editor));

        // Create a list for the namespaces
        List<string> namespaceList = new List<string>();

        // Create a list that will hold all the classes the suplied namespace is executing
        List<string> returnList = new List<string>();

        foreach (Type type in asm.GetTypes())
        {
            if (type.Namespace == nameSpace)
                namespaceList.Add(type.Name);
        }

        // Now loop through all the classes returned above and add them to our classesName list
        foreach (String className in namespaceList)
            returnList.Add(className);

        return returnList;
    }
}
