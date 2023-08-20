using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeshTools
{
	internal static class ShapeBuilder
    {
        public static List<Shape> GetShapes(ICollection<Vector2Int> cellPositions, Vector2Int padding)
        {
            var bounds = Vector2IntExtensions.GetBounds(cellPositions);
            bounds.min -=  padding;
            bounds.max += new Vector2Int(padding.x + 1, padding.y + 1);

            var externalBorder = new Border(new List<Vector2Int>
            {
                bounds.min,
                new Vector2Int(bounds.xMin, bounds.yMax),
                bounds.max,
                new Vector2Int(bounds.xMax, bounds.yMin),
            });

            // todo: bevel the outerborder here?

            var borders = EdgeSolver.GetBorders(cellPositions);
            borders.Add(externalBorder);

            var nodes = borders.Select((border, i) => new Node(i, border)).ToList();

            foreach (var node in nodes)
            {
                foreach (var otherNode in nodes)
                {
                    if (node.Equals(otherNode))
                        continue;
                    if (node.Border.Contains(otherNode.Border))
                    {
                        node.Children.Add(otherNode);
                    }
                }
            }

            var safety = 1000;
            var root = nodes.Last();
            root.Rank = 0;

            var current = new List<Node> {root};

            while (current.Count > 0)
            {
                if (safety-- < 0)
                {
                    ErrorLogger.Log($"{nameof(ShapeBuilder)} exceeded maximum iterations on hierarchy solver");
                    break;
                }

                var visited = new List<Node>();
                foreach (var node in current)
                {
                    var newChildren = new HashSet<Node>(node.Children.Where(x => IsUnique(x, node)));
                    foreach (var child in newChildren)
                        child.Rank = node.Rank + 1;

                    node.Children = newChildren;
                    visited.AddRange(newChildren);
                }

                current = visited;
            }

            var evenNodes = nodes.Where(x => x.Rank % 2 == 0);
            var shapes = new List<Shape>();

            foreach (var node in evenNodes)
            {
                shapes.Add(new Shape
                {
                    IsRoot = node.Rank == 0,
                    Outline = node.Border,
                    Holes = node.Children.Select(x => x.Border).ToList()
                });
            }

            return shapes;
        }

		private static bool IsUnique(Node uniqueChild, Node node)
        {
            var count = 0;
            return Count(count, uniqueChild, node) == 1;
        }

		private static int Count(int count, Node uniqueChild, Node node)
        {
            foreach (var child in node.Children)
            {
                if (child.Equals(uniqueChild))
                    count++;
                else
                    count = Count(count, uniqueChild, child);
            }

            return count;
        }
    }
}
