using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public class LeafNode : TreeNode {

    #region variables

    private Func<ReturnValues> succeedCondition;
    private Action nodeAction;

    #endregion variables

    public LeafNode(string name, Action action, Func<ReturnValues> succeedCondition, BehaviourTreeEngine behaviourTree)
    {
        this.succeedCondition = succeedCondition;
        this.nodeAction = action;
        base.HasSubmachine = false;
        base.StateNode = new State(name, NodeAction, behaviourTree);
        base.behaviourTree = behaviourTree;
    }

    public LeafNode(string name, State stateNode, BehaviourEngine behaviourEngine)
    {
        base.HasSubmachine = true;
        base.StateNode = stateNode;
        base.behaviourTree = behaviourEngine as BehaviourTreeEngine;
    }

    /// <summary>
    /// The action the node will do when transitioning to it
    /// </summary>
    private void NodeAction()
    {
        //Console.WriteLine("Nodo hoja action");
        if (ReturnValue == ReturnValues.Running) {
            nodeAction();
        }
    }

    public override void Update()
    {
        if (ReturnNodeValue() != ReturnValues.Running) {
            ReturnToParent();
        }
    }

    public override ReturnValues ReturnNodeValue()
    {
        if(succeedCondition != null)
            ReturnValue = succeedCondition();

        return ReturnValue;
    }
}