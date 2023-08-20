using System.Collections.Generic;
using DG.Tweening;
using Extensions.List;
using MeshTools;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game
{
	public class LevelInitialize : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI levelText;
		[HideInInspector] public Transform currentLevelContainer;
		[HideInInspector] public Color floorBaseColor;
		
		private Transform _mazeBackground;
		private GameObject _mazeWallGameObject;
		private LevelDataContainer CurrentLevelData => LevelManager.CurrentLevelData;
		readonly Dictionary<int, int[]> _shuffledLevelOrder = new Dictionary<int, int[]>();

		public void Initialize()
		{
			var levelInfo = Resources.Load<TextAsset>(LevelImportTags.LevelInfo).text;
			LevelManager.ReplaceLevelCount(JsonUtility.FromJson<LevelInfo>(levelInfo).LevelCount);
			
			if(LevelManager.LevelCount == LevelManager.CurrentLevel)
				LevelManager.ReplaceCurrentLevel(Random.Range(1, LevelManager.LevelCount));
			
			levelText.text = $"Level {LevelManager.CurrentLevel+1}";
			CreateLevel(LevelManager.CurrentLevel);
		}
        

		private void CreateLevel(int level)
		{
			var levelString = Resources.Load<TextAsset>(LevelImportTags.GetLevelName(level)).text;
			var data = JsonUtility.FromJson<LevelDataContainer>(levelString);
			LevelManager.ReplaceCurrentLevelData(data);
			SetupLevel();
		}

		private void SetupLevel()
		{
			floorBaseColor = ConfigMap.Instance.playerColorConfig.PlayerColors.GetRandom(floorBaseColor);

			if (currentLevelContainer != null)
			{
				currentLevelContainer.DetachChildren();
				Destroy(currentLevelContainer.gameObject);
			}

			currentLevelContainer = new GameObject("LevelContainer").transform;

			var mazeOffset = ConfigMap.Instance.mazeConfig.MazeOffset;
			currentLevelContainer.position += (mazeOffset + CurrentLevelData.MovableCellPosList.GetMax().x) *
											LevelManager.CurrentLevel * Vector3.right;

			if (_mazeBackground == null)
			{
				CreateMazeBackground();
			}

			CreateMazeWalls();
			CreateFloors();
			CreatePlayer();
		}

		private void CreateMazeBackground()
		{
			Addressables.InstantiateAsync(MazeAssetCatalog.Instance.mazeBackground, currentLevelContainer,true).Completed +=
				handle =>
				{
					var mazeBackgroundObject = handle.Result;
					_mazeBackground = mazeBackgroundObject.transform;
					var mazeBgController = mazeBackgroundObject.GetComponent<MazeBackgroundController>();
					mazeBgController.ReplaceColor(ConfigMap.Instance.mazeConfig.MazeColor);
					MazeManager.Instance.mazeBackground = _mazeBackground.transform;
				};
		}

		private void CreateMazeWalls()
		{
			var mazeConfig = ConfigMap.Instance.mazeConfig;
			Addressables.InstantiateAsync(MazeAssetCatalog.Instance.mazeWall, currentLevelContainer,true).Completed +=
				handle =>
				{
					var mazeWallObject = handle.Result;
					MazeManager.Instance.mazeWallObject = mazeWallObject;
					var meshGenerationContext = new MeshGenerationContext(CurrentLevelData.MovableCellPosList,
						mazeConfig.CellSize,
						mazeConfig.MazeBorderDepth, mazeConfig.CornerConfig);
					var wallMesh = GridCutout.GetSingleMesh(meshGenerationContext);

					Addressables.InstantiateAsync(MazeAssetCatalog.Instance.mazeWallMesh, mazeWallObject.transform).Completed +=
						operationHandle =>
						{
							_mazeWallGameObject = operationHandle.Result;
							_mazeWallGameObject.transform.position = Vector3.zero;

							_mazeWallGameObject.GetComponent<MeshFilter>().mesh = wallMesh;
							_mazeWallGameObject.GetComponent<MeshRenderer>().material.color = mazeConfig.MazeColor;
							DOVirtual.DelayedCall(.25f, SetCamera);
						};
				};
		}

		private void CreateFloors()
		{
			foreach (var floorCoord in CurrentLevelData.MovableCellPosList)
			{
				Addressables.InstantiateAsync(MazeAssetCatalog.Instance.floor, currentLevelContainer,true).Completed +=
					handle =>
					{
						var floorObject = handle.Result;
						var floorController = floorObject.GetComponent<FloorController>();
						floorObject.transform.position = new Vector3(floorCoord.x, floorCoord.y,
							ConfigMap.Instance.mazeConfig.MazeBorderDepth);
						floorController.ReplaceColor(floorBaseColor.Desaturate(0.2f));
						
						if (CurrentLevelData.ColorChangeCellPos == floorCoord)
						{
							floorController.isColorChangeFloor = true;
							var colorList = ConfigMap.Instance.playerColorConfig.PlayerColors;
							colorList.Shuffle();
							floorController.colorList = colorList;
							floorController.StartColorChanging();
						}
						MazeManager.Instance.FloorList.Add(floorController);
					};
			}
		}

		private void CreatePlayer()
		{
			var mazeConfig = ConfigMap.Instance.mazeConfig;
			var playerPos = new Vector3(CurrentLevelData.PlayerCellPos.x, CurrentLevelData.PlayerCellPos.y,
				mazeConfig.MazeBorderDepth * 0);


			Addressables.InstantiateAsync(PlayerAssetCatalog.Instance.player, currentLevelContainer,true).Completed +=
				handle =>
				{
					var playerObject = handle.Result;
					var playerController = playerObject.GetComponent<PlayerController>();
					playerObject.transform.position = playerPos;
					PlayerManager.Instance.PlayerControllerList.Add(playerController);
				};
		}

		private void SetCamera()
		{
			
			SwipeInputManager.Instance.IsInputBlock = true;
			var mazeSize = CurrentLevelData.MovableCellPosList.GetMax();
			var camNextPosX = transform.position.x + (mazeSize.x - 1) / 2f;
			var gameCam = Camera.main; //TODO: optimize

			FitMazeIntoCamera(gameCam, camNextPosX, out var cameraLocalPoint, out var cameraParentPoint);

			var camSequence = DOTween.Sequence();

			var cameraTransform = gameCam.transform;
			camSequence
				.Append(cameraTransform.DOLocalMove(cameraLocalPoint, .5f))
				.Join(cameraTransform.parent.DOMove(cameraParentPoint, .5f))
				.AppendCallback(() => LevelManager.Instance.IsReady = true)
				.AppendInterval(0.1f)
				.OnComplete(() =>
				{
					SwipeInputManager.Instance.IsInputBlock = false;
					TutorialManager.Instance.Initialize();
				});
		}

		private void FitMazeIntoCamera(Camera gameCam, float camNextPosX, out Vector3 cameraLocalPoint,
			out Vector3 cameraParentPoint)
		{
			var mazeSize = CurrentLevelData.MovableCellPosList.GetMax();

			var cameraTransform = gameCam.transform;
			var gridSize = mazeSize;

			var boundCenter = transform.position;
			boundCenter.x = camNextPosX;
			var boundSize = new Vector3(gridSize.x + 1, gridSize.y + 1, 1);
			var bounds = new Bounds(boundCenter, boundSize);

			var mazeConfig = ConfigMap.Instance.mazeConfig;
			bounds.size += (Vector3)mazeConfig.CameraBoundsOffset;

			var cameraSpaceMin = cameraTransform.InverseTransformPoint(new Vector3(bounds.min.x, bounds.max.y));
			var cameraSpaceMax = cameraTransform.InverseTransformPoint(new Vector3(bounds.max.x, bounds.min.y));
			var boundsCameraSpace = new Bounds(.5f * (cameraSpaceMin + cameraSpaceMax), cameraSpaceMax - cameraSpaceMin);

			var distanceY = boundsCameraSpace.size.y * 0.5f / Mathf.Tan(gameCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
			var distanceX = 1 / gameCam.aspect * boundsCameraSpace.size.x * 0.5f /
							Mathf.Tan(gameCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
			var zDisplacement = Mathf.Abs(boundsCameraSpace.center.z - boundsCameraSpace.min.z);
			distanceY += zDisplacement;
			distanceX += zDisplacement;
			var distance = Mathf.Max(distanceX, distanceY, mazeConfig.MinCameraZoom);

			cameraLocalPoint = new Vector3(0f, 0f, -distance);
			cameraParentPoint = bounds.center.SetZ(0) + mazeConfig.CameraOffset;
		}
	}
}