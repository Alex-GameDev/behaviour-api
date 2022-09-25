using System;
using System.Collections.Generic;

public abstract class Fusion : Factor
{
    #region variables

    protected List<Factor> factors;
    
    #endregion

    #region methods
    public Fusion(List<Factor> factors)
    {
        this.factors = factors;
    }
    #endregion
}
