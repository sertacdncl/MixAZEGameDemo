using System.Collections.Generic;
using UnityEngine;

namespace MeshTools
{
    public class Border
    {
        public List<Vector2Int> Points { get; }
        public float Size { get; }
		private readonly Rect2D _aabb;

        public Border(List<Vector2Int> points)
        {
            Points = points;
            _aabb = GetAABB(points);
            Size = _aabb.Width + _aabb.Height;
        }

        public static Rect2D GetAABB(IList<Vector2Int> points)
        {
            float xMin = int.MaxValue;
            float xMax = int.MinValue;
            float yMin = int.MaxValue;
            float yMax = int.MinValue;

            for (int i = 0; i < points.Count; i++)
            {
                var x = points[i].x;
                var y = points[i].y;

                if (x < xMin) xMin = x;
                if (x > xMax) xMax = x;

                if (y < yMin) yMin = y;
                if (y > yMax) yMax = y;
            }

            return new Rect2D(
                new Vector2(xMin, yMin),
                new Vector2(xMin, yMax),
                new Vector2(xMax, yMax),
                new Vector2(xMax, yMin)
            );
        }

        public bool Contains(Border other)
        {
            foreach (var p in other.Points)
            {
                if (!Contains(p))
                    return false;
            }

            return true;
        }

        public bool Contains(Vector2 p)
        {
            if (!_aabb.Contains(p))
                return false;

            int i, j;
            var c = false;
            var pointCount = Points.Count;

            for (i = 0, j = pointCount - 1; i < pointCount; j = i++)
            {
                var p0 = Points[j];
                var p1 = Points[i];

                if (p1.y > p.y != p0.y > p.y && p.x < (p0.x - p1.x) * (p.y - p1.y) / (p0.y - p1.y) + p1.x)
                    c = !c;
            }

            return c;
        }
    }
}
