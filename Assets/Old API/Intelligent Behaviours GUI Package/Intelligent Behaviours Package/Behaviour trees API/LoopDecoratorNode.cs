using System;
using System.Collections;
using System.Collections.Generic;

public class LoopDecoratorNode : TreeNode {

    #region variables

    private int loopTimes;
    private int timesLooped;

    #endregion variables

    /// <summary>
    /// Runs the node a specific number of times
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node which will be executed</param>
    /// <param name="loopTimes">The number of times the node will execute the child</param>
    /// <param name="behaviourTree">The behaviour tree the node belongs to</param>
    public LoopDecoratorNode(string name, TreeNode child, int loopTimes, BehaviourTreeEngine behaviourTree)
    {
        this.loopTimes = loopTimes;
        this.timesLooped = 0;
        base.Child = child;
        Child.ParentNode = this;
        base.StateNode = new State(name, ToChild, behaviourTree);
        base.behaviourTree = behaviourTree;
    }

    /// <summary>
    /// Runs the node infinite number of times
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node which will be executed</param>
    /// <param name="behaviourTree">The behaviour tree the node belongs to</param>
    public LoopDecoratorNode(string name, TreeNode child, BehaviourTreeEngine behaviourTree)
    {
        this.loopTimes = -1;
        this.timesLooped = 0;
        base.Child = child;
        Child.ParentNode = this;
        base.StateNode = new State(name, () => { }, behaviourTree); //Null action to prevent errors
        base.behaviourTree = behaviourTree;
    }

    private void ToChild()
    {
        if(Child.ReturnValue != ReturnValues.Running)
            return;

        ReturnValue = ReturnValues.Running;
        Child.ReturnValue = ReturnValues.Running;
        new Transition("to child", StateNode, new PushPerception(behaviourTree), Child.StateNode, behaviourTree)
            .FireTransition();

        behaviourTree.ActiveNode = Child;
    }

    private void Loop()
    {
        ReturnValue = ReturnValues.Running;
        Child.ReturnValue = ReturnValues.Running;
        behaviourTree.ActiveNode = Child;
        new Transition("action again", StateNode, new PushPerception(behaviourTree), Child.StateNode, behaviourTree)
            .FireTransition();
    }

    public override void Update()
    {
        if (!firstExecution) { ToChild(); firstExecution = true; }; // First loop goes to child
        if (Child.ReturnValue != ReturnValues.Running) {
            if(ReturnNodeValue() != ReturnValues.Running) {
                ReturnToParent();
                Child.Reset();
                timesLooped = 0;
            }
        }
    }

    public override ReturnValues ReturnNodeValue()
    {
        timesLooped += (loopTimes != -1) ? 1 : 0;
        if (loopTimes == -1) {
            Loop();
        }
        else if(timesLooped < loopTimes) {
            Loop();
        }
        else {
            ReturnValue = Child.ReturnValue;
        }

        return ReturnValue;
    }

    public override void Reset()
    {
        base.Reset();
        timesLooped = 0;
    }
}