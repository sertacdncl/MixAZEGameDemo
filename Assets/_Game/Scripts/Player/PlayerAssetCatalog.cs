using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game
{
	public class PlayerAssetCatalog : Singleton<PlayerAssetCatalog>
	{
		public AssetReference player;
		public AssetReference hitParticle;
		public List<AudioClip> hitWallClips;
	}
}