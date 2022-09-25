using System.Collections;
using System.Collections.Generic;

public class AndPerception : Perception {

    #region variables

    public Perception PerceptionLeft { get; }
    public Perception PerceptionRight { get; }

    private bool perLeft, perRight, reseter;

    #endregion variables

    public AndPerception(Perception perceptionLeft, Perception perceptionRight, BehaviourEngine behaviourEngine) : base()
    {
        this.PerceptionLeft = perceptionLeft;
        this.PerceptionRight = perceptionRight;
        this.reseter = true;
        base.behaviourEngine = behaviourEngine;
    }

    public override bool Check()
    {
        if(reseter)
            ResetPerceptions();

        if(!perLeft)
            perLeft = PerceptionLeft.Check();
        if(!perRight)
            perRight = PerceptionRight.Check();

        if(perLeft && perRight) {
            reseter = true;
            return true;
        }
        else {
            return false;
        }
    }

    private void ResetPerceptions()
    {
        perLeft = perRight = false;
        reseter = false;
    }

    public override void Reset()
    {
        PerceptionLeft.Reset();
        PerceptionRight.Reset();
    }
}