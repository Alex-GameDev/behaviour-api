using System;
using System.Collections.Generic;

public class ExpCurve: Curve
{
    #region variables

    private float k, c, b;

    #endregion

    #region constructors
    /// <summary>
    /// Creates an exponential function factor that modify the value of the factor provided.
    /// </summary>
    /// <param name="f">The <see cref="Factor"/> provided to get a new value from it.</param>
    /// <param name="exp">The exponent of the curve</param>
    /// <param name="despX">The horizontal displacement provided to the curve. Optional parameter.</param>
    /// <param name="despY">The vertical displacement provided to the curve. Optional parameter.</param>
    public ExpCurve(Factor f, float exp = 1, float despX = 0, float despY = 0) : base(f)
    {
        this.k = exp;
        this.c = despX;
        this.b = despY;
        this.factor = f;
    }

    #endregion

    public override float getValue()
    {
        return (float) (Math.Pow(factor.getValue() - c, k) + b);
    }

    /// <summary>
    /// Sets a new value to the exponent.
    /// </summary>
    public void setK(float _k)
    {
        this.k = _k;
    }

    /// <summary>
    /// Sets a new value to the horizontal displacement.
    /// </summary>
    public void setC(float _c)
    {
        this.c = _c;
    }

    /// <summary>
    /// Sets a new value to the vertical displacement.
    /// </summary>
    public void setB(float _b)
    {
        this.b = _b;
    }
}