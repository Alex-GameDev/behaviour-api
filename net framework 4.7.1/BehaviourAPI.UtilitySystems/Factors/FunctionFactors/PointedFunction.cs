using System.Collections.Generic;

namespace BehaviourAPI.UtilitySystems
{
    using Core;
    public class PointedFunction : FunctionFactor
    {
        public List<Vector2> points = new List<Vector2>();
        // Points shouldn't be changed directly using the list

        public PointedFunction SetPoints(List<Vector2> points)
        {
            this.points = points;
            points.Sort((u, v) => u.x.CompareTo(v.x));
            return this;
        }

        protected override float Evaluate(float x)
        {
            int id = FindClosestLowerId(x);
            if (id == -1) 
                return points[0].y;
            else if(id == points.Count - 1) 
                return points[points.Count - 1].x;
            else
            {
                var delta = (x - points[id].x) / points[id + 1].x - points[id].x;
                return points[id].y * (1 - delta) + points[id + 1].x * delta;
            }
        }

        int FindClosestLowerId(float x)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (x <= points[i].x) return i - 1;
            }
            return points.Count - 1;
        }
    }
}
