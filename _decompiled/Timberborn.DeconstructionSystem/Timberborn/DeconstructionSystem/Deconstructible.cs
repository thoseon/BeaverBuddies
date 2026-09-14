using Timberborn.BaseComponentSystem;

namespace Timberborn.DeconstructionSystem;

public class Deconstructible : BaseComponent
{
	public void DisableDeconstruction()
	{
		DisableComponent();
	}
}
