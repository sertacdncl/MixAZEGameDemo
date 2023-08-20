using System.Collections.Generic;

namespace Game
{
	public class PlayerManager : Singleton<PlayerManager>
	{
		public List<PlayerController> PlayerControllerList { get; private set; } = new();
	}
}