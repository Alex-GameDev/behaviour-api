using System;
using System.Collections;
using System.Collections.Generic;
//using Stateless;

public class StateMachineEngine : BehaviourEngine
{

    /// <summary>
    /// Creates a behaviour if State Machine type that CANNOT be a submachine
    /// </summary>
    public StateMachineEngine()
    {
        this.transitions = new Dictionary<string, Transition>();
        this.states = new Dictionary<string, State>();
        base.IsSubMachine = false;

        entryState = new State("Entry_Machine", this);
        this.actualState = entryState;
        states.Add(entryState.Name, entryState);
        Active = true;
    }

    /// <summary>
    /// Creates a behaviour of State Machine type
    /// </summary>
    /// <param name="isSubmachine">Is this State Machine a submachine?</param>
    public StateMachineEngine(bool isSubmachine)
    {
        this.transitions = new Dictionary<string, Transition>();
        this.states = new Dictionary<string, State>();
        base.IsSubMachine = isSubmachine;

        entryState = new State("Entry_Machine", this);
        this.actualState = entryState;
        states.Add(entryState.Name, entryState);
        Active = (isSubmachine) ? false : true;
    }

    /// <summary>
    /// Update has to be called once per frame
    /// </summary>
    public void Update()
    {
        foreach (Transition transition in transitions.Values)
        {
            if (transition.StateFrom == this.actualState)
            {
                if (transition.Perception.Check())
                {
                    //Console.WriteLine("Transicion lanzada " + transition.Name);
                    Fire(transition);
                    break;
                }
            }
        }
    }

    #region create states

    /// <summary>
    /// Creates an empty entry state for the State Machine
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    /// <returns></returns>
    public State CreateEntryState(string stateName)
    {
        if (!states.ContainsKey(stateName))
        {
            State state = new State(stateName, this);
            states.Add(state.Name, state);
            if (Active)
            {
                new Transition("Entry_transition", entryState, new PushPerception(this), state, this)
                    .FireTransition();
            }
            entryState = state;

            return state;
        }
        else
        {
            throw new System.DuplicateWaitObjectException(stateName, "The State already exists in the state machine");
        }
    }

    /// <summary>
    /// Creates an entry state for the State Machine
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    /// <param name="action">The action the state will execute when the state machine enters it. MUST be a void method</param>
    /// <returns></returns>
    public State CreateEntryState(string stateName, Action action)
    {
        if (!states.ContainsKey(stateName))
        {
            State state = new State(stateName, action, this);
            states.Add(state.Name, state);
            if (Active)
            {
                new Transition("Entry_transition", entryState, new PushPerception(this), state, this)
                    .FireTransition();
            }
            entryState = state;

            return state;
        }
        else
        {
            throw new System.DuplicateWaitObjectException(stateName, "The State already exists in the state machine");
        }
    }

    /// <summary>
    /// Adds an empty state to the State Machine
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    public State CreateState(string stateName)
    {
        if (!states.ContainsKey(stateName))
        {
            State state = new State(stateName, this);
            states.Add(state.Name, state);

            return state;
        }
        else
        {
            throw new DuplicateWaitObjectException(stateName, "The State already exists in the state machine");
        }
    }

    /// <summary>
    /// Adds a new <see cref="State"/> to the State Machine
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    /// <param name="action">The action that the state will execute when the state machine enters it. MUST be a void method</param>
    /// <returns></returns>
    public State CreateState(string stateName, Action action)
    {
        if (!states.ContainsKey(stateName))
        {
            State state = new State(stateName, action, this);
            states.Add(state.Name, state);

            return state;
        }
        else
        {
            throw new DuplicateWaitObjectException(stateName, "The State already exists in the state machine");
        }
    }

    /// <summary>
    /// Adds a new <see cref="State"/> to the State Machine
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    /// <param name="action">The action that the state will execute when the state machine enters it. MUST be a void method</param>
    /// <returns></returns>
    public State CreateState(string stateName, Action<Perception> action)
    {
        if (!states.ContainsKey(stateName))
        {
            State state = new State(stateName, action, this);
            states.Add(state.Name, state);

            return state;
        }
        else
        {
            throw new DuplicateWaitObjectException(stateName, "The State already exists in the state machine");
        }
    }

    /// <summary>
    /// Adds a type of <see cref="State"/> with a sub-state machine in it and its transition to the entry state
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    /// <param name="subBehaviourEngine">The sub behaviour engine inside the state. It will enter in the entry state of the sub behaviour engine</param>
    public State CreateSubStateMachine(string stateName, BehaviourEngine subBehaviourEngine)
    {
        State stateTo = subBehaviourEngine.GetEntryState();
        State state = new State(stateName, subBehaviourEngine.GetState("Entry_Machine"), stateTo, subBehaviourEngine, this);
        states.Add(state.Name, state);

        return state;
    }

    /// <summary>
    /// Adds a type of <see cref="State"/> with a sub-state machine in it and its transition to the state <paramref name="stateTo"/>
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="subStateMachine">The sub-state machine inside the state</param>
    /// <param name="stateTo">The name of the state where the sub-state machine will enter</param>
    /// <returns></returns>
    public State CreateSubStateMachine(string stateName, StateMachineEngine subStateMachine, State stateTo)
    {
        State state = new State(stateName, subStateMachine.GetState("Entry_Machine"), stateTo, subStateMachine, this);
        states.Add(state.Name, state);

        return state;
    }

    #endregion create states

    #region create transitions

    /// <summary>
    /// Creates a new <see cref="Transition"/> that goes from one <see cref="State"/> to another when is triggered
    /// </summary>
    /// <param name="transitionName">The name of the transition</param>
    /// <param name="stateFrom">The <see cref="State"/> where the transition comes from</param>
    /// <param name="perception">The <see cref="Perception"/> that will trigger the transition</param>
    /// <param name="stateTo">The <see cref="State"/> where the transition goes to</param>
    public Transition CreateTransition(string transitionName, State stateFrom, Perception perception, State stateTo)
    {
        if (!transitions.ContainsKey(transitionName))
        {
            Transition transition = new Transition(transitionName, stateFrom, perception, stateTo, this);
            transitions.Add(transitionName, transition);

            return transition;
        }
        else
        {
            throw new DuplicateWaitObjectException(transitionName, "The Transition already exists in the state machine");
        }
    }

    #endregion create transitions
}