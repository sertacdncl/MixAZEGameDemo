using System;
using UnityEngine.AddressableAssets;

namespace Game
{
	[Serializable]
	public class MazeAssetCatalog : Singleton<MazeAssetCatalog>
	{
		public AssetReference mazeBackground;
		public AssetReference mazeWall;
		public AssetReference mazeWallMesh;
		public AssetReference floor;
	}
}