using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public class Transition {

    #region variables
    public enum TRANSITION_TYPE { NORMAL, SUPER_TRANSITION };
    public TRANSITION_TYPE type;
    public string Name { get; }
    public State StateFrom { get; }
    public Perception Perception { get; }
    public State StateTo { get; }
    public BehaviourEngine BehaviourEngine { get; }

    #endregion variables

    /// <summary>
    /// Creates a transition between states from a <see cref="BehaviourStateMachine"/>
    /// </summary>
    /// <param name="name">The name of the transition</param>
    /// <param name="stateFrom">The exit state of the transition</param>
    /// <param name="perception">The perception that activates the transition</param>
    /// <param name="stateTo">The entry state of the transition</param>
    /// <param name="behaviourEngine">The machine the transitions belongs to</param>
    public Transition(string name, State stateFrom, Perception perception, State stateTo, BehaviourEngine behaviourEngine)
    {
        this.Name = name;
        this.StateFrom = stateFrom;
        this.Perception = perception;
        this.StateTo = stateTo;
        this.BehaviourEngine = behaviourEngine;
        this.type = TRANSITION_TYPE.NORMAL;

        BehaviourEngine.Configure(StateFrom)
            .OnExit(this.Name, () => { Perception.Reset(); });
    }

    /// <summary>
    /// Creates a transition between two <see cref="BehaviourEngine"/>, one is the submachine the other the supermachine
    /// </summary>
    /// <param name="name">The name of the transition</param>
    /// <param name="stateFrom">The exit state of the transition</param>
    /// <param name="perception">The perception that activates the transition</param>
    /// <param name="stateTo">the entry state of the transition</param>
    /// <param name="superMachine">The supermachine</param>
    /// <param name="subMachine">The submachine</param>
    public Transition(string name, State stateFrom, Perception perception, State stateTo, BehaviourEngine superMachine, BehaviourEngine subMachine)
    {
        this.Name = name;
        this.StateFrom = stateFrom;
        this.Perception = perception;
        this.StateTo = stateTo;
        this.type = TRANSITION_TYPE.SUPER_TRANSITION;

        if (stateFrom.BehaviourEngine == superMachine) { // Exits from the super-machine
            this.BehaviourEngine = superMachine;
            Perception.SetBehaviourMachine(superMachine);
            superMachine.Configure(StateFrom)
                .OnExit(this.Name, () => Perception.Reset())
                .InternalTransition(Perception, this.Name, () => ExitTransition(StateFrom, StateTo, subMachine.GetState("Entry_Machine"), superMachine, subMachine));
        }
        else { // Exits from the sub-machine
            this.BehaviourEngine = subMachine;
            Perception.SetBehaviourMachine(subMachine);
            subMachine.Configure(StateFrom)
                .OnExit(this.Name, () => Perception.Reset())
                .InternalTransition(Perception, this.Name, () => ExitTransition(StateFrom, StateTo, subMachine.GetState("Entry_Machine"), superMachine, subMachine));
        }
    }

    /// <summary>
    /// Executes the exit transitions between a submachine and the supermachine
    /// </summary>
    /// <param name="stateFrom">The exit state of the transition</param>
    /// <param name="stateTo">The entry state of the transition</param>
    /// <param name="entrySubMachineState">The 'Entry_State' of the submachine</param>
    /// <param name="superMachine">The supermachine where the trasition is going to</param>
    /// <param name="subMachine">The subsmachine the transition is leaving</param>
    private void ExitTransition(State stateFrom, State stateTo, State entrySubMachineState, BehaviourEngine superMachine, BehaviourEngine subMachine)
    {
        if(stateFrom.BehaviourEngine == superMachine) { // Exits from the super-machine
            // Transitions from Submachine.CurrentState -> Submachine.EntryState
            Transition returnTransition = new Transition("reset_submachine", subMachine.actualState, new PushPerception(subMachine), entrySubMachineState, subMachine);
            returnTransition.FireTransition();

            subMachine.Reset();
            subMachine.Active = false;
            superMachine.Active = true;

            // Transitions from stateFrom -> stateTo
            Transition superTransition = new Transition("to_state", stateFrom, new PushPerception(superMachine), stateTo, superMachine);
            superTransition.FireTransition();
        }
        else { // Exits from the sub-machine
            // Transitions from stateFrom -> Submachine.EntryState
            Transition returnTransition = new Transition("reset_submachine", stateFrom, new PushPerception(subMachine), entrySubMachineState, subMachine);
            returnTransition.FireTransition();

            subMachine.Reset();
            subMachine.Active = false;
            superMachine.Active = true;

            // Transitions from Supermachine.CurrentState -> stateTo
            Transition superTransition = new Transition("to_state", superMachine.actualState, new PushPerception(superMachine), stateTo, superMachine);
            superTransition.FireTransition();
        }
    }

    /// <summary>
    /// Creates a transition between one <see cref="BehaviourEngine."/>, the submachine; and one <see cref="BehaviourTreeEngine"/>, the supermachine.
    /// </summary>
    /// <param name="name">The name of the transition</param>
    /// <param name="stateFrom">The exit state of the transition</param>
    /// <param name="perception">The perception that activates the transition</param>
    /// <param name="stateTo">The node where the Behaviour Tree is coming back</param>
    /// <param name="superMachine">The supermachine</param>
    /// <param name="subMachine">The submachine</param>
    public Transition(string name, State stateFrom, Perception perception, LeafNode stateTo, ReturnValues returnValue, BehaviourTreeEngine superMachine, BehaviourEngine subMachine)
    {
        this.Name = name;
        this.StateFrom = stateFrom;
        this.Perception = perception;
        this.StateTo = stateTo.StateNode;
        this.type = TRANSITION_TYPE.SUPER_TRANSITION;

        if (StateFrom.BehaviourEngine == superMachine) { // Exits from the super-machine
            this.BehaviourEngine = superMachine;
            Perception.SetBehaviourMachine(superMachine);
            superMachine.Configure(StateFrom)
                .OnExit(this.Name, () => Perception.Reset())
                .InternalTransition(Perception, this.Name, () => ExitTransition(StateFrom, stateTo, returnValue, subMachine.GetState("Entry_Machine"), superMachine, subMachine));
        }
        else { // Exits from the sub-machine
            this.BehaviourEngine = subMachine;
            Perception.SetBehaviourMachine(subMachine);
            subMachine.Configure(StateFrom)
                .OnExit(this.Name, () => Perception.Reset())
                .InternalTransition(Perception, this.Name, () => ExitTransition(StateFrom, stateTo, returnValue, subMachine.GetState("Entry_Machine"), superMachine, subMachine));
        }
    }

    /// <summary>
    /// Creates a transition between one <see cref="BehaviourTreeEngine"/>, the submachine; and another <see cref="BehaviourTreeEngine"/>, the supermachine.
    /// </summary>
    /// <param name="name">The name of the transition</param>
    /// <param name="nodeFrom">The exit node of the transition</param>
    /// <param name="stateTo">The node where the Behaviour tree is coming back</param>
    /// <param name="superMachine">The supermachine</param>
    /// <param name="subMachine">The submachine</param>
    public Transition(string name, TreeNode nodeFrom, LeafNode stateTo, BehaviourTreeEngine superMachine, BehaviourTreeEngine subMachine)
    {
        this.Name = name;
        this.StateFrom = nodeFrom.StateNode;
        this.StateTo = stateTo.StateNode;
        this.Perception = new OrPerception(new BehaviourTreeStatusPerception(subMachine, ReturnValues.Succeed, subMachine),
                                            new BehaviourTreeStatusPerception(subMachine, ReturnValues.Failed, subMachine),
                                            subMachine);
        this.type = TRANSITION_TYPE.SUPER_TRANSITION;

        if (StateFrom.BehaviourEngine == superMachine) { // Exits from the super-machine
            this.BehaviourEngine = superMachine;
            Perception.SetBehaviourMachine(superMachine);
            superMachine.Configure(StateFrom)
                .OnExit(this.Name, () => Perception.Reset())
                .InternalTransition(Perception, this.Name, () => ExitTransition(StateFrom, stateTo, subMachine.GetRootNode().ReturnValue, subMachine.GetState("Entry_Machine"), superMachine, subMachine));
        }
        else { // Exits from the sub-machine
            this.BehaviourEngine = subMachine;
            Perception.SetBehaviourMachine(subMachine);
            subMachine.Configure(StateFrom)
                .OnExit(this.Name, () => Perception.Reset())
                .InternalTransition(Perception, this.Name, () => ExitTransition(StateFrom, stateTo, subMachine.GetRootNode().ReturnValue, subMachine.GetState("Entry_Machine"), superMachine, subMachine));
        }
    }

    /// <summary>
    /// Executes the exit transitions between a submachine and the supermachine which is a <see cref="BehaviourTreeEngine"/>
    /// </summary>
    /// <param name="stateFrom">The exit state of the transition</param>
    /// <param name="stateTo">The entry state of the transition</param>
    /// <param name="entrySubMachineState">The 'Entry_State' of the submachine</param>
    /// <param name="superMachine">The supermachine where the trasition is going to</param>
    /// <param name="subMachine">The subsmachine the transition is leaving</param>
    private void ExitTransition(State stateFrom, LeafNode stateTo, ReturnValues returnValue, State entrySubMachineState, BehaviourTreeEngine superMachine, BehaviourEngine subMachine)
    {
        if(stateFrom.BehaviourEngine == superMachine) { // Exits from the super-machine
            // Transitions from Submachine.CurrentState -> Submachine.EntryState
            Transition returnTransition = new Transition("reset_submachine", subMachine.actualState, new PushPerception(subMachine), entrySubMachineState, subMachine);
            returnTransition.FireTransition();

            subMachine.Reset();
            subMachine.Active = false;
            superMachine.Active = true;
            stateTo.ReturnValue = returnValue;

            // Transitions from stateFrom -> stateTo
            // Maybe I should change this too, as I changed the next one?
            // TODO review if this is working properly

            Transition superTransition = new Transition("to_state", stateFrom, new PushPerception(superMachine), stateTo.StateNode, superMachine);
            superTransition.FireTransition();
        }
        else { // Exits from the sub-machine
            // Transitions from stateFrom -> Submachine.EntryState
            Transition returnTransition = new Transition("reset_submachine", stateFrom, new PushPerception(subMachine), entrySubMachineState, subMachine);
            returnTransition.FireTransition();

            subMachine.Reset();
            subMachine.Active = false;
            superMachine.Active = true;
            stateTo.ReturnValue = returnValue;

            // Transitions from Supermachine.CurrentState -> stateTo

            /* ¿BUG?
             * stateTo SHOULD BE THE SAME AS SuperMachine.CurrentState  --> We are exiting from a machine
             * that returns Succeed or Failed to the SuperMachine LeafNode, if we enter again to that Node
             * we are again putting the ReturnType to "process", so we enter in an infinite loop
             */

            //Transition superTransition = new Transition("to_state", superMachine.actualState, new PushPerception(superMachine), stateTo.StateNode, superMachine);
            //superTransition.FireTransition();
        }
    }

    /// <summary>
    /// Fires the transition
    /// </summary>
    /// <returns>The transition was successfully  fired or not</returns>
    public bool FireTransition()
    {
        //  The current state is equals to the StateFrom     The state from the stateFrom's machine (the super machine) is equals to the stateFrom
        if(BehaviourEngine.actualState == StateFrom || StateFrom.BehaviourEngine.actualState == StateFrom) {
            //Debug.Log("Transition fired: " + StateFrom.Name + " -> " + StateTo.Name);
            //Console.WriteLine("Transition fired: " + StateFrom.Name + " -> " + StateTo.Name);
            StateFrom.Exit(this.Name);
            if (this.type == TRANSITION_TYPE.SUPER_TRANSITION) StateFrom.InternalTransition(this.Name);

            // To change the actualState of the StateFrom Machine, the states should be from the same machine
            if(StateTo.BehaviourEngine == StateFrom.BehaviourEngine){
                StateFrom.BehaviourEngine.actualState = StateTo;
            }

            //Entry is done in StateFrom.InternalTransition so there's no need to do the entry again
            if (this.type != TRANSITION_TYPE.SUPER_TRANSITION) StateTo.Entry();

            return true;
        }

        return false;
    }
}