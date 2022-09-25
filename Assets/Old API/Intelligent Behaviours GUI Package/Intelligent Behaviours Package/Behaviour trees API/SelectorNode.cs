using System;
using System.Collections;
using System.Collections.Generic;

public class SelectorNode : TreeNode {

    #region variables

    private List<TreeNode> childrenNodes;
    private TreeNode selectedChild;
    private int childrenIndex;

    #endregion variables

    public SelectorNode(string name, BehaviourTreeEngine behaviourTree)
    {
        this.childrenNodes = new List<TreeNode>();
        this.childrenIndex = 0;
        base.HasSubmachine = false;
        base.behaviourTree = behaviourTree;
        base.StateNode = new State(name, () => { }, behaviourTree); // ACTION vacío para evitar errores
    }

    public void AddChild(TreeNode childNode)
    {
        childrenNodes.Add(childNode);
        childNode.ParentNode = this;
    }

    /// <summary>
    /// Starts to select the children
    /// </summary>
    private void SelectChild()
    {
        if(ReturnNodeValue() != ReturnValues.Running) {
            selectedChild = childrenNodes[childrenIndex - 1];
            ReturnToParent();
            ResetChildren();
            childrenIndex = 0;
            return;
        }

        ReturnValue = ReturnValues.Running;

        if (childrenIndex == 0)
        {
            //Console.WriteLine("Transicion a primer hijo");
            new Transition("to first child", StateNode, new PushPerception(behaviourTree), childrenNodes[childrenIndex].StateNode, behaviourTree)
                .FireTransition();
        }
        else
        {
            try
            {
                //Console.WriteLine("Transicion al siguiente hijo");
                new Transition("to next child", StateNode, new PushPerception(behaviourTree), childrenNodes[childrenIndex].StateNode, behaviourTree)
                    .FireTransition();
            }
            catch
            {
                ReturnToParent();
                ResetChildren();
                childrenIndex = 0;
                ReturnValue = ReturnValues.Failed;
            }

        }

        // Activates de child node in the Behaviour tree
        if (childrenNodes[childrenIndex].ReturnValue == ReturnValues.Running)
        {
            behaviourTree.ActiveNode = childrenNodes[childrenIndex]; 
            //Console.WriteLine(behaviourTree.ActiveNode.StateNode.Name + " activo");
        }

        childrenIndex++;
    }

    private void ResetChildren()
    {
        foreach(TreeNode child in childrenNodes) {
            child.ReturnValue = ReturnValues.Running;
        }
    }

    public override void Update()
    {
        if (childrenIndex == 0)
        {
            SelectChild();
        }
        else
        {
            if (childrenNodes[childrenIndex - 1].ReturnValue == ReturnValues.Failed)
            {
                //Console.WriteLine("Actualiza primer if");
                if (childrenIndex < childrenNodes.Count)
                {
                    SelectChild();
                }
                else if (ReturnNodeValue() == ReturnValues.Failed)
                {
                    ReturnToParent();
                    ResetChildren();
                    childrenIndex = 0;
                    ReturnValue = ReturnValues.Failed;
                }
            }
            else if (childrenNodes[childrenIndex - 1].ReturnValue == ReturnValues.Succeed)
            {
                //Console.WriteLine("Actualiza segundo if");
                if (ReturnNodeValue() == ReturnValues.Succeed)
                {
                    selectedChild = childrenNodes[childrenIndex - 1];
                    ReturnToParent();
                    ResetChildren();
                    childrenIndex = 0;
                    ReturnValue = ReturnValues.Succeed;
                }
            }
        }
    }

    public override ReturnValues ReturnNodeValue()
    {
        ReturnValue = ReturnValues.Failed;
        foreach(TreeNode childNode in childrenNodes) {
            if(childNode.ReturnValue == ReturnValues.Succeed) {
                ReturnValue = ReturnValues.Succeed;
                break;
            }
            else if(childNode.ReturnValue == ReturnValues.Running) {
                ReturnValue = ReturnValues.Running;
                break;
            }
        }

        return ReturnValue;
    }

    public override void Reset()
    {
        base.Reset();
        childrenIndex = 0;
    }
}