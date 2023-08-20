using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game
{
	public class LevelAssetCatalog : Singleton<LevelAssetCatalog>
	{
		public AssetReference winParticle;
		public AudioClip levelCompleteSfx;
		public AudioClip winParticleSfx;
	}
}