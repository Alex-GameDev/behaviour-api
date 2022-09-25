using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Globalization;

public class TransitionGUI : GUIElement
{
    /// <summary>
    /// Name of the <see cref="TransitionGUI"/>
    /// </summary>
    public string transitionName = "";

    private NodeEditor privateEditor;

    /// <summary>
    /// Reference to the NodeEditor window
    /// </summary>
    public NodeEditor sender
    {
        get
        {
            if (!privateEditor)
                privateEditor = EditorWindow.GetWindow<NodeEditor>();
            return privateEditor;
        }
    }

    /// <summary>
    /// <see cref="BaseNode"/> that this transition comes from
    /// </summary>
    public BaseNode fromNode;

    /// <summary>
    /// <see cref="BaseNode"/> that this transition goes to
    /// </summary>
    public BaseNode toNode;

    /// <summary>
    /// Initial width of the <see cref="GUIElement.windowRect"/>
    /// </summary>
    public static int baseWidth = 200;

    /// <summary>
    /// Initial height of the <see cref="GUIElement.windowRect"/>
    /// </summary>
    public static int baseHeight = 70;

    /// <summary>
    /// Variable width value of the <see cref="TransitionGUI.windowRect"/>
    /// </summary>
    public int width;

    /// <summary>
    /// Variable height value of the <see cref="TransitionGUI.windowRect"/>
    /// </summary>
    public int height;

    /// <summary>
    /// The root <see cref="PerceptionGUI"/> used to go through the others like navigating a tree
    /// </summary>
    public PerceptionGUI rootPerception;

    /// <summary>
    /// Weight value for <see cref="toNode"/> is a Weighted Fusion node
    /// </summary>
    public float weight = 1.0f;

    public bool isExit = false;

    /// <summary>
    /// The Initializer for the <seealso cref="TransitionGUI"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void InitTransitionGUI(ClickableElement parent, BaseNode from, BaseNode to, bool comesFromXML = false)
    {
        var foo = sender;

        this.identificator = UniqueID();

        this.transitionName = parent.elementNamer.AddName(identificator, "New Transition ");

        this.width = baseWidth;
        this.height = baseHeight;

        this.fromNode = from;
        this.toNode = to;

        this.rootPerception = CreateInstance<PerceptionGUI>();
        this.rootPerception.InitPerceptionGUI(perceptionType.Push);

        if (comesFromXML)
        {
            ((BehaviourNode)toNode).index = ((BehaviourNode)to).index;
        }
        else if (fromNode is BehaviourNode && ((((BehaviourNode)fromNode).type == behaviourType.Sequence && !((BehaviourNode)fromNode).isRandom) || ((BehaviourNode)fromNode).type == behaviourType.Selector))
        {
            ((BehaviourNode)toNode).index = ((BehaviourTree)parent).ChildrenCount((BehaviourNode)fromNode) + 1;
        }
    }

    /// <summary>
    /// The Initializer for the <seealso cref="TransitionGUI"/> when it is being loaded from an XML
    /// </summary>
    /// <param name="name"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void InitTransitionGUIFromXML(ClickableElement parent, BaseNode from, BaseNode to, string id, string name, PerceptionGUI rootPerception, bool isExit, float weight = 1.0f)
    {
        var foo = sender;

        this.identificator = id;

        this.transitionName = parent.elementNamer.AddName(identificator, name);

        this.width = baseWidth;
        this.height = baseHeight;

        this.weight = weight;

        this.fromNode = from;
        this.toNode = to;

        this.rootPerception = rootPerception;

        this.isExit = isExit;

        if (fromNode is BehaviourNode && ((((BehaviourNode)fromNode).type == behaviourType.Sequence && !((BehaviourNode)fromNode).isRandom) || ((BehaviourNode)fromNode).type == behaviourType.Selector))
        {
            ((BehaviourNode)toNode).index = ((BehaviourNode)to).index;
        }
    }

    /// <summary>
    /// Creates and returns an <see cref="XMLElement"/> that corresponds to this <see cref="TransitionGUI"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override XMLElement ToXMLElement(params object[] args)
    {
        XMLElement result = new XMLElement
        {
            name = CleanName(this.transitionName),
            elemType = this.GetType().ToString(),
            windowPosX = this.windowRect.x,
            windowPosY = this.windowRect.y,
            Id = this.identificator,
            weight = this.weight,
            fromId = this.fromNode?.identificator,
            toId = this.toNode?.identificator,
            perception = this.rootPerception.ToPerceptionXML(),
            isExit = this.isExit
        };

        return result;
    }

    /// <summary>
    /// Creates a copy of this <see cref="TransitionGUI"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override GUIElement CopyElement(params object[] args)
    {
        BaseNode fromNode;
        BaseNode toNode;

        if (args.Count() > 1)
        {
            fromNode = (BaseNode)args[0];
            toNode = (BaseNode)args[1];
        }
        else
        {
            fromNode = this.fromNode;
            toNode = this.toNode;
        }

        TransitionGUI result = CreateInstance<TransitionGUI>();
        result.identificator = this.identificator;
        result.transitionName = this.transitionName;
        result.windowRect = new Rect(this.windowRect);
        result.width = this.width;
        result.height = this.height;
        result.weight = this.weight;
        result.fromNode = fromNode;
        result.toNode = toNode;
        result.rootPerception = (PerceptionGUI)rootPerception.CopyElement();
        result.isExit = this.isExit;

        return result;
    }

    /// <summary>
    /// Gets the type of the <see cref="TransitionGUI"/> properly written
    /// </summary>
    /// <returns></returns>
    public override string GetTypeString()
    {
        return "Transition";
    }

    /// <summary>
    /// Draws the <see cref="PerceptionGUI"/> as a foldout. Works recursively when there's a <see cref="PerceptionGUI"/> in another
    /// </summary>
    /// <param name="heightAcc"></param>
    /// <param name="widthAcc"></param>
    /// <param name="currentPerception"></param>
    private void PerceptionFoldout(ref int heightAcc, ref int widthAcc, ref PerceptionGUI currentPerception)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        try
        {
            currentPerception.openFoldout = EditorGUILayout.Foldout(currentPerception.openFoldout, currentPerception.type.ToString() + "Perception");

            if (currentPerception.openFoldout)
            {
                heightAcc += 30;

                if (GUILayout.Button(currentPerception.type.ToString(), EditorStyles.toolbarDropDown))
                {
                    GenericMenu toolsMenu = new GenericMenu();

                    foreach (string name in Enum.GetNames(typeof(perceptionType)))
                    {
                        toolsMenu.AddItem(new GUIContent(name), false, ChangeType, new string[] { name, currentPerception.identificator });
                    }

                    toolsMenu.DropDown(new Rect(8 + widthAcc * 0.5f, Event.current.mousePosition.y + 10, 0, 0));
                    EditorGUIUtility.ExitGUI();
                }

                switch (currentPerception.type)
                {
                    case perceptionType.Push:
                        heightAcc += 40;
                        GUILayout.Label("You will be able to\nfire this transition\nmanually through code", new GUIStyle(Styles.SubTitleText)
                        {
                            fontStyle = FontStyle.Italic
                        }, GUILayout.Height(50));
                        break;
                    case perceptionType.Timer:
                        heightAcc += 20;

                        GUILayout.BeginHorizontal();
                        try
                        {
                            GUILayout.Label("Time: ", new GUIStyle(Styles.TitleText)
                            {
                                alignment = TextAnchor.MiddleCenter
                            }, GUILayout.Height(20), GUILayout.ExpandWidth(true));


                            float.TryParse(EditorGUILayout.TextField(currentPerception.timerNumber.ToString(CultureInfo.CreateSpecificCulture("en-US")), new GUIStyle(Styles.TitleText)
                            {
                                alignment = TextAnchor.MiddleCenter
                            }, GUILayout.Height(20), GUILayout.Width(50)), NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-US"), out float number);

                            GUILayout.Label(currentPerception.timerUnit, new GUIStyle(Styles.TitleText)
                            {
                                alignment = TextAnchor.MiddleRight
                            }, GUILayout.Height(20), GUILayout.ExpandWidth(true));

                            currentPerception.timerNumber = number;
                        }
                        finally
                        {
                            GUILayout.EndHorizontal();
                        }
                        break;
                    case perceptionType.Value:
                        heightAcc += 40;
                        GUILayout.Label("You will have to\nwrite the lambda function\nfor this in code", new GUIStyle(Styles.SubTitleText)
                        {
                            fontStyle = FontStyle.Italic
                        }, GUILayout.Height(50));
                        break;
                    case perceptionType.IsInState:
                        heightAcc += 30;

                        GUILayout.Space(5);

                        List<FSM> subFSMsList = new List<FSM>();
                        foreach (ClickableElement elem in sender.Elements)
                        {
                            subFSMsList.AddRange(elem.GetSubElems(true).Where(e => e is FSM).Cast<FSM>());
                        }

                        GUI.enabled = subFSMsList.Count > 0;

                        try
                        {
                            if (GUILayout.Button(currentPerception.elemName, EditorStyles.toolbarDropDown))
                            {
                                GenericMenu toolsMenu = new GenericMenu();

                                List<string> list = subFSMsList.Select(e => e.elementName).ToList();
                                list.Sort();

                                foreach (string name in list)
                                {
                                    toolsMenu.AddItem(new GUIContent(name), false, (current) =>
                                    {
                                        if (((PerceptionGUI)current).elemName != name)
                                        {
                                            ((PerceptionGUI)current).elemName = name;
                                            ((PerceptionGUI)current).stateName = "Select a State";
                                        }
                                    }, currentPerception);
                                }

                                toolsMenu.DropDown(new Rect(8 + widthAcc * 0.5f, Event.current.mousePosition.y + 10, 0, 0));
                                EditorGUIUtility.ExitGUI();
                            }

                            if (subFSMsList.Count > 0 && currentPerception.elemName != "Select a FSM" && currentPerception.elemName != "Select a BT" && !string.IsNullOrEmpty(currentPerception.elemName))
                            {
                                heightAcc += 30;

                                GUILayout.Space(5);

                                string auxName = currentPerception.elemName;

                                FSM selectedFSM = subFSMsList.Where(e => e.elementName == auxName).FirstOrDefault();
                                List<BaseNode> subStatesList = selectedFSM ? selectedFSM.nodes.Where(s => s.subElem == null).ToList() : new List<BaseNode>();

                                GUI.enabled = subStatesList.Count > 0;

                                if (GUILayout.Button(currentPerception.stateName, EditorStyles.toolbarDropDown))
                                {
                                    GenericMenu toolsMenu = new GenericMenu();

                                    List<string> list = subStatesList.Select(s => s.nodeName).ToList();
                                    list.Sort();

                                    foreach (string name in list)
                                    {
                                        toolsMenu.AddItem(new GUIContent(name), false, (current) =>
                                        {
                                            ((PerceptionGUI)current).stateName = name;
                                        }, currentPerception);
                                    }

                                    toolsMenu.DropDown(new Rect(8 + widthAcc * 0.5f, Event.current.mousePosition.y + 10, 0, 0));
                                    EditorGUIUtility.ExitGUI();
                                }
                            }
                        }
                        finally
                        {
                            GUI.enabled = true;
                        }

                        break;
                    case perceptionType.BehaviourTreeStatus:
                        heightAcc += 30;

                        GUILayout.Space(5);

                        List<BehaviourTree> subBTsList = new List<BehaviourTree>();

                        foreach (ClickableElement elem in sender.Elements)
                        {
                            subBTsList.AddRange(elem.GetSubElems(true).Where(e => e is BehaviourTree).Cast<BehaviourTree>());
                        }

                        GUI.enabled = subBTsList.Count > 0;
                        if (GUILayout.Button(currentPerception.elemName, EditorStyles.toolbarDropDown))
                        {
                            GenericMenu toolsMenu = new GenericMenu();

                            List<string> list = subBTsList.Select(e => e.elementName).ToList();
                            list.Sort();

                            foreach (string name in list)
                            {
                                toolsMenu.AddItem(new GUIContent(name), false, (current) =>
                                {
                                    if (((PerceptionGUI)current).elemName != name)
                                    {
                                        ((PerceptionGUI)current).elemName = name;
                                        ((PerceptionGUI)current).status = ReturnValues.Succeed;
                                    }
                                }, currentPerception);
                            }

                            toolsMenu.DropDown(new Rect(8 + widthAcc * 0.5f, Event.current.mousePosition.y + 10, 0, 0));
                            EditorGUIUtility.ExitGUI();
                        }
                        GUI.enabled = true;

                        if (subBTsList.Count > 0 && currentPerception.elemName != "Select a FSM" && currentPerception.elemName != "Select a BT" && !string.IsNullOrEmpty(currentPerception.elemName))
                        {
                            heightAcc += 30;

                            GUILayout.Space(5);

                            if (GUILayout.Button(currentPerception.status.ToString(), EditorStyles.toolbarDropDown))
                            {
                                GenericMenu toolsMenu = new GenericMenu();

                                foreach (string name in Enum.GetNames(typeof(ReturnValues)))
                                {
                                    toolsMenu.AddItem(new GUIContent(name), false, (current) =>
                                    {
                                        ((PerceptionGUI)current).status = (ReturnValues)Enum.Parse(typeof(ReturnValues), name);
                                    }, currentPerception);
                                }

                                toolsMenu.DropDown(new Rect(8 + widthAcc * 0.5f, Event.current.mousePosition.y + 10, 0, 0));
                                EditorGUIUtility.ExitGUI();
                            }
                        }
                        break;
                    case perceptionType.And:
                        heightAcc += 60;
                        widthAcc += 20;

                        PerceptionFoldout(ref heightAcc, ref widthAcc, ref currentPerception.firstChild);
                        GUILayout.Label("-AND-", Styles.TitleText, GUILayout.Height(20));
                        PerceptionFoldout(ref heightAcc, ref widthAcc, ref currentPerception.secondChild);
                        break;
                    case perceptionType.Or:
                        heightAcc += 60;
                        widthAcc += 20;

                        PerceptionFoldout(ref heightAcc, ref widthAcc, ref currentPerception.firstChild);
                        GUILayout.Label("-OR-", Styles.TitleText, GUILayout.Height(20));
                        PerceptionFoldout(ref heightAcc, ref widthAcc, ref currentPerception.secondChild);
                        break;
                    case perceptionType.Custom:
                        heightAcc += 70;
                        widthAcc += 30;

                        GUILayout.BeginHorizontal();
                        try
                        {
                            GUILayout.Label("Name: ", new GUIStyle(Styles.TitleText)
                            {
                                alignment = TextAnchor.MiddleCenter
                            }, GUILayout.Height(20), GUILayout.Width(width * 0.3f));


                            string name = GUILayout.TextField(currentPerception.customName, new GUIStyle(Styles.TitleText)
                            {
                                alignment = TextAnchor.MiddleCenter
                            }, GUILayout.Height(20), GUILayout.Width(100));

                            currentPerception.customName = name;
                        }
                        finally
                        {
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.Label("You will have to code\nthe Check method\nin the generated script", new GUIStyle(Styles.SubTitleText)
                        {
                            fontStyle = FontStyle.Italic
                        }, GUILayout.Height(50));
                        break;
                }
            }
        }
        finally
        {
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }

    /// <summary>
    /// Changes the <see cref="perceptionType"/> of a <see cref="PerceptionGUI"/> (its id is in <paramref name="param"/>) to a new selected type (it's in <paramref name="param"/>)
    /// </summary>
    /// <param name="param"></param>
    public void ChangeType(object param)
    {
        string[] data = (string[])param;
        string newType = data[0];
        string id = data[1];

        ChangeTypeRecursive(ref rootPerception, id, (perceptionType)Enum.Parse(typeof(perceptionType), newType));
    }

    /// <summary>
    /// Recursive function for <see cref="ChangeType(object)"/>
    /// </summary>
    /// <param name="perception"></param>
    /// <param name="id"></param>
    /// <param name="newType"></param>
    public void ChangeTypeRecursive(ref PerceptionGUI perception, string id, perceptionType newType)
    {
        if (perception.identificator == id)
        {
            switch (perception.type)
            {
                case perceptionType.Timer:
                    if (newType != perceptionType.Timer)
                    {
                        perception.InitPerceptionGUI(newType);
                    }
                    break;
                case perceptionType.Value:
                    if (newType != perceptionType.Value)
                    {
                        perception.InitPerceptionGUI(newType);
                    }
                    break;
                case perceptionType.IsInState:
                    if (newType != perceptionType.IsInState)
                    {
                        perception.InitPerceptionGUI(newType);
                    }
                    break;
                case perceptionType.BehaviourTreeStatus:
                    if (newType != perceptionType.BehaviourTreeStatus)
                    {
                        perception.InitPerceptionGUI(newType);
                    }
                    break;
                case perceptionType.And:
                case perceptionType.Or:
                    if (newType == perceptionType.And || newType == perceptionType.Or)
                    {
                        perception.type = newType;
                    }
                    else
                    {
                        perception.InitPerceptionGUI(newType);
                    }
                    break;
                default:
                    perception.InitPerceptionGUI(newType);
                    break;
            }

            perception.openFoldout = true;
        }
        else
        {
            if (perception.firstChild != null && perception.secondChild != null)
            {
                ChangeTypeRecursive(ref perception.firstChild, id, newType);
                ChangeTypeRecursive(ref perception.secondChild, id, newType);
            }
        }
    }

    /// <summary>
    /// Draws all the elements inside the <see cref="TransitionGUI"/>
    /// </summary>
    public override void DrawWindow()
    {
        switch (sender.currentElem.GetType().ToString())
        {
            case nameof(FSM):
                DefaultPerceptionEditor();
                break;
            case nameof(BehaviourTree):
                if (isExit)
                {
                    DefaultPerceptionEditor();
                }
                else
                {
                    // If the parent node is a non-random sequence we add an index selector
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Label("Order:", Styles.CenteredTitleText, GUILayout.Width(30), GUILayout.Height(20));
                    GUILayout.Space(5);
                    if (GUILayout.Button(((BehaviourNode)toNode).index.ToString(), EditorStyles.toolbarDropDown))
                    {
                        GenericMenu toolsMenu = new GenericMenu();

                        for (int i = 1; i <= ((BehaviourTree)sender.currentElem).ChildrenCount((BehaviourNode)fromNode); i++)
                        {
                            int newId = i;

                            toolsMenu.AddItem(new GUIContent(i.ToString()), false, () =>
                            {
                                if (newId != ((BehaviourNode)toNode).index)
                                {
                                    NodeEditorUtilities.GenerateUndoStep(sender.currentElem);
                                    ((BehaviourNode)fromNode).ReorderIndices((BehaviourNode)toNode, newId);
                                    ((BehaviourNode)toNode).index = newId;
                                }
                            });
                        }

                        toolsMenu.DropDown(new Rect(0, Event.current.mousePosition.y, 0, 0));
                        EditorGUIUtility.ExitGUI();
                    }
                    GUILayout.EndHorizontal();
                }
                break;
            case nameof(UtilitySystem):
                if (isExit)
                {
                    DefaultPerceptionEditor();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    GUILayout.Label("Weight:", Styles.CenteredTitleText, GUILayout.Width(20), GUILayout.Height(25));

                    // Check if the user changes the textArea
                    float.TryParse(EditorGUILayout.TextField(weight.ToString(CultureInfo.CreateSpecificCulture("en-US")), Styles.CenteredTitleText,
                        GUILayout.ExpandWidth(true), GUILayout.Height(25)), NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-US"), out weight);

                    GUILayout.EndHorizontal();
                }
                break;
        }
    }

    private void DefaultPerceptionEditor()
    {
        int heightAcc = 0;
        int widthAcc = 0;

        transitionName = CleanName(EditorGUILayout.TextArea(transitionName, Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.Height(25)));

        // Narrower area than the main rect
        Rect areaRect = new Rect(baseWidth * 0.1f, 40, width * 0.8f, height);

        GUILayout.BeginArea(areaRect);
        try
        {
            PerceptionFoldout(ref heightAcc, ref widthAcc, ref rootPerception);
        }
        finally
        {
            GUILayout.EndArea();
        }

        // Increase the size depending on the open foldouts
        height = baseHeight + heightAcc;
        width = baseWidth + widthAcc;
    }
}
