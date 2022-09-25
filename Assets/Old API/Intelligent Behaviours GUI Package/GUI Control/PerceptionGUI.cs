using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public enum perceptionType
{
    Push,
    Timer,
    Value,
    IsInState,
    BehaviourTreeStatus,
    And,
    Or,
    Custom
}

public class PerceptionGUI : GUIElement
{
    /// <summary>
    /// Type of perception
    /// </summary>
    public perceptionType type;

    /// <summary>
    /// Parameter for the Timer Perception
    /// </summary>
    public float timerNumber;

    /// <summary>
    /// Change this to get the timer to work in ms
    /// </summary>
    public bool timerInSeconds = true;

    public string timerUnit
    {
        get
        {
            if (timerInSeconds)
                return "s";
            else
                return "ms";
        }
    }

    /// <summary>
    /// Name of the selected <see cref="FSM"/> or <see cref="BehaviourTree"/> for IsInState and BehaviourTreeStatus Perceptions
    /// </summary>
    public string elemName; // Should be a reference to a FSM of BT, not just the name, ¡¡¡¡which means to change the ToXML, the Copy, etc!!!!

    /// <summary>
    /// Name for the Custom Perception class
    /// </summary>
    public string customName;

    /// <summary>
    /// Name of the state selected for IsInState Perception
    /// </summary>
    public string stateName; // Should be a reference to a StateNode, not just the name, ¡¡¡¡which means to change the ToXML, the Copy, etc!!!!

    /// <summary>
    /// Status selected for BehaviourTreeStatus Perception
    /// </summary>
    public ReturnValues status;

    /// <summary>
    /// True if the foldout content of the <see cref="PerceptionGUI"/> should be shown
    /// </summary>
    public bool openFoldout;

    /// <summary>
    /// First <see cref="PerceptionGUI"/> of an And/Or Perception
    /// </summary>
    public PerceptionGUI firstChild;

    /// <summary>
    /// Second <see cref="PerceptionGUI"/> of an And/Or Perception
    /// </summary>
    public PerceptionGUI secondChild;

    /// <summary>
    /// Returns the <see cref="perceptionType"/> properly written
    /// </summary>
    /// <returns></returns>
    public override string GetTypeString()
    {
        return type.ToString() + " Perception";
    }

    /// <summary>
    /// The Initializer for the <seealso cref="PerceptionGUI"/>
    /// </summary>
    /// <param name="type"></param>
    public void InitPerceptionGUI(perceptionType type)
    {
        identificator = UniqueID();

        this.type = type;

        timerNumber = 0;
        customName = "CustomName";
        openFoldout = false;

        if (type == perceptionType.IsInState)
        {
            elemName = "Select a FSM";
        }
        else if (type == perceptionType.BehaviourTreeStatus)
        {
            elemName = "Select a BT";
        }
        else
        {
            elemName = "";
        }

        stateName = "Select a State";

        status = ReturnValues.Succeed;

        if (type == perceptionType.And || type == perceptionType.Or)
        {
            firstChild = CreateInstance<PerceptionGUI>();
            firstChild.InitPerceptionGUI(perceptionType.Push);
            secondChild = CreateInstance<PerceptionGUI>();
            secondChild.InitPerceptionGUI(perceptionType.Push);
        }
        else
        {
            firstChild = null;
            secondChild = null;
        }
    }

    /// <summary>
    /// Creates and returns a <see cref="XMLPerception"/> that corresponds to this <see cref="PerceptionGUI"/>
    /// </summary>
    /// <returns></returns>
    public XMLPerception ToPerceptionXML()
    {
        XMLPerception result = new XMLPerception
        {
            Id = this.identificator,
            type = this.type,
            timerNumber = this.timerNumber,
            customName = this.customName,
            elemName = this.elemName,
            stateName = this.stateName,
            status = this.status,
            openFoldout = this.openFoldout
        };

        if (this.firstChild != null)
            result.firstChild = this.firstChild.ToPerceptionXML();
        if (this.secondChild != null)
            result.secondChild = this.secondChild.ToPerceptionXML();

        return result;
    }

    /// <summary>
    /// Not implemented
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override XMLElement ToXMLElement(params object[] args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a copy of this <see cref="PerceptionGUI"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public override GUIElement CopyElement(params object[] args)
    {
        PerceptionGUI result = CreateInstance<PerceptionGUI>();

        result.identificator = this.identificator;
        result.windowRect = new Rect(this.windowRect);
        result.type = this.type;
        result.timerNumber = this.timerNumber;
        result.customName = this.customName;
        result.elemName = this.elemName;
        result.stateName = this.stateName;
        result.status = this.status;
        result.openFoldout = this.openFoldout;

        if (this.firstChild != null)
            result.firstChild = (PerceptionGUI)this.firstChild.CopyElement();
        if (this.secondChild != null)
            result.secondChild = (PerceptionGUI)this.secondChild.CopyElement();

        return result;
    }

    /// <summary>
    /// Draws all the elements inside the <see cref="PerceptionGUI"/> window
    /// </summary>
    public override void DrawWindow()
    {
        throw new NotImplementedException();
    }
}
