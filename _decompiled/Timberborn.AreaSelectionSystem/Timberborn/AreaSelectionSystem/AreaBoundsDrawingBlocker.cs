using Timberborn.BaseComponentSystem;

namespace Timberborn.AreaSelectionSystem;

public class AreaBoundsDrawingBlocker : BaseComponent
{
	public void DisableBlocking()
	{
		DisableComponent();
	}
}
