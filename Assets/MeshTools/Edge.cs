using System;
using UnityEngine;

namespace MeshTools
{
	internal class Edge : IEquatable<Edge>
    {
        public Vector2Int Position0 { get; }
        public Vector2Int Position1 { get; }
        public Vector2Int ReferencePoint { get; }

        public Edge(Vector2Int position0, Vector2Int position1, Vector2Int referencePoint)
        {
            Position0 = position0;
            Position1 = position1;
            ReferencePoint = referencePoint;
        }

        public bool Contains(Vector2Int point) => Position0 == point || Position1 == point;

        public bool Equals(Edge other) => other != null && Position0.Equals(other.Position0) && Position1.Equals(other.Position1);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Edge) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position0.GetHashCode() * 397) ^ Position1.GetHashCode();
            }
        }
    }
}
