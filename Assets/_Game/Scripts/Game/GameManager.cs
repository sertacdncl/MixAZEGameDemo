using Game;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	private void Start()
	{
		SwipeInputManager.Instance.IsInputBlock = true;
		LoadAddressables();
		LevelManager.Instance.initializer.Initialize();
	}

	public void UnloadLevel()
	{
		//TODO:Change this
		Addressables.ReleaseInstance(PlayerManager.Instance.PlayerControllerList[0].gameObject);
		PlayerManager.Instance.PlayerControllerList.Clear();

		foreach (var floorController in MazeManager.Instance.FloorList)
		{
			Addressables.ReleaseInstance(floorController.gameObject);
		}
		MazeManager.Instance.FloorList.Clear();

		// Addressables.ReleaseInstance(MazeManager.Instance.mazeBackground.gameObject);
		Addressables.ReleaseInstance(MazeManager.Instance.mazeWallObject);
	}
	
	public void LoadLevel()
	{
		SwipeInputManager.Instance.IsInputBlock = true;
		LevelManager.Instance.IsReady = false;
		LoadAddressables();
		LevelManager.Instance.initializer.Initialize();
	}
	
	public void Restart()
	{
		SceneManager.LoadScene(0);
	}

	private static void LoadAddressables()
	{
		var mazeAssetCatalog = MazeAssetCatalog.Instance;
		var playerAssetCatalog = PlayerAssetCatalog.Instance;
		var levelAssetCatalog = LevelAssetCatalog.Instance;
		Addressables.LoadAssetAsync<GameObject>(mazeAssetCatalog.mazeBackground);
		Addressables.LoadAssetAsync<GameObject>(mazeAssetCatalog.mazeWall);
		Addressables.LoadAssetAsync<GameObject>(mazeAssetCatalog.floor);
		Addressables.LoadAssetAsync<GameObject>(playerAssetCatalog.player);
		Addressables.LoadAssetAsync<GameObject>(playerAssetCatalog.hitParticle);
		Addressables.LoadAssetAsync<GameObject>(levelAssetCatalog.winParticle);
	}
}