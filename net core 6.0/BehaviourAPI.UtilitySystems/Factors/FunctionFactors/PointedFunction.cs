using System.Numerics;

namespace BehaviourAPI.UtilitySystems
{
    public class PointedFunction : FunctionFactor
    {
        public List<Vector2> points = new List<Vector2>();
        // Points shouldn't be changed directly using the list

        public PointedFunction SetPoints(List<Vector2> points)
        {
            this.points = points;
            points.Sort((u, v) => u.X.CompareTo(v.X));
            return this;
        }

        protected override float Evaluate(float x)
        {
            int id = findClosestLowerId(x);
            if (id == -1) 
                return points[0].Y;
            else if(id == points.Count - 1) 
                return points[points.Count - 1].X;
            else
            {
                var delta = (x - points[id].X) / points[id + 1].X - points[id].X;
                return points[id].Y * (1 - delta) + points[id + 1].Y * delta;
            }
        }

        int findClosestLowerId(float x)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (x <= points[i].X) return i - 1;
            }
            return points.Count - 1;
        }
    }
}
