using System.Collections;
using System.Collections.Generic;

public class OrPerception : Perception {

    #region variables

    public Perception PerceptionLeft { get; }
    public Perception PerceptionRight { get; }

    #endregion variables

    public OrPerception(Perception perceptionLeft, Perception perceptionRight, BehaviourEngine behaviourEngine)
    {
        this.PerceptionLeft = perceptionLeft;
        this.PerceptionRight = perceptionRight;
        base.behaviourEngine = behaviourEngine;
    }

    public override bool Check()
    {
        if(PerceptionLeft.Check() || PerceptionRight.Check()) {
            return true;
        }
        else {
            return false;
        }
    }

    public override void Reset()
    {
        PerceptionLeft.Reset();
        PerceptionRight.Reset();
    }
}