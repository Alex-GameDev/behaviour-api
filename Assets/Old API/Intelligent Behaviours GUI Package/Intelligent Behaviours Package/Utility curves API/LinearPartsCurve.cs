using System;
using System.Collections.Generic;

public class LinearPartsCurve : Curve
{
    #region variables
    private List<Point2D> points;
    #endregion

    #region constructors
    /// <summary>
    /// Creates a linear by parts function factor that modify the value of the factor provided.
    /// </summary>
    /// <param name="f">The <see cref="Factor"/> provided to get a new value from it.</param>
    /// <param name="points">The bunch of <see cref="Point2D"/> provided to the curve.</param>
    public LinearPartsCurve(Factor f, List<Point2D> points) : base(f)
    {
        this.setPoints(points);
    }
    #endregion

    #region methods

    /// <summary>
    /// Sets a new bunch of <see cref="Point2D"/> to the curve.
    /// </summary>
    public void setPoints(List<Point2D> points)
    {
        this.points = points;

        this.points.Sort((p1, p2) =>
        {
            return p1.x.CompareTo(p2.x);
        });
    }

    public override float getValue()
    {
        float returnValue = 0.0f;
        float x = factor.getValue();
        //if (x < 0) x = 0; if (x > 1) x = 1;

        for(int i = 0; i < points.Count; i++)
        {
            float xPoint = points[i].x;
            if (i == 0 && x < xPoint) { returnValue = points[i].y; break; };
            if ((i == points.Count - 1) && x > xPoint) { returnValue = points[i].y; break; };
            if (x == xPoint) { returnValue = points[i].y; break; }

            if (x > xPoint && x < points[i + 1].x)
            {
                returnValue = ((x-xPoint)/(points[i+1].x-xPoint))*(points[i + 1].y - points[i].y) + points[i].y;
                break;
            }
        }

        return returnValue;
    }

    #endregion
}
