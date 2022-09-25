using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Globalization;

public enum behaviourType
{
    Sequence,
    Selector,
    Leaf,
    LoopN,
    LoopUntilFail,
    Inverter,
    DelayT,
    Succeeder,
    Conditional
}

public class BehaviourNode : BaseNode
{
    /// <summary>
    /// The type of <see cref="BehaviourNode"/>
    /// </summary>
    public behaviourType type;

    /// <summary>
    /// Parameter used for Decorator Nodes that are <see cref="behaviourType.DelayT"/>
    /// </summary>
    public float delayTime = 0.0f;

    /// <summary>
    /// Parameter used for Decorator Nodes that are <see cref="behaviourType.LoopN"/>
    /// </summary>
    public int Nloops = 0;

    /// <summary>
    /// True if this <see cref="BehaviourNode"/> is the root of the <see cref="BehaviourTree"/>
    /// </summary>
    public bool isRoot = false;

    /// <summary>
    /// Parameter for Sequence Nodes
    /// </summary>
    public bool isRandom = false;

    /// <summary>
    /// Parameter for Sequence Nodes
    /// </summary>
    public bool isInfinite = false;

    /// <summary>
    /// Index of this node if its father is a non-random Sequence Node
    /// </summary>
    public int index;

    /// <summary>
    /// private children counter used for sequence nodes
    /// </summary>
    private int currentChildrenCount;

    /// <summary>
    /// Returns the <see cref="behaviourType"/> properly written
    /// </summary>
    /// <returns></returns>
    public override string GetTypeString()
    {
        if (subElem is null)
            return type.ToString();
        else
            return subElem.GetTypeString();
    }

    /// <summary>
    /// The Initializer for the <seealso cref="BehaviourNode"/>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="typeNumber"></param>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    /// <param name="subElem"></param>
    public void InitBehaviourNode(ClickableElement parent, behaviourType type, float posx, float posy, ClickableElement subElem = null, string id = null)
    {
        InitBaseNode(parent, id);

        this.type = type;

        if (subElem != null)
        {
            this.subElem = subElem;
            nodeName = this.subElem.elementName;
            windowRect = new Rect(posx, posy, ClickableElement.width, ClickableElement.height);
        }
        else
        {
            nodeName = parent.elementNamer.AddName(identificator, "New " + type + " Node ");
            windowRect = new Rect(posx, posy, width, height);
        }
    }

    /// <summary>
    /// The Initializer for the <seealso cref="BehaviourNode"/> when it is being loaded from an XML
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="typeNumber"></param>
    /// <param name="posx"></param>
    /// <param name="posy"></param>
    /// <param name="subElem"></param>
    public void InitBehaviourNodeFromXML(ClickableElement parent, behaviourType type, float posx, float posy, string id, string name, float delayTime, int Nloops, bool isRandom, bool isInfinite, int index, ClickableElement subElem = null)
    {
        InitBaseNode(parent, id);

        this.type = type;

        if (subElem != null)
        {
            this.subElem = subElem;
            nodeName = this.subElem.elementName;
            windowRect = new Rect(posx, posy, ClickableElement.width, ClickableElement.height);
        }
        else
        {
            nodeName = parent.elementNamer.AddName(id, name);
            windowRect = new Rect(posx, posy, width, height);
        }

        this.delayTime = delayTime;
        this.Nloops = Nloops;
        this.isRandom = isRandom;
        this.isInfinite = isInfinite;
        this.index = index;
    }

    /// <summary>
    /// Draws all the elements inside the <see cref="BehaviourNode"/>
    /// </summary>
    public override void DrawWindow()
    {
        // If this node is a non-random sequence, check if there's a new child or one was deleted, then ReIndex
        if ((this.type == behaviourType.Sequence && !this.isRandom) || this.type == behaviourType.Selector)
        {
            List<BehaviourNode> children = ((BehaviourTree)parent).ChildrenGet(this).OrderBy(n => n.index).ToList();
            int updatedChildrenCount = children.Count;

            if (updatedChildrenCount != currentChildrenCount)
                ReIndex(children);

            currentChildrenCount = updatedChildrenCount;
        }

        switch (type)
        {
            case behaviourType.Sequence:
                nodeName = CleanName(EditorGUILayout.TextArea(nodeName, Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)));

                GUILayout.BeginArea(new Rect(windowRect.width * 0.25f, windowRect.height - 20, windowRect.width * 0.5f, height * 0.3f));
                isRandom = GUILayout.Toggle(isRandom, "Random", new GUIStyle(GUI.skin.toggle) { alignment = TextAnchor.MiddleCenter });
                GUILayout.EndArea();
                break;
            case behaviourType.Selector:
            case behaviourType.Leaf:
                nodeName = CleanName(EditorGUILayout.TextArea(nodeName, Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)));
                break;
            case behaviourType.LoopN:
                GUILayout.BeginArea(new Rect(windowRect.width * 0.65f, windowRect.height * 0.4f, windowRect.width * 0.5f, height * 0.3f));
                isInfinite = GUILayout.Toggle(isInfinite, "∞", new GUIStyle(GUI.skin.toggle) { alignment = TextAnchor.MiddleLeft });
                GUILayout.EndArea();
                if (isInfinite)
                    EditorGUILayout.TextField("∞", Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25));
                else
                    int.TryParse(EditorGUILayout.TextField(Nloops.ToString(), Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)), out Nloops);
                break;
            case behaviourType.DelayT:
                float.TryParse(EditorGUILayout.TextField(delayTime.ToString(CultureInfo.CreateSpecificCulture("en-US")), Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)), NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-US"), out delayTime);
                break;
        }
    }

    /// <summary>
    /// Creates and returns an <see cref="XMLElement"/> that corresponds to this <see cref="BehaviourNode"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns>The <see cref="XMLElement"/> corresponding to this <see cref="BehaviourNode"/></returns>
    public override XMLElement ToXMLElement(params object[] args)
    {
        BehaviourTree parentTree = (BehaviourTree)args[0];

        XMLElement result;
        if (this.subElem)
        {
            result = this.subElem.ToXMLElement();
        }
        else
        {
            result = new XMLElement
            {
                name = CleanName(this.nodeName),
                elemType = this.GetType().ToString(),
                windowPosX = this.windowRect.x,
                windowPosY = this.windowRect.y,
                isRandom = this.isRandom,
                isInfinite = this.isInfinite,
                delayTime = this.delayTime,
                Nloops = this.Nloops,
                index = this.index,

                nodes = parentTree.transitions.FindAll(o => !o.isExit && this.Equals(o.fromNode)).Select(o => o.toNode).Cast<BehaviourNode>().ToList().ConvertAll((node) =>
                {
                    return node.ToXMLElement(parentTree);
                }),
            };
        }

        result.Id = this.identificator;
        result.secondType = this.type.ToString();

        return result;
    }

    /// <summary>
    /// Creates a copy of this <see cref="BehaviourNode"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override GUIElement CopyElement(params object[] args)
    {
        BehaviourTree parent = (BehaviourTree)args[0];

        BehaviourNode result = CreateInstance<BehaviourNode>();

        result.identificator = this.identificator;
        result.nodeName = this.nodeName;
        result.parent = parent;
        result.type = this.type;
        result.windowRect = new Rect(this.windowRect);
        result.isRoot = this.isRoot;
        result.isRandom = this.isRandom;
        result.isInfinite = this.isInfinite;
        result.delayTime = this.delayTime;
        result.Nloops = this.Nloops;
        result.index = this.index;

        if (this.subElem)
        {
            result.subElem = (ClickableElement)this.subElem.CopyElement(parent);
        }

        return result;
    }

    /// <summary>
    /// Re-orders the incices based on the fact that one (<paramref name="nodeToChange"/>) was changed to a new index (<paramref name="newIndex"/>)
    /// </summary>
    /// <param name="nodeToChange"></param>
    /// <param name="newIndex"></param>
    public void ReorderIndices(BehaviourNode nodeToChange, int newIndex)
    {
        List<BehaviourNode> children = ((BehaviourTree)parent).ChildrenGet(this).OrderBy(n => n.index).ToList();

        ReIndex(children);

        if (newIndex < nodeToChange.index)
            for (int i = newIndex - 1; i < nodeToChange.index - 1; i++)
                children[i].index++;

        if (newIndex > nodeToChange.index)
            for (int i = newIndex - 1; i > nodeToChange.index - 1; i--)
                children[i].index--;
    }

    /// <summary>
    /// Rewrites the indices of the list <paramref name="children"/>
    /// </summary>
    /// <param name="children"></param>
    public void ReIndex(List<BehaviourNode> children)
    {
        foreach (int id in children.Select(n => n.index))
        {
            if (children.FindAll(n => n.index == id).Count > 1 || id == 0 || id > children.Count)
            {
                for (int i = 0; i < children.Count; i++)
                    children[i].index = i + 1;
                break;
            }
        }
    }
}
