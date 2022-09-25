using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public class BehaviourTreeStatusPerception : Perception {

    #region variables

    private BehaviourTreeEngine behaviourTree;
    private ReturnValues BehaviourTreeStatus;

    #endregion variables

    public BehaviourTreeStatusPerception(BehaviourTreeEngine behaviourTree, ReturnValues statusToReach, BehaviourEngine behaviourEngine)
    {
        this.behaviourTree = behaviourTree;
        this.BehaviourTreeStatus = statusToReach;
        this.behaviourEngine = behaviourEngine;
    }

    public override bool Check()
    {
        if(behaviourTree.GetRootNode().ReturnValue == BehaviourTreeStatus) {
            return true;
        }
        else {
            return false;
        }
    }
}