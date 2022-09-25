using System;
using System.Collections;
using System.Collections.Generic;
//using Stateless;

public class BehaviourTreeEngine : BehaviourEngine {

    #region variables

    public TreeNode ActiveNode { get; set; }

    private TreeNode rootNodeBT;
    private List<TreeNode> BTnodes;

    #endregion variables

    /// <summary>
    /// Creates a behaviour of Behaviour Tree type that CANNOT be a submachine
    /// </summary>
    /// <param name="isSubmachine"></param>
    public BehaviourTreeEngine()
    {
        base.transitions = new Dictionary<string, Transition>();
        base.states = new Dictionary<string, State>();
        this.BTnodes = new List<TreeNode>();
        base.IsSubMachine = false;

        entryState = new State("Entry_Machine", this);
        this.actualState = entryState;
        states.Add(entryState.Name, entryState);

        Active = true;
    }

    /// <summary>
    /// Creates a behaviour of Behaviour Tree type
    /// </summary>
    /// <param name="isSubmachine">Is this a submachine?</param>
    public BehaviourTreeEngine(bool isSubmachine)
    {
        base.transitions = new Dictionary<string, Transition>();
        base.states = new Dictionary<string, State>();
        this.BTnodes = new List<TreeNode>();
        base.IsSubMachine = isSubmachine;

        entryState = new State("Entry_Machine", this);
        this.actualState = entryState;
        states.Add(entryState.Name, entryState);

        Active = (isSubmachine) ? false : true;
    }

    /// <summary>
    /// Set the root node of the Behaviour tree
    /// </summary>
    /// <param name="rootNode">The root node of the Behaviour tree</param>
    public void SetRootNode(TreeNode rootNode)
    {
        ActiveNode = rootNode;
        rootNodeBT = rootNode;

        if(Active) {
            new Transition("Entry_transition", entryState, new PushPerception(this), rootNode.StateNode, this)
                .FireTransition();
        }
        entryState = rootNode.StateNode;
    }

    /// <summary>
    /// Get the root node of the Behaviour tree
    /// </summary>
    /// <returns></returns>
    public TreeNode GetRootNode()
    {
        return rootNodeBT;
    }

    /// <summary>
    /// Update has to be called once per frame
    /// </summary>
    public void Update()
    {
        if(!Active)
            return;

        if(ActiveNode != null && ActiveNode.ReturnValue == ReturnValues.Running) { // Node with NO submachine in it
            //Console.WriteLine(ActiveNode.StateNode.Name + " " + ActiveNode.ReturnValue);
            ActiveNode.Update();
        }
        else if(ActiveNode.HasSubmachine) { // Node with a submachine in it
            ActiveNode.Update();
        }
        else if(IsSubMachine) { // Behaviour tree submachine of another Behaviour tree
            transitions.TryGetValue("Exit_Transition", out Transition exitTransition);

            if(exitTransition.Perception.Check()) {
                Fire(exitTransition);
            }
        }
    }

    public override void Reset()
    {
        foreach(TreeNode node in BTnodes) {
            node.Reset();
        }
        ActiveNode = rootNodeBT;
    }

    #region create composite nodes

    /// <summary>
    /// Creates a new <see cref="SequenceNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="isRandomSequence">Is the sequence random?</param>
    /// <returns></returns>
    public SequenceNode CreateSequenceNode(string name, bool isRandomSequence)
    {
        if(!states.ContainsKey(name)) {
            SequenceNode sequenceNode = new SequenceNode(name, isRandomSequence, this);
            BTnodes.Add(sequenceNode);
            states.Add(name, sequenceNode.StateNode);

            return sequenceNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    /// <summary>
    /// Creates a new <see cref="SelectorNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <returns></returns>
    public SelectorNode CreateSelectorNode(string name)
    {
        if(!states.ContainsKey(name)) {
            SelectorNode selectorNode = new SelectorNode(name, this);
            BTnodes.Add(selectorNode);
            states.Add(name, selectorNode.StateNode);

            return selectorNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    #endregion create composite nodes

    #region create decorator nodes

    /// <summary>
    /// Creates a new finite <see cref="LoopDecoratorNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node</param>
    /// <param name="loopTimes">The number of times the child will be looped</param>
    /// <returns></returns>
    public LoopDecoratorNode CreateLoopNode(string name, TreeNode child, int loopTimes)
    {
        if(!states.ContainsKey(name)) {
            LoopDecoratorNode loopNode = new LoopDecoratorNode(name, child, loopTimes, this);
            BTnodes.Add(loopNode);
            states.Add(name, loopNode.StateNode);

            return loopNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    /// <summary>
    /// Creates a new infinite <see cref="LoopDecoratorNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node</param>
    /// <returns></returns>
    public LoopDecoratorNode CreateLoopNode(string name, TreeNode child)
    {
        if(!states.ContainsKey(name)) {
            LoopDecoratorNode loopNode = new LoopDecoratorNode(name, child, this);
            BTnodes.Add(loopNode);
            states.Add(name, loopNode.StateNode);

            return loopNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    /// <summary>
    /// Creates a new <see cref="LoopUntilFailDecoratorNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node</param>
    /// <returns></returns>
    public LoopUntilFailDecoratorNode CreateLoopUntilFailNode(string name, TreeNode child)
    {
        if(!states.ContainsKey(name)) {
            LoopUntilFailDecoratorNode loopUntilFailNode = new LoopUntilFailDecoratorNode(name, child, this);
            BTnodes.Add(loopUntilFailNode);
            states.Add(name, loopUntilFailNode.StateNode);

            return loopUntilFailNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    /// <summary>
    /// Creates a new <see cref="TimerDecoratorNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node</param>
    /// <param name="time">The time it will take to execute the child node, in seconds</param>
    /// <returns></returns>
    public TimerDecoratorNode CreateTimerNode(string name, TreeNode child, float time)
    {
        if(!states.ContainsKey(name)) {
            TimerDecoratorNode timerNode = new TimerDecoratorNode(name, child, time, this);
            BTnodes.Add(timerNode);
            states.Add(name, timerNode.StateNode);

            return timerNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    /// <summary>
    /// Creates a new <see cref="InverterDecoratorNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node</param>
    /// <returns></returns>
    public InverterDecoratorNode CreateInverterNode(string name, TreeNode child)
    {
        if(!states.ContainsKey(name)) {
            InverterDecoratorNode inverterNode = new InverterDecoratorNode(name, child, this);
            BTnodes.Add(inverterNode);
            states.Add(name, inverterNode.StateNode);

            return inverterNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    /// <summary>
    /// Creates a new <see cref="SucceederDecoratorNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node</param>
    /// <returns></returns>
    public SucceederDecoratorNode CreateSucceederNode(string name, TreeNode child)
    {
        if(!states.ContainsKey(name)) {
            SucceederDecoratorNode succeederNode = new SucceederDecoratorNode(name, child, this);
            BTnodes.Add(succeederNode);
            states.Add(name, succeederNode.StateNode);

            return succeederNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    /// <summary>
    /// Creates a new <see cref="ConditionalDecoratorNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="child">The child of the node</param>
    /// <param name="condition">The condition the node must accomplish to succeed</param>
    /// <returns></returns>
    public ConditionalDecoratorNode CreateConditionalNode(string name, TreeNode child, Perception conditionPerception)
    {
        if(!states.ContainsKey(name)) {
            ConditionalDecoratorNode conditionalNode = new ConditionalDecoratorNode(name, child, conditionPerception, this);
            BTnodes.Add(conditionalNode);
            states.Add(name, conditionalNode.StateNode);

            return conditionalNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    #endregion create decorator nodes

    #region create leaf nodes

    /// <summary>
    /// Creates a new <see cref="LeafNode"/> in the behaviour tree
    /// </summary>
    /// <param name="name">The name of the node</param>
    /// <param name="action">The action the node will execute</param>
    /// <param name="succeedCondition">The condition the node must accomplish to succeed</param>
    /// <returns></returns>
    public LeafNode CreateLeafNode(string name, Action action, Func<ReturnValues> succeedCondition)
    {
        if(!states.ContainsKey(name)) {
            LeafNode leafNode = new LeafNode(name, action, succeedCondition, this);
            BTnodes.Add(leafNode);
            states.Add(name, leafNode.StateNode);

            return leafNode;
        }
        else {
            throw new DuplicateWaitObjectException(name, "The node already exists in the behaviour tree");
        }
    }

    #endregion create leaf nodes

    #region create sub-state machines

    /// <summary>
    /// Adds a type of <see cref="LeafNode"/> with a sub-behaviour engine in it and its transition to the entry state
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    /// <param name="subBehaviourTree">The sub-behaviour tree inside the </param>
    public LeafNode CreateSubBehaviour(string nodeName, BehaviourEngine subBehaviourEngine)
    {
        State stateTo = subBehaviourEngine.GetEntryState();
        State state = new State(nodeName, subBehaviourEngine.GetState("Entry_Machine"), stateTo, subBehaviourEngine, this);
        LeafNode leafNode = new LeafNode("Node to return", state, this);
        subBehaviourEngine.NodeToReturn = leafNode;
        states.Add(leafNode.StateNode.Name, leafNode.StateNode);

        return subBehaviourEngine.NodeToReturn;
    }

    /// <summary>
    /// Adds a type of <see cref="State"/> with a sub-state machine in it and its transition to the state <paramref name="stateTo"/>
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="subStateMachine">The sub-state machine inside the state</param>
    /// <param name="stateTo">The name of the state where the sub-state machine will enter</param>
    /// <returns></returns>
    public LeafNode CreateSubBehaviour(string nodeName, StateMachineEngine subStateMachine, State stateTo)
    {
        State state = new State(nodeName, subStateMachine.GetState("Entry_Machine"), stateTo, subStateMachine, this);
        LeafNode leafNode = new LeafNode("Node to return", state, this);
        subStateMachine.NodeToReturn = leafNode;
        states.Add(leafNode.StateNode.Name, leafNode.StateNode);

        return subStateMachine.NodeToReturn;
    }

    #endregion create sub-state machines
}