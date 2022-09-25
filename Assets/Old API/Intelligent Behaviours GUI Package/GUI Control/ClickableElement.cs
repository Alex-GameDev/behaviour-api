using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ClickableElement : GUIElement
{
    /// <summary>
    /// Width of the <see cref="ClickableElement.windowRect"/>
    /// </summary>
    public static int width = 150;

    /// <summary>
    /// Height of the <see cref="ClickableElement.windowRect"/>
    /// </summary>
    public static int height = 70;

    /// <summary>
    /// Name of the <see cref="ClickableElement"/>
    /// </summary>
    public string elementName = "";

    /// <summary>
    /// List of <see cref="BaseNode"/>
    /// </summary>
    public List<BaseNode> nodes = new List<BaseNode>();

    /// <summary>
    /// List of <see cref="TransitionGUI"/> that connect the <see cref="nodes"/>
    /// </summary>
    public List<TransitionGUI> transitions = new List<TransitionGUI>();

    /// <summary>
    /// List of <see cref="Error"/> of this <see cref="ClickableElement"/>
    /// </summary>
    public List<Error> errors = new List<Error>();

    /// <summary>
    /// List of <see cref="Warning"/> of this <see cref="ClickableElement"/>
    /// </summary>
    public List<Warning> warnings = new List<Warning>();

    private NodeEditor privateEditor;

    /// <summary>
    /// Reference for <see cref="NodeEditor"/>
    /// </summary>
    protected NodeEditor editor
    {
        get
        {
            if (!privateEditor)
                privateEditor = EditorWindow.GetWindow<NodeEditor>();
            return privateEditor;
        }
    }

    /// <summary>
    /// The <see cref="UniqueNamer"/> for managing the names of the elements inside this <see cref="ClickableElement"/>
    /// </summary>
    public UniqueNamer elementNamer;

    public bool HasExitTransition
    {
        get
        {
            return GetExitTransition() != null;
        }
    }

    public bool NeedsExitTransition
    {
        get
        {
            if (HasExitTransition || parent is null)
                return false;

            switch (parent.GetType().ToString())
            {
                case nameof(FSM):
                    return false;
                case nameof(BehaviourTree):
                    if (this is FSM)
                        return true;
                    else
                        return false;
                case nameof(UtilitySystem):
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// The Initializer for the <seealso cref="ClickableElement"/>
    /// </summary>
    /// <param name="id"></param>
    protected void InitClickableElement(string id = null)
    {
        var foo = editor;

        elementNamer = CreateInstance<UniqueNamer>();
        if (id == null)
            identificator = UniqueID();
        else
            identificator = id;
    }

    /// <summary>
    /// Draws all the elements inside the <see cref="ClickableElement"/> window
    /// </summary>
    public override void DrawWindow()
    {
        elementName = CleanName(EditorGUILayout.TextArea(elementName, Styles.TitleText, GUILayout.ExpandWidth(true), GUILayout.Height(25)));
    }

    /// <summary>
    /// Draws all <see cref="TransitionGUI"/> curves for the <see cref="ClickableElement"/>
    /// </summary>
    public abstract void DrawCurves();

    /// <summary>
    /// Returns the list of <see cref="ClickableElement"/> that exist inside each node of this <see cref="ClickableElement"/> 
    /// </summary>
    /// <returns>The list of <see cref="ClickableElement"/> that exist inside each node of this <see cref="ClickableElement"/></returns>
    public abstract List<ClickableElement> GetSubElems(bool includeSelf = false);

    /// <summary>
    /// Add <paramref name="trans"/> as an Exit transition
    /// </summary>
    /// <param name="trans"></param>
    public void AddAsExit(TransitionGUI trans)
    {
        TransitionGUI exitTrans = transitions.Find(t => t.isExit);
        if (exitTrans)
        {
            if (transitions.Remove(exitTrans))
                elementNamer.RemoveName(exitTrans.identificator);
        }

        trans.isExit = true;
        transitions.Add(trans);
    }

    /// <summary>
    /// Get the Exit transition
    /// </summary>
    /// <param name="trans"></param>
    public TransitionGUI GetExitTransition()
    {
        return transitions.Find(t => t.isExit);
    }

    /// <summary>
    /// Checks if <paramref name="name"/> exists more than <paramref name="threshold"/> times
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool CheckNameExisting(string name, int threshold)
    {
        int repeatedNames = 0;

        switch (this.GetType().ToString())
        {
            case nameof(FSM):
            case nameof(BehaviourTree):
            case nameof(UtilitySystem):
                foreach (BaseNode node in this.nodes)
                {
                    if (node.nodeName == name)
                    {
                        repeatedNames++;
                    }
                    if (node.subElem != null)
                    {
                        if (node.subElem.CheckNameExisting(name, 0))
                            repeatedNames++;
                    }
                }

                if (this is FSM)
                {
                    foreach (TransitionGUI transition in this.transitions)
                    {
                        if (transition.transitionName == name)
                        {
                            repeatedNames++;
                        }
                    }
                }

                break;
        }

        return repeatedNames > threshold;
    }

    /// <summary>
    /// Returns this <see cref="ClickableElement"/>'s errors and all of its children's
    /// </summary>
    /// <returns></returns>
    public List<KeyValuePair<ClickableElement, Error>> GetErrors()
    {
        List<KeyValuePair<ClickableElement, Error>> result = new List<KeyValuePair<ClickableElement, Error>>();

        foreach (Error error in errors)
        {
            result.Add(new KeyValuePair<ClickableElement, Error>(this, error));
        }

        foreach (ClickableElement subElem in GetSubElems())
        {
            result.AddRange(subElem.GetErrors());
        }

        return result;
    }

    /// <summary>
    /// Returns this <see cref="ClickableElement"/>'s warnings and all of its children's
    /// </summary>
    /// <returns></returns>
    public List<KeyValuePair<ClickableElement, Warning>> GetWarnings()
    {
        List<KeyValuePair<ClickableElement, Warning>> result = new List<KeyValuePair<ClickableElement, Warning>>();

        foreach (Warning warning in warnings)
        {
            result.Add(new KeyValuePair<ClickableElement, Warning>(this, warning));
        }

        foreach (ClickableElement subElem in GetSubElems())
        {
            result.AddRange(subElem.GetWarnings());
        }

        return result;
    }

    /// <summary>
    /// Add an <see cref="Error"/> that is happening right now
    /// </summary>
    /// <param name="error"></param>
    public void AddError(Error error)
    {
        if (!errors.Contains(error))
            errors.Add(error);
    }

    /// <summary>
    /// Remove an <see cref="Error"/> that is no longer happening
    /// </summary>
    /// <param name="error"></param>
    public void RemoveError(Error error)
    {
        errors.Remove(error);
    }

    /// <summary>
    /// Add an <see cref="Warning"/> that is happening right now
    /// </summary>
    /// <param name="error"></param>
    public void AddWarning(Warning warning)
    {
        if (!warnings.Contains(warning))
            warnings.Add(warning);
    }

    /// <summary>
    /// Remove an <see cref="Warning"/> that is no longer happening
    /// </summary>
    /// <param name="error"></param>
    public void RemoveWarning(Warning warning)
    {
        warnings.Remove(warning);
    }

    public ClickableElement GetMaxParent()
    {
        ClickableElement maxParent = this;

        while (maxParent.parent)
            maxParent = maxParent.parent;

        return maxParent;
    }
}
