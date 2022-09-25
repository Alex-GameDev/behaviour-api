using System;
using System.Collections.Generic;
using System.Linq;

public class MaxFusion : Fusion
{
    #region constructors
    /// <summary>
    /// Creates a fusion factor that chooses the highest factor value.
    /// </summary>
    /// <param name="factors">The <see cref="Factor"/> list provided to get the highest value</param>
    public MaxFusion(List<Factor> factors) : base(factors) { }
    #endregion

    #region methods

    public override float getValue()
    {
        return factors.Max(f => f.getValue());
    }

    #endregion
}
