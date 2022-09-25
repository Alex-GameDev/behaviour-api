using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FSM : ClickableElement
{
    /// <summary>
    /// Returns true if this <see cref="FSM"/> has an <see cref="EntryState"/>
    /// </summary>
    public bool HasEntryState
    {
        get
        {
            return nodes.Any(n => ((StateNode)n).type == stateType.Entry);
        }
    }

    /// <summary>
    /// The Initializer for the <seealso cref="FSM"/>
    /// </summary>
    /// <param name="editor"></param>
    /// <param name="parent"></param>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    public void InitFSM(ClickableElement parent, float posx, float posy)
    {
        InitClickableElement();

        this.parent = parent;

        if (parent != null)
            elementName = parent.elementNamer.AddName(identificator, "New FSM ");
        else
            elementName = editor.editorNamer.AddName(identificator, "New FSM ");

        windowRect = new Rect(posx, posy, width, height);

        // Create the entry state
        StateNode entryNode = CreateInstance<StateNode>();
        entryNode.InitStateNode(this, stateType.Entry, 50, 50);

        if (entryNode != null)
            AddEntryState(entryNode);
    }

    /// <summary>
    /// The Initializer for the <seealso cref="FSM"/> when it is being loaded from XML
    /// </summary>
    /// <param name="editor"></param>
    /// <param name="parent"></param>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    public void InitFSMFromXML(ClickableElement parent, float posx, float posy, string id, string name)
    {
        InitClickableElement(id);

        this.parent = parent;

        if (parent != null)
            elementName = parent.elementNamer.AddName(identificator, name);
        else
            elementName = editor.editorNamer.AddName(identificator, name);

        windowRect = new Rect(posx, posy, width, height);
    }

    /// <summary>
    /// Creates and returns an <see cref="XMLElement"/> that corresponds to this <see cref="FSM"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override XMLElement ToXMLElement(params object[] args)
    {
        XMLElement result = new XMLElement
        {
            name = CleanName(this.elementName),
            elemType = this.GetType().ToString(),
            windowPosX = this.windowRect.x,
            windowPosY = this.windowRect.y,
            nodes = nodes.ConvertAll((node) =>
            {
                return node.ToXMLElement();
            }),
            transitions = transitions.ConvertAll((trans) =>
            {
                return trans.ToXMLElement();
            }),
            Id = this.identificator
        };

        return result;
    }

    /// <summary>
    /// Creates a copy of this <see cref="FSM"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override GUIElement CopyElement(params object[] args)
    {
        ClickableElement parent = (ClickableElement)args[0];

        FSM result = CreateInstance<FSM>();

        result.identificator = this.identificator;
        result.elementNamer = CreateInstance<UniqueNamer>();
        result.elementName = this.elementName;
        result.parent = parent;
        result.windowRect = new Rect(this.windowRect);

        result.nodes = this.nodes.Select(o => (BaseNode)o.CopyElement(result)).ToList();
        result.transitions = this.transitions.Select(o =>
        (TransitionGUI)o.CopyElement(result.nodes.Find(n => n.identificator == o.fromNode.identificator),
                                     result.nodes.Find(n => n.identificator == o.toNode.identificator))).ToList();

        foreach (StateNode elem in result.nodes)
        {
            if (elem.type == stateType.Entry)
            {
                result.SetAsEntry(elem);
            }
        }

        return result;
    }

    /// <summary>
    /// Returns the type properly written
    /// </summary>
    /// <returns></returns>
    public override string GetTypeString()
    {
        return "FSM";
    }

    /// <summary>
    /// Add <paramref name="node"/> as an <see cref="EntryState"/>
    /// </summary>
    /// <param name="node"></param>
    public void AddEntryState(StateNode node)
    {
        node.type = stateType.Entry;
        nodes.Add(node);
    }

    /// <summary>
    /// Convert <paramref name="node"/> to <see cref="EntryState"/>
    /// </summary>
    /// <param name="node"></param>
    public void SetAsEntry(StateNode node)
    {
        //Previous Entry Node set to Unconnected
        StateNode entryState = (StateNode)nodes.Find(n => ((StateNode)n).type == stateType.Entry);
        if (entryState)
            entryState.type = stateType.Unconnected;

        node.type = stateType.Entry;

        CheckConnected();
    }

    /// <summary>
    /// Check if <paramref name="node"/> is the <see cref="EntryState"/>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool isEntryState(StateNode node)
    {
        return node.type == stateType.Entry;
    }

    /// <summary>
    /// Draws all <see cref="transitions"/> curves for the <see cref="FSM"/>
    /// </summary>
    public override void DrawCurves()
    {
        foreach (TransitionGUI elem in transitions)
        {
            if (elem.fromNode is null || elem.toNode is null)
                continue;

            if (elem.isExit && !(this.parent is BehaviourTree || this.parent is UtilitySystem))
            {
                DeleteTransition(elem);
                break;
            }

            bool isDouble = false;
            bool isLoop = false;
            bool isExit = false;

            Rect fromNodeRect = new Rect(elem.fromNode.windowRect);
            Rect toNodeRect = new Rect(elem.toNode.windowRect);

            if (transitions.Exists(t => t.fromNode.Equals(elem.toNode) && t.toNode.Equals(elem.fromNode)))
                isDouble = true;

            if (elem.fromNode.Equals(elem.toNode))
                isLoop = true;

            if (elem.isExit)
            {
                isExit = true;
                isLoop = false;
                isDouble = false;
            }

            NodeEditor.DrawNodeCurve(fromNodeRect, toNodeRect, editor.focusedObjects.Contains(elem), isDouble, isLoop, isExit);
        }
    }

    /// <summary>
    /// Delete <paramref name="node"/> and all <see cref="TransitionGUI"/> connected to it
    /// </summary>
    /// <param name="node"></param>
    public void DeleteNode(StateNode node, bool deleteTransitions = true)
    {
        if (nodes.Remove(node))
        {
            if (deleteTransitions)
            {
                List<TransitionGUI> nodeTransitionsList = transitions.Where(t => t.fromNode.Equals(node) || t.toNode.Equals(node)).ToList();

                // We have to use a temp list because we are deleting its elements in the middle of the loop
                foreach (TransitionGUI nodeTransition in nodeTransitionsList)
                {
                    DeleteTransition(nodeTransition);
                }
            }

            if (node.subElem == null)
                elementNamer.RemoveName(node.identificator);
            else
                elementNamer.RemoveName(node.subElem.identificator);
        }
    }

    /// <summary>
    /// Delete <paramref name="transition"/>
    /// </summary>
    /// <param name="transition"></param>
    public void DeleteTransition(TransitionGUI transition)
    {
        if (transitions.Remove(transition))
        {
            CheckConnected();

            elementNamer.RemoveName(transition.identificator);
        }
    }

    /// <summary>
    /// Recalculate every <see cref="StateNode"/>'s state of connection to the <see cref="EntryState"/>
    /// </summary>
    /// <param name="baseNode"></param>
    public void CheckConnected(StateNode baseNode = null)
    {
        if (baseNode == null)
        {
            baseNode = (StateNode)nodes.Find(n => ((StateNode)n).type == stateType.Entry);

            foreach (StateNode elem in nodes.FindAll(o => ((StateNode)o).type != stateType.Entry))
            {
                elem.type = stateType.Unconnected;
            }

            if (!nodes.Contains(baseNode))
                return;
        }
        else if (baseNode.type == stateType.Unconnected)
        {
            baseNode.type = stateType.Default;
        }
        else
        {
            return;
        }

        foreach (TransitionGUI nodeTransition in transitions.FindAll(t => !t.isExit && t.fromNode && t.toNode && t.fromNode.Equals(baseNode)))
        {
            CheckConnected((StateNode)nodeTransition.toNode);
        }
    }

    /// <summary>
    /// Add <paramref name="newTransition"/> to the <see cref="FSM"/>
    /// </summary>
    /// <param name="newTransition"></param>
    public void AddTransition(TransitionGUI newTransition)
    {
        transitions.Add(newTransition);

        CheckConnected();
    }

    /// <summary>
    /// Returns the list of <see cref="ClickableElement"/> that exist inside each <see cref="StateNode"/> of this <see cref="FSM"/> 
    /// </summary>
    /// <returns>The list of <see cref="ClickableElement"/> that exist inside each <see cref="StateNode"/> of this <see cref="FSM"/></returns>
    public override List<ClickableElement> GetSubElems(bool includeSelf = false)
    {
        List<ClickableElement> result = new List<ClickableElement>();

        foreach (StateNode node in nodes)
        {
            if (node.subElem != null)
            {
                result.AddRange(node.subElem.GetSubElems());
                result.Add(node.subElem);
            }
        }

        if (includeSelf)
            result.Add(this);

        return result;
    }
}
