using System.Collections;
using System.Collections.Generic;

public abstract class Perception {

    #region variables

    protected BehaviourEngine behaviourEngine;

    #endregion variables

    public virtual bool Check()
    {
        return false;
    }

    public virtual void Reset()
    {
        return;
    }


    /// <summary>
    /// Fires the perception so the transition happens
    /// </summary>
    public void Fire()
    {
        behaviourEngine.Fire(this);
    }

    public void SetBehaviourMachine(BehaviourEngine behaviourEngine)
    {
        this.behaviourEngine = behaviourEngine;
    }
}