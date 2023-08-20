using TriangleNet;
using UnityEngine;

namespace MeshTools
{
	internal class ErrorLogger : MonoBehaviour
    {
		private static readonly Color Color = new Color(1, 0f, 0.5f, 1);

        public static void Log(string message)
            => Debug.LogError(
                $"<color=#{(byte) (Color.r * 255f):X2}{(byte) (Color.g * 255f):X2}{(byte) (Color.b * 255f):X2}> <b>[ERROR]: </b>{message}</color>");
    }
}