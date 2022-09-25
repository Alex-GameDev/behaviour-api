using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class NodeEditor : EditorWindow
{
    /// <summary>
    /// Name of the window that will show on top
    /// </summary>
    const string editorTitle = "Intelligent Behaviours GUI";

    /// <summary>
    /// Stores the position of the cursor in every GUI iteration
    /// </summary>
    private Vector2 mousePos;

    /// <summary>
    /// Stores the <see cref="BaseNode"/> that is currently being used for <see cref="makeTransitionMode"/>, <see cref="makeAttachedNode"/> and <see cref="makeConnectionMode"/>
    /// </summary>
    private BaseNode selectednode;

    /// <summary>
    /// Stores the <see cref="TransitionGUI"/> that is currently being reconnected
    /// </summary>
    private TransitionGUI selectedTransition;

    /// <summary>
    /// The list of <see cref="GUIElement"/> that are currently selected by the user
    /// </summary>
    public List<GUIElement> focusedObjects = new List<GUIElement>();

    /// <summary>
    /// The list of copied <see cref="GUIElement"/> that will be pasted
    /// </summary>
    public List<GUIElement> clipboard = new List<GUIElement>();

    /// <summary>
    /// The list of references to every <see cref="GUIElement"/> that will be deleted after pasting the cut objects
    /// </summary>
    public List<GUIElement> cutObjects = new List<GUIElement>();

    /// <summary>
    /// The <see cref="ClickableElement"/> from which <see cref="cutObjects"/> were cut from
    /// </summary>
    public ClickableElement cutFromElement;

    /// <summary>
    /// The <see cref="BaseNode"/> that is being created in a <see cref="BehaviourTree"/> or <see cref="UtilitySystem"/>. Used to keep track of it while in <see cref="makeAttachedNode"/>
    /// </summary>
    private BaseNode toCreateNode;

    /// <summary>
    /// True when the user is in the process of creating a new <see cref="TransitionGUI"/>
    /// </summary>
    private bool makeTransitionMode = false;

    /// <summary>
    /// True when the user is in the process of reconnecting a <see cref="TransitionGUI"/>
    /// </summary>
    private bool reconnectTransitionMode = false;

    /// <summary>
    /// True when the user is in the process of creating a new <see cref="BaseNode"/> from the <see cref="selectednode"/>, so it gets attached
    /// </summary>
    private bool makeAttachedNode = false;

    /// <summary>
    /// True when the user is in the process of connecting a <see cref="BaseNode"/>
    /// </summary>
    private bool makeConnectionMode = false;

    /// <summary>
    /// List of <see cref="ClickableElement"/> in the editor
    /// </summary>
    public List<ClickableElement> Elements = new List<ClickableElement>();

    /// <summary>
    /// Active <see cref="ClickableElement"/> that is open
    /// </summary>
    public ClickableElement currentElem;

    /// <summary>
    /// The <see cref="UniqueNamer"/> for managing the names of the elements in the editor
    /// </summary>
    public UniqueNamer editorNamer;

    /// <summary>
    /// True if the <see cref="PopupWindow"/> is on screen
    /// </summary>
    public bool popupShown;

    /// <summary>
    /// Variable width of the top bar depending on the length of what is being displayed
    /// </summary>
    private float topBarOffset;

    /// <summary>
    /// Vertical offset for exitTransitions
    /// </summary>
    private static float exitTransVerticalOffset = 80;

    /// <summary>
    /// Fixed offset for the <see cref="TransitionGUI.windowRect"/> when there's two of them in the same pair of nodes
    /// </summary>
    private static float doubleTransRectOffset = 35;

    /// <summary>
    /// Fixed offset for the <see cref="TransitionGUI.windowRect"/> when there's two of them in the same pair of nodes
    /// </summary>
    private static float doubleTransCurveOffset = 20;

    private static bool CtrlDown = false;

    private Texture BoxTexture;

    /// <summary>
    /// This will be called when the user opens the window
    /// </summary>
    [MenuItem("Window/" + editorTitle)]
    static void ShowEditor()
    {
        // We clear any UndoSteps from previous uses of the GUI
        NodeEditorUtilities.ClearUndoSteps();

        // Close any previous window
        GetWindow<PopupWindow>().Close();
        GetWindow<NodeEditor>().Close();

        // Open a new Editor Window
        // And reset the editorNamer
        GetWindow<NodeEditor>(editorTitle).editorNamer = CreateInstance<UniqueNamer>();
    }

    /// <summary>
    /// Called once every frame
    /// </summary>
    private void OnGUI()
    {
        Event e = Event.current;
        mousePos = e.mousePosition;

        // Show the GUI elements that are over the rest of the GUI
        #region GUI overlay elements
        ShowColorLegend();
        ShowTopBar();
        if (currentElem != null)
        {
            GUI.enabled = NodeEditorUtilities.undoStepsSaved > 0;
            if (GUI.Button(new Rect(position.width - 80, 0, 25, 20), "<-", Styles.TopBarButton))
            {
                Undo();
            }

            GUI.enabled = NodeEditorUtilities.redoStepsSaved > 0;
            if (GUI.Button(new Rect(position.width - 55, 0, 25, 20), "->", Styles.TopBarButton))
            {
                Redo();
            }

            GUI.enabled = true;
        }
        ShowOptions();
        ShowErrorsAndWarnings();

        #endregion

        // Draw the curves
        #region Curves Drawing

        if ((makeTransitionMode || makeAttachedNode || makeConnectionMode) && selectednode != null)
        {
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
            Rect nodeRect = new Rect(selectednode.windowRect);

            if ((currentElem is BehaviourTree && makeConnectionMode) || (currentElem is UtilitySystem && makeAttachedNode))
                DrawNodeCurve(mouseRect, nodeRect, true);
            else
                DrawNodeCurve(nodeRect, mouseRect, true);
        }

        if (reconnectTransitionMode)
        {
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
            Rect nodeRect = new Rect(selectedTransition.fromNode.windowRect);

            if (!cutObjects.Contains(selectedTransition))
                cutObjects.Add(selectedTransition);

            DrawNodeCurve(nodeRect, mouseRect, true);
        }

        if (currentElem)
        {
            currentElem.DrawCurves();
        }

        #endregion

        // Check for errors or warnings and add them to the list
        #region Errors and Warnings Control

        if (currentElem)
        {
            switch (currentElem.GetType().ToString())
            {
                case nameof(FSM):
                    if (!((FSM)currentElem).HasEntryState)
                    {
                        currentElem.AddError(Error.NoEntryState);
                    }
                    else
                    {
                        currentElem.RemoveError(Error.NoEntryState);
                    }

                    if (currentElem.nodes.Exists(n => ((StateNode)n).type == stateType.Unconnected))
                    {
                        currentElem.AddWarning(Warning.UnconnectedNode);
                    }
                    else
                    {
                        currentElem.RemoveWarning(Warning.UnconnectedNode);
                    }
                    break;

                case nameof(BehaviourTree):
                    if (currentElem.nodes.Where(n => ((BehaviourNode)n).isRoot).Count() > 1)
                    {
                        currentElem.AddError(Error.MoreThanOneRoot);
                    }
                    else
                    {
                        currentElem.RemoveError(Error.MoreThanOneRoot);
                    }

                    if (((BehaviourTree)currentElem).BadRootCheck())
                    {
                        currentElem.AddWarning(Warning.LeafIsRoot);
                    }
                    else
                    {
                        currentElem.RemoveWarning(Warning.LeafIsRoot);
                    }
                    break;

                case nameof(UtilitySystem):
                    if (currentElem.nodes.Exists(n => ((UtilityNode)n).type != utilityType.Variable && !currentElem.transitions.Exists(t => t.toNode.Equals(n))))
                    {
                        currentElem.AddWarning(Warning.NoFactors);
                    }
                    else
                    {
                        currentElem.RemoveWarning(Warning.NoFactors);
                    }

                    if (currentElem.nodes.Exists(n => ((UtilityNode)n).type == utilityType.Fusion && ((UtilityNode)n).fusionType == fusionType.Weighted && currentElem.transitions.Exists(t => t.toNode.Equals(n) && t.weight == 0)))
                    {
                        currentElem.AddWarning(Warning.WeightZero);
                    }
                    else
                    {
                        currentElem.RemoveWarning(Warning.WeightZero);
                    }
                    break;
            }

            if (currentElem.NeedsExitTransition)
            {
                currentElem.AddError(Error.NoExitTransition);
            }
            else
            {
                currentElem.RemoveError(Error.NoExitTransition);
            }

            // Check repeated names
            bool repeatedNames = false;

            foreach (BaseNode node in currentElem.nodes)
            {
                if (currentElem.CheckNameExisting(node.nodeName, 1))
                    repeatedNames = true;
            }

            foreach (TransitionGUI transition in currentElem.transitions)
            {
                if (currentElem.CheckNameExisting(transition.transitionName, 1))
                    repeatedNames = true;
            }

            if (repeatedNames)
                currentElem.AddError(Error.RepeatedName);
            else
                currentElem.RemoveError(Error.RepeatedName);
        }

        #endregion

        // Control for the events called by the mouse
        #region Mouse Click Control

        if (e.type == EventType.MouseDown)
        {
            // Check where it clicked
            int[] results = ClickedOnCheck();

            bool clickedOnElement = Convert.ToBoolean(results[0]);
            bool clickedOnWindow = Convert.ToBoolean(results[1]);
            bool clickedOnLeaf = Convert.ToBoolean(results[2]);
            bool clickedOnVariable = Convert.ToBoolean(results[3]);
            bool decoratorWithOneChild = Convert.ToBoolean(results[4]);
            bool actionWithOneFactor = Convert.ToBoolean(results[5]);
            bool curveWithOneFactor = Convert.ToBoolean(results[6]);
            bool nodeWithAscendants = Convert.ToBoolean(results[7]);
            bool clickedOnTransition = Convert.ToBoolean(results[8]);
            int selectIndex = results[9];

            // Click derecho
            if (e.button == 1)
            {
                if (!makeTransitionMode && !reconnectTransitionMode && !makeAttachedNode && !makeConnectionMode)
                {
                    if (!clickedOnElement && !clickedOnWindow && !clickedOnTransition)
                    {
                        focusedObjects.Clear();
                    }

                    // Set menu items
                    GenericMenu menu = new GenericMenu();

                    if (currentElem)
                        switch (currentElem.GetType().ToString())
                        {
                            case nameof(FSM):
                                if (!clickedOnWindow && !clickedOnTransition)
                                {
                                    menu.AddItem(new GUIContent("Add State"), false, ContextCallback, new string[] { "Node", selectIndex.ToString() });
                                }
                                else if (clickedOnWindow)
                                {
                                    menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, new string[] { "makeTransition", selectIndex.ToString() });

                                    if ((currentElem.parent is BehaviourTree || currentElem.parent is UtilitySystem) && !currentElem.transitions.Exists(t => t.isExit && t.fromNode.Equals(currentElem.nodes[selectIndex])))
                                    {
                                        menu.AddItem(new GUIContent("Add Exit Transition"), false, ContextCallback, new string[] { "exitTrans", selectIndex.ToString() });
                                    }
                                    if (!((FSM)currentElem).isEntryState((StateNode)currentElem.nodes[selectIndex]))
                                    {
                                        menu.AddItem(new GUIContent("Convert to Entry State"), false, ContextCallback, new string[] { "entryState", selectIndex.ToString() });
                                    }
                                }
                                else if (clickedOnTransition)
                                {
                                    menu.AddItem(new GUIContent("Reconnect Transition"), false, ContextCallback, new string[] { "reconnectTransition", selectIndex.ToString() });
                                    menu.AddItem(new GUIContent("Flip Transition"), false, ContextCallback, new string[] { "flipTransition", selectIndex.ToString() });
                                }
                                menu.AddSeparator("");
                                break;
                            case nameof(BehaviourTree):
                                if (!clickedOnWindow && !clickedOnTransition)
                                {
                                    if (currentElem.nodes.Count == 0)
                                    {
                                        menu.AddItem(new GUIContent("Add Sequence"), false, ContextCallback, new string[] { "Sequence", selectIndex.ToString() });
                                        menu.AddItem(new GUIContent("Add Selector"), false, ContextCallback, new string[] { "Selector", selectIndex.ToString() });
                                        menu.AddSeparator("");
                                    }
                                }
                                else if (clickedOnWindow)
                                {
                                    if (nodeWithAscendants)
                                        menu.AddItem(new GUIContent("Disconnect Node"), false, ContextCallback, new string[] { "disconnectNode", selectIndex.ToString() });
                                    else
                                        menu.AddItem(new GUIContent("Connect Node"), false, ContextCallback, new string[] { "connectNode", selectIndex.ToString() });

                                    menu.AddSeparator("");

                                    if (!clickedOnLeaf)
                                    {
                                        if (decoratorWithOneChild)
                                        {
                                            menu.AddDisabledItem(new GUIContent("Add Sequence"));
                                            menu.AddDisabledItem(new GUIContent("Add Selector"));
                                            menu.AddDisabledItem(new GUIContent("Add Leaf Node"));
                                            menu.AddDisabledItem(new GUIContent("Decorator Nodes"));
                                            menu.AddSeparator("");
                                            menu.AddDisabledItem(new GUIContent("Add FSM"));
                                            menu.AddDisabledItem(new GUIContent("Add Behaviour Tree"));
                                            menu.AddDisabledItem(new GUIContent("Add Utility System"));
                                        }
                                        else
                                        {
                                            menu.AddItem(new GUIContent("Add Sequence"), false, ContextCallback, new string[] { "Sequence", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Add Selector"), false, ContextCallback, new string[] { "Selector", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Add Leaf Node"), false, ContextCallback, new string[] { "leafNode", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Decorator Nodes/Add Loop (N)"), false, ContextCallback, new string[] { "loopN", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Decorator Nodes/Add LoopU (Until Fail)"), false, ContextCallback, new string[] { "loopUFail", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Decorator Nodes/Add Inverter"), false, ContextCallback, new string[] { "inverter", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Decorator Nodes/Add Timer"), false, ContextCallback, new string[] { "timer", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Decorator Nodes/Add Succeeder"), false, ContextCallback, new string[] { "succeeder", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Decorator Nodes/Add Conditional"), false, ContextCallback, new string[] { "conditional", selectIndex.ToString() });
                                            menu.AddSeparator("");
                                            menu.AddItem(new GUIContent("Add FSM"), false, ContextCallback, new string[] { "FSM", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Add Behaviour Tree"), false, ContextCallback, new string[] { "BT", selectIndex.ToString() });
                                            menu.AddItem(new GUIContent("Add Utility System"), false, ContextCallback, new string[] { "US", selectIndex.ToString() });
                                        }

                                        menu.AddSeparator("");
                                    }
                                    else
                                    {
                                        if (currentElem.parent is UtilitySystem && !currentElem.transitions.Exists(t => t.isExit && t.fromNode.Equals(currentElem.nodes[selectIndex])))
                                        {
                                            menu.AddItem(new GUIContent("Add Exit Transition"), false, ContextCallback, new string[] { "exitTrans", selectIndex.ToString() });
                                        }
                                    }
                                }
                                break;
                            case nameof(UtilitySystem):
                                if (!clickedOnWindow && !clickedOnTransition)
                                {
                                    menu.AddItem(new GUIContent("Add Action"), false, ContextCallback, new string[] { "Action", selectIndex.ToString() });
                                    menu.AddSeparator("");
                                }
                                else if (clickedOnWindow)
                                {
                                    if (((UtilityNode)currentElem.nodes[selectIndex]).type != utilityType.Action)
                                    {
                                        menu.AddItem(new GUIContent("Connect Node"), false, ContextCallback, new string[] { "connectNode", selectIndex.ToString() });
                                        menu.AddSeparator("");
                                    }

                                    if (!clickedOnVariable && !actionWithOneFactor && !curveWithOneFactor)
                                    {
                                        menu.AddItem(new GUIContent("Factors/Add Variable"), false, ContextCallback, new string[] { "Variable", selectIndex.ToString() });
                                        menu.AddItem(new GUIContent("Factors/Add Fusion"), false, ContextCallback, new string[] { "Fusion", selectIndex.ToString() });
                                        menu.AddItem(new GUIContent("Factors/Add Curve"), false, ContextCallback, new string[] { "Curve", selectIndex.ToString() });
                                        menu.AddSeparator("");
                                    }

                                    if (((UtilityNode)currentElem.nodes[selectIndex]).type == utilityType.Action && currentElem.parent is UtilitySystem && !currentElem.transitions.Exists(t => t.isExit && t.fromNode.Equals(currentElem.nodes[selectIndex])))
                                    {
                                        menu.AddItem(new GUIContent("Add Exit Transition"), false, ContextCallback, new string[] { "exitTrans", selectIndex.ToString() });
                                    }
                                }
                                break;
                        }

                    if (clickedOnElement || clickedOnWindow || clickedOnTransition)
                    {
                        ClickableElement element = null;
                        if (currentElem is FSM && clickedOnWindow)
                        {
                            element = currentElem.nodes[selectIndex].subElem;
                        }
                        else if (currentElem is BehaviourTree && clickedOnLeaf)
                        {
                            element = currentElem.nodes[selectIndex].subElem;
                        }
                        else if (currentElem is UtilitySystem && clickedOnWindow)
                        {
                            element = currentElem.nodes[selectIndex].subElem;
                        }
                        else if (!currentElem)
                        {
                            element = Elements[selectIndex];
                        }

                        if (element)
                        {
                            menu.AddItem(new GUIContent("Save"), false, SaveElem, element);
                            menu.AddItem(new GUIContent("Export Code"), false, ExportCode, element);
                            menu.AddSeparator("");
                        }

                        menu.AddItem(new GUIContent("Delete"), false, ContextCallback, new string[] { "deleteElement", selectIndex.ToString(), clickedOnTransition.ToString() });
                    }
                    else
                    {
                        if (!(currentElem is BehaviourTree))
                        {
                            menu.AddItem(new GUIContent("Add FSM"), false, ContextCallback, new string[] { "FSM", selectIndex.ToString() });
                            menu.AddItem(new GUIContent("Add Behaviour Tree"), false, ContextCallback, new string[] { "BT", selectIndex.ToString() });
                            menu.AddItem(new GUIContent("Add Utility System"), false, ContextCallback, new string[] { "US", selectIndex.ToString() });
                            menu.AddSeparator("");
                        }

                        menu.AddItem(new GUIContent("Load Element from file"), false, LoadSaveFile);
                    }

                    menu.AddSeparator("");

                    if (focusedObjects.Count > 0)
                    {
                        menu.AddItem(new GUIContent("Cut"), false, Cut);
                        menu.AddItem(new GUIContent("Copy"), false, Copy);
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Cut"));
                        menu.AddDisabledItem(new GUIContent("Copy"));
                    }

                    if (clipboard.Count > 0)
                    {
                        menu.AddItem(new GUIContent("Paste"), false, Paste);
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    }

                    menu.ShowAsContext();
                    e.Use();
                }
                //Click derecho estando en uno de estos modos, lo cancela
                else
                {
                    makeTransitionMode = false;
                    reconnectTransitionMode = false;
                    cutObjects.Remove(selectedTransition);
                    selectedTransition = null;
                    makeAttachedNode = false;
                    makeConnectionMode = false;
                }
            }

            // Click izquierdo
            else if (e.button == 0)
            {
                GUI.FocusControl(null);
                if (!CtrlDown)
                {
                    focusedObjects.Clear();
                }

                GUIElement selNode = null;

                if (clickedOnElement)
                {
                    selNode = Elements[selectIndex];
                }
                else if (clickedOnTransition)
                {
                    if (currentElem) selNode = currentElem.transitions[selectIndex];
                }
                else if (clickedOnWindow)
                {
                    if (currentElem) selNode = currentElem.nodes[selectIndex];
                }
                else
                {
                    focusedObjects.Clear();

                    e.Use();
                }

                if (focusedObjects.Contains(selNode))
                {
                    focusedObjects.Remove(selNode);
                    if (currentElem is BehaviourTree || currentElem is UtilitySystem)
                    {
                        focusedObjects.Remove(currentElem.transitions.Where(c => c.toNode.Equals(selNode)).FirstOrDefault());
                    }
                }
                else if (selNode)
                {
                    focusedObjects.Add(selNode);

                    if (currentElem is BehaviourTree || currentElem is UtilitySystem)
                    {
                        GUIElement transToAdd = currentElem.transitions.Where(c => c.toNode.Equals(selNode)).FirstOrDefault();
                        if (transToAdd) focusedObjects.Add(transToAdd);
                    }
                }

                if (e.clickCount == 2)
                {
                    if (selNode is ClickableElement)
                    {
                        focusedObjects.Clear();

                        currentElem = (ClickableElement)selNode;

                        NodeEditorUtilities.ClearUndoSteps();
                        e.Use();
                    }
                    else if (selNode is BaseNode && ((BaseNode)selNode).subElem != null)
                    {
                        focusedObjects.Clear();

                        currentElem = ((BaseNode)selNode).subElem;

                        NodeEditorUtilities.ClearUndoSteps();
                        e.Use();
                    }
                }

                // Manage the makeXmode things
                if (currentElem)
                    switch (currentElem.GetType().ToString())
                    {
                        case nameof(FSM):
                            if (makeTransitionMode)
                            {
                                if (clickedOnWindow)// && !currentElem.nodes[selectIndex].Equals(selectednode))
                                {
                                    if (!currentElem.transitions.Exists(t => !t.isExit && t.fromNode.Equals(selectednode) && t.toNode.Equals(currentElem.nodes[selectIndex])))
                                    {
                                        NodeEditorUtilities.GenerateUndoStep(currentElem);

                                        TransitionGUI transition = CreateInstance<TransitionGUI>();
                                        transition.InitTransitionGUI(currentElem, selectednode, currentElem.nodes[selectIndex]);

                                        ((FSM)currentElem).AddTransition(transition);
                                    }

                                    makeTransitionMode = false;
                                    selectednode = null;
                                }

                                if (!clickedOnWindow)
                                {
                                    makeTransitionMode = false;
                                    selectednode = null;
                                }

                                e.Use();
                            }
                            if (reconnectTransitionMode)
                            {
                                if (clickedOnWindow
                                    && !currentElem.nodes[selectIndex].Equals(selectedTransition.toNode)
                                    && !currentElem.transitions.Exists(t => !t.isExit && t.fromNode.Equals(selectedTransition.fromNode) && t.toNode.Equals(currentElem.nodes[selectIndex])))
                                {
                                    NodeEditorUtilities.GenerateUndoStep(currentElem);

                                    selectedTransition.toNode = currentElem.nodes[selectIndex];

                                    ((FSM)currentElem).CheckConnected();
                                }

                                reconnectTransitionMode = false;
                                cutObjects.Remove(selectedTransition);
                                selectedTransition = null;

                                e.Use();
                            }
                            break;
                        case nameof(BehaviourTree):
                            if (makeAttachedNode)
                            {
                                NodeEditorUtilities.GenerateUndoStep(currentElem);

                                toCreateNode.windowRect.position = new Vector2(mousePos.x, mousePos.y);
                                currentElem.nodes.Add((BehaviourNode)toCreateNode);

                                TransitionGUI transition = CreateInstance<TransitionGUI>();
                                transition.InitTransitionGUI(currentElem, selectednode, toCreateNode);

                                currentElem.transitions.Add(transition);

                                makeAttachedNode = false;
                                selectednode = null;
                                toCreateNode = null;

                                e.Use();
                            }
                            if (makeConnectionMode)
                            {
                                if (clickedOnWindow && !((BehaviourTree)currentElem).ConnectedCheck((BehaviourNode)selectednode, (BehaviourNode)currentElem.nodes[selectIndex]) && !decoratorWithOneChild && !(((BehaviourNode)currentElem.nodes[selectIndex]).type == behaviourType.Leaf))
                                {
                                    NodeEditorUtilities.GenerateUndoStep(currentElem);

                                    TransitionGUI transition = CreateInstance<TransitionGUI>();
                                    transition.InitTransitionGUI(currentElem, currentElem.nodes[selectIndex], selectednode);
                                    currentElem.transitions.Add(transition);

                                    ((BehaviourNode)selectednode).isRoot = false;
                                }

                                makeConnectionMode = false;
                                selectednode = null;

                                e.Use();
                            }
                            break;
                        case nameof(UtilitySystem):
                            if (makeAttachedNode)
                            {
                                NodeEditorUtilities.GenerateUndoStep(currentElem);

                                toCreateNode.windowRect.position = new Vector2(mousePos.x, mousePos.y);
                                currentElem.nodes.Add((UtilityNode)toCreateNode);

                                TransitionGUI transition = CreateInstance<TransitionGUI>();
                                transition.InitTransitionGUI(currentElem, toCreateNode, selectednode);

                                currentElem.transitions.Add(transition);

                                makeAttachedNode = false;
                                selectednode = null;
                                toCreateNode = null;

                                e.Use();
                            }
                            if (makeConnectionMode)
                            {
                                if (clickedOnWindow && !((UtilitySystem)currentElem).ConnectedCheck((UtilityNode)selectednode, (UtilityNode)currentElem.nodes[selectIndex]) && !actionWithOneFactor && !curveWithOneFactor && !(((UtilityNode)currentElem.nodes[selectIndex]).type == utilityType.Variable))
                                {
                                    NodeEditorUtilities.GenerateUndoStep(currentElem);

                                    TransitionGUI transition = CreateInstance<TransitionGUI>();
                                    transition.InitTransitionGUI(currentElem, selectednode, currentElem.nodes[selectIndex]);
                                    currentElem.transitions.Add(transition);
                                }

                                makeConnectionMode = false;
                                selectednode = null;

                                e.Use();
                            }
                            break;
                    }
            }
        }

        #endregion

        // Control for the events called by the keyboard
        #region Key Press Control

        if (e.type == EventType.KeyUp)
        {
            switch (e.keyCode)
            {
                case KeyCode.Delete:
                    if (makeTransitionMode || reconnectTransitionMode)
                    {
                        makeTransitionMode = false;
                        reconnectTransitionMode = false;
                        cutObjects.Remove(selectedTransition);
                        selectedTransition = null;
                        break;
                    }
                    if (focusedObjects.Count > 0 && GUIUtility.keyboardControl == 0)
                    {
                        PopupWindow.InitDelete(focusedObjects.ToArray());
                        e.Use();
                    }
                    break;
                case KeyCode.Escape:
                    if (makeTransitionMode || reconnectTransitionMode || makeAttachedNode || makeConnectionMode)
                    {
                        makeTransitionMode = false;
                        makeAttachedNode = false;
                        makeConnectionMode = false;
                        focusedObjects.Clear();

                        reconnectTransitionMode = false;
                        cutObjects.Remove(selectedTransition);
                        selectedTransition = null;
                        break;
                    }
                    currentElem = currentElem?.parent;
                    NodeEditorUtilities.ClearUndoSteps();
                    e.Use();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    if (GUIUtility.keyboardControl != 0)
                    {
                        GUI.FocusControl(null);
                        e.Use();
                    }
                    else if (focusedObjects.LastOrDefault() is ClickableElement)
                    {
                        currentElem = (ClickableElement)focusedObjects.LastOrDefault();
                        e.Use();
                    }
                    else if (((BaseNode)focusedObjects.LastOrDefault())?.subElem != null)
                    {
                        currentElem = ((BaseNode)focusedObjects.LastOrDefault()).subElem;
                        e.Use();
                    }
                    focusedObjects.Clear();
                    NodeEditorUtilities.ClearUndoSteps();
                    break;
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                    CtrlDown = false;
                    break;
            }
        }

        if (e.type == EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                    CtrlDown = true;
                    break;
                case KeyCode.S:
                    if (CtrlDown) SaveElem(currentElem);
                    break;
                case KeyCode.C:
                    if (CtrlDown) Copy();
                    break;
                case KeyCode.X:
                    if (CtrlDown) Cut();
                    break;
                case KeyCode.V:
                    if (CtrlDown) Paste();
                    break;
                case KeyCode.Z:
                    if (CtrlDown)
                    {
                        Undo();
                        e.Use();
                    }
                    break;
                case KeyCode.Y:
                    if (CtrlDown)
                    {
                        Redo();
                        e.Use();
                    }
                    break;
            }
        }

        #endregion

        // Draw the windows
        #region Windows Drawing (has to be done after key/mouse control)

        BeginWindows();

        if (currentElem)
        {
            switch (currentElem.GetType().ToString())
            {
                case nameof(FSM):
                    for (int i = 0; i < currentElem.nodes.Count; i++)
                    {
                        // Write how many errors are in this element
                        string errorAbb = "";
                        int errorCount = currentElem.nodes[i].subElem ? currentElem.nodes[i].subElem.GetErrors().Count : 0;
                        int warningCount = currentElem.nodes[i].subElem ? currentElem.nodes[i].subElem.GetWarnings().Count : 0;

                        if (errorCount + warningCount > 0)
                        {
                            errorAbb += "\n (";

                            if (errorCount > 0)
                            {
                                errorAbb += errorCount + " error/s";
                            }
                            if (warningCount > 0)
                            {
                                if (errorCount > 0)
                                    errorAbb += ", ";
                                errorAbb += warningCount + " warning/s";
                            }

                            errorAbb += ")";
                        }

                        currentElem.nodes[i].windowRect = GUI.Window(i, currentElem.nodes[i].windowRect, DrawNodeWindow, currentElem.nodes[i].GetTypeString() + errorAbb, new GUIStyle(Styles.SubTitleText)
                        {
                            normal = new GUIStyleState()
                            {
                                background = GetBackground(currentElem.nodes[i])
                            }
                        });
                    }

                    for (int i = 0; i < currentElem.transitions.Count; i++)
                    {
                        Vector2 offset = Vector2.zero;
                        Rect transitionRect;
                        TransitionGUI elem = currentElem.transitions[i];

                        if (elem.isExit || elem.fromNode is null || elem.toNode is null)
                            continue;

                        // If there is two transitions with the same two nodes (->)
                        //                                                     (<-)
                        if (currentElem.transitions.Exists(t => t.fromNode && t.toNode && t.fromNode.Equals(elem.toNode) && t.toNode.Equals(elem.fromNode)))
                        {
                            float ang = Vector2.SignedAngle(elem.toNode.windowRect.position - elem.fromNode.windowRect.position, Vector2.right);

                            if (ang > -45 && ang <= 45)
                            {
                                offset.y = doubleTransRectOffset;
                                offset.x = doubleTransRectOffset;
                            }
                            else if (ang > 45 && ang <= 135)
                            {
                                offset.x = doubleTransRectOffset;
                                offset.y = -doubleTransRectOffset;
                            }
                            else if ((ang > 135 && ang <= 180) || (ang > -180 && ang <= -135))
                            {
                                offset.y = -doubleTransRectOffset;
                                offset.x = -doubleTransRectOffset;
                            }
                            else if (ang > -135 && ang <= -45)
                            {
                                offset.x = -doubleTransRectOffset;
                                offset.y = doubleTransRectOffset;
                            }
                        }

                        // If it's a loop transition
                        if (elem.fromNode.Equals(elem.toNode))
                        {
                            offset.x = 100;
                            offset.y = 50;
                        }

                        Vector2 pos = new Vector2(elem.fromNode.windowRect.center.x + (elem.toNode.windowRect.x - elem.fromNode.windowRect.x) / 2,
                                                  elem.fromNode.windowRect.center.y + (elem.toNode.windowRect.y - elem.fromNode.windowRect.y) / 2);
                        transitionRect = new Rect(pos.x - 75, pos.y - 15, elem.width, elem.height);
                        transitionRect.position += offset;

                        elem.windowRect = GUI.Window(i + currentElem.nodes.Count, transitionRect, DrawTransitionBox, "", new GUIStyle(Styles.SubTitleText)
                        {
                            normal = new GUIStyleState()
                            {
                                background = GetBackground(elem)
                            }
                        });
                    }
                    break;
                case nameof(BehaviourTree):
                    for (int i = 0; i < currentElem.nodes.Count; i++)
                    {
                        string displayName = "";
                        if (((BehaviourNode)currentElem.nodes[i]).type > behaviourType.Selector)
                            displayName = currentElem.nodes[i].GetTypeString();

                        // Write how many errors are in this element
                        int errorCount = currentElem.nodes[i].subElem ? currentElem.nodes[i].subElem.GetErrors().Count : 0;
                        int warningCount = currentElem.nodes[i].subElem ? currentElem.nodes[i].subElem.GetWarnings().Count : 0;

                        if (errorCount + warningCount > 0)
                        {
                            displayName += "\n (";

                            if (errorCount > 0)
                            {
                                displayName += errorCount + " error/s";
                            }
                            if (warningCount > 0)
                            {
                                if (errorCount > 0)
                                    displayName += ", ";
                                displayName += warningCount + " warning/s";
                            }

                            displayName += ")";
                        }

                        currentElem.nodes[i].windowRect = GUI.Window(i, currentElem.nodes[i].windowRect, DrawNodeWindow, displayName, new GUIStyle(Styles.SubTitleText)
                        {
                            normal = new GUIStyleState()
                            {
                                background = GetBackground(currentElem.nodes[i])
                            }
                        });
                    }

                    foreach (TransitionGUI elem in currentElem.transitions.Where(t => (((BehaviourNode)t.fromNode).type == behaviourType.Sequence && !((BehaviourNode)t.fromNode).isRandom) || ((BehaviourNode)t.fromNode).type == behaviourType.Selector && ((BehaviourTree)currentElem).ChildrenCount((BehaviourNode)t.fromNode) > 1))
                    {
                        if (elem.isExit || elem.fromNode is null || elem.toNode is null)
                            continue;

                        Vector2 pos = new Vector2(elem.toNode.windowRect.x, elem.toNode.windowRect.y);
                        Rect transitionRect = new Rect(pos.x, pos.y - 30, 70, 25);

                        elem.windowRect = GUI.Window(currentElem.transitions.IndexOf(elem) + currentElem.nodes.Count, transitionRect, DrawTransitionBox, "", new GUIStyle(Styles.SubTitleText)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            normal = new GUIStyleState()
                            {
                                //background = GetBackground(elem)
                            }
                        });
                    }
                    break;
                case nameof(UtilitySystem):
                    for (int i = 0; i < currentElem.nodes.Count; i++)
                    {
                        string displayName = "";
                        if (((UtilityNode)currentElem.nodes[i]).type == utilityType.Action)
                        {
                            displayName = currentElem.nodes[i].GetTypeString();

                            // Write how many errors are in this element
                            int errorCount = currentElem.nodes[i].subElem ? currentElem.nodes[i].subElem.GetErrors().Count : 0;
                            int warningCount = currentElem.nodes[i].subElem ? currentElem.nodes[i].subElem.GetWarnings().Count : 0;

                            if (errorCount + warningCount > 0)
                            {
                                displayName += "\n (";

                                if (errorCount > 0)
                                {
                                    displayName += errorCount + " error/s";
                                }
                                if (warningCount > 0)
                                {
                                    if (errorCount > 0)
                                        displayName += ", ";
                                    displayName += warningCount + " warning/s";
                                }

                                displayName += ")";
                            }
                        }

                        currentElem.nodes[i].windowRect = GUI.Window(i, currentElem.nodes[i].windowRect, DrawNodeWindow, displayName, new GUIStyle(Styles.SubTitleText)
                        {
                            normal = new GUIStyleState()
                            {
                                background = GetBackground(currentElem.nodes[i])
                            }
                        });
                    }

                    foreach (TransitionGUI elem in currentElem.transitions.Where(t => ((UtilityNode)t.toNode).type == utilityType.Fusion && ((UtilityNode)t.toNode).fusionType == fusionType.Weighted))
                    {
                        if (elem.isExit || elem.fromNode is null || elem.toNode is null)
                            continue;

                        Vector2 pos = new Vector2(elem.fromNode.windowRect.x, elem.fromNode.windowRect.y);
                        Rect transitionRect = new Rect(pos.x, pos.y - 30, 70, 25);

                        elem.windowRect = GUI.Window(currentElem.transitions.IndexOf(elem) + currentElem.nodes.Count, transitionRect, DrawTransitionBox, "", new GUIStyle(Styles.SubTitleText)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            normal = new GUIStyleState()
                            {
                                //background = GetBackground(elem)
                            }
                        });
                    }
                    break;
            }

            // Exit Transition
            TransitionGUI exitTrans = currentElem.transitions.Find(t => t.isExit);

            if (exitTrans)
            {
                Vector2 exitPos = new Vector2(exitTrans.fromNode.windowRect.center.x,
                                          exitTrans.fromNode.windowRect.center.y + exitTrans.fromNode.windowRect.height / 2 + exitTransVerticalOffset);
                Rect transitionRect = new Rect(exitPos.x - exitTrans.windowRect.width / 2, exitPos.y, exitTrans.width, exitTrans.height);

                exitTrans.windowRect = GUI.Window(currentElem.transitions.IndexOf(exitTrans) + currentElem.nodes.Count, transitionRect, DrawTransitionBox, "", new GUIStyle(Styles.SubTitleText)
                {
                    normal = new GUIStyleState()
                    {
                        background = GetBackground(exitTrans)
                    }
                });
            }
        }
        else
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                // Write how many errors are in this element
                string errorAbb = "";
                int errorCount = Elements[i].GetErrors().Count;
                int warningCount = Elements[i].GetWarnings().Count;

                if (errorCount + warningCount > 0)
                {
                    errorAbb += "\n (";

                    if (errorCount > 0)
                    {
                        errorAbb += errorCount + " error/s";
                    }
                    if (warningCount > 0)
                    {
                        if (errorCount > 0)
                            errorAbb += ", ";
                        errorAbb += warningCount + " warning/s";
                    }

                    errorAbb += ")";
                }

                Elements[i].windowRect = GUI.Window(i, Elements[i].windowRect, DrawElementWindow, Elements[i].GetTypeString() + errorAbb, new GUIStyle(Styles.SubTitleText)
                {
                    normal = new GUIStyleState()
                    {
                        background = GetBackground(Elements[i])
                    }
                });
            }
        }

        EndWindows();

        #endregion

        Repaint();
    }

    private void OnDestroy()
    {
        NodeEditorUtilities.ClearUndoSteps();
        GetWindow<PopupWindow>().Close();
    }

    /// <summary>
    /// Checks what the <see cref="mousePos"/> overlaps with and returns the necessary info
    /// </summary>
    /// <returns>An array of booleans (ints of 2 values) and 1 int are used to determine what element the user clicked on</returns>
    private int[] ClickedOnCheck()
    {
        int clickedOnElement = 0;
        int clickedOnWindow = 0;
        int clickedOnLeaf = 0;
        int clickedOnVariable = 0;
        int decoratorWithOneChild = 0;
        int actionWithOneFactor = 0;
        int curveWithOneFactor = 0;
        int nodeWithAscendants = 0;
        int clickedOnTransition = 0;
        int selectIndex = -1;

        if (currentElem is null)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnElement = 1;
                    break;
                }
            }
        }

        if (currentElem is FSM)
        {
            for (int i = 0; i < currentElem.nodes.Count; i++)
            {
                if (currentElem.nodes[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = 1;
                    break;
                }
            }

            for (int i = 0; i < currentElem.transitions.Count; i++)
            {
                if (currentElem.transitions[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnTransition = 1;
                    break;
                }
            }
        }

        if (currentElem is BehaviourTree)
        {
            for (int i = 0; i < currentElem.nodes.Count; i++)
            {
                if (currentElem.nodes[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = 1;

                    if (currentElem.transitions.Exists(t => t.toNode.Equals(currentElem.nodes[i])))
                        nodeWithAscendants = 1;

                    if (((BehaviourNode)currentElem.nodes[i]).type == behaviourType.Leaf)
                        clickedOnLeaf = 1;

                    else if (((BehaviourNode)currentElem.nodes[i]).type >= behaviourType.LoopN && currentElem.transitions.Exists(t => t.fromNode.Equals(currentElem.nodes[i])))
                        decoratorWithOneChild = 1;

                    break;
                }
            }

            for (int i = 0; i < currentElem.transitions.Count; i++)
            {
                if (currentElem.transitions[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnTransition = 1;
                    break;
                }
            }
        }

        if (currentElem is UtilitySystem)
        {
            for (int i = 0; i < currentElem.nodes.Count; i++)
            {
                if (currentElem.nodes[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = 1;

                    if (((UtilityNode)currentElem.nodes[i]).type == utilityType.Variable)
                        clickedOnVariable = 1;

                    else if (((UtilityNode)currentElem.nodes[i]).type == utilityType.Action && currentElem.transitions.Exists(t => !t.isExit && t.toNode.Equals(currentElem.nodes[i])))
                        actionWithOneFactor = 1;

                    else if (((UtilityNode)currentElem.nodes[i]).type == utilityType.Curve && currentElem.transitions.Exists(t => t.toNode.Equals(currentElem.nodes[i])))
                        curveWithOneFactor = 1;

                    break;
                }
            }

            for (int i = 0; i < currentElem.transitions.Count; i++)
            {
                if (currentElem.transitions[i].windowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnTransition = 1;
                    break;
                }
            }
        }

        return new int[]
        {
            clickedOnElement,
            clickedOnWindow,
            clickedOnLeaf,
            clickedOnVariable,
            decoratorWithOneChild,
            actionWithOneFactor,
            curveWithOneFactor,
            nodeWithAscendants,
            clickedOnTransition,
            selectIndex
        };
    }

    /// <summary>
    /// Draws the color legend
    /// </summary>
    private void ShowColorLegend()
    {
        float rightOffset = 200;
        float bottomOffset = 100;
        float itemWidth = 15;
        float itemHeight = 5;
        string content = "";

        Rect panel = new Rect(position.width - (rightOffset + 10), position.height - (bottomOffset + 35), rightOffset, bottomOffset);
        float leftPos = panel.xMin + 10;
        float topStartPos = panel.yMin + 3;

        // Write the content of the legend
        if (currentElem)
            switch (currentElem.GetType().ToString())
            {
                case nameof(FSM):
                    content = "Entry State\nConnected State\nDisconnected State\nTransition Box\nExit Transition Box";
                    break;
                case nameof(BehaviourTree):
                    content = "Sequence Node\nSelector node\nLeaf Node\nDecorator Node\nExit Transition Box";
                    break;
                case nameof(UtilitySystem):
                    content = "Action Node\nFusion Node\nCurve Node\nVariable Node\nExit Transition Box";
                    break;
            }
        else
            content = "FSM\nBehaviour Tree\nUtility System";

        // Draw panel
        GUI.Box(panel, content, new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.UpperLeft,
            contentOffset = new Vector2(30, 10),
            normal = new GUIStyleState()
            {
                background = GetLegendRect("Box", null)
            },
        });

        // Draw the little rects with the colors
        if (currentElem)
        {
            switch (currentElem.GetType().ToString())
            {
                case nameof(FSM):
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(StateNode), stateType.Entry)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(StateNode), stateType.Default)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(StateNode), stateType.Unconnected)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(TransitionGUI), false)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(TransitionGUI), true)
                        },
                    });
                    break;
                case nameof(BehaviourTree):
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(BehaviourNode), behaviourType.Sequence)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(BehaviourNode), behaviourType.Selector)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(BehaviourNode), behaviourType.Leaf)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(BehaviourNode), behaviourType.LoopN)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(TransitionGUI), true)
                        },
                    });
                    break;
                case nameof(UtilitySystem):
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(UtilityNode), utilityType.Action)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(UtilityNode), utilityType.Fusion)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(UtilityNode), utilityType.Curve)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(UtilityNode), utilityType.Variable)
                        },
                    });
                    GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
                    {
                        normal = new GUIStyleState()
                        {
                            background = GetLegendRect(nameof(TransitionGUI), true)
                        },
                    });
                    break;
            }
        }
        else
        {
            GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = GetLegendRect(nameof(FSM), null)
                },
            });
            GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = GetLegendRect(nameof(BehaviourTree), null)
                },
            });
            GUI.Box(new Rect(leftPos, topStartPos += 15, itemWidth, itemHeight), "", new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = GetLegendRect(nameof(UtilitySystem), null)
                },
            });
        }
    }

    /// <summary>
    /// Draws the top bar elements
    /// </summary>
    private void ShowTopBar()
    {
        // Top Bar
        topBarOffset = 0;
        float yOffset = 0;
        var name = editorTitle;

        if (currentElem != null)
        {
            ShowButtonRecursive(Styles.TopBarButton, currentElem, editorTitle, yOffset);
            if (currentElem != null)
                name = currentElem.elementName;
        }

        var labelWidth = 25 + name.ToCharArray().Length * 6;
        GUI.Label(new Rect(topBarOffset, yOffset, labelWidth, 20), name);
    }

    /// <summary>
    /// Draws the Options button and its content
    /// </summary>
    private void ShowOptions()
    {
        if (GUI.Button(new Rect(position.width - 30, 0, 25, 20), "...", Styles.TopBarButton))
        {
            // Set menu items
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Manual de usuario"), false, () => Application.OpenURL("file:///" + Application.dataPath + "/Intelligent%20Behaviours%20GUI%20Package/Guia%20de%20uso.pdf"));

            if (currentElem != null)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Save Element to file"), false, SaveElem, currentElem);
                menu.AddItem(new GUIContent("Export Code"), false, ExportCode, currentElem);
            }

            menu.ShowAsContext();
        }
    }

    /// <summary>
    /// Shows the buttons with the names of the elements in order of hierarchy
    /// </summary>
    /// <param name="style"></param>
    /// <param name="elem"></param>
    /// <param name="name"></param>
    private void ShowButtonRecursive(GUIStyle style, ClickableElement elem, string name, float yOffset)
    {
        if (elem.parent != null)
        {
            ShowButtonRecursive(style, elem.parent, name, yOffset);
            name = elem.parent.elementName;
        }
        var buttonWidth = 25 + name.ToCharArray().Length * 6;
        if (GUI.Button(new Rect(topBarOffset, yOffset, buttonWidth, 20), name, style))
        {
            currentElem = elem.parent;
        }
        topBarOffset += buttonWidth;
        GUI.Label(new Rect(topBarOffset, yOffset, 15, 20), ">", new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperLeft
        });
        topBarOffset += 12;
    }

    /// <summary>
    /// Configures the Texture using the sprite resources and returns it
    /// </summary>
    /// <param name="elem"></param>
    /// <returns></returns>
    private Texture2D GetBackground(GUIElement elem)
    {
        var isFocused = focusedObjects.Contains(elem);
        var isCut = cutObjects.Contains(elem);
        Color col = Color.white;
        Texture2D originalTexture = null;

        switch (elem.GetType().ToString())
        {
            // FSM
            case nameof(FSM):
                originalTexture = Resources.Load<Texture2D>("FSM_Rect");
                col = Color.blue;
                break;

            // BT
            case nameof(BehaviourTree):
                originalTexture = Resources.Load<Texture2D>("BT_Rect");
                col = Color.cyan;
                break;

            // US
            case nameof(UtilitySystem):
                originalTexture = Resources.Load<Texture2D>("US_Rect");
                col = new Color(0, 0.75f, 0, 1); //dark green
                break;

            // FSM Node
            case nameof(StateNode):
                stateType StType = ((StateNode)elem).type;

                // Nodo normal
                if (((StateNode)elem).subElem == null)
                {
                    switch (StType)
                    {
                        case stateType.Default:
                            originalTexture = Resources.Load<Texture2D>("Def_Node_Rect");
                            col = Color.grey;
                            break;
                        case stateType.Entry:
                            originalTexture = Resources.Load<Texture2D>("Entry_Rect");
                            col = Color.green;
                            break;
                        case stateType.Unconnected:
                            originalTexture = Resources.Load<Texture2D>("Unconnected_Node_Rect");
                            col = Color.red;
                            break;
                        default:
                            col = Color.white;
                            break;
                    }
                }
                // Nodo con sub-elemento
                else
                {
                    switch (StType)
                    {
                        case stateType.Default:
                            originalTexture = Resources.Load<Texture2D>("Def_Sub_Rect");
                            col = Color.grey;
                            break;
                        case stateType.Entry:
                            originalTexture = Resources.Load<Texture2D>("Entry_Sub_Rect");
                            col = Color.green;
                            break;
                        case stateType.Unconnected:
                            originalTexture = Resources.Load<Texture2D>("Unconnected_Sub_Rect");
                            col = Color.red;
                            break;
                        default:
                            col = Color.white;
                            break;
                    }
                }
                break;

            // BehaviourNode
            case nameof(BehaviourNode):
                behaviourType BNType = ((BehaviourNode)elem).type;

                switch (BNType)
                {
                    case behaviourType.Sequence:
                        originalTexture = Resources.Load<Texture2D>("Sequence_Rect");
                        col = Color.yellow;
                        break;
                    case behaviourType.Selector:
                        originalTexture = Resources.Load<Texture2D>("Selector_Rect");
                        col = new Color(1, 0.5f, 0, 1); //orange
                        break;
                    case behaviourType.Leaf:
                        if (((BehaviourNode)elem).subElem == null) //Es un nodo normal
                        {
                            originalTexture = Resources.Load<Texture2D>("Leaf_Rect");
                        }
                        else //Es un subelemento
                        {
                            originalTexture = Resources.Load<Texture2D>("Leaf_Sub_Rect");
                        }
                        col = new Color(0, 0.75f, 0, 1); //dark green
                        break;
                    case behaviourType.LoopN:
                    case behaviourType.LoopUntilFail:
                    case behaviourType.Inverter:
                    case behaviourType.DelayT:
                    case behaviourType.Succeeder:
                    case behaviourType.Conditional:
                        originalTexture = Resources.Load<Texture2D>("Decorator_Rect"); //Hacer un rombo gris
                                                                                       //col = Color.grey;
                        break;
                    default:
                        col = Color.white;
                        break;
                }
                break;

            // UtilityNode
            case nameof(UtilityNode):
                utilityType UNType = ((UtilityNode)elem).type;

                switch (UNType)
                {
                    case utilityType.Variable:
                        originalTexture = Resources.Load<Texture2D>("Variable_Rect");
                        col = new Color(1, 0.5f, 0, 1); //orange
                        break;
                    case utilityType.Fusion:
                        originalTexture = Resources.Load<Texture2D>("Fusion_Rect");
                        col = Color.yellow;
                        break;
                    case utilityType.Action:
                        if (((UtilityNode)elem).subElem == null) //Es un nodo normal
                        {
                            originalTexture = Resources.Load<Texture2D>("Leaf_Rect");
                        }
                        else //Es un subelemento
                        {
                            originalTexture = Resources.Load<Texture2D>("Leaf_Sub_Rect");
                        }
                        col = new Color(0, 0.75f, 0, 1); //dark green
                        break;
                    case utilityType.Curve:
                        if (((UtilityNode)elem).openFoldout)
                        {
                            originalTexture = Resources.Load<Texture2D>("Curve_Rect_Unfolded");
                        }
                        else
                        {
                            originalTexture = Resources.Load<Texture2D>("Curve_Rect_Folded");
                        }
                        col = Color.blue;
                        break;
                    default:
                        col = Color.white;
                        break;
                }
                break;

            // FSM Transition
            case nameof(TransitionGUI):
                if (((TransitionGUI)elem).isExit)
                    originalTexture = Resources.Load<Texture2D>("Exit_Transition_Rect");
                else
                    originalTexture = Resources.Load<Texture2D>("Transition_Rect");
                col = Color.yellow;
                break;
            default:
                col = Color.clear;
                break;
        }

        // Copy the texture, so we don't override its original colors permanently
        Texture2D resultTexture = originalTexture is null ? null : new Texture2D(originalTexture.width, originalTexture.height);

        // If no texture has been found, use a simple colored Rect
        if (originalTexture == null)
        {
            Color[] pix = new Color[2 * 2];

            //Make it look semitransparent when not selected
            if (!isFocused)
                col.a = 0.5f;
            if (isCut)
                col.a = 0.2f;

            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            resultTexture = new Texture2D(2, 2);
            resultTexture.SetPixels(pix);
            resultTexture.Apply();
        }
        else
        {
            Color32[] pixels = originalTexture.GetPixels32();

            if (makeConnectionMode)
            {
                if (currentElem is BehaviourTree)
                {
                    if (((BehaviourTree)currentElem).ConnectedCheck((BehaviourNode)selectednode, (BehaviourNode)elem) || selectednode.Equals(elem) || ((BehaviourNode)elem).type == behaviourType.Leaf || ((BehaviourNode)elem).type >= behaviourType.LoopN && currentElem.transitions.Exists(t => t.fromNode.Equals(elem)))
                    {
                        //Make it look transparent when not connectable to connect mode
                        for (int i = 0; i < pixels.Length; i++)
                        {
                            pixels[i].a = (byte)(pixels[i].a * 64 / 255);
                        }
                    }
                }
                if (currentElem is UtilitySystem)
                {
                    if (elem is UtilityNode)
                    {
                        if (((UtilitySystem)currentElem).ConnectedCheck((UtilityNode)selectednode, (UtilityNode)elem) ||
                            selectednode.Equals(elem) || ((UtilityNode)elem).type == utilityType.Variable ||
                            ((UtilityNode)elem).type == utilityType.Action && currentElem.transitions.Exists(t => t.toNode.Equals(elem)) ||
                            ((UtilityNode)elem).type == utilityType.Curve && currentElem.transitions.Exists(t => t.toNode.Equals(elem)))
                        {
                            //Make it look transparent when not connectable to connect mode
                            for (int i = 0; i < pixels.Length; i++)
                            {
                                pixels[i].a = (byte)(pixels[i].a * 64 / 255);
                            }
                        }
                    }
                    else
                    {
                        //Make it look transparent when not connectable to connect mode
                        for (int i = 0; i < pixels.Length; i++)
                        {
                            pixels[i].a = (byte)(pixels[i].a * 64 / 255);
                        }
                    }
                }
            }
            else if (!isFocused)
            {
                //Make it look semitransparent when not selected
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i].a = (byte)(pixels[i].a * 127 / 255);
                }
            }

            if (isCut)
            {
                //Make it look even more transparent when it's being cut
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i].a = (byte)(pixels[i].a * 64 / 255);
                }
            }

            resultTexture.SetPixels32(pixels);
            resultTexture.Apply();
        }

        return resultTexture;
    }

    /// <summary>
    /// Configures the Texture using the sprite resources and returns it
    /// </summary>
    /// <param name="elem"></param>
    /// <returns></returns>
    private Texture2D GetLegendRect(string type, object subType)
    {
        Color col = Color.white;

        switch (type)
        {
            // Main box
            case "Box":
                col = new Color(0, 0, 0, 0.1f);
                break;

            // FSM
            case nameof(FSM):
                col = Color.blue;
                break;

            // BT
            case nameof(BehaviourTree):
                col = Color.cyan;
                break;

            // US
            case nameof(UtilitySystem):
                col = new Color(0, 0.75f, 0, 1); //dark green
                break;

            // FSM Node
            case nameof(StateNode):
                stateType StType = (stateType)subType;

                switch (StType)
                {
                    case stateType.Default:
                        col = Color.grey;
                        break;
                    case stateType.Entry:
                        col = Color.green;
                        break;
                    case stateType.Unconnected:
                        col = Color.red;
                        break;
                    default:
                        col = Color.white;
                        break;
                }
                break;

            // BehaviourNode
            case nameof(BehaviourNode):
                behaviourType BNType = (behaviourType)subType;

                switch (BNType)
                {
                    case behaviourType.Sequence:
                        col = Color.yellow;
                        break;
                    case behaviourType.Selector:
                        col = new Color(1, 0.5f, 0, 1); //orange
                        break;
                    case behaviourType.Leaf:
                        col = new Color(0, 0.75f, 0, 1); //dark green
                        break;
                    case behaviourType.LoopN:
                    case behaviourType.LoopUntilFail:
                    case behaviourType.Inverter:
                    case behaviourType.DelayT:
                    case behaviourType.Succeeder:
                    case behaviourType.Conditional:
                        col = Color.grey;
                        break;
                    default:
                        col = Color.white;
                        break;
                }
                break;

            // UtilityNode
            case nameof(UtilityNode):
                utilityType UNType = (utilityType)subType;

                switch (UNType)
                {
                    case utilityType.Variable:
                        col = new Color(1, 0.5f, 0, 1); //orange
                        break;
                    case utilityType.Fusion:
                        col = Color.yellow;
                        break;
                    case utilityType.Action:
                        col = new Color(0, 0.75f, 0, 1); //dark green
                        break;
                    case utilityType.Curve:
                        col = Color.blue;
                        break;
                    default:
                        col = Color.white;
                        break;
                }
                break;

            // FSM Transition
            case nameof(TransitionGUI):
                if ((bool)subType)
                    col = new Color(0.37f, 0, 0.5f, 1); //purple
                else
                    col = Color.yellow;
                break;
            default:
                col = Color.clear;
                break;
        }

        Color[] pix = new Color[2 * 2];

        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }

        Texture2D resultTexture = new Texture2D(2, 2);
        resultTexture.SetPixels(pix);
        resultTexture.Apply();

        return resultTexture;
    }

    /// <summary>
    /// The DrawNodeWindow
    /// </summary>
    /// <param name="id"></param>
    void DrawNodeWindow(int id)
    {
        if (currentElem is FSM && currentElem.nodes.Count > id)
        {
            currentElem.nodes[id].DrawWindow();
            if (currentElem.nodes[id].subElem != null)
                currentElem.nodes[id].subElem.elementName = currentElem.nodes[id].nodeName;
        }
        if (currentElem is BehaviourTree && currentElem.nodes.Count > id)
        {
            currentElem.nodes[id].DrawWindow();
            if (currentElem.nodes[id].subElem != null)
                currentElem.nodes[id].subElem.elementName = currentElem.nodes[id].nodeName;
        }
        if (currentElem is UtilitySystem && currentElem.nodes.Count > id)
        {
            currentElem.nodes[id].DrawWindow();
            if (currentElem.nodes[id].subElem != null)
                currentElem.nodes[id].subElem.elementName = currentElem.nodes[id].nodeName;
        }

        GUI.DragWindow();
    }

    /// <summary>
    /// The DrawElementWindow
    /// </summary>
    /// <param name="id"></param>
    void DrawElementWindow(int id)
    {
        if (Elements.Count > id)
            Elements[id].DrawWindow();

        GUI.DragWindow();
    }

    /// <summary>
    /// The DrawTransitionBox
    /// </summary>
    /// <param name="id"></param>
    void DrawTransitionBox(int id)
    {
        if (currentElem && currentElem.transitions.Count > id - currentElem.nodes.Count)
        {
            currentElem.transitions[id - currentElem.nodes.Count].DrawWindow();
        }

        GUI.DragWindow();
    }

    /// <summary>
    /// Performs an action depending on the given <paramref name="data"/>
    /// </summary>
    /// <param name="data"></param>
    void ContextCallback(object data)
    {
        string[] dataList = (string[])data;
        string order = "";
        int index = 0;
        bool isTransition = false;

        switch (dataList.Count())
        {
            case 3:
                isTransition = bool.Parse(dataList[2]);
                goto case 2;
            case 2:
                index = int.Parse(dataList[1]);
                goto case 1;
            case 1:
                order = dataList[0];
                break;
        }

        switch (order)
        {
            case "FSM":
                CreateFSM(index, mousePos.x, mousePos.y);
                break;
            case "BT":
                CreateBT(index, mousePos.x, mousePos.y);
                break;
            case "US":
                CreateUS(index, mousePos.x, mousePos.y);
                break;
            case "Node":
                CreateNode(mousePos.x, mousePos.y);
                break;
            case "Sequence":
                CreateSequence(index, mousePos.x, mousePos.y);
                break;
            case "Selector":
                CreateSelector(index, mousePos.x, mousePos.y);
                break;
            case "leafNode":
                CreateLeafNode(2, index);
                break;
            case "loopN":
                CreateLeafNode(3, index);
                break;
            case "loopUFail":
                CreateLeafNode(4, index);
                break;
            case "inverter":
                CreateLeafNode(5, index);
                break;
            case "timer":
                CreateLeafNode(6, index);
                break;
            case "succeeder":
                CreateLeafNode(7, index);
                break;
            case "conditional":
                CreateLeafNode(8, index);
                break;
            case "Variable":
                CreateVariable(index, mousePos.x, mousePos.y);
                break;
            case "Fusion":
                CreateFusion(index, mousePos.x, mousePos.y);
                break;
            case "Action":
                CreateAction(mousePos.x, mousePos.y);
                break;
            case "Curve":
                CreateCurve(index, mousePos.x, mousePos.y);
                break;
            case "makeTransition":
                MakeTransition(index);
                break;
            case "reconnectTransition":
                ReconnectTransition(index);
                break;
            case "flipTransition":
                FlipTransition(index);
                break;
            case "deleteElement":
                DeleteElement(index, isTransition);
                break;
            case "entryState":
                ConvertToEntry(index);
                break;
            case "disconnectNode":
                DisconnectNode(index);
                break;
            case "connectNode":
                ConnectNode(index);
                break;
            case "exitTrans":
                MakeExitTransition(index);
                break;
        }
    }

    /// <summary>
    /// Calls the utility function to export the code of the <paramref name="elem"/> if there's no errors
    /// </summary>
    void ExportCode(object elem)
    {
        if (((ClickableElement)elem).GetErrors().Count == 0)
        {
            NodeEditorUtilities.GenerateElemScript((ClickableElement)elem);
        }
        else
        {
            PopupWindow.InitExport((ClickableElement)elem);
        }
    }

    /// <summary>
    /// Calls the utility function to save the <paramref name="elem"/>
    /// </summary>
    void SaveElem(object elem)
    {
        NodeEditorUtilities.GenerateElemXMLFile((ClickableElement)elem);
    }

    /// <summary>
    /// Opens the explorer to select a file, and loads it
    /// </summary>
    void LoadSaveFile()
    {
        XMLElement loadedXML = NodeEditorUtilities.LoadSavedData();

        NodeEditorUtilities.GenerateUndoStep(currentElem);

        LoadElem(loadedXML, currentElem);
    }

    /// <summary>
    /// Loads the XMLElement to the <paramref name="element"/>
    /// </summary>
    ClickableElement LoadElem(XMLElement loadedXML, ClickableElement element, bool withMousePos = true)
    {
        if (loadedXML != null)
        {
            var currentBackup = element;

            ClickableElement newElem;
            switch (loadedXML.elemType)
            {
                case nameof(FSM):
                    newElem = loadedXML.ToFSM(element, null);
                    break;
                case nameof(BehaviourTree):
                    newElem = loadedXML.ToBehaviourTree(element, null);
                    break;
                case nameof(UtilitySystem):
                    newElem = loadedXML.ToUtilitySystem(element, null);
                    break;
                default:
                    Debug.LogError("Wrong content in saved data");
                    return null;
            }

            if (withMousePos)
            {
                newElem.windowRect.x = mousePos.x;
                newElem.windowRect.y = mousePos.y;
            }

            element = currentBackup;

            if (element is null)
            {
                Elements.Add(newElem);
                editorNamer.AddName(newElem.identificator, newElem.elementName);
            }

            return newElem;
        }

        return null;
    }

    /// <summary>
    /// Deletes the <paramref name="elem"/>
    /// </summary>
    /// <param name="elem"></param>
    public void Delete(GUIElement elem, ClickableElement parentElement = null, bool skipUndoStep = false)
    {
        if (elem is null)
        {
            if (Debugger.isDebug)
                Debug.LogError("[Delete] Element was null");
            return;
        }

        if (elem is ClickableElement && parentElement)
        {
            BaseNode state = parentElement.nodes.Find(n => n.subElem && n.subElem.Equals(elem));
            Delete(state, parentElement, true);

            return;
        }

        if (!skipUndoStep)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);
        }

        if (parentElement is null)
        {
            parentElement = currentElem;
        }

        switch (elem.GetType().ToString())
        {
            case nameof(StateNode):
                StateNode stateNode = (StateNode)elem;
                ((FSM)parentElement).DeleteNode(stateNode, !skipUndoStep);

                focusedObjects.Remove(stateNode);
                break;

            case nameof(BehaviourNode):
                BehaviourNode behaviourNode = (BehaviourNode)elem;
                ((BehaviourTree)parentElement).DeleteNode(behaviourNode, !skipUndoStep);

                focusedObjects.Remove(behaviourNode);
                break;

            case nameof(UtilityNode):
                UtilityNode utilityNode = (UtilityNode)elem;
                ((UtilitySystem)parentElement).DeleteNode(utilityNode, !skipUndoStep);

                focusedObjects.Remove(utilityNode);
                break;

            case nameof(TransitionGUI):
                TransitionGUI transition = (TransitionGUI)elem;

                switch (parentElement.GetType().ToString())
                {
                    case nameof(FSM):
                        ((FSM)parentElement).DeleteTransition(transition);
                        focusedObjects.Remove(transition);
                        break;
                    case nameof(BehaviourTree):
                        ((BehaviourTree)parentElement).DeleteConnection(transition);
                        focusedObjects.Remove(transition);
                        break;
                    case nameof(UtilitySystem):
                        ((UtilitySystem)parentElement).DeleteConnection(transition);
                        focusedObjects.Remove(transition);
                        break;
                }
                break;

            case nameof(FSM):
            case nameof(BehaviourTree):
            case nameof(UtilitySystem):
                Elements.Remove((ClickableElement)elem);
                editorNamer.RemoveName(elem.identificator);

                focusedObjects.Remove(elem);
                break;
        }
    }

    /// <summary>
    /// Draws a stylized bezier curve from <paramref name="start"/> to <paramref name="end"/>
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="isFocused"></param>
    /// <param name="isDouble"></param>
    public static void DrawNodeCurve(Rect start, Rect end, bool isFocused, bool isDouble = false, bool isLoop = false, bool isExit = false)
    {
        // We add an offset so the start and end points are centered in their respective nodes

        start.x += start.width / 2;
        start.y += start.height / 2;

        end.x += end.width / 2;
        end.y += end.height / 2;

        // Check which sides to put the curve on

        float ang = Vector2.SignedAngle(end.position - start.position, Vector2.right);
        Vector3 direction = Vector3.right;
        Vector3 startDirection;
        Vector3 endDirection;

        if (isExit)
        {
            ang = -90;

            end.x = end.x;
            end.y = end.center.y + end.height / 2 + exitTransVerticalOffset;
        }

        if (isLoop)
        {
            startDirection = Vector3.right * 3;
            endDirection = Vector3.down * 3;

            start.x += start.width / 2;
            end.y += end.height / 2;
        }
        else
        {
            if (ang > -45 && ang <= 45)
            {
                direction = Vector3.right;

                start.x += start.width / 2;
                end.x -= end.width / 2;

                if (isDouble)
                {
                    start.y += doubleTransCurveOffset;
                    end.y += doubleTransCurveOffset;
                }
            }
            else if (ang > 45 && ang <= 135)
            {
                direction = Vector3.down;

                start.y -= start.height / 2;
                end.y += end.height / 2;

                if (isDouble)
                {
                    start.x += doubleTransCurveOffset;
                    end.x += doubleTransCurveOffset;
                }
            }
            else if ((ang > 135 && ang <= 180) || (ang > -180 && ang <= -135))
            {
                direction = Vector3.left;

                start.x -= start.width / 2;
                end.x += end.width / 2;

                if (isDouble)
                {
                    start.y -= doubleTransCurveOffset;
                    end.y -= doubleTransCurveOffset;
                }
            }
            else if (ang > -135 && ang <= -45)
            {
                direction = Vector3.up;

                start.y += start.height / 2;
                end.y -= end.height / 2;

                if (isDouble)
                {
                    start.x -= doubleTransCurveOffset;
                    end.x -= doubleTransCurveOffset;
                }
            }

            startDirection = direction;
            endDirection = direction;
        }

        // Draw curve
        // Curve parameters
        float tangentMagnitude = 50;

        if (isExit)
            tangentMagnitude = 0;

        Vector3 startPos = new Vector3(start.x, start.y, 0);
        Vector3 endPos = new Vector3(end.x, end.y, 0);
        Vector3 startTan = startPos + startDirection * tangentMagnitude;
        Vector3 endTan = endPos - endDirection * tangentMagnitude;

        // Arrow parameters
        Vector3 pos1 = endPos - endDirection.normalized * 10;
        Vector3 pos2 = endPos - endDirection.normalized * 10;

        if (endDirection.normalized == Vector3.up || endDirection.normalized == Vector3.down)
        {
            pos1.x += 6;
            pos2.x -= 6;
        }
        else
        {
            pos1.y += 6;
            pos2.y -= 6;
        }

        // Color
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        // Factor that determines how big the shadow/light of the curve will be
        int focusFactor = 3;

        // If the curve is focused it will look white, and with a bigger shadow to look more bright
        if (isFocused)
        {
            shadowCol = new Color(1, 1, 1, 0.1f);
            focusFactor = 10;
        }

        // Draw the whole thing a number of times with increasing width
        // so it looks like a shadow when unfocused and like a backlight when focused
        for (int i = 0; i < focusFactor; i++)
        {
            // Draw the curve
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);

            // Draw arrow
            Handles.DrawBezier(pos1, endPos, pos1, endPos, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(pos2, endPos, pos2, endPos, shadowCol, null, (i + 1) * 5);
        }

        // Draw it with width 1 (sharper looking)
        // Draw the curve
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);

        // Draw arrow
        Handles.DrawBezier(pos1, endPos, pos1, endPos, Color.black, null, 1);
        Handles.DrawBezier(pos2, endPos, pos2, endPos, Color.black, null, 1);
    }

    /// <summary>
    /// Creates a <see cref="FSM"/>
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateFSM(int nodeIndex, float posX, float posY)
    {
        FSM newFSM = CreateInstance<FSM>();
        newFSM.InitFSM(currentElem, posX, posY);

        if (currentElem is null)
        {
            Elements.Add(newFSM);
        }

        if (currentElem is FSM)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            StateNode node = CreateInstance<StateNode>();
            node.InitStateNode(currentElem, stateType.Unconnected, newFSM.windowRect.position.x, newFSM.windowRect.position.y, newFSM);

            if (!((FSM)currentElem).HasEntryState)
            {
                ((FSM)currentElem).AddEntryState(node);
            }
            else
            {
                currentElem.nodes.Add(node);
            }
        }

        if (currentElem is BehaviourTree)
        {
            BehaviourNode node = CreateInstance<BehaviourNode>();
            node.InitBehaviourNode(currentElem, behaviourType.Leaf, newFSM.windowRect.x, newFSM.windowRect.y, newFSM);

            selectednode = currentElem.nodes[nodeIndex];
            toCreateNode = node;
            makeAttachedNode = true;
        }

        if (currentElem is UtilitySystem)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            UtilityNode node = CreateInstance<UtilityNode>();
            node.InitUtilityNode(currentElem, utilityType.Action, posX, posY, newFSM);

            currentElem.nodes.Add(node);
        }
    }

    /// <summary>
    /// Creates a <see cref="BehaviourTree"/>
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateBT(int nodeIndex, float posX, float posY)
    {
        BehaviourTree newBT = CreateInstance<BehaviourTree>();
        newBT.InitBehaviourTree(currentElem, posX, posY);

        if (!string.IsNullOrEmpty(name))
        {
            newBT.elementName = name;
        }

        if (currentElem is null)
        {
            Elements.Add(newBT);
        }

        if (currentElem is FSM)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            StateNode node = CreateInstance<StateNode>();
            node.InitStateNode(currentElem, stateType.Unconnected, newBT.windowRect.position.x, newBT.windowRect.position.y, newBT);

            if (!((FSM)currentElem).HasEntryState)
            {
                ((FSM)currentElem).AddEntryState(node);
            }
            else
            {
                currentElem.nodes.Add(node);
            }
        }

        if (currentElem is BehaviourTree)
        {
            BehaviourNode node = CreateInstance<BehaviourNode>();
            node.InitBehaviourNode(currentElem, behaviourType.Leaf, newBT.windowRect.x, newBT.windowRect.y, newBT);

            selectednode = currentElem.nodes[nodeIndex];
            toCreateNode = node;
            makeAttachedNode = true;
        }

        if (currentElem is UtilitySystem)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            UtilityNode node = CreateInstance<UtilityNode>();
            node.InitUtilityNode(currentElem, utilityType.Action, posX, posY, newBT);

            currentElem.nodes.Add(node);
        }
    }

    /// <summary>
    /// Creates a <see cref="UtilitySystem"/>
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateUS(int nodeIndex, float posX, float posY)
    {
        UtilitySystem newUS = CreateInstance<UtilitySystem>();
        newUS.InitUtilitySystem(currentElem, posX, posY);

        if (!string.IsNullOrEmpty(name))
        {
            newUS.elementName = name;
        }

        if (currentElem is null)
        {
            Elements.Add(newUS);
        }

        if (currentElem is FSM)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            StateNode node = CreateInstance<StateNode>();
            node.InitStateNode(currentElem, stateType.Unconnected, newUS.windowRect.position.x, newUS.windowRect.position.y, newUS);

            if (!((FSM)currentElem).HasEntryState)
            {
                ((FSM)currentElem).AddEntryState(node);
            }
            else
            {
                currentElem.nodes.Add(node);
            }
        }

        if (currentElem is BehaviourTree)
        {
            BehaviourNode node = CreateInstance<BehaviourNode>();
            node.InitBehaviourNode(currentElem, behaviourType.Leaf, newUS.windowRect.x, newUS.windowRect.y, newUS);

            selectednode = currentElem.nodes[nodeIndex];
            toCreateNode = node;
            makeAttachedNode = true;
        }

        if (currentElem is UtilitySystem)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            UtilityNode node = CreateInstance<UtilityNode>();
            node.InitUtilityNode(currentElem, utilityType.Action, posX, posY, newUS);

            currentElem.nodes.Add(node);
        }
    }

    /// <summary>
    /// Creates a <see cref="StateNode"/>
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateNode(float posX, float posY)
    {
        NodeEditorUtilities.GenerateUndoStep(currentElem);

        StateNode node = CreateInstance<StateNode>();
        node.InitStateNode(currentElem, stateType.Unconnected, posX, posY);

        if (!((FSM)currentElem).HasEntryState)
        {
            ((FSM)currentElem).AddEntryState(node);
        }
        else
        {
            currentElem.nodes.Add(node);
        }
    }

    /// <summary>
    /// Creates a <see cref="BehaviourNode"/> of type Sequence
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateSequence(int nodeIndex, float posX = 50, float posY = 50)
    {
        BehaviourNode node = CreateInstance<BehaviourNode>();
        node.InitBehaviourNode(currentElem, behaviourType.Sequence, posX, posY);

        if (nodeIndex > -1)
        {
            selectednode = currentElem.nodes[nodeIndex];
            toCreateNode = node;
            makeAttachedNode = true;
        }
        else
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            node.isRoot = true;
            currentElem.nodes.Add(node);
        }
    }

    /// <summary>
    /// Creates a <see cref="BehaviourNode"/> of type Selector
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateSelector(int nodeIndex, float posX = 50, float posY = 50)
    {
        BehaviourNode node = CreateInstance<BehaviourNode>();
        node.InitBehaviourNode(currentElem, behaviourType.Selector, posX, posY);

        if (nodeIndex > -1)
        {
            selectednode = currentElem.nodes[nodeIndex];
            toCreateNode = node;
            makeAttachedNode = true;
        }
        else
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            node.isRoot = true;
            currentElem.nodes.Add(node);
        }
    }

    /// <summary>
    /// Creates a <see cref="BehaviourNode"/> of type Leaf
    /// </summary>
    /// <param name="type"></param>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateLeafNode(int type, int nodeIndex, float posX = 50, float posY = 50)
    {
        BehaviourNode node = CreateInstance<BehaviourNode>();
        node.InitBehaviourNode(currentElem, (behaviourType)type, posX, posY);

        selectednode = currentElem.nodes[nodeIndex];
        toCreateNode = node;
        makeAttachedNode = true;
    }

    /// <summary>
    /// Creates a <see cref="UtilityNode"/> of type Variable
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateVariable(int nodeIndex, float posX = 50, float posY = 50)
    {
        UtilityNode node = CreateInstance<UtilityNode>();
        node.InitUtilityNode(currentElem, utilityType.Variable, posX, posY);

        selectednode = currentElem.nodes[nodeIndex];
        toCreateNode = node;
        makeAttachedNode = true;
    }

    /// <summary>
    /// Creates a <see cref="UtilityNode"/> of type Fusion
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateFusion(int nodeIndex, float posX = 50, float posY = 50)
    {
        UtilityNode node = CreateInstance<UtilityNode>();
        node.InitUtilityNode(currentElem, utilityType.Fusion, posX, posY);

        selectednode = currentElem.nodes[nodeIndex];
        toCreateNode = node;
        makeAttachedNode = true;
    }

    /// <summary>
    /// Creates a <see cref="UtilityNode"/> of type Action
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateAction(float posX = 50, float posY = 50)
    {
        NodeEditorUtilities.GenerateUndoStep(currentElem);

        UtilityNode node = CreateInstance<UtilityNode>();
        node.InitUtilityNode(currentElem, utilityType.Action, posX, posY);

        currentElem.nodes.Add(node);
    }

    /// <summary>
    /// Creates a <see cref="UtilityNode"/> of type Curve
    /// </summary>
    /// <param name="nodeIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void CreateCurve(int nodeIndex, float posX = 50, float posY = 50)
    {
        UtilityNode node = CreateInstance<UtilityNode>();
        node.InitUtilityNode(currentElem, utilityType.Curve, posX, posY);

        selectednode = currentElem.nodes[nodeIndex];
        toCreateNode = node;
        makeAttachedNode = true;
    }

    /// <summary>
    /// Enter <see cref="makeTransitionMode"/> (mouse carries the other end of the transition until you click somewhere else)
    /// </summary>
    /// <param name="selectIndex"></param>
    private void MakeTransition(int selectIndex)
    {
        makeTransitionMode = true;

        if (currentElem is FSM)
            selectednode = currentElem.nodes[selectIndex];

        if (currentElem is BehaviourTree)
            selectednode = currentElem.nodes[selectIndex];
    }

    /// <summary>
    /// Enter <see cref="makeTransitionMode"/> like in <see cref="MakeTransition(int)"/> but with a transition selected for reconnection
    /// </summary>
    /// <param name="selectIndex"></param>
    private void ReconnectTransition(int selectIndex)
    {
        reconnectTransitionMode = true;

        if (currentElem is FSM)
        {
            selectedTransition = currentElem.transitions[selectIndex];
        }
    }

    /// <summary>
    /// Flip the transition (fromNode is toNode and viceversa)
    /// </summary>
    /// <param name="selectIndex"></param>
    private void FlipTransition(int selectIndex)
    {
        if (currentElem is FSM)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem);

            TransitionGUI transition = currentElem.transitions[selectIndex];

            TransitionGUI coupleTransition = currentElem.transitions.Find(t => t.fromNode.Equals(transition.toNode) && t.toNode.Equals(transition.fromNode));

            if (coupleTransition != null)
            {
                BaseNode tempNode1 = coupleTransition.fromNode;

                coupleTransition.fromNode = coupleTransition.toNode;
                coupleTransition.toNode = tempNode1;
            }

            BaseNode tempNode2 = transition.fromNode;

            transition.fromNode = transition.toNode;
            transition.toNode = tempNode2;

            ((FSM)currentElem).CheckConnected();
        }
    }

    /// <summary>
    /// Makes a <see cref="PopupWindow"/> appear that will delete the clicked node if accepted
    /// </summary>
    private void DeleteElement(int selectIndex, bool isTransition)
    {
        if (currentElem is null)
        {
            PopupWindow.InitDelete(Elements[selectIndex]);
        }
        else
        {
            if (isTransition)
            {
                PopupWindow.InitDelete(currentElem.transitions[selectIndex]);
            }
            else
            {
                PopupWindow.InitDelete(currentElem.nodes[selectIndex]);
            }
        }
    }

    /// <summary>
    /// Converts the clicked node into Entry State
    /// </summary>
    private void ConvertToEntry(int selectIndex)
    {
        NodeEditorUtilities.GenerateUndoStep(currentElem);

        ((FSM)currentElem).SetAsEntry((StateNode)currentElem.nodes[selectIndex]);
    }

    /// <summary>
    /// Disconnects the clicked node
    /// </summary>
    private void DisconnectNode(int selectIndex)
    {
        NodeEditorUtilities.GenerateUndoStep(currentElem);

        BehaviourNode selNode = (BehaviourNode)currentElem.nodes[selectIndex];

        foreach (TransitionGUI tr in currentElem.transitions.FindAll(t => !t.isExit && t.toNode.Equals(selNode)))
        {
            ((BehaviourTree)currentElem).DeleteConnection(tr);
        }

        selNode.isRoot = true;
    }

    /// <summary>
    /// Enter <see cref="makeConnectionMode"/>
    /// </summary>
    /// <param name="nodeIndex"></param>
    private void ConnectNode(int selectIndex)
    {
        if (currentElem is BehaviourTree)
            selectednode = currentElem.nodes[selectIndex];
        if (currentElem is UtilitySystem)
            selectednode = currentElem.nodes[selectIndex];
        makeConnectionMode = true;
    }

    private void MakeExitTransition(int selectIndex)
    {
        NodeEditorUtilities.GenerateUndoStep(currentElem);

        TransitionGUI transition = CreateInstance<TransitionGUI>();
        transition.InitTransitionGUI(currentElem, currentElem.nodes[selectIndex], currentElem.nodes[selectIndex], false);

        currentElem.AddAsExit(transition);
    }

    /// <summary>
    /// Shows the errors that currently exist on the bottom left corner of the window
    /// </summary>
    private void ShowErrorsAndWarnings()
    {
        string maxPriorityErrorOrWarning = "";
        int currentPriority = 0;
        bool errorShown = false;
        GUIStyle style = Styles.ErrorPrompt;

        // We use a list of KeyValuePairs so we can have duplicate keys
        List<KeyValuePair<ClickableElement, Error>> errors = new List<KeyValuePair<ClickableElement, Error>>();
        List<KeyValuePair<ClickableElement, Warning>> warnings = new List<KeyValuePair<ClickableElement, Warning>>();

        // Add the errors and warnings to the lists
        if (currentElem)
        {
            errors.AddRange(currentElem.GetErrors());
            warnings.AddRange(currentElem.GetWarnings());
        }
        else
        {
            foreach (ClickableElement elem in Elements)
            {
                errors.AddRange(elem.GetErrors());
                warnings.AddRange(elem.GetWarnings());
            }
        }

        // Select the highest priority warning to show
        foreach (var warning in warnings)
        {
            if ((int)warning.Value > currentPriority)
            {
                maxPriorityErrorOrWarning = Logs.WarningToString(warning.Value, warning.Key);
                currentPriority = (int)warning.Value;
            }
        }

        currentPriority = 0;

        // Select the highest priority error to show overriding the warning selected before. If there is no errors, the selected warning will remain
        foreach (var error in errors)
        {
            if ((int)error.Value > currentPriority)
            {
                errorShown = true;
                maxPriorityErrorOrWarning = Logs.ErrorToString(error.Value, error.Key);
                currentPriority = (int)error.Value;
            }
        }

        // Write the "(...)" part at the end of the prompt
        if (errors.Count + warnings.Count > 1)
        {
            maxPriorityErrorOrWarning += " (";

            if (errors.Count > (errorShown ? 1 : 0))
            {
                maxPriorityErrorOrWarning += "+" + (errors.Count - (errorShown ? 1 : 0)) + " error/s";
            }
            if (warnings.Count > (errorShown ? 0 : 1))
            {
                if (errors.Count > 1)
                    maxPriorityErrorOrWarning += ", ";
                maxPriorityErrorOrWarning += "+" + (warnings.Count - (errorShown ? 0 : 1)) + " warning/s";
            }

            maxPriorityErrorOrWarning += ")";
        }

        if (!string.IsNullOrEmpty(maxPriorityErrorOrWarning))
        {
            // If the shown prompt is an error, make the text background look red, if not, orange
            if (errorShown)
                style.normal.background = GetLabelBackground(new Color(1, 0, 0, 0.6f));
            else
                style.normal.background = GetLabelBackground(new Color(1, 0.8f, 0, 0.6f));
            style.contentOffset = new Vector2(5, 5);
            EditorGUI.LabelField(new Rect(new Vector2(0, position.height - 25), maxSize), maxPriorityErrorOrWarning, style);
        }
    }

    public Texture2D GetLabelBackground(Color color)
    {
        // Grey colored background for the label
        Texture2D backgroundTex = new Texture2D(2, 2);
        Color[] pix = new Color[2 * 2];

        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = color; // grey with 0.2 alpha
        }

        backgroundTex.SetPixels(pix);
        backgroundTex.Apply();

        return backgroundTex;
    }

    /// <summary>
    /// Takes all the copied elements and gives them a new identificator
    /// </summary>
    /// <param name="elements"></param>
    private void ReIdentifyElements(List<GUIElement> elements)
    {
        foreach (GUIElement elem in elements)
        {
            elem.identificator = GUIElement.UniqueID();

            if (elem is BaseNode && ((BaseNode)elem).subElem != null)
            {
                ((BaseNode)elem).subElem.identificator = GUIElement.UniqueID();

                switch (((BaseNode)elem).subElem.GetType().ToString())
                {
                    case nameof(FSM):
                        ReIdentifyElements(((BaseNode)elem).subElem.nodes.Cast<GUIElement>().ToList());
                        ReIdentifyElements(((BaseNode)elem).subElem.transitions.Cast<GUIElement>().ToList());
                        break;
                    case nameof(BehaviourTree):
                        ReIdentifyElements(((BaseNode)elem).subElem.nodes.Cast<GUIElement>().ToList());
                        ReIdentifyElements(((BaseNode)elem).subElem.transitions.Cast<GUIElement>().ToList());
                        break;
                    case nameof(UtilitySystem):
                        ReIdentifyElements(((BaseNode)elem).subElem.nodes.Cast<GUIElement>().ToList());
                        ReIdentifyElements(((BaseNode)elem).subElem.transitions.Cast<GUIElement>().ToList());
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Copy the currently focused elements
    /// </summary>
    private void Copy()
    {
        clipboard = focusedObjects.Select(o => o.CopyElement(currentElem)).ToList();

        // We update every transition's references to the nodes, because if we don't, apparently it caused a bug
        foreach (GUIElement elem in clipboard)
        {
            if (elem is TransitionGUI)
            {
                ((TransitionGUI)elem).fromNode = (BaseNode)clipboard.Find(n => n.identificator == ((TransitionGUI)elem).fromNode.identificator);
                ((TransitionGUI)elem).toNode = (BaseNode)clipboard.Find(n => n.identificator == ((TransitionGUI)elem).toNode.identificator);
            }
        }

        // Reidentify after to make sure any references to other GUIElements are properly copied
        ReIdentifyElements(clipboard);

        cutObjects.Clear();
    }

    /// <summary>
    /// Cut the currently focused elements
    /// </summary>
    private void Cut()
    {
        Copy();
        cutObjects = new List<GUIElement>(focusedObjects);
        cutFromElement = currentElem;
    }

    /// <summary>
    /// Paste the elements that are currently in the clipboard
    /// </summary>
    private void Paste()
    {
        if (RecursiveContains(currentElem, cutObjects))
        {
            PopupWindow.InitWarningPopup("You can't cut something and paste it inside itself");
        }
        else
        {
            if (currentElem is null)
            {
                if (clipboard.Any(e => !(e is ClickableElement || (e is BaseNode && ((BaseNode)e).subElem != null) || e is TransitionGUI)))
                {
                    Debug.LogError("[ERROR] Couldn't paste these elements in this place");
                }
                else
                {
                    foreach (GUIElement elem in cutObjects)
                        Delete(elem, cutFromElement);

                    foreach (GUIElement elem in clipboard)
                    {
                        ClickableElement clickElem;

                        if (elem is ClickableElement)
                        {
                            clickElem = (ClickableElement)elem;
                        }
                        else if (elem is BaseNode)
                        {
                            clickElem = ((BaseNode)elem).subElem;
                        }
                        else
                        {
                            continue;
                        }

                        clickElem.parent = null;

                        Elements.Add(clickElem);

                        clickElem.elementName = editorNamer.AddName(clickElem.identificator, clickElem.elementName);
                    }
                }
            }

            if (currentElem is FSM)
            {
                if (clipboard.Any(e => !(e is StateNode || e is TransitionGUI || e is ClickableElement || (e is BaseNode && ((BaseNode)e).subElem != null))))
                {
                    Debug.LogError("[ERROR] Couldn't paste these elements in this place");
                }
                else
                {
                    foreach (GUIElement elem in cutObjects)
                        Delete(elem, cutFromElement);

                    foreach (GUIElement elem in clipboard.Where(e => !(e is TransitionGUI)))
                    {
                        StateNode newElem;

                        if (elem is ClickableElement)
                        {
                            ClickableElement newSubElem = (ClickableElement)elem.CopyElement(currentElem);

                            newSubElem.elementName = currentElem.elementNamer.AddName(newSubElem.identificator, newSubElem.elementName);

                            newElem = CreateInstance<StateNode>();
                            newElem.InitStateNode(currentElem, stateType.Unconnected, elem.windowRect.x, elem.windowRect.y, newSubElem, elem.identificator);
                        }
                        else
                        {
                            ((BaseNode)elem).parent = currentElem;

                            if (((BaseNode)elem).subElem is null)
                            {
                                newElem = (StateNode)elem;

                                newElem.nodeName = currentElem.elementNamer.AddName(newElem.identificator, newElem.nodeName);
                            }
                            else
                            {
                                ClickableElement newSubElem = (ClickableElement)((BaseNode)elem).subElem.CopyElement(currentElem);

                                newSubElem.elementName = currentElem.elementNamer.AddName(newSubElem.identificator, newSubElem.elementName);

                                newElem = CreateInstance<StateNode>();
                                newElem.InitStateNode(currentElem, stateType.Unconnected, elem.windowRect.x, elem.windowRect.y, newSubElem, elem.identificator);
                            }
                        }

                        newElem.type = stateType.Unconnected;

                        currentElem.nodes.Add(newElem);

                        if (!((FSM)currentElem).HasEntryState)
                        {
                            ((FSM)currentElem).SetAsEntry(newElem);
                        }

                        ((FSM)currentElem).CheckConnected();
                    }

                    // We paste transitions after to make sure we can refer to the nodes that they connect
                    foreach (TransitionGUI transition in clipboard.Where(e => e is TransitionGUI))
                    {
                        if (transition.fromNode is StateNode && transition.toNode is StateNode)
                        {
                            transition.transitionName = currentElem.elementNamer.AddName(transition.identificator, transition.transitionName);

                            // We reassign the connecting nodes. This is to fix a bug that made them look unconnected
                            transition.fromNode = currentElem.nodes.Find(s => s.Equals(transition.fromNode));
                            transition.toNode = currentElem.nodes.Find(s => s.Equals(transition.toNode));

                            currentElem.transitions.Add(transition);
                        }
                    }
                }
            }

            if (currentElem is BehaviourTree)
            {
                if (clipboard.Any(e => !(e is BehaviourNode || e is TransitionGUI || e is ClickableElement || (e is BaseNode && ((BaseNode)e).subElem != null))))
                {
                    Debug.LogError("[ERROR] Couldn't paste these elements in this place");
                }
                else
                {
                    foreach (GUIElement elem in cutObjects)
                        Delete(elem, cutFromElement);

                    foreach (GUIElement elem in clipboard.Where(n => !(n is TransitionGUI)))
                    {
                        BehaviourNode newElem;

                        if (elem is ClickableElement)
                        {
                            ClickableElement newSubElem = (ClickableElement)elem.CopyElement(currentElem);

                            newSubElem.elementName = currentElem.elementNamer.AddName(newSubElem.identificator, newSubElem.elementName);

                            newElem = CreateInstance<BehaviourNode>();
                            newElem.InitBehaviourNode(currentElem, behaviourType.Leaf, ((ClickableElement)elem).windowRect.x, ((ClickableElement)elem).windowRect.y, newSubElem, elem.identificator);
                        }
                        else
                        {
                            ((BaseNode)elem).parent = currentElem;

                            if (((BaseNode)elem).subElem is null)
                            {
                                newElem = (BehaviourNode)elem;

                                newElem.nodeName = currentElem.elementNamer.AddName(newElem.identificator, newElem.nodeName);
                            }
                            else
                            {
                                ClickableElement newSubElem = (ClickableElement)((BaseNode)elem).subElem.CopyElement(currentElem);

                                newSubElem.elementName = currentElem.elementNamer.AddName(newSubElem.identificator, newSubElem.elementName);

                                newElem = CreateInstance<BehaviourNode>();
                                newElem.InitBehaviourNode(currentElem, behaviourType.Leaf, ((BaseNode)elem).windowRect.x, ((BaseNode)elem).windowRect.y, newSubElem, elem.identificator);
                            }
                        }

                        currentElem.nodes.Add(newElem);
                    }

                    // We paste transitions after to make sure we can refer to the nodes that they connect
                    foreach (TransitionGUI transition in clipboard.Where(n => n is TransitionGUI))
                    {
                        if (transition.fromNode is BehaviourNode && (transition.toNode is BehaviourNode))
                        {
                            transition.transitionName = currentElem.elementNamer.AddName(transition.identificator, transition.transitionName);

                            // We reassign the connecting nodes. This is to fix a bug that made them look unconnected
                            transition.fromNode = currentElem.nodes.Find(s => s.Equals(transition.fromNode));
                            transition.toNode = currentElem.nodes.Find(s => s.Equals(transition.toNode));

                            currentElem.transitions.Add(transition);
                        }
                    }
                }
            }

            if (currentElem is UtilitySystem)
            {
                if (clipboard.Any(e => !(e is UtilityNode || e is TransitionGUI || e is ClickableElement || (e is BaseNode && ((BaseNode)e).subElem != null))))
                {
                    Debug.LogError("[ERROR] Couldn't paste these elements in this place");
                }
                else
                {
                    foreach (GUIElement elem in cutObjects)
                        Delete(elem, cutFromElement);

                    foreach (GUIElement elem in clipboard.Where(n => !(n is TransitionGUI)))
                    {
                        UtilityNode newElem;

                        if (elem is ClickableElement)
                        {
                            ClickableElement newSubElem = (ClickableElement)elem.CopyElement(currentElem);

                            newSubElem.elementName = currentElem.elementNamer.AddName(newSubElem.identificator, newSubElem.elementName);

                            newElem = CreateInstance<UtilityNode>();
                            newElem.InitUtilityNode(currentElem, utilityType.Action, ((ClickableElement)elem).windowRect.x, ((ClickableElement)elem).windowRect.y, newSubElem, elem.identificator);
                        }
                        else
                        {
                            ((BaseNode)elem).parent = currentElem;

                            if (((BaseNode)elem).subElem is null)
                            {
                                newElem = (UtilityNode)elem;

                                newElem.nodeName = currentElem.elementNamer.AddName(newElem.identificator, newElem.nodeName);
                            }
                            else
                            {
                                ClickableElement newSubElem = (ClickableElement)((BaseNode)elem).subElem.CopyElement(currentElem);

                                newSubElem.elementName = currentElem.elementNamer.AddName(newSubElem.identificator, newSubElem.elementName);

                                newElem = CreateInstance<UtilityNode>();
                                newElem.InitUtilityNode(currentElem, utilityType.Action, ((BaseNode)elem).windowRect.x, ((BaseNode)elem).windowRect.y, newSubElem, elem.identificator);
                            }
                        }

                        currentElem.nodes.Add(newElem);
                    }

                    // We paste transitions after to make sure we can refer to the nodes that they connect
                    foreach (TransitionGUI transition in clipboard.Where(n => n is TransitionGUI))
                    {
                        if (transition.fromNode is UtilityNode && transition.toNode is UtilityNode)
                        {
                            transition.transitionName = currentElem.elementNamer.AddName(transition.identificator, transition.transitionName);

                            // We reassign the connecting nodes. This is to fix a bug that made them look unconnected
                            transition.fromNode = currentElem.nodes.Find(s => s.Equals(transition.fromNode));
                            transition.toNode = currentElem.nodes.Find(s => s.Equals(transition.toNode));

                            currentElem.transitions.Add(transition);
                        }
                    }
                }
            }

            if (cutObjects.Count > 0)
            {
                clipboard.Clear();
            }

            cutObjects.Clear();

            NodeEditorUtilities.GenerateUndoStep(currentElem);
        }
    }

    /// <summary>
    /// Undo one step
    /// </summary>
    private void Undo()
    {
        if (NodeEditorUtilities.undoStepsSaved > 0)
        {
            NodeEditorUtilities.GenerateRedoStep(currentElem);

            // We save the node's transitions to reconnect them later if the parent is not the main page
            if (currentElem.parent)
            {
                BaseNode node = currentElem.parent.nodes.Find(n => n.subElem && n.subElem.Equals(currentElem));

                List<TransitionGUI> transitionsIn = currentElem.parent.transitions.FindAll(t => t.toNode.Equals(node));
                List<TransitionGUI> transitionsOut = currentElem.parent.transitions.FindAll(t => t.fromNode.Equals(node));

                // Delete the current element and load the one from the UndoSteps to replace it
                Delete(currentElem, currentElem.parent, true);
                currentElem = LoadElem(NodeEditorUtilities.LoadUndoStep(), currentElem.parent, false);

                node = currentElem.parent.nodes.Find(n => n.subElem && n.subElem.Equals(currentElem));

                // We reconnect the transitions if the parent is not the main page
                foreach (TransitionGUI trans in transitionsIn)
                {
                    currentElem.parent.transitions.FirstOrDefault(t => t.Equals(trans)).toNode = node;

                    if (currentElem.parent is FSM)
                        ((FSM)currentElem.parent).CheckConnected();
                }

                foreach (TransitionGUI trans in transitionsOut)
                {
                    currentElem.parent.transitions.FirstOrDefault(t => t.Equals(trans)).fromNode = node;

                    if (currentElem.parent is FSM)
                        ((FSM)currentElem.parent).CheckConnected();
                }
            }
            else
            {
                // Delete the current element and load the one from the UndoSteps to replace it
                Delete(currentElem, currentElem.parent, true);
                currentElem = LoadElem(NodeEditorUtilities.LoadUndoStep(), currentElem.parent, false);
            }
        }
    }

    /// <summary>
    /// Redo one step
    /// </summary>
    private void Redo()
    {
        if (NodeEditorUtilities.redoStepsSaved > 0)
        {
            NodeEditorUtilities.GenerateUndoStep(currentElem, false);

            // We save the node's transitions to reconnect them later if the parent is not the main page
            if (currentElem.parent)
            {
                BaseNode node = currentElem.parent.nodes.Find(n => n.subElem && n.subElem.Equals(currentElem));

                List<TransitionGUI> transitionsIn = currentElem.parent.transitions.FindAll(t => t.toNode.Equals(node));
                List<TransitionGUI> transitionsOut = currentElem.parent.transitions.FindAll(t => t.fromNode.Equals(node));

                // Delete the current element and load the one from the RedoSteps to replace it
                Delete(currentElem, currentElem.parent, true);
                currentElem = LoadElem(NodeEditorUtilities.LoadRedoStep(), currentElem.parent, false);

                node = currentElem.parent.nodes.Find(n => n.subElem && n.subElem.Equals(currentElem));

                // We reconnect the transitions if the parent is not the main page
                foreach (TransitionGUI trans in transitionsIn)
                {
                    currentElem.parent.transitions.FirstOrDefault(t => t.Equals(trans)).toNode = node;

                    if (currentElem.parent is FSM)
                        ((FSM)currentElem.parent).CheckConnected();
                }

                foreach (TransitionGUI trans in transitionsOut)
                {
                    currentElem.parent.transitions.FirstOrDefault(t => t.Equals(trans)).fromNode = node;

                    if (currentElem.parent is FSM)
                        ((FSM)currentElem.parent).CheckConnected();
                }
            }
            else
            {
                // Delete the current element and load the one from the RedoSteps to replace it
                Delete(currentElem, currentElem.parent, true);
                currentElem = LoadElem(NodeEditorUtilities.LoadRedoStep(), currentElem.parent, false);
            }
        }
    }

    /// <summary>
    /// Check if <paramref name="elem"/> is in <paramref name="list"/>. If it's not, check every list inside it recursively
    /// </summary>
    /// <param name="elem"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    private bool RecursiveContains(ClickableElement elem, List<GUIElement> list)
    {
        if (list.Any(n => (n is BaseNode && ((BaseNode)n).subElem != null && ((BaseNode)n).subElem.Equals(elem)) ||
                          (n is ClickableElement && n.Equals(elem))))
        {
            return true;
        }
        else
        {
            foreach (GUIElement e in list)
            {
                ClickableElement element = null;

                if (e is ClickableElement)
                {
                    element = (ClickableElement)e;
                }
                if (e is BaseNode && ((BaseNode)e).subElem != null)
                {
                    element = ((BaseNode)e).subElem;
                }

                if (element)
                {
                    return RecursiveContains(elem, element.nodes.Cast<GUIElement>().ToList());
                }
            }

            return false;
        }
    }
}
