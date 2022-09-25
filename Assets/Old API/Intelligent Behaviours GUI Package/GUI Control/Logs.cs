using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The numbers in these enums define the priority
public enum Error
{
    NoEntryState = 4,
    MoreThanOneRoot = 3,
    RepeatedName = 2,
    NoExitTransition = 1,
}

public enum Warning
{
    LeafIsRoot = 4,
    NoFactors = 3,
    UnconnectedNode = 2,
    WeightZero = 1,
}

public class Logs
{
    /// <summary>
    /// Transforms the <paramref name="error"/> into a pre-defined message
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static string ErrorToString(Error error, ClickableElement current)
    {
        string prompt = "Error at " + (current ? current.elementName : "unknown") + ": ";

        switch (error)
        {
            case Error.NoEntryState:
                prompt += "You can't have a FSM without an Entry State";
                break;
            case Error.MoreThanOneRoot:
                prompt += "You can't have a BT with more than one Root";
                break;
            case Error.RepeatedName:
                prompt += "You can't have two elements with the same name";
                break;
            case Error.NoExitTransition:
                prompt += "You can't have a " + current.GetTypeString() + " inside a " + current.parent.GetTypeString() + " with no Exit Transition";
                break;
            default:
                prompt += "Unknown error :(";
                break;
        }

        return prompt;
    }

    /// <summary>
    /// Transforms the <paramref name="warning"/> into a pre-defined message
    /// </summary>
    /// <param name="warning"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public static string WarningToString(Warning warning, ClickableElement current)
    {
        string prompt = "Warning at " + (current ? current.elementName : "unknown") + ": ";

        switch (warning)
        {
            case Warning.LeafIsRoot:
                prompt += "A Leaf Node cannot be the root of the Behaviour Tree";
                break;
            case Warning.NoFactors:
                prompt += "At least one Node does not have any Factors connected to it, meaning that line of Action will be ignored";
                break;
            case Warning.UnconnectedNode:
                prompt += "At least one State node is disconnected from the entry state, meaning it will be ignored";
                break;
            case Warning.WeightZero:
                prompt += "Having a Factor with a weight value of zero means it will be ignored";
                break;
            default:
                prompt += "Unknown warning :(";
                break;
        }

        return prompt;
    }
}
