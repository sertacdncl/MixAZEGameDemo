using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
	public class SwipeInputManager : Singleton<SwipeInputManager>
	{
		public bool IsInputBlock { get; set; }
		private Vector2 _startTouchPosition;
		private Vector2Int _swipeDirection;
		private bool _hasSwipeStarted;

		private void Update()
		{
			if (Input.GetMouseButtonDown(0) && !CheckOnUI())
			{
				_startTouchPosition = Input.mousePosition;
				_hasSwipeStarted = true;
			}

			if (Input.GetMouseButton(0) && _hasSwipeStarted)
			{
				var swipeDirection = (Vector2)Input.mousePosition - _startTouchPosition;
				var horizontalDelta = Mathf.Abs(swipeDirection.x);
				var verticalDelta = Mathf.Abs(swipeDirection.y);
				var swipeInputThreshold = ConfigMap.Instance.mazeConfig.SwipeInputThreshold;

				if (horizontalDelta > swipeInputThreshold || verticalDelta > swipeInputThreshold)
				{
					var directionValue = GetVector2IntDirectionValue(horizontalDelta, verticalDelta, swipeDirection);

					if (!IsInputBlock)
					{
						if (TutorialManager.IsTutorialActive)
							if (directionValue != TutorialManager.Instance.tutorialData.tutorialDirection)
								return;

						_swipeDirection = directionValue;
						if(directionValue != Vector2Int.zero)
							MovePlayer();
						_startTouchPosition = Input.mousePosition;
					}
				}
			}

			if (Input.GetMouseButtonUp(0) && _hasSwipeStarted)
			{
				_hasSwipeStarted = false;
				_startTouchPosition = Vector2.zero;
			}
		}

		private void MovePlayer()
		{
			var swipeDir = _swipeDirection;
			var levelData = LevelManager.CurrentLevelData;
			var cellPosList = levelData.MovableCellPosList;

			var moveData = GetMoveData(cellPosList, swipeDir);
			var sortedMoveData = GetSortedMoveData(moveData);
			var finalMoveData = GetReCalculatedMoveData(sortedMoveData, swipeDir);

			UpdatePlayerMoveData(finalMoveData, cellPosList, swipeDir);
		}

		private Dictionary<Vector2Int, List<PlayerController>> GetMoveData(List<Vector2Int> cellPosList, Vector2Int swipeDir)
		{
			var playerControllerList = PlayerManager.Instance.PlayerControllerList;
			var moveData = new Dictionary<Vector2Int, List<PlayerController>>();

			foreach (var playerController in playerControllerList)
			{
				var playerPos = playerController.transform.position.ToVector2Int();
				var targetPos = GetFurthestMovableCell(cellPosList, playerPos, swipeDir);

				if (!cellPosList.Contains(targetPos)) continue;

				if (!moveData.ContainsKey(targetPos))
					moveData.Add(targetPos, new List<PlayerController>());

				moveData[targetPos].Add(playerController);
			}

			return moveData;
		}

		private Dictionary<Vector2Int, List<PlayerController>> GetSortedMoveData(
			Dictionary<Vector2Int, List<PlayerController>> moveData)
		{
			foreach (var data in moveData)
			{
				if (data.Value.Count > 1)
				{
					SortMoveData(data.Value, data.Key);
				}
			}

			return moveData;
		}

		void SortMoveData(List<PlayerController> players, Vector2Int targetPosition)
		{
			players.Sort((a, b) =>
			{
				var aPosition = a.transform.position.ToVector2Int();
				var bPosition = b.transform.position.ToVector2Int();
				var aDistance = Vector2Int.Distance(aPosition, targetPosition);
				var bDistance = Vector2Int.Distance(bPosition, targetPosition);
				return aDistance.CompareTo(bDistance);
			});
		}

		void UpdatePlayerMoveData(Dictionary<Vector2Int, List<PlayerController>> reCalculatedMoveData,
			List<Vector2Int> cellPosList, Vector2Int swipeDir)
		{
			foreach (var (targetPos, players) in reCalculatedMoveData)
			{
				foreach (var playerController in players)
				{
					if (playerController.IsMoving)
						continue;
					
					playerController.IsMoving = true;
					var playerPos = playerController.transform.position.ToVector2Int();
					var canMovableCellList = GetMovableCellList(cellPosList, playerPos, swipeDir);
					var changeColorCell = GetChangeColorCell(canMovableCellList);
					var floorColorsForPaint = GetFloorColorsForPaint(changeColorCell, canMovableCellList, playerController);
					canMovableCellList.Remove(targetPos);

					canMovableCellList.Add(targetPos);
					if (playerPos == targetPos && canMovableCellList.Count == 1)
					{
						playerController.IsMoving = false;
						continue;
					}
					
					
					playerController.Move(canMovableCellList, swipeDir);
					MazeManager.Instance.PaintFloors(canMovableCellList, floorColorsForPaint);
				}
			}
		}

		private static Dictionary<Vector2Int, Color> GetFloorColorsForPaint(Vector2Int changeColorCell, List<Vector2Int> canMovableCellList,
			PlayerController playerController)
		{
			var colorChangeDictionary = new Dictionary<Vector2Int, Color>();

			if (changeColorCell != new Vector2Int(-99, -99))
			{
				var changeColorCellIndex = canMovableCellList.FindIndex(x => x == changeColorCell);
				if (changeColorCellIndex != -1)
				{
					for (int i = 0; i < changeColorCellIndex; i++)
					{
						colorChangeDictionary.Add(canMovableCellList[i], playerController.Color);
					}

					var excludedColorList = new List<Color>()
					{
						playerController.Color,
						LevelManager.Instance.initializer.floorBaseColor
					};
					
					var newColor = ConfigMap.Instance.playerColorConfig.PlayerColors.GetRandom(excludedColorList);
					playerController.ChangePlayerColor(newColor);
					for (int i = changeColorCellIndex; i < canMovableCellList.Count; i++)
					{
						colorChangeDictionary.Add(canMovableCellList[i], newColor);
					}
				}
				else
				{
					foreach (var floorCoord in canMovableCellList)
					{
						colorChangeDictionary.Add(floorCoord, playerController.Color);
					}
				}
			}
			else
			{
				foreach (var floorCoord in canMovableCellList)
				{
					colorChangeDictionary.Add(floorCoord, playerController.Color);
				}
			}

			return colorChangeDictionary;
		}

		private Vector2Int GetChangeColorCell(List<Vector2Int> canMovableCellList)
		{
			var changeColorCell = new Vector2Int(-99, -99);
			var colorChangeFloorIndex = MazeManager.Instance.FloorList.FindIndex(x => x.isColorChangeFloor);
			if(colorChangeFloorIndex != -1)
				changeColorCell = MazeManager.Instance.FloorList[colorChangeFloorIndex].transform.position.ToVector2Int();
			return changeColorCell;
		}

		Dictionary<Vector2Int, List<PlayerController>> GetReCalculatedMoveData(
			Dictionary<Vector2Int, List<PlayerController>> sortedMoveData, Vector2Int swipeDir)
		{
			var reCalculatedMoveData = new Dictionary<Vector2Int, List<PlayerController>>();
			foreach (var (targetPos, players) in sortedMoveData)
			{
				for (int i = 0; i < players.Count; i++)
				{
					var playerController = players[i];
					var lastTargetPos = targetPos - (i * swipeDir);

					if (lastTargetPos.x < 0 || lastTargetPos.y < 0)
						lastTargetPos = playerController.transform.position.ToVector2Int();

					if (!reCalculatedMoveData.ContainsKey(lastTargetPos))
						reCalculatedMoveData.Add(lastTargetPos, new List<PlayerController>());

					reCalculatedMoveData[lastTargetPos].Add(playerController);
				}
			}

			return reCalculatedMoveData;
		}

		Vector2Int GetFurthestMovableCell(List<Vector2Int> cellPosList, Vector2Int startPos, Vector2Int dir)
			=> GetMovableCellList(cellPosList, startPos, dir)[^1];

		List<Vector2Int> GetMovableCellList(List<Vector2Int> cellPosList, Vector2Int startPos, Vector2Int dir)
		{
			var movableCellList = new List<Vector2Int> { startPos };
			var nextPosition = startPos + dir;
			while (cellPosList.Contains(nextPosition))
			{
				movableCellList.Add(nextPosition);
				nextPosition += dir;
			}

			return movableCellList;
		}

		private Vector2Int GetVector2IntDirectionValue(float horizontalDelta, float verticalDelta, Vector2 swipeDirection)
		{
			Vector2Int dir;

			if (horizontalDelta > verticalDelta)
				dir = swipeDirection.x < 0 ? Vector2Int.left : Vector2Int.right;
			else
				dir = swipeDirection.y < 0 ? Vector2Int.down : Vector2Int.up;

			return dir;
		}

		private bool CheckOnUI()
		{
			if (Application.isMobilePlatform)
			{
				return !IsOnUI(0);
			}

			return IsOnUI();
		}

		private bool IsOnUI(int fingerId = -1)
		{
			if (EventSystem.current != null)
			{
				if (fingerId == -1)
				{
					return EventSystem.current.IsPointerOverGameObject();
				}

				return EventSystem.current.IsPointerOverGameObject(fingerId);
			}

			return false;
		}
	}
}