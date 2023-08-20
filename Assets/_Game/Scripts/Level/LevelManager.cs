using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game
{
	public class LevelManager : Singleton<LevelManager>
	{
		public LevelInitialize initializer;
		public LevelCompletePanel completePanel;
		public bool IsReady { get; set; }
		public static LevelDataContainer CurrentLevelData { get; private set; }

		public static int CurrentLevel
		{
			get
			{
				if (!PlayerPrefs.HasKey("CurrentLevel"))
					PlayerPrefs.SetInt("CurrentLevel", 0);

				return PlayerPrefs.GetInt("CurrentLevel");
			}
			private set => PlayerPrefs.SetInt("CurrentLevel", value);
		}

		public static int LevelCount { get; private set; }
		public static void ReplaceLevelCount(int levelCount) => LevelCount = levelCount;

		public static void ReplaceCurrentLevel(int currentLevel) => CurrentLevel = currentLevel;

		public static void ReplaceCurrentLevelData(LevelDataContainer levelDataContainer) =>
			CurrentLevelData = levelDataContainer;

		public static void NextLevel()
		{
			CurrentLevel++;
			GameManager.Instance.UnloadLevel();
			GameManager.Instance.LoadLevel();
		}

		public static void ResetLevel() => CurrentLevel = 1;

		public void CheckLevelComplete()
		{
			var floorCount = MazeManager.Instance.FloorList.Count;
			var emptyVectorForColorChange = new Vector2Int(-99, -99);
			var isColorChangeExist = CurrentLevelData.ColorChangeCellPos != emptyVectorForColorChange;

			var checkCount = isColorChangeExist ? floorCount - 1 : floorCount;
			var samePaintedCellsCount = 0;
			foreach (var floorController in MazeManager.Instance.FloorList)
			{
				if (floorController.IsPainted &&
					PlayerManager.Instance.PlayerControllerList[0].Color == floorController.CurrentColor)
					samePaintedCellsCount++;
			}

			if (samePaintedCellsCount == checkCount)
				LevelComplete();
		}

		private void LevelComplete()
		{
			var levelAssetCatalog = LevelAssetCatalog.Instance;
			Addressables.InstantiateAsync(levelAssetCatalog.winParticle, Camera.main.transform).Completed += handle =>
			{
				AudioManager.Instance.PlaySound(levelAssetCatalog.winParticleSfx, volume: 0.3f);
				DOVirtual.DelayedCall(1f, () =>
				{
					AudioManager.Instance.PlaySound(levelAssetCatalog.levelCompleteSfx);
					completePanel.Show();
				});
				DOVirtual.DelayedCall(5f, () =>
				{
					
					Addressables.ReleaseInstance(handle.Result);
				});
			};
		}
	}
}