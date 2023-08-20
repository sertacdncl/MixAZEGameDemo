using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
	public class MazeManager : Singleton<MazeManager>
	{
		public List<FloorController> FloorList { get; set; } = new();
		public Transform mazeBackground;
		public GameObject mazeWallObject;

		public void PaintFloors(List<Vector2Int> movePath, Dictionary<Vector2Int,Color> colorList)
		{
			foreach (var floorController in FloorList)
			{
				var floorTransform = floorController.transform;
				var floorCoord = floorTransform.position.ToVector2Int();
				if (movePath.Contains(floorCoord))
				{
					if(floorController.isColorChangeFloor)
						continue;

					var targetColor = colorList[floorCoord];
					if (floorController.IsPainted)
					{
						if (colorList.ContainsKey(floorCoord))
						{
							if(floorController.CurrentColor.Equals(targetColor))
								continue;
						}
						else
						{
							continue;
						}
						
					}
					
					floorController.IsPainted = true;
					var perTime = 0.2f / movePath.Count;
					DOVirtual.DelayedCall(perTime, () => { floorController.ReplaceColor(targetColor); });
				}
			}
		}
	}
}