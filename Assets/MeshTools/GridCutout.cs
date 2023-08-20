using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using UnityEngine;

namespace MeshTools
{
    public enum CornerType
    {
        CubicBezier,
        Circle,
        Sharp
    }
    [System.Serializable]
    public class CornerConfig
    {
        public CornerType Type = CornerType.Circle;
        public float Radius = 0.25f;
        public int VertexCount = 5;
        public void ValidateConfig()
        {
            if (Type == CornerType.Sharp)
            {
                Radius = 0.001f;
                VertexCount = 1;
            }

            Radius = Mathf.Max(0.001f, Radius);
            VertexCount = Mathf.Max(1, VertexCount);
        }
    }

    public static class GridCutout
    {
		private static readonly GenericMesher GenericMesher = new GenericMesher();
        
        public static Mesh GetSingleMesh(MeshGenerationContext context)
        {
            if (context == null)
                return null;

            var shapes = ShapeBuilder.GetShapes(context.CellPositions, context.PlanePaddingSize);
            var offset = -.5f * Vector2.one;

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var triangleId = 0;

            foreach (var shape in shapes)
            {
                var border = shape.Outline;
                var outline = shape.IsRoot
                    ? border.Points.Select(x => x + offset).ToList()
                    : GetWithRoundedCorners(border.Points, offset, context.Config);

                var holes = shape.Holes.Select(hole => GetWithRoundedCorners(hole.Points, offset, context.Config).ToList())
                    .ToArray();

                Triangulate(triangleId, outline, out var verts, out var tris, holes);
                vertices.AddRange(verts);
                triangles.AddRange(tris);
                triangleId = triangles.Max() + 1;

                if (!shape.IsRoot)
                {
                    Extrude(triangleId, outline, out verts, out tris, false, context.WallDepth);
                    vertices.AddRange(verts);
                    triangles.AddRange(tris);
                    triangleId = triangles.Max() + 1;
                }

                foreach (var hole in holes)
                {
                    Extrude(triangleId, hole, out verts, out tris, true, context.WallDepth);
                    vertices.AddRange(verts);
                    triangles.AddRange(tris);
                    triangleId = triangles.Max() + 1;
                    context.LineRendererPoints.Add(hole.Select(p => new Vector3(p.x, p.y, -0.05f)).ToArray());
                }
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

            return mesh;
        }

		private static void Triangulate(int triangle0, IList<Vector2> polygonPoints, out List<Vector3> verts,
            out List<int> tris,
            params List<Vector2>[] holes)
        {
            var polygon = new Polygon();
            polygon.Add(new Contour(polygonPoints.Select(v => new Vertex(v.x, v.y)).ToArray()));

            foreach (var hole in holes)
                polygon.Add(new Contour(hole.Select(v => new Vertex(v.x, v.y)).ToArray()), true);

            var mesh = GenericMesher.Triangulate(polygon);

            verts = new List<Vector3>(new Vector3[mesh.Triangles.Count * 3]);
            tris = new List<int>(new int[mesh.Triangles.Count * 3]);
            var triangleIndex = 0;
            foreach (var triangle in mesh.Triangles)
            {
                var vertex0 = triangle.GetVertex(0);
                var vertex1 = triangle.GetVertex(1);
                var vertex2 = triangle.GetVertex(2);

                var p0 = new Vector3((float) vertex0.X, (float) vertex0.Y, 0);
                var p1 = new Vector3((float) vertex1.X, (float) vertex1.Y, 0);
                var p2 = new Vector3((float) vertex2.X, (float) vertex2.Y, 0);

                verts[triangleIndex] = p0;
                verts[triangleIndex + 1] = p1;
                verts[triangleIndex + 2] = p2;

                tris[triangleIndex] = triangle0 + triangleIndex + 2;
                tris[triangleIndex + 1] = triangle0 + triangleIndex + 1;
                tris[triangleIndex + 2] = triangle0 + triangleIndex;

                triangleIndex += 3;
            }
        }

		private static void Extrude(int triangleId, IList<Vector2> points, out List<Vector3> vertices, out List<int> triangles,
            bool isHole,
            float depth = 1)
        {
            var offSet = new Vector3(0, 0, depth);

            vertices = new List<Vector3>();
            triangles = new List<int>();

            //points = points.ToList().Bevel(.1f, 10);


            var count = points.Count;
            for (int i = 0; i < count; i++)
            {
                var p0 = (Vector3) points[i];
                var p1 = p0 + offSet;

                vertices.Add(p0);
                vertices.Add(p1);

                var t0 = 2 * i;
                var t1 = 2 * i + 1;
                var t2 = 2 * (i + 1);
                var t3 = 2 * (i + 1) + 1;

                t0 %= 2 * count;
                t1 %= 2 * count;
                t2 %= 2 * count;
                t3 %= 2 * count;

                if (isHole)
                {
                    triangles.Add(t0 + triangleId);
                    triangles.Add(t2 + triangleId);
                    triangles.Add(t1 + triangleId);

                    triangles.Add(t1 + triangleId);
                    triangles.Add(t2 + triangleId);
                    triangles.Add(t3 + triangleId);
                }
                else
                {
                    triangles.Add(t0 + triangleId);
                    triangles.Add(t1 + triangleId);
                    triangles.Add(t2 + triangleId);

                    triangles.Add(t2 + triangleId);
                    triangles.Add(t1 + triangleId);
                    triangles.Add(t3 + triangleId);
                }
            }
        }


		private static List<Vector2> GetWithRoundedCorners(IList<Vector2Int> gridHoles, Vector2 offset, CornerConfig config)
        {
            var radius = config.Radius;
            var cornerType = config.Type;
            var vertexCount = config.VertexCount;

            var roundedPoints = new List<Vector2>();

            switch (cornerType)
            {
                case CornerType.CubicBezier:
                    for (var i = 0; i < gridHoles.Count; i++)
                    {
                        var p0 = gridHoles[i] + offset;
                        var p1 = gridHoles[(i + 1) % gridHoles.Count] + offset;
                        var p2 = gridHoles[(i + 2) % gridHoles.Count] + offset;
                        var d0 = (p0 - p1).normalized * radius;
                        var d1 = (p2 - p1).normalized * radius;

                        for (var n = 0; n < vertexCount; n++)
                        {
                            var t = n / (vertexCount - 1f);
                            roundedPoints.Add(Bezier.GetPoint(p1 + d0, p1, p1 + d1, t));
                        }
                    }
                    break;
                case CornerType.Circle:
                    for (var i = 0; i < gridHoles.Count; i++)
                    {
                        var p0 = gridHoles[i] + offset;
                        var p1 = gridHoles[(i + 1) % gridHoles.Count] + offset;
                        var p2 = gridHoles[(i + 2) % gridHoles.Count] + offset;
                        var d0 = (p0 - p1).normalized * radius;
                        var d1 = (p2 - p1).normalized * radius;

                        var rotA = Quaternion.identity;
                        var rotB = Quaternion.FromToRotation(-d0, d1);
                        for (var n = 0; n < vertexCount; n++)
                        {
                            var t = n / (vertexCount - 1f);
                            roundedPoints.Add(p1 + d1 + d0 + (Vector2) (Quaternion.Lerp(rotA, rotB, t) * -d1));
                        }
                    }
                    break;
                case CornerType.Sharp:
                    for (var i = 0; i < gridHoles.Count; i++)
                    {
                        var p0 = gridHoles[i] + offset;

                        roundedPoints.Add(p0);
                        roundedPoints.Add(p0 + new Vector2(0.0001f, 0.0001f));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cornerType), cornerType, null);
            }

            return roundedPoints;
        }
    }
}
