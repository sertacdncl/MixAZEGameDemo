using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeshTools
{
	internal static class EdgeSolver
    {
		private static readonly Vector2Int[] Directions =
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
        };

		private static readonly Vector2Int[] Diagonals =
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, 0),
        };


        public static List<Border> GetBorders(ICollection<Vector2Int> cellPositions)
        {
            var edges = new HashSet<Edge>();
            var cellSet = new HashSet<Vector2Int>(cellPositions);

            foreach (var cellPosition in cellPositions)
            {
                for (var i = 0; i < Directions.Length; i++)
                {
                    var direction = Directions[i];
                    var neighbourCellPos = cellPosition + direction;
                    if (!cellSet.Contains(neighbourCellPos))
                    {
                        var position0 = cellPosition + Diagonals[i];
                        var position1 = cellPosition + Diagonals[(i + 1) % 4];
                        edges.Add(new Edge(position0, position1, cellPosition));
                    }
                }
            }

            var borders = new List<Border>();

            var safety = 1000;
            while (edges.Count > 0)
            {
                safety--;
                if (safety < 0)
                {
                    ErrorLogger.Log($"{nameof(EdgeSolver)} exceeded maximum iterations on border building");
                    break;
                }

                var sortedBorder = TryBuildBorder(edges);

                if (!IsClockwise(sortedBorder))
                    sortedBorder.Reverse();
                Simplify(sortedBorder);
                borders.Add(new Border(sortedBorder));
            }

            return borders;
        }

		private static bool IsClockwise(IList<Vector2Int> polygon)
        {
            var edgeSum = 0f;
            for (var i = 0; i < polygon.Count; i++)
            {
                var p0 = polygon[i];
                var p1 = polygon[(i + 1) % polygon.Count];
                edgeSum += (p1.x - p0.x) * (p1.y + p0.y);
            }

            return edgeSum > 0;
        }

		private static void Simplify(List<Vector2Int> sortedBorder)
        {
            for (var i = 0; i < sortedBorder.Count; i++)
            {
                var p0 = sortedBorder[(i - 1 + sortedBorder.Count) % sortedBorder.Count];
                var p1 = sortedBorder[i];
                var p2 = sortedBorder[(i + 1) % sortedBorder.Count];

                if (p0.y == p1.y && p1.y == p2.y)
                {
                    sortedBorder.Remove(sortedBorder[i]);
                    i--;
                }

                if (p0.x == p1.x && p1.x == p2.x)
                {
                    sortedBorder.Remove(sortedBorder[i]);
                    i--;
                }
            }
        }

		private static List<Vector2Int> TryBuildBorder(HashSet<Edge> edges)
        {
            var sortedBorder = new List<Vector2Int>();

            var currentEdge = edges.First();
            var currentPoint = currentEdge.Position0;

            sortedBorder.Add(currentPoint);
            var safety = 1000;

            currentPoint = currentEdge.Position1;
            var containsEdge = true;
            var possibleEdges = new List<Edge>();
            while (containsEdge)
            {
                safety--;
                if (safety < 0)
                {
                    ErrorLogger.Log($"{nameof(EdgeSolver)} exceeded maximum iterations on edge solving");
                    break;
                }

                edges.Remove(currentEdge);
                possibleEdges.Clear();
                foreach (var edge in edges)
                {
                    if (edge.Contains(currentPoint))
                        possibleEdges.Add(edge);
                }

                Edge minEdge = null;
                if (possibleEdges.Count == 1)
                    minEdge = possibleEdges[0];
                else if (possibleEdges.Count > 1)
                    minEdge = possibleEdges.FirstOrDefault(edge => edge.ReferencePoint == currentEdge.ReferencePoint);

                if (minEdge != null)
                {
                    sortedBorder.Add(currentPoint);
                    currentEdge = minEdge;
                    currentPoint = currentPoint == minEdge.Position0 ? minEdge.Position1 : minEdge.Position0;
                }
                else
                {
                    containsEdge = false;
                }
            }

            return sortedBorder;
        }
    }
}
