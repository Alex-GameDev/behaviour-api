using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class XMLElement
{
    // General properties

    /// <summary>
    /// Unique identificator
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The type of element
    /// </summary>
    public string elemType { get; set; }

    /// <summary>
    /// Auxiliar type used for when using only <see cref="elemType"/> is not enough
    /// </summary>
    public string secondType { get; set; } = "";

    /// <summary>
    /// Auxiliar type used for when using only <see cref="elemType"/> and <see cref="secondType"/> is not enough
    /// </summary>
    public string thirdType { get; set; } = "";

    /// <summary>
    /// Auxiliar type used for when using only <see cref="elemType"/>, <see cref="secondType"/> and <see cref="thirdType"/> is not enough
    /// </summary>
    public string fourthType { get; set; } = "";

    /// <summary>
    /// Name of the element
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// X position of the element window
    /// </summary>
    public float windowPosX { get; set; }

    /// <summary>
    /// Y position of the element window
    /// </summary>
    public float windowPosY { get; set; }

    // Clickable Elements properties

    /// <summary>
    /// List of nodes of the <see cref="ClickableElement"/>
    /// </summary>
    public List<XMLElement> nodes { get; set; }

    /// <summary>
    /// List of transitions of the <see cref="FSM"/>
    /// </summary>
    public List<XMLElement> transitions { get; set; }

    // Behaviour Nodes properties

    /// <summary>
    /// Parameter for Sequence Nodes
    /// </summary>
    public bool isRandom { get; set; }

    /// <summary>
    /// Parameter for Sequence Nodes
    /// </summary>
    public bool isInfinite { get; set; }

    /// <summary>
    /// Parameter for DelayT Nodes
    /// </summary>
    public float delayTime { get; set; }

    /// <summary>
    /// Parameter for LoopN Nodes
    /// </summary>
    public int Nloops { get; set; }

    /// <summary>
    /// Parameter for childs of Sequence Nodes
    /// </summary>
    public int index { get; set; }

    // Utility Nodes properties

    /// <summary>
    /// Parameter for Variable nodes
    /// </summary>
    public float variableMin { get; set; }

    /// <summary>
    /// Parameter for Variable nodes
    /// </summary>
    public float variableMax { get; set; }

    /// <summary>
    /// Parameter for Curve nodes
    /// </summary>
    public float slope { get; set; }

    /// <summary>
    /// Parameter for Curve nodes
    /// </summary>
    public float exp { get; set; }

    /// <summary>
    /// Parameter for Curve nodes
    /// </summary>
    public float displX { get; set; }

    /// <summary>
    /// Parameter for Curve nodes
    /// </summary>
    public float displY { get; set; }

    /// <summary>
    /// Parameter for Curve nodes
    /// </summary>
    public List<Vector2> points { get; set; }

    // Transitions properties

    /// <summary>
    /// Identificator of the <see cref="TransitionGUI.fromNode"/>
    /// </summary>
    public string fromId { get; set; }

    /// <summary>
    /// Identificator of the <see cref="TransitionGUI.toNode"/>
    /// </summary>
    public string toId { get; set; }

    /// <summary>
    /// The <see cref="XMLPerception"/> of this <see cref="TransitionGUI"/>'s perception
    /// </summary>
    public XMLPerception perception { get; set; }

    /// <summary>
    /// Weight for weighted Fusion nodes
    /// </summary>
    public float weight { get; set; }

    /// <summary>
    /// Wether this is an Exit transition or not
    /// </summary>
    public bool isExit { get; set; }

    // Conversion Methods

    /// <summary>
    /// Creates and returns the <see cref="FSM"/> corresponding to this <see cref="XMLElement"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="selectedNode"></param>
    /// <returns></returns>
    public FSM ToFSM(ClickableElement parent, BaseNode selectedNode = null)
    {
        FSM fsm = ScriptableObject.CreateInstance<FSM>();
        fsm.InitFSMFromXML(parent, this.windowPosX, this.windowPosY, this.Id, this.name);

        foreach (XMLElement node in this.nodes)
        {
            switch (node.elemType)
            {
                case nameof(FSM):
                    node.ToFSM(fsm, null);
                    break;
                case nameof(BehaviourTree):
                    node.ToBehaviourTree(fsm, null);
                    break;
                case nameof(UtilitySystem):
                    node.ToUtilitySystem(fsm, null);
                    break;
                case nameof(StateNode):
                    StateNode state = node.ToStateNode(fsm);

                    if (node.secondType.Equals(stateType.Entry.ToString()))
                    {
                        fsm.AddEntryState(state);
                    }
                    else
                    {
                        fsm.nodes.Add(state);
                    }
                    break;
                default:
                    Debug.LogError("Wrong content in saved data");
                    break;
            }
        }

        foreach (XMLElement trans in this.transitions)
        {
            BaseNode node1 = fsm.nodes.Where(n => n.identificator == trans.fromId || n.subElem?.identificator == trans.fromId).FirstOrDefault();
            BaseNode node2 = fsm.nodes.Where(n => n.identificator == trans.toId || n.subElem?.identificator == trans.toId).FirstOrDefault();
            if (node1 != null && node2 != null)
                fsm.AddTransition(trans.ToTransitionGUI(fsm, node1, node2));
        }

        if (parent)
        {
            switch (parent.GetType().ToString())
            {
                case nameof(FSM):
                    StateNode state = ScriptableObject.CreateInstance<StateNode>();
                    state.InitStateNodeFromXML(parent, stateType.Unconnected, fsm.windowRect.position.x, fsm.windowRect.position.y, this.Id, this.name, fsm);

                    if (this.secondType.Equals(stateType.Entry.ToString()))
                    {
                        ((FSM)parent).AddEntryState(state);
                    }
                    else
                    {
                        parent.nodes.Add(state);
                    }
                    break;
                case nameof(BehaviourTree):
                    BehaviourNode node = ScriptableObject.CreateInstance<BehaviourNode>();
                    node.InitBehaviourNode(parent, behaviourType.Leaf, fsm.windowRect.x, fsm.windowRect.y, fsm);

                    parent.nodes.Add(node);

                    if (selectedNode != null)
                    {
                        TransitionGUI transition = ScriptableObject.CreateInstance<TransitionGUI>();
                        transition.InitTransitionGUI(parent, selectedNode, node);

                        parent.transitions.Add(transition);

                        selectedNode = node;
                    }
                    break;
                case nameof(UtilitySystem):
                    UtilityNode utilNode = ScriptableObject.CreateInstance<UtilityNode>();
                    utilNode.InitUtilityNode(parent, utilityType.Action, fsm.windowRect.position.x, fsm.windowRect.position.y, fsm);

                    parent.nodes.Add(utilNode);
                    break;
            }
        }

        return fsm;
    }

    /// <summary>
    /// Creates and returns the <see cref="StateNode"/> corresponding to this <see cref="XMLElement"/>
    /// </summary>
    /// <returns></returns>
    public StateNode ToStateNode(FSM parent)
    {
        StateNode node = ScriptableObject.CreateInstance<StateNode>();
        node.InitStateNodeFromXML(parent, stateType.Unconnected, this.windowPosX, this.windowPosY, this.Id, this.name);

        return node;
    }

    /// <summary>
    /// Creates and returns the <see cref="TransitionGUI"/> corresponding to this <see cref="XMLElement"/>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public TransitionGUI ToTransitionGUI(ClickableElement parent, BaseNode from, BaseNode to)
    {
        TransitionGUI transition = ScriptableObject.CreateInstance<TransitionGUI>();
        transition.InitTransitionGUIFromXML(parent, from, to, this.Id, this.name, this.perception.ToGUIElement(), this.isExit, this.weight);

        return transition;
    }

    /// <summary>
    /// Creates and returns the <see cref="BehaviourTree"/> corresponding to this <see cref="XMLElement"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="selectedNode"></param>
    /// <returns></returns>
    public BehaviourTree ToBehaviourTree(ClickableElement parent, BaseNode selectedNode = null)
    {
        BehaviourTree bt = ScriptableObject.CreateInstance<BehaviourTree>();
        bt.InitBehaviourTreeFromXML(parent, this.windowPosX, this.windowPosY, this.Id, this.name);

        foreach (XMLElement node in this.nodes)
        {
            switch (node.elemType)
            {
                case nameof(FSM):
                    node.ToFSM(bt, null);
                    break;
                case nameof(BehaviourTree):
                    node.ToBehaviourTree(bt, null);
                    break;
                case nameof(UtilitySystem):
                    node.ToUtilitySystem(bt, null);
                    break;
                case nameof(BehaviourNode):
                    node.ToBehaviourNode(null, bt, parent);
                    break;
                default:
                    Debug.LogError("Wrong content in saved data");
                    break;
            }
        }

        foreach (XMLElement trans in this.transitions)
        {
            BaseNode node1 = bt.nodes.Where(n => n.identificator == trans.fromId || n.subElem?.identificator == trans.fromId).FirstOrDefault();
            BaseNode node2 = bt.nodes.Where(n => n.identificator == trans.toId || n.subElem?.identificator == trans.toId).FirstOrDefault();
            if (node1 != null && node2 != null)
                bt.transitions.Add(trans.ToTransitionGUI(bt, node1, node2));
        }

        if (parent)
        {
            switch (parent.GetType().ToString())
            {
                case nameof(FSM):
                    StateNode state = ScriptableObject.CreateInstance<StateNode>();
                    state.InitStateNodeFromXML(parent, stateType.Unconnected, bt.windowRect.position.x, bt.windowRect.position.y, this.Id, this.name, bt);

                    if (this.secondType.Equals(stateType.Entry.ToString()))
                    {
                        ((FSM)parent).AddEntryState(state);
                    }
                    else
                    {
                        parent.nodes.Add(state);
                    }
                    break;
                case nameof(BehaviourTree):
                    BehaviourNode node = ScriptableObject.CreateInstance<BehaviourNode>();
                    node.InitBehaviourNode(parent, behaviourType.Leaf, bt.windowRect.x, bt.windowRect.y, bt);

                    parent.nodes.Add(node);

                    if (selectedNode != null)
                    {
                        TransitionGUI transition = ScriptableObject.CreateInstance<TransitionGUI>();
                        transition.InitTransitionGUI(parent, selectedNode, node);

                        parent.transitions.Add(transition);

                        selectedNode = node;
                    }
                    break;
                case nameof(UtilitySystem):
                    UtilityNode utilNode = ScriptableObject.CreateInstance<UtilityNode>();
                    utilNode.InitUtilityNode(parent, utilityType.Action, bt.windowRect.position.x, bt.windowRect.position.y, bt);

                    parent.nodes.Add(utilNode);
                    break;
            }
        }

        return bt;
    }

    /// <summary>
    /// Creates the <see cref="BehaviourNode"/> corresponding to this <see cref="XMLElement"/>
    /// </summary>
    /// <param name="selectedNode"></param>
    /// <param name="currentTree"></param>
    /// <param name="currentElement"></param>
    public void ToBehaviourNode(BaseNode selectedNode, BehaviourTree currentTree, ClickableElement currentElement)
    {
        switch (this.elemType)
        {
            case nameof(FSM):
                this.ToFSM(currentElement, selectedNode);
                break;
            case nameof(BehaviourTree):
                this.ToBehaviourTree(currentElement, selectedNode);
                break;
            case nameof(UtilitySystem):
                this.ToUtilitySystem(currentElement, selectedNode);
                break;
            case nameof(BehaviourNode):
                BehaviourNode nodeBT = ScriptableObject.CreateInstance<BehaviourNode>();
                nodeBT.InitBehaviourNodeFromXML(currentTree, (behaviourType)Enum.Parse(typeof(behaviourType), this.secondType), this.windowPosX, this.windowPosY, this.Id, this.name, this.delayTime, this.Nloops, this.isRandom, this.isInfinite, this.index);

                currentTree.nodes.Add(nodeBT);

                if (selectedNode)
                {
                    TransitionGUI transition = ScriptableObject.CreateInstance<TransitionGUI>();
                    transition.InitTransitionGUI(currentTree, selectedNode, nodeBT, true);

                    currentTree.transitions.Add(transition);
                }
                else
                {
                    nodeBT.isRoot = true;
                }

                foreach (XMLElement childState in this.nodes)
                {
                    childState.ToBehaviourNode(nodeBT, currentTree, currentTree);
                }
                break;
            default:
                Debug.LogError("Wrong content in saved data");
                break;
        }
    }

    /// <summary>
    /// Creates and returns the <see cref="FSM"/> corresponding to this <see cref="XMLElement"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="selectedNode"></param>
    /// <returns></returns>
    public UtilitySystem ToUtilitySystem(ClickableElement parent, BaseNode selectedNode = null)
    {
        UtilitySystem utilSystem = ScriptableObject.CreateInstance<UtilitySystem>();
        utilSystem.InitUtilitySystemFromXML(parent, this.windowPosX, this.windowPosY, this.Id, this.name);

        foreach (XMLElement node in this.nodes)
        {
            switch (node.elemType)
            {
                case nameof(FSM):
                    node.ToFSM(utilSystem, null);
                    break;
                case nameof(BehaviourTree):
                    node.ToBehaviourTree(utilSystem, null);
                    break;
                case nameof(UtilitySystem):
                    node.ToUtilitySystem(utilSystem, null);
                    break;
                case nameof(UtilityNode):
                    UtilityNode state = node.ToUtilityNode(utilSystem);

                    utilSystem.nodes.Add(state);
                    break;
                default:
                    Debug.LogError("Wrong content in saved data");
                    break;
            }
        }

        foreach (XMLElement trans in this.transitions)
        {
            BaseNode node1 = utilSystem.nodes.Where(n => n.identificator == trans.fromId || n.subElem?.identificator == trans.fromId).FirstOrDefault();
            BaseNode node2 = utilSystem.nodes.Where(n => n.identificator == trans.toId || n.subElem?.identificator == trans.toId).FirstOrDefault();
            if (node1 != null && node2 != null)
                utilSystem.transitions.Add(trans.ToTransitionGUI(utilSystem, node1, node2));
        }

        if (parent)
        {
            switch (parent.GetType().ToString())
            {
                case nameof(FSM):
                    StateNode state = ScriptableObject.CreateInstance<StateNode>();
                    state.InitStateNodeFromXML(parent, stateType.Unconnected, utilSystem.windowRect.position.x, utilSystem.windowRect.position.y, this.Id, this.name, utilSystem);

                    if (this.secondType.Equals(stateType.Entry.ToString()))
                    {
                        ((FSM)parent).AddEntryState(state);
                    }
                    else
                    {
                        parent.nodes.Add(state);
                    }
                    break;
                case nameof(BehaviourTree):
                    BehaviourNode node = ScriptableObject.CreateInstance<BehaviourNode>();
                    node.InitBehaviourNode(parent, behaviourType.Leaf, utilSystem.windowRect.x, utilSystem.windowRect.y, utilSystem);

                    parent.nodes.Add(node);

                    if (selectedNode != null)
                    {
                        TransitionGUI transition = ScriptableObject.CreateInstance<TransitionGUI>();
                        transition.InitTransitionGUI(parent, selectedNode, node);

                        parent.transitions.Add(transition);

                        selectedNode = node;
                    }
                    break;
                case nameof(UtilitySystem):
                    UtilityNode utilNode = ScriptableObject.CreateInstance<UtilityNode>();
                    utilNode.InitUtilityNode(parent, utilityType.Action, utilSystem.windowRect.position.x, utilSystem.windowRect.position.y, utilSystem);

                    parent.nodes.Add(utilNode);
                    break;
            }
        }

        return utilSystem;
    }

    /// <summary>
    /// Creates and returns the <see cref="StateNode"/> corresponding to this <see cref="XMLElement"/>
    /// </summary>
    /// <returns></returns>
    public UtilityNode ToUtilityNode(UtilitySystem parent)
    {
        UtilityNode node = ScriptableObject.CreateInstance<UtilityNode>();
        node.InitUtilityNodeFromXML(parent, (utilityType)Enum.Parse(typeof(utilityType), this.secondType),
            (fusionType)Enum.Parse(typeof(fusionType), this.thirdType),
            (curveType)Enum.Parse(typeof(curveType), this.fourthType),
            this.windowPosX, this.windowPosY, this.Id, this.name, this.variableMax, this.variableMin, this.slope, this.exp, this.displX, this.displY, this.points);

        return node;
    }
}
