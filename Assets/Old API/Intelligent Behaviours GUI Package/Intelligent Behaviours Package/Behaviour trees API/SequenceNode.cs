using System;
using System.Collections;
using System.Collections.Generic;

public class SequenceNode : TreeNode {

    #region variables

    private List<TreeNode> childrenNodes;
    private int childrenIndex;
    private bool randomSequence;

    #endregion variables

    public SequenceNode(string name, bool randomSequence, BehaviourTreeEngine behaviourTree)
    {
        this.childrenNodes = new List<TreeNode>();
        this.childrenIndex = 0;
        this.randomSequence = randomSequence;
        base.HasSubmachine = false;
        base.behaviourTree = behaviourTree;
        base.StateNode = new State(name, () => { }, behaviourTree); // ACTION vacío para evitar errores
        // ¿BUG? Si se mete en FireNextNode en la transición, podría haber un error en cadena
        // Selector dentro de Sequence: nodo activo se queda en Selector y no en su hijo

        if(randomSequence) {
            RandomizeChildren();
        }
    }

    /// <summary>
    /// Adds a child to the node
    /// </summary>
    /// <param name="childNode">The child node</param>
    public void AddChild(TreeNode childNode)
    {
        childrenNodes.Add(childNode);
        childNode.ParentNode = this;
    }

    private void RandomizeChildren()
    {
        List<TreeNode> randomChildren = new List<TreeNode>();
        Random random = new Random();
        while(childrenNodes.Count > 0) {
            int randomIndex = random.Next(0, childrenNodes.Count);
            randomChildren.Add(childrenNodes[randomIndex]);
            childrenNodes.RemoveAt(randomIndex);
        }
        childrenNodes = randomChildren;
    }

    /// <summary>
    /// Fires the transition to the next node in the sequence
    /// </summary>
    private void FireNextNode()
    {
        if(ReturnNodeValue() != ReturnValues.Running) {
            ReturnToParent();
            ResetChildren();
            childrenIndex = 0;
            return;
        }

        ReturnValue = ReturnValues.Running;

        if (childrenIndex == 0)
        {
            new Transition("to the first node", StateNode, new PushPerception(behaviourTree), childrenNodes[childrenIndex].StateNode, behaviourTree)
                .FireTransition();
        }
        else
        {
            new Transition("to next node", StateNode, new PushPerception(behaviourTree), childrenNodes[childrenIndex].StateNode, behaviourTree)
                .FireTransition();
        }

        // Activates de child node in the Behaviour tree
        if (childrenNodes[childrenIndex].ReturnValue == ReturnValues.Running) {
            behaviourTree.ActiveNode = childrenNodes[childrenIndex];
        }

        childrenIndex++;
    }

    public override void Update()
    {
        if(childrenIndex == 0)
        {
            FireNextNode();
        } else
        {
            if (childrenNodes[childrenIndex - 1].ReturnValue == ReturnValues.Succeed)
            {
                if (childrenIndex < childrenNodes.Count)
                {
                    FireNextNode();
                }
                else if (ReturnNodeValue() == ReturnValues.Succeed)
                {
                    ReturnToParent();
                    ResetChildren();
                    childrenIndex = 0;
                    ReturnValue = ReturnValues.Succeed;
                }
            }
            else if (childrenNodes[childrenIndex - 1].ReturnValue == ReturnValues.Failed)
            {
                ReturnToParent();
                ResetChildren();
                childrenIndex = 0;
                ReturnValue = ReturnValues.Failed;
            }
        }
        
    }

    private void ResetChildren()
    {
        foreach(TreeNode child in childrenNodes) {
            child.ReturnValue = ReturnValues.Running;
        }
    }

    public override ReturnValues ReturnNodeValue()
    {
        ReturnValue = ReturnValues.Succeed;
        foreach(TreeNode childNode in childrenNodes) {
            if(childNode.ReturnValue == ReturnValues.Failed) {
                ReturnValue = ReturnValues.Failed;
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