using Timberborn.TickSystem;

namespace Timberborn.GameScene;

public class GameSceneTickingMode : ITickingMode
{
	public bool SingletonIsActiveInThisMode(object singleton)
	{
		return true;
	}
}
