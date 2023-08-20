using System.Collections.Generic;
using MeshTools;
using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "MazeConfig", menuName = "ScriptableObjects/Configs/MazeConfig", order = 1)]
	public class MazeConfig : ScriptableObject
	{
		[Header("Maze Settings")]
		public Vector2Int CellSize;
		public Color MazeColor;
		public int MazeBorderDepth;
		public int MazeOffset;

        [Header("Camera Settings")]
		public Vector2 CameraBoundsOffset;
		public float MinCameraZoom;
		public Vector3 CameraOffset;

		[Header("Tutorial")]
		public List<Vector2Int> TutorialDirections;

		public float HitWallParticleDestroyDuration;
		public float SwipeInputThreshold;
		public CornerConfig CornerConfig;
		public GameObject MazeWallMesh;
	}
}