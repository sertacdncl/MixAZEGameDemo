using System.Collections;
using Extensions;
using UnityEngine;

namespace Game
{
	public class TutorialManager : Singleton<TutorialManager>
	{
		public static bool IsTutorialActive { get; set; }
		public TutorialData tutorialData;

		[SerializeField] private TutorialHandController tutorialHand;

		public void Initialize()
		{
			if(PlayerPrefs.HasKey("IsTutorialComplete"))
				if(PlayerPrefs.GetInt("IsTutorialComplete").ToBool())
					return;
			StartTutorial();
		}

		private void StartTutorial()
		{
			IsTutorialActive = true;
			var mazeConfig = ConfigMap.Instance.mazeConfig;
			var firstTutorialDir = mazeConfig.TutorialDirections[0];
			tutorialData = new TutorialData
			{
				tutorialId = 0,
				tutorialDirection = firstTutorialDir
			};

			var playerPos = PlayerManager.Instance.PlayerControllerList[0].transform.position;
			tutorialHand.gameObject.SetActive(true);
			tutorialHand.MoveHand(playerPos, firstTutorialDir);
		}

		public void CompleteTutorial()
		{
			PlayerPrefs.SetInt("IsTutorialComplete", 1);
			tutorialHand.Stop();
			tutorialHand.gameObject.SetActive(false);
			IsTutorialActive = false;
		}
	}
}