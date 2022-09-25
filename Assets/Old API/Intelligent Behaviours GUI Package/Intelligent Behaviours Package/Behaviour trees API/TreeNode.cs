using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public abstract class TreeNode {

    #region variables

    public State StateNode { get; set; }
    public TreeNode ParentNode { get; set; }
    public TreeNode Child { get; set; }
    public ReturnValues ReturnValue { get; set; }
    public bool HasSubmachine { get; set; }

    protected BehaviourTreeEngine behaviourTree;

    protected bool firstExecution = false;

    #endregion variables

    /// <summary>
    /// Updates the node
    /// </summary>
    public virtual void Update()
    {
    }

    /// <summary>
    /// Has the node succeed in its task?
    /// </summary>
    /// <returns></returns>
    public virtual ReturnValues ReturnNodeValue()
    {
        ReturnValue = Child.ReturnValue;

        return ReturnValue;
    }

    /// <summary>
    /// Resets the node
    /// </summary>
    public virtual void Reset()
    {
        ReturnValue = ReturnValues.Running;
        firstExecution = false;
    }

    /// <summary>
    /// Returns to and active the parent node of the active node
    /// </summary>
    public void ReturnToParent()
    {
        if(ParentNode != null) {
            behaviourTree.ActiveNode = ParentNode;

            new Transition("to parent", StateNode, new PushPerception(behaviourTree), ParentNode.StateNode, behaviourTree)
                .FireTransition();
        }
    }
}