using System.Collections;
using System.Collections.Generic;

public class IsInStatePerception : Perception {

    #region variables

    private State stateToLook;
    private BehaviourEngine engineToLook;

    #endregion variables

    /// <summary>
    /// Creates a <see cref="Perception"/> that returns true when a machine is in the State/Action provided
    /// </summary>
    /// <param name="engineToLook">The engine that contains the State/Action to look</param>
    /// <param name="stateToLookFor">The name of the State or Action that you want to fetch</param>
    /// <param name="behaviourEngine">The behaviout engine that contains this perception</param>
    public IsInStatePerception(BehaviourEngine engineToLook, string stateToLookFor, BehaviourEngine behaviourEngine)
    {
        this.engineToLook = engineToLook;
        this.stateToLook = engineToLook.GetState(stateToLookFor);
        base.behaviourEngine = behaviourEngine;
    }

    public override bool Check()
    {
        if(engineToLook.actualState == stateToLook) {
            return true;
        }

        return false;
    }
}