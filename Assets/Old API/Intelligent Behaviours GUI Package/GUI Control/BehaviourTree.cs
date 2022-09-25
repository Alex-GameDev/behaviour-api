using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BehaviourTree : ClickableElement
{
    /// <summary>
    /// The Initializer for the <seealso cref="BehaviourNode"/>
    /// </summary>
    /// <param name="editor"></param>
    /// <param name="parent"></param>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    /// <param name="id"></param>
    public void InitBehaviourTree(ClickableElement parent, float posx, float posy)
    {
        InitClickableElement();

        this.parent = parent;

        if (parent != null)
            elementName = parent.elementNamer.AddName(identificator, "New BT ");
        else
            elementName = editor.editorNamer.AddName(identificator, "New BT ");

        windowRect = new Rect(posx, posy, width, height);
    }

    /// <summary>
    /// The Initializer for the <seealso cref="BehaviourNode"/> when it is being loaded from an XML
    /// </summary>
    /// <param name="editor"></param>
    /// <param name="parent"></param>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    /// <param name="id"></param>
    public void InitBehaviourTreeFromXML(ClickableElement parent, float posx, float posy, string id, string name)
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
    /// Creates and returns an <see cref="XMLElement"/> that corresponds to this <see cref="BehaviourTree"/>
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
            nodes = nodes.FindAll(o => ((BehaviourNode)o).isRoot).ConvertAll((rootNode) =>
            {
                return rootNode.ToXMLElement(this);
            }),
            transitions = transitions.FindAll(t => t.isExit).ConvertAll((exit) =>
            {
                return exit.ToXMLElement(this);
            }),
            Id = this.identificator
        };

        return result;
    }

    /// <summary>
    /// Creates a copy of this <see cref="BehaviourTree"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override GUIElement CopyElement(params object[] args)
    {
        ClickableElement parent = (ClickableElement)args[0];

        BehaviourTree result = CreateInstance<BehaviourTree>();

        result.identificator = this.identificator;
        result.elementNamer = CreateInstance<UniqueNamer>();
        result.elementName = this.elementName;
        result.parent = parent;
        result.windowRect = new Rect(this.windowRect);

        result.nodes = this.nodes.Select(o => (BaseNode)o.CopyElement(result)).ToList();
        result.transitions = this.transitions.Select(o =>
        (TransitionGUI)o.CopyElement(result.nodes.Find(n => n.identificator == o.fromNode.identificator),
                                     result.nodes.Find(n => n.identificator == o.toNode.identificator))).ToList();

        return result;
    }

    /// <summary>
    /// Returns the type properly written
    /// </summary>
    /// <returns></returns>
    public override string GetTypeString()
    {
        return "Behaviour Tree";
    }

    /// <summary>
    /// Draws all <see cref="TransitionGUI"/> curves for the <see cref="BehaviourTree"/>
    /// </summary>
    public override void DrawCurves()
    {
        foreach (TransitionGUI elem in transitions)
        {
            if (elem.fromNode is null || elem.toNode is null)
                continue;

            if (elem.isExit && !(this.parent is UtilitySystem))
            {
                if (transitions.Remove(elem))
                    elementNamer.RemoveName(elem.identificator);
                break;
            }

            Rect fromNodeRect = new Rect(elem.fromNode.windowRect);
            Rect toNodeRect = new Rect(elem.toNode.windowRect);

            NodeEditor.DrawNodeCurve(fromNodeRect, toNodeRect, editor.focusedObjects.Contains(elem), false, false, elem.isExit);
        }
    }

    /// <summary>
    /// Deletes the <paramref name="node"/> and its children
    /// </summary>
    /// <param name="node"></param>
    public void DeleteNode(BehaviourNode node, bool deleteTransitions = true)
    {
        if (nodes.Remove(node))
        {
            if (deleteTransitions)
            {
                foreach (TransitionGUI transition in transitions.FindAll(t => node.Equals(t.fromNode)))
                {
                    DeleteConnection(transition);
                    DeleteNode((BehaviourNode)transition.toNode);
                }
                foreach (TransitionGUI transition in transitions.FindAll(t => node.Equals(t.toNode)))
                {
                    DeleteConnection(transition);
                }
            }

            if (node.subElem == null)
                elementNamer.RemoveName(node.identificator);
            else
                elementNamer.RemoveName(node.subElem.identificator);
        }
    }

    /// <summary>
    /// Deletes the <paramref name="connection"/>
    /// </summary>
    /// <param name="connection"></param>
    public void DeleteConnection(TransitionGUI connection)
    {
        if (transitions.Remove(connection))
            elementNamer.RemoveName(connection.identificator);
    }

    /// <summary>
    /// Returns how many children <paramref name="node"/> has
    /// </summary>
    /// <param name="node"></param>
    /// <param name="deepCount"> True if it should count children of its children recursively to infinity </param>
    /// <returns>The number of children <paramref name="node"/> has</returns>
    public int ChildrenCount(BehaviourNode node, bool deepCount = false)
    {
        int res = 0;

        foreach (TransitionGUI transition in transitions.FindAll(t => !t.isExit && node.Equals(t.fromNode)))
        {
            res += 1;

            if (deepCount)
                res += ChildrenCount((BehaviourNode)transition.toNode, true);
        }

        return res;
    }

    /// <summary>
    /// Returns all children <paramref name="node"/> has
    /// </summary>
    /// <param name="node"></param>
    /// <param name="deepCount"> True if it should count children of its children recursively to infinity </param>
    /// <returns>The number of children <paramref name="node"/> has</returns>
    public List<BehaviourNode> ChildrenGet(BehaviourNode node, bool deepCount = false)
    {
        List<BehaviourNode> children = new List<BehaviourNode>();

        foreach (TransitionGUI transition in transitions.FindAll(t => !t.isExit && node.Equals(t.fromNode)))
        {
            children.Add((BehaviourNode)transition.toNode);

            if (deepCount)
                children.AddRange(ChildrenGet((BehaviourNode)transition.toNode, true));
        }

        return children;
    }

    /// <summary>
    /// Return true if the root node is not a good type for being a root
    /// </summary>
    /// <param name="i"></param>
    /// <returns>The number of children <paramref name="paramNode"/> has</returns>
    public bool BadRootCheck(BehaviourNode paramNode = null)
    {
        BehaviourNode node;

        if (paramNode)
        {
            node = paramNode;
        }
        else
        {
            node = (BehaviourNode)nodes.Where(n => ((BehaviourNode)n).isRoot).FirstOrDefault();
            if (!node)
            {
                return false;
            }
        }

        if (node.type == behaviourType.Leaf)
            return true;

        if (node.type < behaviourType.Leaf)
            return false;

        foreach (TransitionGUI transition in transitions.FindAll(t => !t.isExit && node.Equals(t.fromNode)))
        {
            return BadRootCheck((BehaviourNode)transition.toNode);
        }

        return false;
    }

    /// <summary>
    /// Checks wether <paramref name="start"/> could ever reach <paramref name="end"/> in the <see cref="BehaviourTree"/> execution
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public bool ConnectedCheck(BehaviourNode start, BehaviourNode end)
    {
        foreach (TransitionGUI transition in transitions.FindAll(t => !t.isExit && start.Equals(t.fromNode)))
        {
            if (end.Equals((BehaviourNode)transition.toNode))
            {
                return true;
            }
            if (ConnectedCheck((BehaviourNode)transition.toNode, end))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the list of <see cref="ClickableElement"/> that exist inside each <see cref="BehaviourNode"/> of this <see cref="BehaviourTree"/> 
    /// </summary>
    /// <returns>The list of <see cref="ClickableElement"/> that exist inside each <see cref="BehaviourNode"/> of this <see cref="BehaviourTree"/></returns>
    public override List<ClickableElement> GetSubElems(bool includeSelf = false)
    {
        List<ClickableElement> result = new List<ClickableElement>();

        foreach (BehaviourNode node in nodes)
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
