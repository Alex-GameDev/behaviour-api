using System;
using System.Collections.Generic;
using System.Linq;

public class MinFusion : Fusion
{
    #region constructors
    /// <summary>
    /// Creates a fusion factor that chooses the lowest factor value.
    /// </summary>
    /// <param name="factors">The <see cref="Factor"/> list provided to get the lowest value</param>
    public MinFusion(List<Factor> factors) : base(factors) { }
    #endregion

    #region methods
    public override float getValue()
    {
        return factors.Min(f => f.getValue());
    }
    #endregion
}
