using DG.Tweening;
using Extensions;
using UnityEngine;

namespace Game
{
	public class TutorialHandController : MonoBehaviour
	{
		[SerializeField] SpriteRenderer hand;
		[SerializeField] Sprite fingerUp;
		[SerializeField] Sprite fingerDown;

		[SerializeField] float duration;
		[SerializeField] Ease ease;
		[SerializeField] float delay;

		Sequence _sequence;
		
		public void Show(bool show)
		{
			if(!TutorialManager.IsTutorialActive)
				return;
			hand.enabled = show;
		}

		public void MoveHand(Vector3 playerPos, Vector2Int direction)
		{
			var offsetY = -0.5f;
			var startPos = playerPos.With(y:offsetY) - Vector3.forward;
			var destPos = startPos + direction.ToViewPosition() * 3f;

			var points = new Vector3[] { startPos, destPos };

			_sequence?.Kill();

			hand.color = hand.color.With(a:0);
			hand.transform.localPosition = points[0];
			_sequence = DOTween.Sequence()
				.AppendCallback(() => { hand.sprite = fingerUp; })
				.Append(hand.DOFade(1f, .3f))
				.AppendInterval(0.01f)
				.AppendCallback(() => { hand.sprite = fingerDown; })
				.Append(hand.transform.DOLocalPath(points, duration).SetEase(ease))
				.AppendCallback(() => { hand.sprite = fingerUp; })
				.AppendInterval(.3f)
				.Append(hand.DOFade(0f, .3f))
				.SetLoops(-1, LoopType.Restart)
				.Pause();

			DOVirtual.DelayedCall(delay, () =>
			{
				delay = 0;
				_sequence?.Play();
			});

			_sequence.OnKill(() => hand.sprite = fingerUp);
		}

		public void Stop()
		{
			Show(false);
			_sequence?.Kill();
			DOTween.KillAll();
		}
	}
}