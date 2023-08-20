using UnityEngine;

namespace Game
{
	public class MazeBackgroundController : MonoBehaviour
	{
		[SerializeField] private new Renderer renderer;

		public void ReplaceColor(Color color)
		{
			renderer.material.color = color;
		}
	}
}