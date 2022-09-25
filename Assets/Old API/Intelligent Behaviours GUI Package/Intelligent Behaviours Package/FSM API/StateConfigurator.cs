using System;
using System.Collections;
using System.Collections.Generic;

public class StateConfigurator {
    #region variables

    public enum STATE_TYPE {EMPTY, NOT_EMPTY};
    public STATE_TYPE stateType { get; set; }
    public Action entry { get; set; }

    public Dictionary<String,Action> exit;
    public Dictionary<String, Action> internalTransition;

    #endregion variables

    public StateConfigurator(STATE_TYPE st){
        this.stateType = st;
        exit = new Dictionary<string, Action>();
        internalTransition = new Dictionary<string, Action>();
    }

    #region methods
    public StateConfigurator OnEntry(Action method){
        entry = method;
        return this;
    }

    public StateConfigurator OnExit(String name, Action method){
        if (!exit.ContainsKey(name)) exit.Add(name, method);
        return this;
    }

    public StateConfigurator InternalTransition(Perception perception, String name, Action method){
        /* TODO ¿Es necesaria la percepción?*/
        if (!internalTransition.ContainsKey(name)) internalTransition.Add(name, method);
        return this;
    }
    #endregion methods
}