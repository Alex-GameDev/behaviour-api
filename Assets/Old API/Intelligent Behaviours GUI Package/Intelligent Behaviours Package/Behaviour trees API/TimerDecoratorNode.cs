using System;
using System.Collections;
using System.Collections.Generic;

public class TimerDecoratorNode : TreeNode {

    #region variables

    private float time;
    private bool transitionLaunched;
    private Transition timerTransition;
    #endregion variables

    public TimerDecoratorNode(string name, TreeNode child, float time, BehaviourTreeEngine behaviourTree)
    {
        this.time = time;
        this.transitionLaunched = false;
        base.Child = child;
        Child.ParentNode = this;
        base.StateNode = new State(name, () => { }, behaviourTree); // Empty action to prevent going to child too early
        base.behaviourTree = behaviourTree;
    }

    private void ToChild()
    {
        if(Child.ReturnValue != ReturnValues.Running)
            return;

        ReturnValue = ReturnValues.Running;
        Child.ReturnValue = ReturnValues.Running;
        timerTransition = new Transition("to child", StateNode, new TimerPerception(time, behaviourTree), Child.StateNode, behaviourTree);
    }

    public override void Update()
    {
        if (!firstExecution) { ToChild(); firstExecution = true; }; // First loop goes to child
        if (!transitionLaunched && timerTransition.Perception.Check()) {
            timerTransition.FireTransition();
            transitionLaunched = true;
            behaviourTree.ActiveNode = Child;
        }

        if(transitionLaunched) {
            if(Child.ReturnValue != ReturnValues.Running) {
                if(ReturnNodeValue() != ReturnValues.Running) {
                    ReturnToParent();
                    Child.Reset();
                    transitionLaunched = false;
                }
            }
        }
    }

    public override void Reset()
    {
        base.Reset();
        transitionLaunched = false;
        if(timerTransition != null) {
            timerTransition.Perception.Reset();
        }
    }
}