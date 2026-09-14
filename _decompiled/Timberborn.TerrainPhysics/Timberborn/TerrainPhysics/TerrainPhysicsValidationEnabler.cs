using Timberborn.SingletonSystem;

namespace Timberborn.TerrainPhysics;

public class TerrainPhysicsValidationEnabler : IPostLoadableSingleton
{
	public bool Enabled { get; private set; }

	public void PostLoad()
	{
		Enable();
	}

	public void Enable()
	{
		Enabled = true;
	}
}
