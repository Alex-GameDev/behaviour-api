using System;
using System.Collections;
using System.Collections.Generic;

public class State {

    #region variables

    public string Name { get; }
    public Perception StatePerception { get; }
    public BehaviourEngine BehaviourEngine { get; }

    public StateConfigurator configurator;

    private Action StateActionVoid;
    private Action<Perception> StateActionPerception;

    #endregion variables

    #region constructors

    /// <summary>
    /// Empty state
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    /// <param name="behaviourEngine">The machine which the state belongs to</param>
    public State(string stateName, BehaviourEngine behaviourEngine)
    {
        this.Name = stateName;
        this.BehaviourEngine = behaviourEngine;
        this.configurator = new StateConfigurator(StateConfigurator.STATE_TYPE.EMPTY);
    }

    /// <summary>
    /// State that runs a method with no parameters
    /// </summary>
    /// <param name="stateName">Name of the state</param>
    /// <param name="method">Method with no parameters. MUST be a void method</param>
    /// <param name="behaviourEngine">The machine which the state belongs to</param>
    public State(string stateName, Action method, BehaviourEngine behaviourEngine)
    {
        this.Name = stateName;
        this.StateActionVoid = method;
        this.BehaviourEngine = behaviourEngine;
        this.configurator = new StateConfigurator(StateConfigurator.STATE_TYPE.NOT_EMPTY);

        BehaviourEngine.Configure(this)
            .OnEntry(() => method());
    }

    /// <summary>
    /// State that runs a method with a <see cref="Perception"/> parameter
    /// </summary>
    /// <param name="stateName">Name of the state</param>
    /// <param name="method">Method with no parameters. MUST be a void method</param>
    /// <param name="behaviourEngine">The machine which the state belongs to</param>
    public State(string stateName, Action<Perception> method, BehaviourEngine behaviourEngine)
    {
        this.Name = stateName;
        this.StateActionPerception = method;
        this.StatePerception = (Perception)method.Method.GetParameters().GetValue(0);
        this.BehaviourEngine = behaviourEngine;
        this.configurator = new StateConfigurator(StateConfigurator.STATE_TYPE.NOT_EMPTY);

        BehaviourEngine.Configure(this)
            .OnEntry(() => method((Perception)method.Method.GetParameters().GetValue(0)));
    }

    /// <summary>
    /// Creates a state with a sub-machine in it
    /// </summary>
    /// <param name="stateName">The name of the state</param>
    /// <param name="subStateMachine">The sub-state machine that the state has</param>
    /// <param name="entryState">The entry state of the submachine</param>
    /// <param name="stateMachineB">The state machine which the state belongs to</param>
    public State(string stateName, State entrySubMachineState, State stateTo, BehaviourEngine subMachine, BehaviourEngine behaviourEngine)
    {
        this.Name = stateName;
        this.BehaviourEngine = behaviourEngine;
        this.configurator = new StateConfigurator(StateConfigurator.STATE_TYPE.NOT_EMPTY);

        BehaviourEngine.Configure(this)
            .OnEntry(() => EntrySubmachine(entrySubMachineState, stateTo, subMachine, behaviourEngine));
    }

    #endregion constructors

    private void EntrySubmachine(State entrySubmachineState, State stateTo, BehaviourEngine subMachine, BehaviourEngine behaviourEngine)
    {
        if(subMachine.actualState != subMachine.GetState("Entry_Machine"))
            return;

        new Transition("Entry_submachine state", subMachine.actualState, new PushPerception(subMachine), stateTo, subMachine)
            .FireTransition();

        behaviourEngine.Active = false;
        subMachine.Active = true;
    }

    public void Entry(){
        if(this.configurator.stateType != StateConfigurator.STATE_TYPE.EMPTY){
            configurator.entry();
        }
    }
    public void InternalTransition(String tName)
    {
        Action toExecute;
        if (configurator.internalTransition.TryGetValue(tName, out toExecute))
        {
            toExecute.Invoke();
        }
    }

    public void Exit(String tName) {
        Action toExecute;
        if (configurator.exit.TryGetValue(tName, out toExecute))
        {
            toExecute.Invoke();
        }              
    }
}