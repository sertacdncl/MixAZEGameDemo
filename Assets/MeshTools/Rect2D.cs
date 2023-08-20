using UnityEngine;

namespace MeshTools
{
    public readonly struct Rect2D
    {
        public readonly Vector2 P0;
        public readonly Vector2 P1;
        public readonly Vector2 P2;
        public readonly Vector2 P3;
        public readonly float Width;
        public readonly float Height;

        public Rect2D(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
            P3 = p3;

            Width = p3.x - p0.x;
            Height = p1.y - p0.y;
        }

        public bool Contains(Vector2 p)
        {
            var ab = P1 - P0;
            var bc = P2 - P1;
            var am = p - P0;
            var bm = p - P1;
            var dotAbam = Vector2.Dot(ab, am);
            var dorBcbm = Vector2.Dot(bc, bm);

            return 0 <= dotAbam && dotAbam <= Vector2.Dot(ab, ab) &&
                   0 <= dorBcbm && dorBcbm <= Vector2.Dot(bc, bc);
        }
    }
}