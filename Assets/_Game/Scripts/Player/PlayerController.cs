using System.Collections.Generic;
using DG.Tweening;
using Extensions.List;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace Game
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private new Renderer renderer;
		public bool IsMoving { get; set; }
		public Color Color => renderer.material.color;
		
		private Color _color;
		private Sequence _hitBounceSequence;
		private Vector2Int _currentDir;

		public void ReplaceColor(Color color)
		{
			renderer.material.color = color;
		}

		public void Move(List<Vector2Int> movePath, Vector2Int direction)
		{
			_currentDir = direction;
			var mazeConfig = ConfigMap.Instance.mazeConfig;
			var destPos = movePath[^1].ToViewPosition().WithZ(0);
			var playerMeshTransform = renderer.transform;

			var targetEulerAngles = Vector3.zero;
			var eulerAngles = playerMeshTransform.eulerAngles;
			targetEulerAngles = direction.x switch
			{
				> 0 => new Vector3(0f, eulerAngles.y + -90f, 0f),
				< 0 => new Vector3(0f, eulerAngles.y + 90f, 0f),
				_ => direction.y switch
				{
					> 0 => new Vector3(eulerAngles.y + 90f, 0f, 0f),
					< 0 => new Vector3(eulerAngles.y + -90f, 0f, 0),
					_ => targetEulerAngles
				}
			};

			var scaleSize = Vector3.one + direction.ToViewPosition().Abs() * .25f;
			renderer.transform.DOScale(scaleSize, 0.2f).SetEase(Ease.OutCubic);

			transform.DOMove(destPos, 0.2f).SetEase(Ease.OutCubic).onComplete += () =>
			{
				transform.position = destPos;
				IsMoving = false;
				OnMoveComplete();
			};
			playerMeshTransform.DOLocalRotate(targetEulerAngles, 0.5f, RotateMode.WorldAxisAdd)
				.SetEase(Ease.OutCubic);
		}
		
		private void OnMoveComplete()
		{
			IsMoving = false;
			PlayHitBounceAnimation();
			PlayWallHitSound();
			CreateHitParticle();
			if(TutorialManager.IsTutorialActive)
				TutorialManager.Instance.CompleteTutorial();
			LevelManager.Instance.CheckLevelComplete();
		}

		private void PlayHitBounceAnimation()
		{
			_hitBounceSequence = DOTween.Sequence();
			var minScale = Vector3.one - _currentDir.ToViewPosition().Abs() * 0.25f;
			_hitBounceSequence.Append(renderer.transform.DOScale(minScale, 0.15f).SetEase(Ease.InOutSine))
				.Append(renderer.transform.DOScale(1f, 0.15f).SetEase(Ease.InOutSine));
		}

		private void PlayWallHitSound()
		{
			var hitToWallClips = PlayerAssetCatalog.Instance.hitWallClips;
			var clip = hitToWallClips.GetRandom();
			var pitch = Random.Range(0.7f, 1.3f);
			var volume = Random.Range(0.5f, 0.7f);
			AudioManager.Instance.PlaySound(clip, pitch,volume);
		}

		private void CreateHitParticle()
		{
			//TODO: Do this with pool
			var pos = transform.position.ToVector2Int();
			var color = renderer.material.color;
			var hitParticleAsset = PlayerAssetCatalog.Instance.hitParticle;
			var effectPos = pos.ToVector3WorldPositionWithOffset(Vector3.forward * 0.5f, LevelManager.Instance.initializer.currentLevelContainer);
			Addressables.InstantiateAsync(hitParticleAsset).Completed += handle =>
			{
				var particleObject = handle.Result;
				var particleController = particleObject.GetComponent<ParticleController>();
				particleController.PlayParticle(effectPos, Quaternion.identity, color);
			};
		}
		
		public void ChangePlayerColor(Color color, bool animated = true)
		{
			_color = color;
			renderer.material.DOColor(color, animated ? 0.1f : 0.001f);
		}
	}
}