using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Microsoft.CSharp;

public class NodeEditorUtilities
{
    /// <summary>
    /// C# Script Icon
    /// </summary>
    private readonly static Texture2D scriptIcon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);

    /// <summary>
    /// 4 blank spaces to imitate pressing tab
    /// </summary>
    static readonly string tab = "    ";

    /// <summary>
    /// Text to add at the end of an Action's name
    /// </summary>
    static readonly string actionsEnding = "Action";

    /// <summary>
    /// Text to add at the end of a Conditions method's name
    /// </summary>
    static readonly string conditionsEnding = "SuccessCheck";

    /// <summary>
    /// Text to add at the end of the CreateMethod of a <see cref="FSM"/>
    /// </summary>
    static readonly string FSMCreateName = "StateMachine";

    /// <summary>
    /// Template for the CreateMethod of a <see cref="FSM"/>
    /// </summary>
    static readonly string FSMCreateTemplate = "\n"
        + tab + "private void Create#CREATENAME#()\n"
        + tab + "{\n"
        + tab + tab + "// Perceptions\n"
        + tab + tab + "// Modify or add new Perceptions, see the guide for more\n"
        + tab + tab + "#PERCEPTIONS#\n"
        + tab + tab + "// States\n"
        + tab + tab + "#STATES#\n"
        + tab + tab + "// Transitions#TRANSITIONS#\n"
        + tab + tab + "\n"
        + tab + tab + "// ExitPerceptions\n"
        + tab + tab + "#EXIT_PERCEPTIONS#\n"
        + tab + tab + "// ExitTransitions\n"
        + tab + tab + "#EXITS#\n"
        + tab + "}";

    /// <summary>
    /// Text to add at the end of the CreateMethod of a <see cref="BehaviourTree"/>
    /// </summary>
    static readonly string BTCreateName = "BehaviourTree";

    /// <summary>
    /// Template for the CreateMethod of a <see cref="BehaviourTree"/>
    /// </summary>
    static readonly string BTCreateTemplate = "\n"
        + tab + "private void Create#CREATENAME#()\n"
        + tab + "{\n"
        + tab + tab + "// Nodes\n"
        + tab + tab + "#NODES#\n"
        + tab + tab + "#CHILDS#\n"
        + tab + tab + "// SetRoot\n"
        + tab + tab + "#SETROOT#\n"
        + tab + tab + "\n"
        + tab + tab + "// ExitPerceptions\n"
        + tab + tab + "#EXIT_PERCEPTIONS#\n"
        + tab + tab + "// ExitTransitions\n"
        + tab + tab + "#EXITS#\n"
        + tab + "}";

    /// <summary>
    /// Text to add at the end of the CreateMethod of a <see cref="UtilitySystem"/>
    /// </summary>
    static readonly string USCreateName = "UtilitySystem";

    /// <summary>
    /// Template for the CreateMethod of a <see cref="UtilitySystem"/>
    /// </summary>
    static readonly string USCreateTemplate = "\n"
        + tab + "private void Create#CREATENAME#()\n"
        + tab + "{\n"
        + tab + tab + "// FACTORS\n"
        + tab + tab + "#FACTORS#\n"
        + tab + tab + "// ACTIONS\n"
        + tab + tab + "#ACTIONS#\n"
        + tab + tab + "\n"
        + tab + tab + "// ExitPerceptions\n"
        + tab + tab + "#EXIT_PERCEPTIONS#\n"
        + tab + tab + "// ExitTransitions\n"
        + tab + tab + "#EXITS#\n"
        + tab + "}";

    /// <summary>
    /// Text to add at the end of a FSM's name
    /// </summary>
    static readonly string FSMEnding = "_FSM";

    /// <summary>
    /// Text to add at the end of a BT's name
    /// </summary>
    static readonly string BTEnding = "_BT";

    /// <summary>
    /// Text to add at the end of a US's name
    /// </summary>
    static readonly string USEnding = "_US";

    /// <summary>
    /// Text to add at the end of a subFSM's name
    /// </summary>
    static readonly string subFSMEnding = "_SubFSM";

    /// <summary>
    /// Text to add at the end of a subBT's name
    /// </summary>
    static readonly string subBTEnding = "_SubBT";

    /// <summary>
    /// Text to add at the end of a subUS's name
    /// </summary>
    static readonly string subUSEnding = "_SubUS";

    /// <summary>
    /// Name of the GUI package folder
    /// </summary>
    static readonly string GUIpackageName = "Intelligent Behaviours GUI Package";

    /// <summary>
    /// Name for the saves Folder
    /// </summary>
    static readonly string savesFolderName = "Saves";

    /// <summary>
    /// Name for the scripts Folder
    /// </summary>
    static readonly string scriptsFolderName = "Scripts";

    /// <summary>
    /// Name for the undo steps folder
    /// </summary>
    static readonly string undoStepsFolder = "UndoSteps";

    /// <summary>
    /// Name for the redo steps folder
    /// </summary>
    static readonly string redoStepsFolder = "RedoSteps";

    /// <summary>
    /// The <see cref="UniqueNamer"/> for managing the names of the variables of the perceptions
    /// </summary>
    static UniqueNamer uniqueNamer;

    /// <summary>
    /// Maximum number of steps decided by the user in settings
    /// </summary>
    static int maxUndoSteps = 100;

    /// <summary>
    /// Number of XML files in the undo steps folder
    /// </summary>
    public static int undoStepsSaved
    {
        get
        {
            string path = "Assets/" + GUIpackageName + "/" + undoStepsFolder;
            if (AssetDatabase.IsValidFolder(path))
            {
                string[] files = Directory.GetFiles(path).Where(s => s.EndsWith(".xml")).ToArray();
                return files.Count();
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Number of XML files in the redo steps folder
    /// </summary>
    public static int redoStepsSaved
    {
        get
        {
            string path = "Assets/" + GUIpackageName + "/" + redoStepsFolder;
            if (AssetDatabase.IsValidFolder(path))
            {
                string[] files = Directory.GetFiles(path).Where(s => s.EndsWith(".xml")).ToArray();
                return files.Count();
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Generates a new C# script for an <paramref name="elem"/>
    /// </summary>
    /// <param name="elem"></param>
    public static void GenerateElemScript(ClickableElement elem)
    {
        uniqueNamer = ScriptableObject.CreateInstance<UniqueNamer>();

        string path = "Machine_Template.cs";

        string[] guids = AssetDatabase.FindAssets(path);
        if (guids.Length == 0)
        {
            Debug.LogWarning(path + ".txt not found in asset database");
            return;
        }
        string templatePath = AssetDatabase.GUIDToAssetPath(guids[0]);

        string basePath = "Assets/" + GUIpackageName;
        string fullPath = basePath + "/" + scriptsFolderName;

        // Create Asset
        if (!AssetDatabase.IsValidFolder(fullPath))
            AssetDatabase.CreateFolder(basePath, scriptsFolderName);

        string scriptPath = EditorUtility.SaveFilePanel("Select a folder for the script", fullPath, CleanName(elem.elementName) + ".cs", "CS");

        if (!string.IsNullOrEmpty(scriptPath))
        {
            UnityEngine.Object o = CreateScriptFromTemplate(scriptPath, templatePath, elem);
            AssetDatabase.Refresh();
            ProjectWindowUtil.ShowCreatedAsset(o);
        }
    }

    /// <summary>
    /// Generates a new XML file for an <paramref name="elem"/>
    /// </summary>
    /// <param name="elem"></param>
    public static void GenerateElemXMLFile(ClickableElement elem)
    {
        string basePath = "Assets/" + GUIpackageName;
        string fullPath = basePath + "/" + savesFolderName;

        // Create Asset
        if (!AssetDatabase.IsValidFolder(fullPath))
            AssetDatabase.CreateFolder(basePath, savesFolderName);

        string path = EditorUtility.SaveFilePanel("Select a folder for the save file", fullPath, CleanName(elem.elementName) + "_savedData.xml", "XML");

        if (!string.IsNullOrEmpty(path))
        {
            UnityEngine.Object o = CreateXMLFromElem(path, elem);
            AssetDatabase.Refresh();
            ProjectWindowUtil.ShowCreatedAsset(o);
        }
    }

    /// <summary>
    /// Generates a new XML file for an <paramref name="elem"/> adding it to the undo steps or redo steps, if you set fromRedo true
    /// </summary>
    /// <param name="elem"></param>
    public static void GenerateUndoStep(ClickableElement elem, bool clearRedo = true)
    {
        if (clearRedo)
            ClearRedoSteps();

        if (elem is null) // For now
            return;

        string basePath = "Assets/" + GUIpackageName;
        string path = basePath + "/" + undoStepsFolder;

        // Create Asset
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(basePath, undoStepsFolder);

        // Generate the file
        string[] files = Directory.GetFiles(path).Where(s => s.EndsWith(".xml")).ToArray();
        int numOfStepsSaved = files.Count();
        string fullPath = path + "/current_step" + numOfStepsSaved + ".xml";

        // If the max number of undo steps has been reached overwrite the oldest xml file
        if (numOfStepsSaved >= maxUndoSteps)
        {
            Dictionary<string, DateTime> dates = new Dictionary<string, DateTime>();

            foreach (string filePath in files)
            {
                dates.Add(filePath, Directory.GetCreationTime(filePath));
            }

            // Select the oldest one and get its path, so it gets overwriten
            var oldest = dates.OrderBy(x => x.Value).FirstOrDefault();
            fullPath = oldest.Key;
        }

        // Finally create the XML file for the step
        if (!string.IsNullOrEmpty(fullPath))
        {
            CreateXMLFromElem(fullPath, elem);
        }
    }

    /// <summary>
    /// Generates a new XML file for an <paramref name="elem"/> adding it to the redo steps
    /// </summary>
    /// <param name="elem"></param>
    public static void GenerateRedoStep(ClickableElement elem)
    {
        if (elem is null) // For now
            return;

        string basePath = "Assets/" + GUIpackageName;
        string path = basePath + "/" + redoStepsFolder;

        // Create Asset
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(basePath, redoStepsFolder);

        // Generate the file
        string[] files = Directory.GetFiles(path).Where(s => s.EndsWith(".xml")).ToArray();
        int numOfStepsSaved = files.Count();
        string fullPath = path + "/current_step" + numOfStepsSaved + ".xml";

        // If the max number of undo steps has been reached overwrite the oldest xml file
        if (numOfStepsSaved >= maxUndoSteps)
        {
            Dictionary<string, DateTime> dates = new Dictionary<string, DateTime>();

            foreach (string filePath in files)
            {
                dates.Add(filePath, Directory.GetCreationTime(filePath));
            }

            // Select the oldest one and get its path, so it gets overwriten
            var oldest = dates.OrderBy(x => x.Value).FirstOrDefault();
            fullPath = oldest.Key;
        }

        if (!string.IsNullOrEmpty(fullPath))
        {
            CreateXMLFromElem(fullPath, elem);
        }
    }

    /// <summary>
    /// Shows the File Panel, and returns the <see cref="XMLElement"/> corresponding to the XML file that the user selects
    /// </summary>
    /// <returns></returns>
    public static XMLElement LoadSavedData()
    {
        string path = EditorUtility.OpenFilePanel("Open a save file", "Assets/" + GUIpackageName + "/" + savesFolderName, "XML");

        if (string.IsNullOrEmpty(path))
            return null;

        return LoadXML(path);
    }

    /// <summary>
    /// Load a step from the undo steps folder
    /// </summary>
    /// <returns></returns>
    public static XMLElement LoadUndoStep()
    {
        // Get last step path
        string path;
        string[] files = Directory.GetFiles("Assets/" + GUIpackageName + "/" + undoStepsFolder).Where(s => s.EndsWith(".xml")).ToArray();

        Dictionary<string, DateTime> dates = new Dictionary<string, DateTime>();

        foreach (string filePath in files)
        {
            dates.Add(filePath, Directory.GetLastAccessTime(filePath));
        }

        // Select the oldest one and get its path, so it gets overwriten
        var last = dates.OrderByDescending(x => x.Value).FirstOrDefault();
        path = last.Key;

        //Load the XMLElement for that path
        if (string.IsNullOrEmpty(path))
            return null;

        path = path.Replace("\\", "/");

        XMLElement result = LoadXML(path);

        File.Delete(path);

        return result;
    }

    /// <summary>
    /// Load a step from the redo steps folder
    /// </summary>
    /// <returns></returns>
    public static XMLElement LoadRedoStep()
    {
        // Get last step path
        string path;
        string[] files = Directory.GetFiles("Assets/" + GUIpackageName + "/" + redoStepsFolder).Where(s => s.EndsWith(".xml")).ToArray();

        Dictionary<string, DateTime> dates = new Dictionary<string, DateTime>();

        foreach (string filePath in files)
        {
            dates.Add(filePath, Directory.GetLastAccessTime(filePath));
        }

        // Select the oldest one and get its path, so it gets overwriten
        var last = dates.OrderByDescending(x => x.Value).FirstOrDefault();
        path = last.Key;

        //Load the XMLElement for that path
        if (string.IsNullOrEmpty(path))
            return null;

        path = path.Replace("\\", "/");

        XMLElement result = LoadXML(path);

        File.Delete(path);

        return result;
    }

    /// <summary>
    /// Deletes the directory with the undo steps files
    /// </summary>
    public static void ClearUndoSteps()
    {
        ClearRedoSteps();

        string path = "Assets/" + GUIpackageName + "/" + undoStepsFolder;

        if (AssetDatabase.IsValidFolder(path))
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }

    /// <summary>
    /// Deletes the directory with the redo steps files
    /// </summary>
    public static void ClearRedoSteps()
    {
        string path = "Assets/" + GUIpackageName + "/" + redoStepsFolder;

        if (AssetDatabase.IsValidFolder(path))
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }

    /// <summary>
    /// Creates Script from <paramref name="templatePath"/>
    /// </summary>
    /// <param name="pathName"></param>
    /// <param name="templatePath"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static UnityEngine.Object CreateScriptFromTemplate(string pathName, string templatePath, object obj)
    {
        string folderPath = pathName.Substring(0, pathName.LastIndexOf("/") + 1);
        string scriptName = pathName.Substring(pathName.LastIndexOf("/") + 1).Replace(".cs", "");

        if (File.Exists(templatePath))
        {
            // Read procedures
            StreamReader reader = new StreamReader(templatePath);
            string templateText = reader.ReadToEnd();
            reader.Close();

            if (obj is ClickableElement)
            {
                ClickableElement elem = (ClickableElement)obj;

                // Replace the tags with the corresponding parts
                List<ClickableElement> subElems = new List<ClickableElement>();

                templateText = templateText.Replace("#SCRIPTNAME#", CleanName(scriptName));
                templateText = templateText.Replace("#MACHINENAME#", GetMachineName(elem));
                templateText = templateText.Replace("#ELEMNAME#", CleanName(elem.elementName) + GetEnding(elem));

                string createMethod = GetCreateMethod(ref templateText, elem, false, ref subElems, folderPath); // We do this to be able to alter templateText from
                templateText = templateText.Replace("#CREATE#", createMethod);                              // within GetCreateMethod without overriding it at Replace

                templateText = GetAllSubElemsRecursive(templateText, ref subElems, folderPath);
                templateText = templateText.Replace("#SUBELEM_CREATE#", string.Empty);
                templateText = templateText.Replace("#VAR_DECL#", string.Empty);
                templateText = templateText.Replace("#MACHINE_INIT#", string.Empty);

                templateText = templateText.Replace("#ACTIONS#", GetActionMethods(elem));

                // SubFSM
                templateText = templateText.Replace("#SUBELEM_DECL#", GetSubElemDecl(subElems));
                templateText = templateText.Replace("#SUBELEM_INIT#", GetSubElemInit(subElems));
                templateText = templateText.Replace("#SUBELEM_UPDATE#", GetSubElemUpdate(subElems));
            }
            else if (obj is string)
            {
                string elemName = obj.ToString();

                templateText = templateText.Replace("#CUSTOMNAME#", elemName);
            }

            /// You can replace as many tags you make on your templates, just repeat Replace function
            /// e.g.:
            /// templateText = templateText.Replace("#NEWTAG#", "MyText");

            // Write procedures
            UTF8Encoding encoding = new UTF8Encoding(true, false);

            StreamWriter writer = new StreamWriter(Path.GetFullPath(pathName), false, encoding);
            writer.Write(templateText);
            writer.Close();

            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }
        else
        {
            Debug.LogError(string.Format("The template file was not found: {0}", templatePath));
            return null;
        }
    }

    /// <summary>
    /// Creates XML object from <paramref name="elem"/>, serializes it to a file and saves it in <paramref name="pathName"/>
    /// </summary>
    /// <param name="pathName"></param>
    /// <param name="elem"></param>
    /// <returns></returns>
    private static UnityEngine.Object CreateXMLFromElem(string pathName, ClickableElement elem)
    {
        var data = elem.ToXMLElement();

        // Serialize to XML
        using (var stream = new FileStream(pathName, FileMode.Create))
        {
            XmlSerializer serial = new XmlSerializer(typeof(XMLElement));
            serial.Serialize(stream, data);
        }

        return null;
    }

    /// <summary>
    /// Loads an XML file and converts it to <see cref="XMLElement"/>
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static XMLElement LoadXML(string fileName)
    {
        XmlSerializer serial = new XmlSerializer(typeof(XMLElement));

        serial.UnknownNode += new XmlNodeEventHandler(UnknownNode);
        serial.UnknownAttribute += new XmlAttributeEventHandler(UnknownAttribute);

        FileStream fs = new FileStream(fileName, FileMode.Open);

        XMLElement result = (XMLElement)serial.Deserialize(fs);
        fs.Close();

        return result;
    }

    /// <summary>
    /// Event that is called when an node is unknown in the serialization
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void UnknownNode(object sender, XmlNodeEventArgs e)
    {
        Debug.LogError("[XMLSerializer] Unknown Node:" + e.Name + "\t" + e.Text);
    }

    /// <summary>
    /// Event that is called when an attribute is unknown in the serialization
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void UnknownAttribute(object sender, XmlAttributeEventArgs e)
    {
        System.Xml.XmlAttribute attr = e.Attr;
        Debug.LogError("[XMLSerializer] Unknown attribute " +
        attr.Name + "='" + attr.Value + "'");
    }

    /// <summary>
    /// Modifies the <paramref name="name"/> to be usable in code
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string CleanName(string name)
    {
        string result;
        var numberChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var spacesAndNewlines = new[] { ' ', '\n' };
        var keywords = new[]
        {
            "bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "double", "float", "decimal",
            "string", "char", "void", "object", "typeof", "sizeof", "null", "true", "false", "if", "else", "while", "for", "foreach", "do", "switch",
            "case", "default", "lock", "try", "throw", "catch", "finally", "goto", "break", "continue", "return", "public", "private", "internal",
            "protected", "static", "readonly", "sealed", "const", "fixed", "stackalloc", "volatile", "new", "override", "abstract", "virtual",
            "event", "extern", "ref", "out", "in", "is", "as", "params", "__arglist", "__makeref", "__reftype", "__refvalue", "this", "base",
            "namespace", "using", "class", "struct", "interface", "enum", "delegate", "checked", "unchecked", "unsafe", "operator", "implicit", "explicit"
        };

        result = name.Trim(spacesAndNewlines);
        result = string.Concat(result.Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c) && !char.IsSymbol(c)));
        result = result.TrimStart(numberChars);

        // If result is a keyword, we add @ before it which makes it a valid identifier
        if (keywords.Contains(result))
            result = "@" + result;

        return result;
    }

    /// <summary>
    /// Returns the machine name
    /// </summary>
    /// <param name="elem"></param>
    /// <returns></returns>
    private static string GetMachineName(ClickableElement elem)
    {
        string machineName = "";

        // Fill the template with the content
        switch (elem.GetType().ToString())
        {
            case nameof(FSM):
                machineName = FSMCreateName;
                break;
            case nameof(BehaviourTree):
                machineName = BTCreateName;
                break;
            case nameof(UtilitySystem):
                machineName = USCreateName;
                break;
        }

        return machineName;
    }

    /// <summary>
    /// Returns the ending of the machine
    /// </summary>
    /// <param name="elem"></param>
    /// <returns></returns>
    private static string GetEnding(ClickableElement elem)
    {
        string machineName = "";

        // Fill the template with the content
        switch (elem.GetType().ToString())
        {
            case nameof(FSM):
                machineName = FSMEnding;
                break;
            case nameof(BehaviourTree):
                machineName = BTEnding;
                break;
            case nameof(UtilitySystem):
                machineName = USEnding;
                break;
        }

        return machineName;
    }

    /// <summary>
    /// Writes the Create method for the <paramref name="elem"/>
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="isSub"></param>
    /// <param name="subElems"></param>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    private static string GetCreateMethod(ref string templateText, ClickableElement elem, bool isSub, ref List<ClickableElement> subElems, string folderPath = null)
    {
        // Set the strings to what they need to be
        string className = CleanName(elem.elementName);
        string engineEnding = "";
        string createName = "";
        string templateSub = "";

        switch (elem.GetType().ToString())
        {
            case nameof(FSM):
                engineEnding = isSub ? subFSMEnding : FSMEnding;
                createName = FSMCreateName;
                templateSub = FSMCreateTemplate;
                break;
            case nameof(BehaviourTree):
                engineEnding = isSub ? subBTEnding : BTEnding;
                createName = BTCreateName;
                templateSub = BTCreateTemplate;
                break;
            case nameof(UtilitySystem):
                engineEnding = isSub ? subUSEnding : USEnding;
                createName = USCreateName;
                templateSub = USCreateTemplate;
                break;
        }

        string machineName = className + engineEnding;
        string createdName = isSub ? machineName : createName;
        string isSubStr = isSub ? "true" : "false";

        // Adjust the template names
        templateSub = templateSub.Replace("#CREATENAME#", createdName);

        templateText = templateText.Replace("#MACHINE_INIT#", machineName + " = new " + createName + "Engine(" + isSubStr + ");\n" + tab + tab + "#MACHINE_INIT#");

        if (isSub)
            templateSub += "#SUBELEM_CREATE#";

        // Fill the template with the content
        switch (elem.GetType().ToString())
        {
            case nameof(FSM):
                templateSub = templateSub.Replace("#PERCEPTIONS#", GetPerceptions(ref templateText, elem, engineEnding, folderPath));
                templateSub = templateSub.Replace("#STATES#", GetStates(ref templateText, elem, engineEnding, ref subElems));
                templateSub = templateSub.Replace("#TRANSITIONS#", GetTransitions(elem, engineEnding));
                break;
            case nameof(BehaviourTree):
                templateSub = templateSub.Replace("#NODES#", GetNodes(ref templateText, elem, engineEnding, ref subElems));
                templateSub = templateSub.Replace("#CHILDS#", GetChilds(elem, ref subElems));
                templateSub = templateSub.Replace("#SETROOT#", GetSetRoot(elem, engineEnding));
                break;
            case nameof(UtilitySystem):
                templateSub = templateSub.Replace("#FACTORS#", GetFactors(ref templateText, elem));
                templateSub = templateSub.Replace("#ACTIONS#", GetActions(ref templateText, elem, engineEnding, ref subElems));
                break;
        }

        templateSub = templateSub.Replace("#EXIT_PERCEPTIONS#", GetExitPerceptions(ref templateText, elem, engineEnding, folderPath));
        templateSub = templateSub.Replace("#EXITS#", GetExitTransitions(elem, engineEnding));

        return templateSub;
    }

    /// <summary>
    /// Writes the Create Method for all <paramref name="subElems"/>
    /// </summary>
    /// <param name="templateText"></param>
    /// <param name="subElems"></param>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    private static string GetAllSubElemsRecursive(string templateText, ref List<ClickableElement> subElems, string folderPath = null)
    {
        List<ClickableElement> subElemsCopy = new List<ClickableElement>();
        foreach (ClickableElement sub in subElems)
        {
            string createMethod = GetCreateMethod(ref templateText, sub, true, ref subElemsCopy, folderPath);
            templateText = templateText.Replace("#SUBELEM_CREATE#", createMethod);
        }
        if (subElemsCopy.Count > 0)
        {
            templateText = GetAllSubElemsRecursive(templateText, ref subElemsCopy, folderPath);
            subElems.AddRange(subElemsCopy);
        }

        return templateText;
    }

    /// <summary>
    /// Writes the declaration of all <paramref name="subElems"/>
    /// </summary>
    /// <param name="subElems"></param>
    /// <returns></returns>
    private static string GetSubElemDecl(List<ClickableElement> subElems)
    {
        string result = string.Empty;

        foreach (ClickableElement sub in subElems)
        {
            string engineEnding = "";
            string type = "";

            switch (sub.GetType().ToString())
            {
                case nameof(FSM):
                    engineEnding = subFSMEnding;
                    type = "StateMachineEngine";
                    break;
                case nameof(BehaviourTree):
                    engineEnding = subBTEnding;
                    type = "BehaviourTreeEngine";
                    break;
                case nameof(UtilitySystem):
                    engineEnding = subUSEnding;
                    type = "UtilitySystemEngine";
                    break;
            }

            string elemName = CleanName(sub.elementName);
            result += "private " + type + " " + elemName + engineEnding + ";\n" + tab;
        }

        return result;
    }

    /// <summary>
    /// Writes the initialization of all <paramref name="subElems"/>
    /// </summary>
    /// <param name="subElems"></param>
    /// <returns></returns>
    private static string GetSubElemInit(List<ClickableElement> subElems)
    {

        string result = string.Empty;

        for (int i = subElems.Count - 1; i >= 0; i--)
        {
            string engineEnding = "";

            switch (subElems[i].GetType().ToString())
            {
                case nameof(FSM):
                    engineEnding = subFSMEnding;
                    break;
                case nameof(BehaviourTree):
                    engineEnding = subBTEnding;
                    break;
                case nameof(UtilitySystem):
                    engineEnding = subUSEnding;
                    break;
            }

            string elemName = CleanName(subElems[i].elementName) + engineEnding;

            result += "Create" + elemName + "();\n" + tab + tab;
        }

        return result;
    }

    /// <summary>
    /// Writes the Update call of all <paramref name="subElems"/>
    /// </summary>
    /// <param name="subElems"></param>
    /// <returns></returns>
    private static string GetSubElemUpdate(List<ClickableElement> subElems)
    {
        string result = string.Empty;

        foreach (ClickableElement sub in subElems)
        {
            string engineEnding = "";
            string elemName = CleanName(sub.elementName);

            switch (sub.GetType().ToString())
            {
                case nameof(FSM):
                    engineEnding = subFSMEnding;
                    break;
                case nameof(BehaviourTree):
                    engineEnding = subBTEnding;
                    break;
                case nameof(UtilitySystem):
                    engineEnding = subUSEnding;
                    break;
            }

            result += "\n" + tab + tab + elemName + engineEnding + ".Update();";
        }

        return result;
    }

    /// <summary>
    /// Writes all the CreateExitTransition calls if necessary
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <returns></returns>
    private static string GetExitTransitions(ClickableElement elem, string engineEnding)
    {
        string result = string.Empty;
        string machineName = CleanName(elem.elementName) + engineEnding;

        foreach (BaseNode node in elem.nodes.Where(n => n.subElem))
        {
            string className = CleanName(node.nodeName);
            string nodeMachineName = className + "_UnknownMachine";

            TransitionGUI exitTransition = node.subElem.GetExitTransition();
            string exitNode = "null";
            string perceptionName = "null";

            if (exitTransition)
            {
                exitNode = CleanName(exitTransition.fromNode.nodeName);

                string typeName;

                if (exitTransition.rootPerception.type == perceptionType.Custom)
                    typeName = CleanName(exitTransition.rootPerception.customName);
                else
                    typeName = exitTransition.rootPerception.type.ToString();

                perceptionName = uniqueNamer.AddName(exitTransition.rootPerception.identificator, typeName + "Perception");
            }

            switch (elem.GetType().ToString())
            {
                // Parent machine is FSM
                case nameof(FSM):
                    switch (node.subElem.GetType().ToString())
                    {
                        case nameof(FSM):
                            nodeMachineName = className + subFSMEnding;
                            break;
                        case nameof(BehaviourTree):
                            nodeMachineName = className + subBTEnding;
                            break;
                        case nameof(UtilitySystem):
                            nodeMachineName = className + subUSEnding;
                            break;
                    }

                    List<TransitionGUI> exitTransitionsList = elem.transitions.Where(t => !t.isExit && t.fromNode.Equals(node)).ToList();

                    foreach (TransitionGUI transition in exitTransitionsList)
                        result += nodeMachineName + ".CreateExitTransition(\"" + nodeMachineName + "Exit" + "\", " + className + ", " + uniqueNamer.GetName(transition.rootPerception.identificator) + ", " + CleanName(transition.toNode.nodeName) + ");\n" + tab + tab;
                    break;

                // Parent machine is BehaviourTree
                case nameof(BehaviourTree):
                    switch (node.subElem.GetType().ToString())
                    {
                        case nameof(FSM):
                            nodeMachineName = className + subFSMEnding;
                            result += nodeMachineName + ".CreateExitTransition(\"" + nodeMachineName + " Exit" + "\", " + exitNode + ", " + perceptionName + ", ReturnValues.Succeed);\n" + tab + tab;
                            break;
                        case nameof(BehaviourTree):
                            nodeMachineName = className + subBTEnding;
                            result += nodeMachineName + ".CreateExitTransition(\"" + nodeMachineName + " Exit" + "\");\n" + tab + tab;
                            break;
                        case nameof(UtilitySystem):
                            // Not contemplated
                            break;
                    }
                    break;

                // Parent machine is UtilitySystem
                case nameof(UtilitySystem):
                    switch (node.subElem.GetType().ToString())
                    {
                        case nameof(FSM):
                            nodeMachineName = className + subFSMEnding;
                            break;
                        case nameof(BehaviourTree):
                            nodeMachineName = className + subBTEnding;
                            exitNode += ".StateNode";
                            break;
                        case nameof(UtilitySystem):
                            nodeMachineName = className + subUSEnding;
                            exitNode += ".utilityState";
                            break;
                    }

                    result += nodeMachineName + ".CreateExitTransition(\"" + nodeMachineName + " Exit" + "\", " + exitNode + ", " + perceptionName + ", " + machineName + ");\n" + tab + tab;
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// Writes all the exit <see cref="Perception"/> declaration and initialization
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    private static string GetExitPerceptions(ref string templateText, ClickableElement elem, string engineEnding, string folderPath = null)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;
        string machineName = className + engineEnding;

        foreach (TransitionGUI transition in elem.transitions.Where(t => t.isExit))
        {
            string transitionName = CleanName(transition.transitionName);

            result += RecursivePerceptions(ref templateText, transition.rootPerception, transitionName, machineName, folderPath);
        }

        return result;
    }

    #region FSM

    /// <summary>
    /// Writes all the <see cref="Perception"/> declaration and initialization
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    private static string GetPerceptions(ref string templateText, ClickableElement elem, string engineEnding, string folderPath = null)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;
        string machineName = className + engineEnding;

        foreach (TransitionGUI transition in elem.transitions.Where(t => !t.isExit))
        {
            string transitionName = CleanName(transition.transitionName);

            result += RecursivePerceptions(ref templateText, transition.rootPerception, machineName, folderPath, transitionName);
        }

        return result;
    }

    /// <summary>
    /// Recursive method for <see cref="GetPerceptions(ClickableElement, string, string)"/>
    /// </summary>
    /// <param name="perception"></param>
    /// <param name="transitionName"></param>
    /// <param name="machineName"></param>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    private static string RecursivePerceptions(ref string templateText, PerceptionGUI perception, string machineName, string folderPath, string transitionName = null)
    {
        string res = "";
        string auxAndOr = "";

        if (perception.type == perceptionType.And || perception.type == perceptionType.Or)
        {
            auxAndOr = perception.type.ToString();
            res += RecursivePerceptions(ref templateText, perception.firstChild, machineName, folderPath);
            res += RecursivePerceptions(ref templateText, perception.secondChild, machineName, folderPath);
        }

        string typeName;
        if (perception.type == perceptionType.Custom)
        {
            typeName = CleanName(perception.customName);

            string scriptName = typeName + "Perception.cs";

            // Generate the script for the custom perception if it doesn't exist already

            string[] assets = AssetDatabase.FindAssets(scriptName);
            if (assets.Length == 0)
            {
                string path = "CustomPerception_Template.cs";

                string[] guids = AssetDatabase.FindAssets(path);
                if (guids.Length == 0)
                {
                    Debug.LogWarning(path + ".txt not found in asset database");
                }
                else
                {
                    string templatePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    UnityEngine.Object o = CreateScriptFromTemplate(folderPath + scriptName, templatePath, typeName);
                }
            }
        }
        else
        {
            typeName = perception.type.ToString();
        }

        string perceptionName;

        if (string.IsNullOrEmpty(transitionName))
        {
            perceptionName = typeName + "Perception";
        }
        else
        {
            perceptionName = transitionName + "Perception";
        }

        perceptionName = uniqueNamer.AddName(perception.identificator, perceptionName);

        templateText = templateText.Replace("#VAR_DECL#", "private " + typeName + "Perception " + perceptionName + ";\n" + tab + "#VAR_DECL#");
        res += perceptionName + " = " + machineName + ".Create" + auxAndOr + "Perception<" + typeName + "Perception" + ">(" + GetPerceptionParameters(perception) + ");\n" + tab + tab;

        return res;
    }

    /// <summary>
    /// Returns the corresponding parameters for the <paramref name="perception"/>
    /// </summary>
    /// <param name="perception"></param>
    /// <returns></returns>
    private static string GetPerceptionParameters(PerceptionGUI perception)
    {
        string result = "";

        switch (perception.type)
        {
            case perceptionType.Value:
                result = "() => false /*Replace this with a boolean function*/";
                break;
            case perceptionType.Timer:
                float timerInSeconds;
                if (perception.timerInSeconds)
                    timerInSeconds = perception.timerNumber;
                else
                    timerInSeconds = perception.timerNumber * 0.001f;

                result = timerInSeconds.ToString(CultureInfo.CreateSpecificCulture("en-US")) + "f";
                break;
            case perceptionType.IsInState:
                result = CleanName(perception.elemName) + subFSMEnding + ", " + "\"" + perception.stateName + "\"";
                break;
            case perceptionType.BehaviourTreeStatus:
                result = CleanName(perception.elemName) + subBTEnding + ", " + "ReturnValues." + perception.status.ToString();
                break;
            case perceptionType.And:
            case perceptionType.Or:
                result = uniqueNamer.GetName(perception.firstChild.identificator) + ", " + uniqueNamer.GetName(perception.secondChild.identificator);
                break;
            case perceptionType.Custom:
                result = "new " + CleanName(perception.customName) + "Perception()";
                break;
        }

        return result;
    }

    /// <summary>
    /// Writes all the <see cref="State"/> declaration and initialization
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <param name="subElems"></param>
    /// <returns></returns>
    private static string GetStates(ref string templateText, ClickableElement elem, string engineEnding, ref List<ClickableElement> subElems)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;
        string machineName = className + engineEnding;

        foreach (StateNode node in elem.nodes)
        {
            string nodeName = CleanName(node.nodeName);

            templateText = templateText.Replace("#VAR_DECL#", "private State " + nodeName + ";\n" + tab + "#VAR_DECL#");

            if (node.subElem is null)
            {
                if (node.type == stateType.Entry)
                {
                    result += nodeName + " = " + machineName + ".CreateEntryState(\"" + node.nodeName + "\", " + nodeName + actionsEnding + ");\n" + tab + tab;
                }
                else
                {
                    result += nodeName + " = " + machineName + ".CreateState(\"" + node.nodeName + "\", " + nodeName + actionsEnding + ");\n" + tab + tab;
                }
            }
            else
            {
                string subEnding = "";

                switch (node.subElem.GetType().ToString())
                {
                    case nameof(FSM):
                        subEnding = subFSMEnding;
                        break;
                    case nameof(BehaviourTree):
                        subEnding = subBTEnding;
                        break;
                    case nameof(UtilitySystem):
                        subEnding = subUSEnding;
                        break;
                }

                result += nodeName + " = " + machineName + ".CreateSubStateMachine(\"" + node.nodeName + "\", " + nodeName + subEnding + ");\n" + tab + tab;
                subElems.Add(node.subElem);
            }
        }

        return result;
    }

    /// <summary>
    /// Writes all the <see cref="Transition"/> declaration and initialization
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <returns></returns>
    private static string GetTransitions(ClickableElement elem, string engineEnding)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;
        string machineName = className + engineEnding;

        foreach (TransitionGUI transition in elem.transitions.Where(t => !t.isExit))
        {
            string transitionName = CleanName(transition.transitionName);
            string fromNodeName = CleanName(transition.fromNode.nodeName);
            string toNodeName = CleanName(transition.toNode.nodeName);

            string typeName = "";

            if (transition.rootPerception.type == perceptionType.Custom)
                typeName = transition.rootPerception.customName;
            else
                typeName = transition.rootPerception.type.ToString();

            if (((StateNode)transition.fromNode).subElem == null)
                result += "\n" + tab + tab + machineName + ".CreateTransition(\"" + transition.transitionName + "\", " + fromNodeName + ", " + uniqueNamer.GetName(transition.rootPerception.identificator) + ", " + toNodeName + ");";
        }

        return result;
    }

    #endregion

    #region Behaviour Tree

    /// <summary>
    /// Writes all the <see cref="TreeNode"/> declaration and initialization, except for Decorators
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <param name="subElems"></param>
    /// <returns></returns>
    private static string GetNodes(ref string templateText, ClickableElement elem, string engineEnding, ref List<ClickableElement> subElems)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;
        string machineName = className + engineEnding;

        foreach (BehaviourNode node in elem.nodes.FindAll(n => ((BehaviourNode)n).type <= behaviourType.Leaf))
        {
            string nodeName = CleanName(node.nodeName);

            switch (node.type)
            {
                case behaviourType.Selector:
                    templateText = templateText.Replace("#VAR_DECL#", "private SelectorNode " + nodeName + ";\n" + tab + "#VAR_DECL#");
                    result += nodeName + " = " + machineName + ".CreateSelectorNode(\"" + node.nodeName + "\");\n" + tab + tab;
                    break;
                case behaviourType.Sequence:
                    templateText = templateText.Replace("#VAR_DECL#", "private SequenceNode " + nodeName + ";\n" + tab + "#VAR_DECL#");
                    result += nodeName + " = " + machineName + ".CreateSequenceNode(\"" + node.nodeName + "\", " + (node.isRandom ? "true" : "false") + ");\n" + tab + tab;
                    break;
                case behaviourType.Leaf:
                    templateText = templateText.Replace("#VAR_DECL#", "private LeafNode " + nodeName + ";\n" + tab + "#VAR_DECL#");

                    if (node.subElem is null)
                    {
                        result += nodeName + " = " + machineName + ".CreateLeafNode(\"" + node.nodeName + "\", " + nodeName + actionsEnding + ", " + nodeName + conditionsEnding + ");\n" + tab + tab;
                    }
                    else
                    {
                        string subEnding = "";

                        switch (node.subElem.GetType().ToString())
                        {
                            case nameof(FSM):
                                subEnding = subFSMEnding;
                                break;
                            case nameof(BehaviourTree):
                                subEnding = subBTEnding;
                                break;
                            case nameof(UtilitySystem):
                                subEnding = subUSEnding;
                                break;
                        }

                        result += nodeName + " = " + machineName + ".CreateSubBehaviour(\"" + node.nodeName + "\", " + nodeName + subEnding + ");\n" + tab + tab;
                        subElems.Add(node.subElem);
                    }
                    break;
            }
        }

        // Decorator nodes
        // We check every node from the root so it is written in order in the generated code

        foreach (BehaviourNode node in elem.nodes.FindAll(o => ((BehaviourNode)o).isRoot))
        {
            RecursiveDecorators(ref templateText, ref result, machineName, elem, node);
        }

        return result;
    }

    /// <summary>
    /// Checks if <paramref name="node"/> is a decorator and if it is, it writes its declaration and initialization
    /// </summary>
    /// <param name="result"></param>
    /// <param name="machineName"></param>
    /// <param name="elem"></param>
    /// <param name="node"></param>
    private static void RecursiveDecorators(ref string templateText, ref string result, string machineName, ClickableElement elem, BehaviourNode node)
    {
        foreach (BehaviourNode childNode in elem.transitions.FindAll(o => !o.isExit && o.fromNode.Equals(node)).Select(o => o.toNode))
        {
            RecursiveDecorators(ref templateText, ref result, machineName, elem, childNode);
        }

        if (node.type > behaviourType.Leaf)
        {
            string nodeName = GetDecoratorName(elem, node);
            string subNodeName;

            switch (node.type)
            {
                case behaviourType.LoopN:
                    templateText = templateText.Replace("#VAR_DECL#", "private LoopDecoratorNode " + nodeName + ";\n" + tab + "#VAR_DECL#");

                    subNodeName = nodeName.Remove(0, "LoopN_".ToCharArray().Count());
                    result += nodeName + " = " + machineName + ".CreateLoopNode(\"" + nodeName + "\", " + subNodeName + (node.isInfinite ? "" : ", " + node.Nloops) + ");\n" + tab + tab;
                    break;
                case behaviourType.LoopUntilFail:
                    templateText = templateText.Replace("#VAR_DECL#", "private LoopUntilFailDecoratorNode " + nodeName + ";\n" + tab + "#VAR_DECL#");

                    subNodeName = nodeName.Remove(0, "LoopUntilFail_".ToCharArray().Count());
                    result += nodeName + " = " + machineName + ".CreateLoopUntilFailNode(\"" + nodeName + "\", " + subNodeName + ");\n" + tab + tab;
                    break;
                case behaviourType.Inverter:
                    templateText = templateText.Replace("#VAR_DECL#", "private InverterDecoratorNode " + nodeName + ";\n" + tab + "#VAR_DECL#");

                    subNodeName = nodeName.Remove(0, "Inverter_".ToCharArray().Count());
                    result += nodeName + " = " + machineName + ".CreateInverterNode(\"" + nodeName + "\", " + subNodeName + ");\n" + tab + tab;
                    break;
                case behaviourType.DelayT:
                    templateText = templateText.Replace("#VAR_DECL#", "private TimerDecoratorNode " + nodeName + ";\n" + tab + "#VAR_DECL#");

                    subNodeName = nodeName.Remove(0, "Timer_".ToCharArray().Count());
                    result += nodeName + " = " + machineName + ".CreateTimerNode(\"" + nodeName + "\", " + subNodeName + ", " + node.delayTime.ToString(CultureInfo.CreateSpecificCulture("en-US")) + "f);\n" + tab + tab;
                    break;
                case behaviourType.Succeeder:
                    templateText = templateText.Replace("#VAR_DECL#", "private SucceederDecoratorNode " + nodeName + ";\n" + tab + "#VAR_DECL#");

                    subNodeName = nodeName.Remove(0, "Succeeder_".ToCharArray().Count());
                    result += nodeName + " = " + machineName + ".CreateSucceederNode(\"" + nodeName + "\", " + subNodeName + ");\n" + tab + tab;
                    break;
                case behaviourType.Conditional:
                    templateText = templateText.Replace("#VAR_DECL#", "private ConditionalDecoratorNode " + nodeName + ";\n" + tab + "#VAR_DECL#");

                    subNodeName = nodeName.Remove(0, "Conditional_".ToCharArray().Count());
                    result += nodeName + " = " + machineName + ".CreateConditionalNode(\"" + nodeName + "\", " + subNodeName + ", null /*Change this for a perception*/);\n" + tab + tab;
                    break;
            }
        }
    }

    /// <summary>
    /// Returns the name of the Decorator <paramref name="node"/>
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    private static string GetDecoratorName(ClickableElement elem, BehaviourNode node)
    {
        if (node.type <= behaviourType.Leaf)
        {
            return CleanName(node.nodeName);
        }

        TransitionGUI decoratorConnection = elem.transitions.Where(t => !t.isExit && node.Equals(t.fromNode)).FirstOrDefault();

        string subNodeName = decoratorConnection ? GetDecoratorName(elem, (BehaviourNode)decoratorConnection.toNode) : "null";

        switch (node.type)
        {
            case behaviourType.LoopN:
                return "LoopN_" + subNodeName;
            case behaviourType.LoopUntilFail:
                return "LoopUntilFail_" + subNodeName;
            case behaviourType.Inverter:
                return "Inverter_" + subNodeName;
            case behaviourType.DelayT:
                return "Timer_" + subNodeName;
            case behaviourType.Succeeder:
                return "Succeeder_" + subNodeName;
            case behaviourType.Conditional:
                return "Conditional_" + subNodeName;
            default:
                return "Unknown_" + subNodeName;
        }
    }

    /// <summary>
    /// Writes all the AddChild methods that are necessary
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="subElems"></param>
    /// <returns></returns>
    private static string GetChilds(ClickableElement elem, ref List<ClickableElement> subElems)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;

        foreach (BehaviourNode node in elem.nodes.Where(n => ((BehaviourNode)n).type < behaviourType.Leaf && ((BehaviourTree)elem).ChildrenCount((BehaviourNode)n, true) > 0))
        {
            string baseNodeName = CleanName(node.nodeName);
            result += "\n" + tab + tab;
            foreach (BehaviourNode toNode in elem.transitions.FindAll(t => !t.isExit && node.Equals(t.fromNode)).Select(o => o.toNode).OrderBy(n => ((BehaviourNode)n).index))
            {
                string toNodeName = CleanName(toNode.nodeName);
                TransitionGUI decoratorConnection = elem.transitions.Where(t => !t.isExit && toNode.Equals(t.fromNode)).FirstOrDefault();

                string nodeName = "null";

                if (decoratorConnection?.toNode)
                {
                    TransitionGUI decoratorConnectionsub = elem.transitions.Where(t => !t.isExit && decoratorConnection.toNode.Equals(t.fromNode)).FirstOrDefault();
                    string subNodeName = "";

                    if (decoratorConnectionsub)
                    {
                        subNodeName = CleanName(decoratorConnectionsub.toNode.nodeName);
                    }
                    else
                    {
                        subNodeName = "null";
                    }

                    switch (((BehaviourNode)decoratorConnection.toNode).type)
                    {
                        case behaviourType.LoopN:
                            nodeName = "LoopN_" + subNodeName;
                            break;
                        case behaviourType.LoopUntilFail:
                            nodeName = "LoopUntilFail_" + subNodeName;
                            break;
                        case behaviourType.Inverter:
                            nodeName = "Inverter_" + subNodeName;
                            break;
                        case behaviourType.DelayT:
                            nodeName = "Timer_" + subNodeName;
                            break;
                        case behaviourType.Succeeder:
                            nodeName = "Succeeder_" + subNodeName;
                            break;
                        case behaviourType.Conditional:
                            nodeName = "Conditional_" + subNodeName;
                            break;
                        default:
                            nodeName = CleanName(decoratorConnection.toNode.nodeName);
                            break;
                    }
                }

                switch (toNode.type)
                {
                    case behaviourType.LoopN:
                        toNodeName = "LoopN_" + nodeName;
                        break;
                    case behaviourType.LoopUntilFail:
                        toNodeName = "LoopUntilFail_" + nodeName;
                        break;
                    case behaviourType.Inverter:
                        toNodeName = "Inverter_" + nodeName;
                        break;
                    case behaviourType.DelayT:
                        toNodeName = "Timer_" + nodeName;
                        break;
                    case behaviourType.Succeeder:
                        toNodeName = "Succeeder_" + nodeName;
                        break;
                    case behaviourType.Conditional:
                        toNodeName = "Conditional_" + nodeName;
                        break;
                }

                result += baseNodeName + ".AddChild(" + toNodeName + ");\n" + tab + tab;
            }
        }

        if (string.IsNullOrEmpty(result))
            return result;

        return "// Child adding" + result;
    }

    /// <summary>
    /// Writes all the SetRootNode methods that are necessary
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <returns></returns>
    private static string GetSetRoot(ClickableElement elem, string engineEnding)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;
        string machineName = className + engineEnding;

        foreach (BehaviourNode node in elem.nodes)
        {
            if (node.isRoot)
            {
                string rootName;

                if (node.type > behaviourType.Leaf)
                {
                    rootName = GetDecoratorName(elem, node);
                }
                else
                {
                    rootName = CleanName(node.nodeName);
                }

                result += machineName + ".SetRootNode(" + rootName + ");";
            }
        }

        return result;
    }

    #endregion

    #region Utility System

    /// <summary>
    /// Writes all the declaration and initialization from a list of type <typeparamref name="TypeInput"/>. The content of .Add() at the initialization, can be defined with <paramref name="func"/>
    /// </summary>
    /// <typeparam name="TypeInput">Type of the <paramref name="preexistingList"/> elements</typeparam>
    /// <typeparam name="TypeResult">Type of the resulting list</typeparam>
    /// <param name="listName"></param>
    /// <param name="preexistingList"></param>
    /// <param name="func">The string of code that will be written inside the Add() for each element like this: "resultList.Add(" + func + ")"</param>
    /// <returns></returns>
    private static string WriteList<TypeInput, TypeResult>(string listName, List<TypeInput> preexistingList, Func<TypeInput, string> func)
    {
        string result = string.Empty;

        result += "List<" + typeof(TypeResult) + "> " + listName + " = new List<" + typeof(TypeResult) + ">\n" + tab + tab;
        result += "{\n" + tab + tab;
        foreach (TypeInput elem in preexistingList)
        {
            result += tab + func(elem) + ",\n" + tab + tab;
        }
        result += "};\n" + tab + tab;

        result += "\n" + tab + tab;

        return result;
    }

    /// <summary>
    /// Writes all the Factor nodes declaration and initialization
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <returns></returns>
    private static string GetFactors(ref string templateText, ClickableElement elem)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;

        foreach (UtilityNode node in elem.nodes)
        {
            string factorName = CleanName(node.nodeName);

            switch (node.type)
            {
                case utilityType.Variable:
                    templateText = templateText.Replace("#VAR_DECL#", "private Factor " + factorName + ";\n" + tab + "#VAR_DECL#");

                    result += factorName + " = " + "new LeafVariable(() => /*Reference to desired variable*/0.0f, " + node.variableMax + ", " + node.variableMin + ");\n" + tab + tab;
                    break;
                case utilityType.Fusion:
                    templateText = templateText.Replace("#VAR_DECL#", "private Factor " + factorName + ";\n" + tab + "#VAR_DECL#");

                    string factorsListName = factorName + "Factors";
                    string weightsListName = factorName + "Weights";

                    switch (node.fusionType)
                    {
                        case fusionType.Weighted:
                            result += WriteList<UtilityNode, Factor>(factorsListName, node.GetWeightsAndFactors().Select(k => k.Value).ToList(), (UtilityNode nodeElem) => CleanName(nodeElem.nodeName));
                            result += WriteList<float, float>(weightsListName, node.GetWeightsAndFactors().Select(k => k.Key).ToList(), (float weightElem) => weightElem.ToString(CultureInfo.CreateSpecificCulture("en-US")) + "f");
                            result += factorName + " = " + "new WeightedSumFusion(" + factorsListName + ", " + weightsListName + ");\n" + tab + tab;
                            break;
                        case fusionType.GetMax:
                            result += WriteList<UtilityNode, Factor>(factorsListName, node.GetWeightsAndFactors().Select(k => k.Value).ToList(), (UtilityNode nodeElem) => CleanName(nodeElem.nodeName));
                            result += factorName + " = " + "new MaxFusion(" + factorsListName + ");\n" + tab + tab;
                            break;
                        case fusionType.GetMin:
                            result += WriteList<UtilityNode, Factor>(factorsListName, node.GetWeightsAndFactors().Select(k => k.Value).ToList(), (UtilityNode nodeElem) => CleanName(nodeElem.nodeName));
                            result += factorName + " = " + "new MinFusion(" + factorsListName + ");\n" + tab + tab;
                            break;
                    }
                    break;
                case utilityType.Curve:
                    templateText = templateText.Replace("#VAR_DECL#", "private Factor " + factorName + ";\n" + tab + "#VAR_DECL#");

                    UtilityNode prevFactor = node.GetWeightsAndFactors().Select(k => k.Value).FirstOrDefault();
                    string prevFactorName;

                    if (prevFactor)
                    {
                        prevFactorName = CleanName(prevFactor.nodeName);
                    }
                    else
                    {
                        prevFactorName = "null";
                    }

                    switch (node.curveType)
                    {
                        case curveType.Linear:
                            result += factorName + " = " + "new LinearCurve(" + prevFactorName + ", " + node.slope + ", " + node.displY + ");\n" + tab + tab;
                            break;
                        case curveType.Exponential:
                            result += factorName + " = " + "new ExpCurve(" + prevFactorName + ", " + node.exp + ", " + node.displX + ", " + node.displY + ");\n" + tab + tab;
                            break;
                        case curveType.LinearParts:
                            string pointsListName = factorName + "Points";
                            result += WriteList<Vector2, Point2D>(pointsListName, node.points, (Vector2 point) => "new Point2D(" + point.x + ", " + point.y + ")");
                            result += factorName + " = " + "new LinearPartsCurve(" + prevFactorName + ", " + pointsListName + ");\n" + tab + tab;
                            break;
                    }
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// Writes all the Action nodes declaration and initialization
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="engineEnding"></param>
    /// <param name="subElems"></param>
    /// <returns></returns>
    private static string GetActions(ref string templateText, ClickableElement elem, string engineEnding, ref List<ClickableElement> subElems)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;
        string machineName = className + engineEnding;

        foreach (UtilityNode node in elem.nodes.Where(n => ((UtilityNode)n).type == utilityType.Action))
        {
            string nodeName = CleanName(node.nodeName);

            templateText = templateText.Replace("#VAR_DECL#", "private UtilityAction " + nodeName + ";\n" + tab + "#VAR_DECL#");

            UtilityNode prevFactor = node.GetWeightsAndFactors().Select(k => k.Value).FirstOrDefault();
            string prevFactorName;

            if (prevFactor)
            {
                prevFactorName = CleanName(prevFactor.nodeName);
            }
            else
            {
                prevFactorName = "null";
            }

            if (node.subElem is null)
            {
                result += nodeName + " = " + machineName + ".CreateUtilityAction(\"" + node.nodeName + "\", " + nodeName + actionsEnding + ", " + prevFactorName + ");\n" + tab + tab;
            }
            else
            {
                string subEnding = "";

                switch (node.subElem.GetType().ToString())
                {
                    case nameof(FSM):
                        subEnding = subFSMEnding;
                        break;
                    case nameof(BehaviourTree):
                        subEnding = subBTEnding;
                        break;
                    case nameof(UtilitySystem):
                        subEnding = subUSEnding;
                        break;
                }
                string subElemName = nodeName + subEnding;

                result += nodeName + " = " + machineName + ".CreateSubBehaviour(\"" + node.nodeName + "\", " + prevFactorName + ", " + subElemName + ");\n" + tab + tab;

                subElems.Add(node.subElem);
            }
        }

        return result;
    }

    #endregion

    /// <summary>
    /// Writes all the methods for the Actions of the <see cref="State"/>
    /// </summary>
    /// <param name="elem"></param>
    /// <returns></returns>
    private static string GetActionMethods(ClickableElement elem)
    {
        string className = CleanName(elem.elementName);
        string result = string.Empty;

        switch (elem.GetType().ToString())
        {
            case nameof(FSM):
                foreach (BaseNode node in elem.nodes)
                {
                    if (node.subElem != null)
                    {
                        result += GetActionMethods(node.subElem);
                    }
                    else
                    {
                        string nodeName = CleanName(node.nodeName);
                        result += "\n" + tab +
                          "private void " + nodeName + actionsEnding + "()\n"
                          + tab + "{\n"
                          + tab + tab + "\n"
                          + tab + "}\n"
                          + tab;
                    }
                }
                break;
            case nameof(BehaviourTree):
                foreach (BaseNode node in elem.nodes.Where(n => ((BehaviourNode)n).type == behaviourType.Leaf))
                {
                    if (node.subElem != null)
                    {
                        result += GetActionMethods(node.subElem);
                    }
                    else
                    {
                        string nodeName = CleanName(node.nodeName);
                        result += "\n" + tab +
                          "private void " + nodeName + actionsEnding + "()\n"
                          + tab + "{\n"
                          + tab + tab + "\n"
                          + tab + "}\n"
                          + tab;

                        result += "\n" + tab +
                          "private ReturnValues " + nodeName + conditionsEnding + "()\n"
                          + tab + "{\n"
                          + tab + tab + "//Write here the code for the success check for " + nodeName + "\n"
                          + tab + tab + "return ReturnValues.Failed;\n"
                          + tab + "}\n"
                          + tab;
                    }
                }
                break;
            case nameof(UtilitySystem):
                foreach (BaseNode node in elem.nodes.Where(n => ((UtilityNode)n).type == utilityType.Action))
                {
                    if (node.subElem != null)
                    {
                        result += GetActionMethods(node.subElem);
                    }
                    else
                    {
                        string nodeName = CleanName(node.nodeName);
                        result += "\n" + tab +
                          "private void " + nodeName + actionsEnding + "()\n"
                          + tab + "{\n"
                          + tab + tab + "\n"
                          + tab + "}\n"
                          + tab;
                    }
                }
                break;
        }

        return result;
    }
}
