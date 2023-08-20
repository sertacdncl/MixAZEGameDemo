using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class Position2IntExtension
    {
        public static Vector2Int ToVector2Int(this Vector3 vector) {
            return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }

        public static Vector3 ToViewPosition(this Vector2Int position)
        {
            return (Vector2)position;
        }

        public static Vector3 ToVector3WorldPositionWithOffset(this Vector2Int pos, Vector3 offset, Transform parent)
        {
            return parent.TransformPoint(new Vector3(pos.x, pos.y, 0) - offset);
        }

        public static Vector2 With(this Vector2 origin, float? x = null, float? y = null)
        {
            return new Vector3(x ?? origin.x, y ?? origin.y);
        }

        public static Vector2Int With(this Vector2Int origin, int? x = null, int? y = null)
        {
            return new Vector2Int(x ?? origin.x, y ?? origin.y);
        }


        public static Vector2Int GetMax(this List<Vector2Int> origin)
        {
            var max = origin[0];
            for (var i = 1; i < origin.Count; i++)
            {
                if (origin[i].x > max.x)
                {
                    max.x = origin[i].x;
                }

                if (origin[i].y > max.y)
                {
                    max.y = origin[i].y;
                }
            }

            return max;
        }

        public static Vector2Int GetRandom(this List<Vector2Int> origin, List<Vector2Int> excluded)
        {
            var randomIndex = Random.Range(0, origin.Count);
            var selected = origin[randomIndex];
            var limit = 20;
            var counter = 0;
            while (excluded.Contains(selected))
            {
                if (counter > limit)
                {
                    Debug.LogError("GetRandom infinite loop");
                    return Vector2Int.zero;
                }
                randomIndex = Random.Range(0, origin.Count);
                selected = origin[randomIndex];
                counter++;
            }

            return selected;
        }


    }
}
