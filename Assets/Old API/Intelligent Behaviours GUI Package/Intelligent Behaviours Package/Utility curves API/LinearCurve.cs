using System;
using System.Collections.Generic;

public class LinearCurve : Curve
{
    #region variables

    private float m, c;

    #endregion

    #region constructors
    /// <summary>
    /// Creates a linear function factor that modify the value of the factor provided.
    /// </summary>
    /// <param name="f">The <see cref="Factor"/> provided to get a new value from it.</param>
    /// <param name="pend">The slope of the curve. Optional paramenter.</param>
    /// <param name="ind">The vertical displacement of the curve. Optional paramenter.</param>
    public LinearCurve(Factor f, float pend = 1, float ind = 0) : base(f)
    {
        this.m = pend;
        this.c = ind;
    }

    #endregion

    public override float getValue()
    {
        return m*factor.getValue() + c;
    }

    /// <summary>
    /// Sets a new value to the slope of the curve.
    /// </summary>
    public void setM(float _m)
    {
        this.m = _m;
    }

    /// <summary>
    /// Sets a new value to the vertical displacement.
    /// </summary>
    public void setC(float _c)
    {
        this.c = _c;
    }
}