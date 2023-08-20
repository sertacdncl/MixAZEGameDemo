using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ParticleController : MonoBehaviour
{
	public ParticleSystem particle;
	
	public void PlayParticle(Vector3 pos, Quaternion quaternion, Color color)
	{
		var particleTransform = particle.transform;
		particleTransform.position = pos;
		particleTransform.rotation = quaternion;
		var particleMain = particle.main;
		particleMain.startColor = color;
		particle.Play();
		
		var destroyDuration = ConfigMap.Instance.mazeConfig.HitWallParticleDestroyDuration;
		DOVirtual.DelayedCall(destroyDuration, () =>
		{
			Addressables.ReleaseInstance(gameObject);
		});
	}
	
}
