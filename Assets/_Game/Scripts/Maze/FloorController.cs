using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
	public class FloorController : MonoBehaviour
	{
		[SerializeField] private new Renderer renderer;
		public bool IsPainted { get; set; }
		public Color CurrentColor => renderer.material.color;

		[FormerlySerializedAs("colorChangeFloor")] public bool isColorChangeFloor;
        public List<Color> colorList;
		
		public void ReplaceColor(Color color)
		{
			renderer.material.color = color;
		}

		private void ChangeColorRandomly()
		{
			renderer.material.DOColor(colorList.GetRandom(CurrentColor), 1f);
		}
		
		public void StartColorChanging()
		{
			if (isColorChangeFloor)
			{
				InvokeRepeating(nameof(ChangeColorRandomly), 0, 1f);
			}
		}
	}
}