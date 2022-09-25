using System.Collections;
using System.Collections.Generic;
using System;

public class ValuePerception : Perception {

    #region variables

    private Func<bool>[] comparisons;

    #endregion variables

    public ValuePerception(Func<bool>[] comparisons, BehaviourEngine behaviourEngine) : base()
    {
        this.comparisons = comparisons;
        base.behaviourEngine = behaviourEngine;
    }

    public override bool Check()
    {
        foreach(Func<bool> result in comparisons) {
            if(!result())
                return false;
        }

        return true;
    }
}