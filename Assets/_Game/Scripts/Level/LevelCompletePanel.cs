using DG.Tweening;
using UnityEngine;

namespace Game
{
	public class LevelCompletePanel : MonoBehaviour
	{
		[SerializeField] private GameObject levelCompletePanel;
		[SerializeField] private Animator animator;
		public void Show()
		{
			levelCompletePanel.SetActive(true);
			animator.SetBool("Close",false);
			animator.SetBool("Show",true);
		}

		public void OnClick_Continue()
		{
			animator.SetBool("Close",true);
			animator.SetBool("Show",false);
			DOVirtual.DelayedCall(0.5f, ()=>levelCompletePanel.SetActive(false));
			SwipeInputManager.Instance.IsInputBlock = true;
			LevelManager.NextLevel();
		}
	}
}