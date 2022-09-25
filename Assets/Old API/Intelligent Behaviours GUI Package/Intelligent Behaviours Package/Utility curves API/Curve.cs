using System;
using System.Collections.Generic;

public abstract class Curve : Factor
{
    #region variables

    protected Factor factor;

    #endregion
    
    public Curve(Factor f)
    {
        this.factor = f;
    }
}
