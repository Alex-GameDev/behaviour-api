using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMLPerception
{
    /// <summary>
    /// Unique identificator of the <see cref="XMLPerception"/>
    /// </summary>
    public string Id;

    /// <summary>
    /// Type of perception
    /// </summary>
    public perceptionType type;

    /// <summary>
    /// Parameter for the Timer Perception
    /// </summary>
    public float timerNumber;

    /// <summary>
    /// Name of the selected <see cref="FSM"/> or <see cref="BehaviourTree"/> for IsInState and BehaviourTreeStatus Perceptions
    /// </summary>
    public string elemName;

    /// <summary>
    /// Name for the Custom Perception class
    /// </summary>
    public string customName;

    /// <summary>
    /// Name of the state selected for IsInState Perception
    /// </summary>
    public string stateName;

    /// <summary>
    /// Status selected for BehaviourTreeStatus Perception
    /// </summary>
    public ReturnValues status;

    /// <summary>
    /// True if the foldout content of the <see cref="PerceptionGUI"/> should be shown
    /// </summary>
    public bool openFoldout;

    /// <summary>
    /// First <see cref="XMLPerception"/> of an And Perception
    /// </summary>
    public XMLPerception firstChild;

    /// <summary>
    /// Second <see cref="XMLPerception"/> of an And Perception
    /// </summary>
    public XMLPerception secondChild;

    /// <summary>
    /// Creates and returns a <see cref="PerceptionGUI"/> that corresponds to this <see cref="XMLPerception"/>
    /// </summary>
    /// <returns></returns>
    public PerceptionGUI ToGUIElement()
    {
        PerceptionGUI result = ScriptableObject.CreateInstance<PerceptionGUI>();
        result.identificator = this.Id;
        result.type = this.type;
        result.timerNumber = this.timerNumber;
        result.customName = this.customName;
        result.elemName = this.elemName;
        result.stateName = this.stateName;
        result.status = this.status;
        result.openFoldout = this.openFoldout;

        if (this.firstChild != null)
            result.firstChild = this.firstChild.ToGUIElement();
        if (this.secondChild != null)
            result.secondChild = this.secondChild.ToGUIElement();

        return result;
    }
}