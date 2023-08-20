using UnityEngine;

namespace Game
{
    public static class Vector3Extension
    {
        public static Vector3 With(this Vector3 origin, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? origin.x, y ?? origin.y, z ?? origin.z);
        }

        public static Vector3 WithZ(this Vector3 position, float z)
        {
            return new Vector3(position.x, position.y, z);
        }

    }
}
