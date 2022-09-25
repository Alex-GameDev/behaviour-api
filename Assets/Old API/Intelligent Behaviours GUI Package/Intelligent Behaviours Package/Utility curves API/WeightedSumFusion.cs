using System;
using System.Collections.Generic;
using System.Linq;

public class WeightedSumFusion : Fusion
{
    private List<float> weights;

    #region constructors

    /// <summary>
    /// Creates a Weighted sum fusion with equal weight for each factor.
    /// </summary>
    public WeightedSumFusion(List<Factor> factors) : base(factors) { }

    /// <summary>
    /// Creates a Weighted sum fusion with custom weights.
    /// WARNING: the sum of the weights should be in total 1.0
    /// </summary>
    public WeightedSumFusion(List<Factor> factors, List<float> weights) : base(factors)
    {
        this.weights = weights;
    }
    #endregion

    #region methods

    public override float getValue()
    {
        if(weights == null) return factors.Sum(f => f.getValue()) / factors.Count;


        float sum = 0.0f;
        for(int i = 0; i < factors.Count; i++)
        {
            float factor = factors[i].getValue();

            sum += factor * weights[i];
        }

        return sum;
    }

    #endregion
}
